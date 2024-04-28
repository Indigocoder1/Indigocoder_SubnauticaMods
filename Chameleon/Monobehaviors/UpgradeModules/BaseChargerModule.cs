using UnityEngine;

namespace Chameleon.Monobehaviors.UpgradeModules
{
    internal abstract class BaseChargerModule<T> : MonoBehaviour where T : BaseChargerFunction
    {
        protected T chargerFunction;

        public virtual void Awake()
        {
            chargerFunction = gameObject.EnsureComponent<T>();
            chargerFunction.installedModules++;
        }

        public virtual void OnDestroy()
        {
            chargerFunction.installedModules--;
        }
    }
}
