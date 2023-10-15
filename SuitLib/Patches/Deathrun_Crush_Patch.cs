namespace SuitLib.Patches
{
    internal static class Deathrun_Crush_Patch
    {
        private static void GetCrushDepth_Patch(TechType suit, float __result)
        {
            foreach (ModdedSuit moddedSuit in ModdedSuitsManager.moddedSuitsList)
            {
                if(moddedSuit.itemTechType != suit)
                {
                    continue;
                }

                __result = moddedSuit.deathrunCrushDepth;
                break;
            }
        }
    }
}
