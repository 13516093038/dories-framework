using System;

namespace GameFramework.Runtime.UI
{
    public interface IResourceProvider
    {
        void LoadAssetAsync<T>(IPanelAddress address, Action<T> callback) where T : UIPanelEntity;

        void UnloadAsset<T>(IPanelAddress address, T asset) where T : UIPanelEntity;
    }
}