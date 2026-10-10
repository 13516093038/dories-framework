using System;

namespace GameFramework.Runtime.UI
{
    public interface IResourceProvider
    {
        void LoadAssetAsync<T>(IPanelAddress address, Action<T> callback) where T : UnityEngine.Object;

        void UnloadAsset<T>(IPanelAddress address, T asset) where T : UnityEngine.Object;
    }
}