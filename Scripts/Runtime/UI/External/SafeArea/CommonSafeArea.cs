using UnityEngine;

namespace GameFramework.UI.External.SafeArea
{
    public class CommonSafeArea : IPlatformSafeArea
    {
        public Rect GetSafeArea() => Screen.safeArea;
    }
}