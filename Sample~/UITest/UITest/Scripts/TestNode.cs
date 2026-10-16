using Dories.YooassetSystem.Runtime.AssetLoader;
using Dories.YooAssetSystem.Runtime.Patch;
using GameFramework.Runtime.UI;
using GameFramework.Test.UITest.Scripts.Panels;
using UnityEngine;

namespace GameFramework.Test.UITest.Scripts
{
    public class TestNode : MonoBehaviour
    {
        [SerializeField] private AssetLoaderEntity _assetLoaderEntity;
        [SerializeField] private PatchEntity _patchEntity;
        [SerializeField] private UISystem _uiSystem;

        private GUIStyle _buttonStyle;

        private bool _isInit = false;

        private void Start()
        {
            _patchEntity
                .BuildNeedUpdateListener((downloader) => { downloader.StartDownload(); })
                .BuildPatchCompleteListener(Init)
                .BuildPatchFailedListener((error) => { Debug.LogError($"Patch failed: {error}"); })
                .BuildPatchErrorListener((error) => { Debug.LogError($"Patch error: {error}"); })
                .StartPatch();
        }

        private void Init()
        {
            Debug.Log("Init");
            _uiSystem.Init();
            _isInit = true;
        }

        private void OnGUI()
        {
            if (!_isInit) return;

            if (_buttonStyle == null)
            {
                _buttonStyle = new GUIStyle(GUI.skin.button);
                _buttonStyle.fontSize = 24;
            }

            if (GUI.Button(new Rect(10, 10, 200, 80), "Preload Panel Test1", _buttonStyle))
            {
                _uiSystem.PreloadPanel<TestPanel1>();
            }

            if (GUI.Button(new Rect(10, 100, 200, 80), "Open Panel Test1", _buttonStyle))
            {
                _uiSystem.OpenPanelAsync<TestPanel1>(UIGroup.Main, layerPriority: 10);
            }

            if (GUI.Button(new Rect(10, 190, 200, 80), "Close Panel Test1", _buttonStyle))
            {
                _uiSystem.ClosePanel<TestPanel1>();
            }

            if (GUI.Button(new Rect(10, 280, 200, 80), "Release Panel Test1", _buttonStyle))
            {
                _uiSystem.ReleasePanel<TestPanel1>();
            }

            if (GUI.Button(new Rect(10, 370, 200, 80), "Add PopUp Panel Test1", _buttonStyle))
            {
                _uiSystem.AddPopUpPanel<TestPanel1>(100, UIGroup.Main, layerPriority: 10);
            }

            if (GUI.Button(new Rect(10, 460, 200, 80), "Remove PopUp Panel Test1", _buttonStyle))
            {
                _uiSystem.RemovePopUpPanel<TestPanel1>();
            }

            if (GUI.Button(new Rect(220, 10, 200, 80), "Preload Panel Test2", _buttonStyle))
            {
                _uiSystem.PreloadPanel<TestPanel2>();
            }

            if (GUI.Button(new Rect(220, 100, 200, 80), "Open Panel Test2", _buttonStyle))
            {
                _uiSystem.OpenPanelAsync<TestPanel2>(UIGroup.Main, layerPriority: 20);
            }

            if (GUI.Button(new Rect(220, 190, 200, 80), "Close Panel Test2", _buttonStyle))
            {
                _uiSystem.ClosePanel<TestPanel2>();
            }

            if (GUI.Button(new Rect(220, 280, 200, 80), "Release Panel Test2", _buttonStyle))
            {
                _uiSystem.ReleasePanel<TestPanel2>();
            }

            if (GUI.Button(new Rect(220, 370, 200, 80), "Add PopUp Panel Test2", _buttonStyle))
            {
                _uiSystem.AddPopUpPanel<TestPanel2>(99, UIGroup.Main, layerPriority: 20);
            }

            if(GUI.Button(new Rect(220, 460, 200, 80), "Remove PopUp Panel Test2", _buttonStyle))
            {
                _uiSystem.RemovePopUpPanel<TestPanel2>();
            }

            if (GUI.Button(new Rect(440, 10, 200, 80), "Start Popup Panel", _buttonStyle))
            {
                _uiSystem.StartPopupPanel();
            }
        }
    }
}