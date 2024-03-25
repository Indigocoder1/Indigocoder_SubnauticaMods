using System.Collections;
using UnityEngine;

namespace Chameleon.Monobehaviors.Cyclops
{
    internal static class CyclopsReferenceManager
    {
        public static GameObject CyclopsReference { get; private set; }
        private static bool loaded;

        public static IEnumerator EnsureCyclopsReference()
        {
            if(CyclopsReference)
            {
                yield break;
            }

            loaded = false;

            yield return new WaitUntil(() => LightmappedPrefabs.main);

            LightmappedPrefabs.main.RequestScenePrefab("Cyclops", new LightmappedPrefabs.OnPrefabLoaded(OnPrefabLoaded));

            yield return new WaitUntil(() => loaded);
        }

        private static void OnPrefabLoaded(GameObject gameObject)
        {
            CyclopsReference = gameObject;
            loaded = true;
        }
    }
}
