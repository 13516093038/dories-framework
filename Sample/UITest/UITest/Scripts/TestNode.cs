using GameFramework.Runtime.GameBase;
using GameFramework.Runtime.UI;
using GameFramework.Test.UITest.Scripts.Panels;
using UnityEngine;

namespace GameFramework.Test.UITest.Scripts
{
    public class TestNode : MonoBehaviour
    {
        private GUIStyle _buttonStyle;

        private void OnGUI()
        {
            // 补丁、设默认包、UISystem.Init 由 UITestPatchProcedure 完成，进入本示例流程后才可用
            if (GameEntry.Procedure?.CurrentProcedure is not UITestProcedure) return;

            if (_buttonStyle == null)
            {
                _buttonStyle = new GUIStyle(GUI.skin.button);
                _buttonStyle.fontSize = 24;
            }

            if (GUI.Button(new Rect(10, 10, 200, 80), "Preload Panel Test1", _buttonStyle))
            {
                GameEntry.UI.PreloadPanel<TestPanel1>();
            }

            if (GUI.Button(new Rect(10, 100, 200, 80), "Open Panel Test1", _buttonStyle))
            {
                GameEntry.UI.OpenPanelAsync<TestPanel1>(UIGroup.Main, layerPriority: 10);
            }

            if (GUI.Button(new Rect(10, 190, 200, 80), "Close Panel Test1", _buttonStyle))
            {
                GameEntry.UI.ClosePanel<TestPanel1>();
            }

            if (GUI.Button(new Rect(10, 280, 200, 80), "Release Panel Test1", _buttonStyle))
            {
                GameEntry.UI.ReleasePanel<TestPanel1>();
            }

            if (GUI.Button(new Rect(10, 370, 200, 80), "Add PopUp Panel Test1", _buttonStyle))
            {
                GameEntry.UI.AddPopUpPanel<TestPanel1>(100, UIGroup.Main, layerPriority: 10);
            }

            if (GUI.Button(new Rect(10, 460, 200, 80), "Remove PopUp Panel Test1", _buttonStyle))
            {
                GameEntry.UI.RemovePopUpPanel<TestPanel1>();
            }

            if (GUI.Button(new Rect(220, 10, 200, 80), "Preload Panel Test2", _buttonStyle))
            {
                GameEntry.UI.PreloadPanel<TestPanel2>();
            }

            if (GUI.Button(new Rect(220, 100, 200, 80), "Open Panel Test2", _buttonStyle))
            {
                GameEntry.UI.OpenPanelAsync<TestPanel2>(UIGroup.Main, layerPriority: 20);
            }

            if (GUI.Button(new Rect(220, 190, 200, 80), "Close Panel Test2", _buttonStyle))
            {
                GameEntry.UI.ClosePanel<TestPanel2>();
            }

            if (GUI.Button(new Rect(220, 280, 200, 80), "Release Panel Test2", _buttonStyle))
            {
                GameEntry.UI.ReleasePanel<TestPanel2>();
            }

            if (GUI.Button(new Rect(220, 370, 200, 80), "Add PopUp Panel Test2", _buttonStyle))
            {
                GameEntry.UI.AddPopUpPanel<TestPanel2>(99, UIGroup.Main, layerPriority: 20);
            }

            if(GUI.Button(new Rect(220, 460, 200, 80), "Remove PopUp Panel Test2", _buttonStyle))
            {
                GameEntry.UI.RemovePopUpPanel<TestPanel2>();
            }

            if (GUI.Button(new Rect(440, 10, 200, 80), "Start Popup Panel", _buttonStyle))
            {
                GameEntry.UI.StartPopupPanel();
            }
        }
    }
}