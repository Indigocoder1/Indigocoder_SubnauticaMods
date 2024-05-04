using UnityEngine;

namespace Chameleon.Monobehaviors.Abstract
{
    internal abstract class PrefabModifier : MonoBehaviour
    {
        public virtual void OnAsyncPrefabTasksCompleted() { }
        public virtual void OnLateMaterialOperation() { }
    }
}
