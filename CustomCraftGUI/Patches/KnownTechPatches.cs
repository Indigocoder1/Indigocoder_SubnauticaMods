using HarmonyLib;
using System.Collections.Generic;

namespace CustomCraftGUI.Patches
{
    [HarmonyPatch(typeof(KnownTech))]
    internal class KnownTechPatches
    {
        [HarmonyPatch(nameof(KnownTech.Initialize)), HarmonyPostfix]
        private static void Initialize_Postfix()
        {
            Plugin.cacheData.defaultTech = KnownTech.defaultTech;

            List<Plugin.SlimAnalysisTech> slimTech = new();
            foreach (var tech in KnownTech.analysisTech)
            {
                slimTech.Add(new(tech.techType, tech.unlockTechTypes));
            }

            Plugin.cacheData.analysisTech = slimTech;
            Plugin.CacheDefaultTech();
        }
    }
}
