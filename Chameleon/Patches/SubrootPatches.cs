using Chameleon.Monobehaviors;
using HarmonyLib;

namespace Chameleon.Patches
{
    [HarmonyPatch(typeof(SubRoot))]
    internal class SubrootPatches
    {
        [HarmonyPatch(nameof(SubRoot.OnProtoDeserialize)), HarmonyPrefix]
        private static bool Deserialize_Prefix(SubRoot __instance)
        {
            if (__instance is not ChameleonSubRoot chameleon) return true;

            chameleon.LoadSaveData(chameleon.GetComponent<PrefabIdentifier>().Id);

            return false; //Temp fix until SubNames are properly implemented (idek if this will get done, just copying the Seal's comments)
        }
    }
}
