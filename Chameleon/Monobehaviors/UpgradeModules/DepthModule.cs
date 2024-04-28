using Chameleon.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Chameleon.Monobehaviors.UpgradeModules
{
    internal abstract class DepthModule : MonoBehaviour, IOnModuleChange
    {
        public abstract float Depth { get; }
        private CrushDamage _damage;

        protected CrushDamage Damage
        {
            get
            {
                if(!_damage)
                {
                    _damage = gameObject.GetComponentInParent<CrushDamage>(true);
                }
                return _damage;
            }
        }

        public void OnChange(TechType techType, bool added)
        {
            float depth = Mathf.Max(Depth, Damage.extraCrushDepth);
            Damage.SetExtraCrushDepth(depth);
        }

        public void OnDisable()
        {
            _damage.SetExtraCrushDepth(0);
        }
    }
}
