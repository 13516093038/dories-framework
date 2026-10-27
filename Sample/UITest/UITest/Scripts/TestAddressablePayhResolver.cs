using System;
using GameFramework.Runtime.UI.External.DoriesYooAsset;

namespace GameFramework.Test.UITest.Scripts
{
    public class TestAddressablePayhResolver : DoriesAddressablePathResolver
    {
        public override string PackageName { get; set; } = "DefaultPackage";
        public override uint Priority { get; set; } = 0;

        public override string AssetPath(Type panelType)
        {
            return $"{panelType.Name}";
        }
    }
}