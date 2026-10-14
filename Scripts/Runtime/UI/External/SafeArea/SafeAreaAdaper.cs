using UnityEngine;

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

            Debug.LogFormat("New safe area applied to {0}: x={1}, y={2}, w={3}, h={4} on full extents w={5}, h={6}",
                name, r.x, r.y, r.width, r.height, Screen.width, Screen.height);
        }
    }
}