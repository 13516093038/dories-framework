using System;
using System.Collections.Generic;
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
    internal sealed class PanelScheduler
    {
        internal sealed class PanelInfo
        {
            public Type PanelType { get;  internal set; }
            public UIGroup Group { get;  internal set; }
            public PanelStatus Status { get; internal set; }
            public UIPanelEntity Entity { get; internal set; }
        }
        private IPanelAddressResolver _panelAddressResolver;
        private IResourceProvider _resourceProvider;

        private Dictionary<Type, PanelInfo> _panels = new Dictionary<Type, PanelInfo>();
        private Dictionary<UIGroup, UIGroupEntity> _uiGroups;
        private Transform  uiSystemTransform;

        internal PanelScheduler(IPanelAddressResolver panelAddressResolver, IResourceProvider resourceProvider, Transform uiSystemTransform)
        {
            _panelAddressResolver = panelAddressResolver;
            _resourceProvider = resourceProvider;
            this.uiSystemTransform = uiSystemTransform;
            _uiGroups = new Dictionary<UIGroup, UIGroupEntity>();

            _panels = new Dictionary<Type, PanelInfo>();

            foreach (var layer in Enum.GetValues(typeof(UIGroup)))
            {
                GameObject go = new GameObject($"UIGroup_{layer}");
                go.transform.SetParent(uiSystemTransform);
                var rectTransform = go.AddComponent<RectTransform>();
                rectTransform.anchorMin = new Vector2(0, 0);
                rectTransform.anchorMax = new Vector2(1, 1);
                rectTransform.pivot = new Vector2(0.5f, 0.5f);
                rectTransform.anchoredPosition = new Vector2(0, 0);
                rectTransform.sizeDelta = new Vector2(0, 0);
                rectTransform.localRotation = Quaternion.identity;
                rectTransform.localScale = new Vector3(1, 1, 1);
                var groupEntity = go.AddComponent<UIGroupEntity>();
                groupEntity._group = (UIGroup)layer;
                _uiGroups.Add(groupEntity._group, groupEntity);
            }
        }
        
        /// <summary>
        /// 添加界面层级设置
        /// </summary>
        /// <param name="panelEntity">界面实体</param>
        /// <param name="group">界面组</param>
        /// <param name="layerPriority">界面层级</param>
        private void AddPanelLayerSetting(UIPanelEntity panelEntity, UIGroup group, int layerPriority)
        {
            var panelLayerEntity = panelEntity.gameObject.AddComponent<PanelLayerEntity>();
            panelEntity.transform.SetParent(_uiGroups[group].transform, false);
            panelLayerEntity.Group = group;
            panelLayerEntity.Layer = _uiGroups[group].GetUILayer(layerPriority);
            panelLayerEntity.InitLayerSettings();
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
        internal async UniTask<UIPanelEntity> OpenPanelAsyncUniTask(Type panelType, UIGroup group, object userData = null, int layerPriority = 0)
        {
            var utcs = new UniTaskCompletionSource<UIPanelEntity>();
            OpenPanelAsync(panelType, group, userData, (panelEntity) =>
            {
                utcs.TrySetResult(panelEntity);
            }, layerPriority);
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
        internal void OpenPanelAsync(Type panelType, UIGroup group, object userData = null, Action<UIPanelEntity> onComplete = null, int layerPriority = 0)
        {
            if (!typeof(UIPanelEntity).IsAssignableFrom(panelType))
            {
                throw new InvalidOperationException($"Panel type {panelType.Name} is not a UIPanelEntity");
            }

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
                    onComplete?.Invoke(panelInfo.Entity);
                }
                else
                {
                    throw new InvalidOperationException($"Panel {panelType.Name} is in an invalid state {panelInfo.Status}");
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
                
                //没有界面信息加载界面
                _resourceProvider.LoadAssetAsync<UIPanelEntity>(_panelAddressResolver.Resolve(panelType), (asset) =>
                {
                    asset.gameObject.SetActive(true);
                    AddPanelLayerSetting(asset, group, layerPriority);
                    asset.OnCreate(userData);
                    asset.OnOpen(userData);
                    panelInfo.Status = PanelStatus.Active;
                    panelInfo.Entity = asset;
                    onComplete?.Invoke(asset);
                });
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
        internal void OpenPanelAsync<T>(UIGroup group, object userData = null, Action<UIPanelEntity> onComplete = null, int layerPriority = 0) where T : UIPanelEntity
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
            if (!typeof(UIPanelEntity).IsAssignableFrom(panelType))
            {
                throw new InvalidOperationException($"Panel type {panelType.Name} is not a UIPanelEntity");
            }

            if (_panels.TryGetValue(panelType, out var panelInfo))
            {
                if (panelInfo.Status != PanelStatus.Active)
                {
                    throw new InvalidOperationException($"Panel {panelType.Name} is not active");
                }
                panelInfo.Entity.OnClose(userData);
                panelInfo.Entity.gameObject.SetActive(false);
                panelInfo.Status = PanelStatus.Inactive;
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
            if (!typeof(UIPanelEntity).IsAssignableFrom(panelType))
            {
                throw new InvalidOperationException($"Panel type {panelType.Name} is not a UIPanelEntity");
            }

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