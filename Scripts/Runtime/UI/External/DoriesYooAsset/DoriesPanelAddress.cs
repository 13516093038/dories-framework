#if DORIES_YOOASSET_SUPPORT
namespace GameFramework.Runtime.UI.External.DoriesYooAsset
{
    public class DoriesPanelAddress : IPanelAddress
    {
        public string packageName { get; set; }
        public string assetPath { get; set; }
        public uint priority { get; set; }
    }
}
#endif