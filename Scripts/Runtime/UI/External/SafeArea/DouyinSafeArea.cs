#if UNITY_WEBGL && DOUYINMINIGAME
using System;
using TTSDK;
using TTSDK.UNBridgeLib.LitJson;
using UnityEngine;

namespace GameFramework.UI.External.SafeArea
{
    /// <summary>
    /// 抖音小游戏安全区。系统区域来自 TT.GetSystemInfo().safeArea（逻辑像素、左上角原点、竖屏正方向），
    /// 上边界再取右上角胶囊按钮的下沿。返回值与 Screen.safeArea 一致：像素坐标，原点在左下角。
    /// </summary>
    public class DouyinSafeArea : IPlatformSafeArea
    {
        public Rect GetSafeArea()
        {
            try
            {
                if (!CanIUse.GetSystemInfo)
                {
                    return Screen.safeArea;
                }

                TTSystemInfo info = TT.GetSystemInfo();
                if (info == null || info.screenWidth <= 0 || info.screenHeight <= 0)
                {
                    return Screen.safeArea;
                }

                return ToScreenRect(info);
            }
            catch (Exception exception)
            {
                Debug.LogWarning($"[DouyinSafeArea] 读取安全区失败，使用 Screen.safeArea。{exception.Message}");
                return Screen.safeArea;
            }
        }

        private static Rect ToScreenRect(TTSystemInfo info)
        {
            float left = info.safeArea.left;
            float top = info.safeArea.top;
            float right = info.safeArea.right > info.safeArea.left
                ? info.safeArea.right
                : info.safeArea.left + info.safeArea.width;
            float bottom = info.safeArea.bottom > info.safeArea.top
                ? info.safeArea.bottom
                : info.safeArea.top + info.safeArea.height;

            if (right <= left || bottom <= top)
            {
                left = 0f;
                top = 0f;
                right = info.screenWidth;
                bottom = info.screenHeight;
            }

            int logicalWidth = info.screenWidth;
            int logicalHeight = info.screenHeight;
            if (info.deviceOrientation == TTDeviceOrientation.Landscape)
            {
                OrientLandscape(ref left, ref top, ref right, ref bottom, ref logicalWidth, ref logicalHeight);
            }

            ApplyMenuButton(ref top, logicalHeight);

            float scaleX = Screen.width / (float)logicalWidth;
            float scaleY = Screen.height / (float)logicalHeight;
            float x = left * scaleX;
            float y = (logicalHeight - bottom) * scaleY;
            float width = (right - left) * scaleX;
            float height = (bottom - top) * scaleY;

            x = Mathf.Clamp(x, 0f, Screen.width);
            y = Mathf.Clamp(y, 0f, Screen.height);
            width = Mathf.Clamp(width, 0f, Screen.width - x);
            height = Mathf.Clamp(height, 0f, Screen.height - y);
            if (width <= 0f || height <= 0f)
            {
                return Screen.safeArea;
            }

            return new Rect(x, y, width, height);
        }

        /// <summary>
        /// safeArea 固定按竖屏正方向返回。横屏时把矩形转到当前屏幕，原点仍是左上角。
        /// </summary>
        private void OrientLandscape(
            ref float left,
            ref float top,
            ref float right,
            ref float bottom,
            ref int logicalWidth,
            ref int logicalHeight)
        {
            bool sizeStillPortrait = logicalWidth < logicalHeight;
            bool areaExceedsScreen = right > logicalWidth + 1f || bottom > logicalHeight + 1f;
            if (!sizeStillPortrait && !areaExceedsScreen)
            {
                return;
            }

            int portraitWidth = sizeStillPortrait ? logicalWidth : logicalHeight;
            int portraitHeight = sizeStillPortrait ? logicalHeight : logicalWidth;
            bool notchOnRight = Screen.orientation == ScreenOrientation.LandscapeRight;

            float rotatedLeft;
            float rotatedTop;
            float rotatedRight;
            float rotatedBottom;
            if (notchOnRight)
            {
                rotatedLeft = portraitHeight - bottom;
                rotatedTop = left;
                rotatedRight = portraitHeight - top;
                rotatedBottom = right;
            }
            else
            {
                rotatedLeft = top;
                rotatedTop = portraitWidth - right;
                rotatedRight = bottom;
                rotatedBottom = portraitWidth - left;
            }

            left = Mathf.Min(rotatedLeft, rotatedRight);
            right = Mathf.Max(rotatedLeft, rotatedRight);
            top = Mathf.Min(rotatedTop, rotatedBottom);
            bottom = Mathf.Max(rotatedTop, rotatedBottom);
            logicalWidth = portraitHeight;
            logicalHeight = portraitWidth;
        }

        private void ApplyMenuButton(ref float top, int logicalHeight)
        {
            if (!CanIUse.GetMenuButtonLayout)
            {
                return;
            }

            JsonData layout = TT.GetMenuButtonLayout();
            if (layout == null)
            {
                return;
            }

            double menuBottom = layout.OptGetDouble("bottom");
            double menuHeight = layout.OptGetDouble("height");
            if (menuHeight <= 0d || menuBottom <= 0d || menuBottom >= logicalHeight)
            {
                return;
            }

            top = Mathf.Max(top, (float)menuBottom);
        }
    }
}
#endif
