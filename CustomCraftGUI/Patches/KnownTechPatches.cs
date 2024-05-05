using HarmonyLib;
using System.Linq;

namespace CustomCraftGUI.Patches
{
    [HarmonyPatch(typeof(KnownTech))]
    internal class KnownTechPatches
    {
        [HarmonyPatch(nameof(KnownTech.Initialize)), HarmonyPostfix]
        private static void Initialize_Postfix()
        {
            Plugin.cacheData.defaultTech = KnownTech.defaultTech;
            Plugin.cacheData.analysisTech = KnownTech.analysisTech;
            Plugin.CacheDefaultTech();
        }
    }
}
