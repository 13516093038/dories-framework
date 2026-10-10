using System;
using System.Collections.Generic;

namespace GameFramework.Runtime.UI
{
    internal sealed partial class PanelScheduler
    {
        private sealed class PopUpPanelInfo
        {
            public Type PanelType;
            public UIGroup Group;
            public object userData;
            public Func<uint> GetPopUpPriority;
            public int LayerPriority;
        }

        private Dictionary<Type, PopUpPanelInfo> _popUpPanels = new Dictionary<Type, PopUpPanelInfo>();
        private bool _isPopuping = false;

        internal bool IsPopuping => _isPopuping;

        internal void AddPopUpPanel(Type type, Func<uint> getPopUpPriority, UIGroup group, bool isPreload = true,
            object userData = null,
            int layerPriority = 0)
        {
            ThrowIfInValidType(type);

            if (_popUpPanels.TryGetValue(type, out var popUpPanelInfo))
            {
                throw new InvalidOperationException($"Pop up panel {type.Name} is already added");
            }

            popUpPanelInfo = new PopUpPanelInfo()
            {
                PanelType = type,
                Group = group,
                userData = userData,
                GetPopUpPriority = getPopUpPriority,
                LayerPriority = layerPriority,
            };

            if (!_preloadPanels.ContainsKey(type) && !_panels.ContainsKey(type) && isPreload)
            {
                PreloadPanel(type);
            }
            _popUpPanels.Add(type, popUpPanelInfo);
        }

        internal void AddPopUpPanel(Type type, uint popUpPriority, UIGroup group, bool isPreload = true, object userData = null,
            int layerPriority = 0)
        {
            AddPopUpPanel(type, () => popUpPriority, group, isPreload, userData, layerPriority);
        }

        internal void AddPopUpPanel<T>(uint popUpPriority, UIGroup group, bool isPreload = true, object userData = null, int layerPriority = 0)
            where T : UIPanelEntity
        {
            AddPopUpPanel(typeof(T), () => popUpPriority, group, isPreload, userData, layerPriority);
        }

        internal void AddPopUpPanel<T>(Func<uint> getPopUpPriority, UIGroup group, bool isPreload = true, object userData = null, int layerPriority = 0)
            where T : UIPanelEntity
        {
            AddPopUpPanel(typeof(T), getPopUpPriority, group, isPreload, userData, layerPriority);
        }

        internal void RemovePopUpPanel(Type type)
        {
            if (_popUpPanels.TryGetValue(type, out var popUpPanelInfo))
            {
                _popUpPanels.Remove(type);
            }
            else
            {
                throw new InvalidOperationException($"Not found pop up panel {type.Name}");
            }
        }

        internal void RemovePopUpPanel<T>() where T : UIPanelEntity
        {
            RemovePopUpPanel(typeof(T));
        }

        internal void StartPopupPanel()
        {
            if (_isPopuping)
            {
                throw new InvalidOperationException("Popup panel is already starting");
            }

            _isPopuping = true;
            PopupNextPanel();
        }

        private void PopupNextPanel()
        {
            Type maxPriorityPanel = GetFirstPopupPanel();
            if (maxPriorityPanel == null)
            {
                _isPopuping = false;
                return;
            }

            var info = _popUpPanels[maxPriorityPanel];
            _popUpPanels.Remove(maxPriorityPanel);
            try
            {
                OpenPanelAsync(maxPriorityPanel, info.Group, info.userData, null, info.LayerPriority, true);
            }
            catch
            {
                _isPopuping = false;
                throw;
            }
        }

        private Type GetFirstPopupPanel()
        {
            if (_popUpPanels.Count == 0)
            {
                return null;
            }

            Type maxPriorityPanel = null;
            uint maxPriority = 0;

            foreach (var panelInfo in _popUpPanels)
            {
                uint priority = panelInfo.Value.GetPopUpPriority();
                if (priority >= maxPriority)
                {
                    maxPriority = priority;
                    maxPriorityPanel = panelInfo.Key;
                }
            }
            return maxPriorityPanel;
        }
    }
}
