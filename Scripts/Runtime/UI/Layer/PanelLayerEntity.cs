using UnityEngine;
using UnityEngine.UI;

namespace GameFramework.Runtime.UI
{
    public class PanelLayerEntity : MonoBehaviour
    {
        private Canvas _canvas;
        private GraphicRaycaster _graphicRaycaster;

        public int Layer { get; internal set; }
        public UIGroup Group { get; internal set; }

        internal void InitLayerSettings()
        {
            _canvas = gameObject.AddComponent<Canvas>();
            _canvas.overrideSorting = true;
            _canvas.sortingOrder = Layer;
            _graphicRaycaster = gameObject.AddComponent<GraphicRaycaster>();
        }
    }
}