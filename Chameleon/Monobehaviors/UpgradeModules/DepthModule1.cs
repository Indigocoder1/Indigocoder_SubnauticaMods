using Chameleon.Attributes;

namespace Chameleon.Monobehaviors.UpgradeModules
{
    [ChameleonUpgradeModule("ChameleonHullModule1")]
    internal class DepthModule1 : DepthModule
    {
        public override float Depth => 300f;
    }
}
