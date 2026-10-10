using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework.Runtime.UI
{
    internal sealed partial class PanelScheduler
    {
        private sealed class PreloadRequest
        {
            public GameObject Prefab;
            public bool Cancelled = false;
            public Action<GameObject> OnComplete;
        }

        private Dictionary<Type, PreloadRequest> _preloadPanels = new Dictionary<Type, PreloadRequest>();

        /// <summary>
        /// 预加载界面
        /// </summary>
        /// <param name="panelType">预加载的界面类型</param>
        /// <param name="onComplete">预加载完成回调</param>
        internal void PreloadPanel(Type panelType, Action onComplete = null)
        {
            ThrowIfInValidType(panelType);

            if (_panels.ContainsKey(panelType))
            {
                throw new InvalidOperationException($"Panel {panelType.Name} is already loaded");
            }

            var request = new PreloadRequest()
            {
                Prefab = null,
                Cancelled = false,
                OnComplete = null,
            };

            if (!_preloadPanels.TryAdd(panelType, request))
            {
                throw new InvalidOperationException($"Panel {panelType.Name} is already preloaded");
            }

            try
            {
                _resourceProvider.LoadAssetAsync<GameObject>(_panelAddressResolver.Resolve(panelType), (asset) =>
                {
                    if (request.Cancelled || !_preloadPanels.ContainsKey(panelType))
                    {
                        _resourceProvider.UnloadAsset(_panelAddressResolver.Resolve(panelType), asset);
                        return;
                    }

                    _preloadPanels[panelType].Prefab = asset;
                    request.OnComplete?.Invoke(asset);
                    onComplete?.Invoke();
                });
            }
            catch (Exception)
            {
                _preloadPanels.Remove(panelType);
                throw;
            }
        }

        /// <summary>
        /// 预加载界面
        /// </summary>
        /// <typeparam name="T">界面类型</typeparam>
        /// <param name="onComplete">预加载完成回调</param>
        internal void PreloadPanel<T>(Action onComplete = null) where T : UIPanelEntity
        {
            PreloadPanel(typeof(T), onComplete);
        }
    }
}
