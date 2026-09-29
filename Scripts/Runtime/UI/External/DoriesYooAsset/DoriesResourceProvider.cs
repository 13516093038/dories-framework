#if DORIES_YOOASSET_SUPPORT
using System;
using Dories.YooassetSystem.Runtime.AssetLoader;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameFramework.Runtime.UI.External.DoriesYooAsset
{
    public class DoriesResourceProvider : IResourceProvider
    {
        private AssetLoaderEntity _assetLoaderEntity;

        public DoriesResourceProvider()
        {
            _assetLoaderEntity = Object.FindObjectOfType<AssetLoaderEntity>();
            if (_assetLoaderEntity == null)
            {
                throw new InvalidOperationException("AssetLoaderEntity not found");
            }
        }

        public void LoadAssetAsync<T>(IPanelAddress address, Action<T> callback) where T : UIPanelEntity
        {
            var doriesAddress = address as DoriesPanelAddress;

            _assetLoaderEntity.LoadAssetAsync<GameObject>(doriesAddress.packageName, doriesAddress.assetPath, prefab =>
            {
                var instance = Object.Instantiate(prefab);
                var panel = instance.GetComponent<T>();
                callback?.Invoke(panel);
            }, doriesAddress.priority);
        }

        public void UnloadAsset<T>(IPanelAddress address, T asset) where T : UIPanelEntity
        {
            var doriesAddress = address as DoriesPanelAddress;
            _assetLoaderEntity.UnloadAsset(doriesAddress.packageName, doriesAddress.assetPath, true);
        }
    }
}
#endif