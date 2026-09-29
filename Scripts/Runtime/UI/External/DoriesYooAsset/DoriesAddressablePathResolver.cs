using System;

#if DORIES_YOOASSET_SUPPORT
namespace GameFramework.Runtime.UI.External.DoriesYooAsset
{
    public class DoriesAddressablePathResolver : IPanelAddressResolver
    {
        public virtual string PackageName { get; set; } = "Your_Package_Name";
        public virtual uint Priority { get; set; }

        public virtual string AssetPath(Type panelType)
        {
            return panelType.Name;
        }

        public IPanelAddress Resolve(Type panelType)
        {
            return new DoriesPanelAddress()
            {
                packageName = PackageName,
                assetPath = AssetPath(panelType),
                priority = Priority
            };
        }
    }
}
#endif