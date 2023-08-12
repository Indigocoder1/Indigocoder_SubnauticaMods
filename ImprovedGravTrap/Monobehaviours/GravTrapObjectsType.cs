using System.Collections.Generic;
using UnityEngine;

namespace ImprovedGravTrap.Monobehaviours
{
    internal class GravTrapObjectsType : MonoBehaviour
    {
        public int techTypeListIndex
        {
            get => _techTypeListIndex;
            set => _techTypeListIndex = value % Main_Plugin.AllowedTypes.Count;
        }
        private int _techTypeListIndex = 0;

        public string GetCurrentListName()
        {
            return Main_Plugin.AllowedTypes[techTypeListIndex].name;
        }

        public bool IsValidTarget(GameObject obj) //Called on each frame for each attracted object
        {
            bool result = false;
            TechType techType = GetObjectTechType(obj);
            Pickupable component = obj.GetComponent<Pickupable>();
            List<TechType> allowedTypes = Main_Plugin.AllowedTypes[techTypeListIndex].techTypes;

            if (!component || !component.attached)
            {
                for (int i = 0; i < allowedTypes.Count; i++)
                {
                    if (allowedTypes[i] == techType)
                    {
                        result = true;
                        break;
                    }
                }
            }
            return result;
        }

        private TechType GetObjectTechType(GameObject obj)
        {
			if (obj.GetComponentInParent<SinkingGroundChunk>() || obj.name.Contains("TreaderShale"))
				return TechType.ShaleChunk;

            if (obj.TryGetComponent<GasPod>(out var gasPod))
                return gasPod.detonated ? TechType.None : TechType.GasPod;

            if (obj.TryGetComponent<Pickupable>(out var p))
                return p.GetTechType();

            return CraftData.GetTechType(obj);
        }

        public void HandleAttracted(GameObject obj, bool added)
        {
            if (added)
            {
                if (obj.TryGetComponent<Crash>(out var crash))
                {
                    crash.AttackLastTarget(); // if target object is CrashFish we want to pull it out
                }
                else if (obj.TryGetComponent<SinkingGroundChunk>(out var chunk))
                {
                    Destroy(chunk);

                    var c = obj.AddComponent<BoxCollider>();
                    c.size = new Vector3(0.736f, 0.51f, 0.564f);
                    c.center = new Vector3(0.076f, 0.224f, 0.012f);

                    obj.transform.Find("models").localPosition = Vector3.zero;
                }
				else if (obj.GetComponent<Pickupable>()?.GetTechType() == TechType.JeweledDiskPiece)
				{
					var rb = obj.GetComponent<Rigidbody>();
					rb.mass = 1f;
					rb.useGravity = false;

					obj.EnsureComponent<WorldForces>();
				}

            }
			if (GetComponent<EnhancedGravSphere>() && obj.TryGetComponent<GasPod>(out var gasPod))
			{
				gasPod.grabbedByPropCannon = added;

				if (!added)
					gasPod.PrepareDetonationTime();
			}
        }
    }
}
