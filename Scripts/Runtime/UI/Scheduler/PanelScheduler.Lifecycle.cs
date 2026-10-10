using System;
#if TASK_BASED_OPEN_PANEL
using System.Threading.Tasks;
#endif
#if DORIES_UNITASK_SUPPORT
using Cysharp.Threading.Tasks;
#endif
using UnityEngine;
using Object = System.Object;

namespace GameFramework.Runtime.UI
{
    internal sealed partial class PanelScheduler
    {
        internal void AutoReleaseUpdate(int autoReleaseTime)
        {
            _pendingReleasePanels.Clear();

            foreach (var panelInfo in _panels)
            {
                if (panelInfo.Value.Status == PanelStatus.Inactive)
                {
                    panelInfo.Value.CloseTime += Time.deltaTime;
                }

                if (panelInfo.Value.CloseTime >= autoReleaseTime)
                {
                    _pendingReleasePanels.Add(panelInfo.Key);
                }
            }

            foreach (var panelType in _pendingReleasePanels)
            {
                ReleasePanel(panelType);
            }
        }

#if TASK_BASED_OPEN_PANEL
        /// <summary>
        /// 异步打开界面
        /// </summary>
        /// <param name="panelType">界面类型</param>
        /// <param name="group">界面组</param>
        /// <param name="userData">用户数据</param>
        /// <param name="layerPriority">界面层级</param>
        /// <returns>界面实体</returns>
        /// <returns></returns>
        internal async Task<UIPanelEntity> OpenPanelAsync(Type panelType, UIGroup group, object userData = null,
            int layerPriority = 0)
        {
            var tcs = new TaskCompletionSource<UIPanelEntity>();
            OpenPanelAsync(panelType, group, userData, tcs.SetResult, layerPriority);
            return await tcs.Task;
        }
#endif
#if DORIES_UNITASK_SUPPORT
        /// <summary>
        /// 异步打开界面
        /// </summary>
        /// <param name="panelType">界面类型</param>
        /// <param name="group">界面组</param>
        /// <param name="userData">用户数据</param>
        /// <param name="layerPriority">界面层级</param>
        /// <returns>界面实体</returns>
        internal async UniTask<UIPanelEntity> OpenPanelAsyncUniTask(Type panelType, UIGroup group,
            object userData = null, int layerPriority = 0)
        {
            var utcs = new UniTaskCompletionSource<UIPanelEntity>();
            OpenPanelAsync(panelType, group, userData, (panelEntity) => { utcs.TrySetResult(panelEntity); },
                layerPriority);
            return await utcs.Task;
        }
#endif

        /// <summary>
        /// 异步打开界面
        /// </summary>
        /// <param name="panelType">界面类型</param>
        /// <param name="group">界面组</param>
        /// <param name="userData">用户数据</param>
        /// <param name="onComplete">打开完成回调</param>
        /// <param name="layerPriority">界面层级</param>
        /// <exception cref="InvalidOperationException"></exception>
        internal void OpenPanelAsync(Type panelType, UIGroup group, object userData = null,
            Action<UIPanelEntity> onComplete = null, int layerPriority = 0, bool openByPopup = false)
        {
            ThrowIfInValidType(panelType);

            if (_panels.TryGetValue(panelType, out var panelInfo))
            {
                //界面已经打开
                if (panelInfo.Status == PanelStatus.Active)
                {
                    throw new InvalidOperationException($"Panel {panelType.Name} is already opened");
                }
                else if (panelInfo.Status == PanelStatus.Loading)
                {
                    throw new InvalidOperationException($"Panel {panelType.Name} is already loading");
                }
                else if (panelInfo.Status == PanelStatus.Inactive)
                {
                    panelInfo.Entity.gameObject.SetActive(true);
                    panelInfo.Status = PanelStatus.Active;
                    panelInfo.Entity.OnOpen(userData);

                    if (openByPopup)
                    {
                        panelInfo.Entity._openByPopup = true;
                        panelInfo.Entity.OnPopupOpen(userData);
                    }

                    onComplete?.Invoke(panelInfo.Entity);
                }
                else
                {
                    throw new InvalidOperationException(
                        $"Panel {panelType.Name} is in an invalid state {panelInfo.Status}");
                }
            }
            else
            {
                panelInfo = new PanelInfo
                {
                    PanelType = panelType,
                    Group = group,
                    Status = PanelStatus.Loading
                };

                _panels.Add(panelType, panelInfo);

                void Show(GameObject prefab)
                {
                    var instance = UnityEngine.Object.Instantiate(prefab);
                    instance.SetActive(true);
                    var panelEntity = instance.GetComponent<UIPanelEntity>();
                    AddPanelLayerSetting(panelEntity, group, layerPriority);
                    panelInfo.Entity = panelEntity;
                    panelInfo.Status = PanelStatus.Active;
                    panelEntity.OnCreate(userData);
                    panelEntity.OnOpen(userData);

                    if (openByPopup)
                    {
                        panelEntity._openByPopup = true;
                        panelEntity.OnPopupOpen(userData);
                    }

                    onComplete?.Invoke(panelEntity);
                }

                if (_preloadPanels.TryGetValue(panelType, out var request))
                {
                    if (request.Prefab != null)
                    {
                        var prefab = request.Prefab;
                        request.OnComplete = null;
                        _preloadPanels.Remove(panelType);
                        Show(prefab);
                    }
                    else
                    {
                        request.OnComplete += prefab =>
                        {
                            _preloadPanels.Remove(panelType);
                            Show(prefab);
                        };
                    }
                }
                else
                {
                    _resourceProvider.LoadAssetAsync<GameObject>(_panelAddressResolver.Resolve(panelType), Show);
                }
            }
        }

        /// <summary>
        /// 异步打开界面
        /// </summary>
        /// <param name="group">界面组</param>
        /// <param name="userData">用户数据</param>
        /// <param name="onComplete">打开完成回调</param>
        /// <param name="layerPriority">界面层级</param>
        /// <typeparam name="T">界面类型</typeparam>
        internal void OpenPanelAsync<T>(UIGroup group, object userData = null, Action<UIPanelEntity> onComplete = null,
            int layerPriority = 0) where T : UIPanelEntity
        {
            OpenPanelAsync(typeof(T), group, userData, onComplete, layerPriority);
        }

        /// <summary>
        /// 关闭界面
        /// </summary>
        /// <param name="panelType">界面类型</param>
        /// <param name="userData">用户数据</param>
        /// <exception cref="InvalidOperationException"></exception>
        internal void ClosePanel(Type panelType, Object userData = null)
        {
            ThrowIfInValidType(panelType);

            if (_panels.TryGetValue(panelType, out var panelInfo))
            {
                if (panelInfo.Status != PanelStatus.Active)
                {
                    throw new InvalidOperationException($"Panel {panelType.Name} is not active");
                }

                panelInfo.Entity.OnClose(userData);
                panelInfo.Entity.gameObject.SetActive(false);
                panelInfo.Status = PanelStatus.Inactive;
                panelInfo.CloseTime = 0;

                if (panelInfo.Entity._openByPopup)
                {
                    panelInfo.Entity._openByPopup = false;
                    PopupNextPanel();
                }
            }
            else
            {
                throw new InvalidOperationException($"Not found panel {panelType.Name}");
            }
        }

        /// <summary>
        /// 关闭界面
        /// </summary>
        /// <param name="userData">用户数据</param>
        /// <typeparam name="T">界面类型</typeparam>
        internal void ClosePanel<T>(Object userData = null) where T : UIPanelEntity
        {
            ClosePanel(typeof(T), userData);
        }

        /// <summary>
        /// 释放界面
        /// </summary>
        /// <param name="panelType">界面类型</param>
        /// <param name="userData">用户数据</param>
        /// <exception cref="InvalidOperationException"></exception>
        internal void ReleasePanel(Type panelType, object userData = null)
        {
            ThrowIfInValidType(panelType);

            if (_panels.TryGetValue(panelType, out var panelInfo))
            {
                if (panelInfo.Status == PanelStatus.Active)
                {
                    ClosePanel(panelType, userData);
                }

                panelInfo.Entity.OnRelease(userData);
                UnityEngine.Object.Destroy(panelInfo.Entity.gameObject);
                _resourceProvider.UnloadAsset(_panelAddressResolver.Resolve(panelType), panelInfo.Entity);
                _panels.Remove(panelType);
            }
            else if (_preloadPanels.TryGetValue(panelType, out var request))
            {
                _preloadPanels.Remove(panelType);
                if (request.Prefab != null)
                {
                    _resourceProvider.UnloadAsset(_panelAddressResolver.Resolve(panelType), request.Prefab);
                }
                else
                {
                    request.Cancelled = true;
                }
            }
            else
            {
                throw new InvalidOperationException($"Not found panel {panelType.Name}");
            }
        }

        /// <summary>
        /// 释放界面
        /// </summary>
        /// <param name="userData">用户数据</param>
        /// <typeparam name="T">界面类型</typeparam>
        internal void ReleasePanel<T>(Object userData = null) where T : UIPanelEntity
        {
            ReleasePanel(typeof(T), userData);
        }
    }
}
