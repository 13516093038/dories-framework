using UnityEngine;
using Dories.Logging;

namespace GameFramework.UI.External.SafeArea
{
    public class SafeAreaAdapter : MonoBehaviour
    {
        private RectTransform Panel;
        private IPlatformSafeArea PlatformSafeArea;

        private void Awake()
        {
            CreatePlatformSafeArea();
            Panel = GetComponent<RectTransform>();
            Refresh();
        }

        private void Refresh()
        {
            Rect safeArea = PlatformSafeArea.GetSafeArea();
            ApplySafeArea(safeArea);
        }

        private void CreatePlatformSafeArea()
        {
#if UNITY_EDITOR
            PlatformSafeArea = new CommonSafeArea();
#elif UNITY_WEBGL && DOUYINMINIGAME
            PlatformSafeArea = new DouyinSafeArea();
#else
            PlatformSafeArea = new CommonSafeArea();
#endif
        }

        void ApplySafeArea(Rect r)
        {
            Vector2 anchorMin = r.position;
            Vector2 anchorMax = r.position + r.size;
            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;
            Panel.anchorMin = anchorMin;
            Panel.anchorMax = anchorMax;

            Log.Debug("SafeArea", $"New safe area applied to {name}: x={r.x}, y={r.y}, w={r.width}, h={r.height} on full extents w={Screen.width}, h={Screen.height}", this);
        }
    }
}