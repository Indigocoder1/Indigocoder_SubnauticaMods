using System;

namespace Chameleon.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ChameleonUpgradeModuleAttribute : Attribute
    {
        public ChameleonUpgradeModuleAttribute(string ModuleTechType) 
        {
            moduleTechType = ModuleTechType;
        }

        public string ModuleTechType
        {
            get
            {
                return moduleTechType;
            }
        }

        private readonly string moduleTechType;
    }
}
