using Chameleon.Attributes;

namespace Chameleon.Monobehaviors.UpgradeModules
{
    [ChameleonUpgradeModule("ChameleonHullModule3")]
    internal class DepthModule3 : DepthModule
    {
        public override float Depth => 900f;
    }
}
