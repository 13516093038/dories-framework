#if DORIES_YOOASSET_SUPPORT
using System;
using Dories.YooassetSystem.Runtime.AssetLoader;
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

        public void LoadAssetAsync<T>(IPanelAddress address, Action<T> callback) where T : Object
        {
            var doriesAddress = address as DoriesPanelAddress;

            _assetLoaderEntity.LoadAssetAsync<T>(doriesAddress.packageName, doriesAddress.assetPath,
                prefab => { callback?.Invoke(prefab); }, doriesAddress.priority);
        }

        public void UnloadAsset<T>(IPanelAddress address, T asset) where T : Object
        {
            var doriesAddress = address as DoriesPanelAddress;
            _assetLoaderEntity.UnloadAsset(doriesAddress.packageName, doriesAddress.assetPath, true);
        }
    }
}
#endif