using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace GameFramework.Runtime.UI
{
    internal sealed partial class PanelScheduler
    {
        internal sealed class PanelInfo
        {
            public Type PanelType { get; internal set; }
            public UIGroup Group { get; internal set; }
            public PanelStatus Status { get; internal set; }
            public UIPanelEntity Entity { get; internal set; }

            internal float CloseTime { get; set; }
        }

        private IPanelAddressResolver _panelAddressResolver;
        private IResourceProvider _resourceProvider;

        private Dictionary<Type, PanelInfo> _panels = new Dictionary<Type, PanelInfo>();
        private Dictionary<UIGroup, UIGroupEntity> _uiGroups;
        private List<Type> _pendingReleasePanels = new List<Type>();
        
        private Transform uiSystemTransform;
        

        internal PanelScheduler(IPanelAddressResolver panelAddressResolver, IResourceProvider resourceProvider,
            Transform uiSystemTransform)
        {
            _panelAddressResolver = panelAddressResolver;
            _resourceProvider = resourceProvider;
            this.uiSystemTransform = uiSystemTransform;
            _uiGroups = new Dictionary<UIGroup, UIGroupEntity>();

            _panels = new Dictionary<Type, PanelInfo>();

            foreach (var layer in Enum.GetValues(typeof(UIGroup)))
            {
                GameObject go = new GameObject($"UIGroup_{layer}");
                go.transform.SetParent(uiSystemTransform, false);
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ThrowIfInValidType(Type panelType)
        {
            if (!typeof(UIPanelEntity).IsAssignableFrom(panelType))
            {
                throw new InvalidOperationException($"Panel type {panelType.Name} is not a UIPanelEntity");
            }
        }
    }
}
