using Chameleon.Craftables;
using HarmonyLib;

namespace Chameleon.Patches
{
    [HarmonyPatch(typeof(Crafter))]
    internal class CrafterPatches
    {
        [HarmonyPatch(nameof(Crafter.Craft))]
        private static void Prefix(TechType techType, ref float duration)
        {
            if(techType == Chameleon_Craftable.techType)
            {
                //I doubt I'll ever remember to come back to/change this number so this is what it's gonna be I guess
                duration = 20f;
            }
        }
    }
}
