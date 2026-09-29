using Dories.YooassetSystem.Runtime.AssetLoader;
using Dories.YooAssetSystem.Runtime.Patch;
using GameFramework.Runtime.UI;
using GameFramework.Test.UITest.Scripts.Panels;
using UnityEngine;

namespace GameFramework.Test.UITest.Scripts
{
    public class TestNode : MonoBehaviour
    {
        //[SerializeField] private GameEntry _gameEntry;
        [SerializeField] private AssetLoaderEntity _assetLoaderEntity;
        [SerializeField] private PatchEntity _patchEntity;
        [SerializeField] private UISystem _uiSystem;

        private void Start()
        {
            _patchEntity
                .BuildNeedUpdateListener((downloader) =>
                {
                   downloader.StartDownload();
                })
                .BuildPatchCompleteListener(Init)
                .BuildPatchFailedListener((error) =>
                {
                    Debug.LogError($"Patch failed: {error}");
                })
                .BuildPatchErrorListener((error) =>
                {
                    Debug.LogError($"Patch error: {error}");
                })
                .StartPatch();
        }

        private void Init()
        {
            Debug.Log("Init");
            _uiSystem.Init();
            OpenPanel();
        }

        private void OpenPanel()
        {
            _uiSystem.OpenPanelAsync<TestPanel1>(UIGroup.Main);
        }
    }   
}