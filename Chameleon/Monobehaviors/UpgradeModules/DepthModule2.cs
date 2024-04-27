using Chameleon.Attributes;

namespace Chameleon.Monobehaviors.UpgradeModules
{
    [ChameleonUpgradeModule("ChameleonHullModule2")]
    internal class DepthModule2 : DepthModule
    {
        public override float Depth => 1200f;
    }
}
