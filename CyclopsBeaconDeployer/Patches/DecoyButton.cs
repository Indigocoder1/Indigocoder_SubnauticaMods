using HarmonyLib;

namespace CyclopsBeaconDeployer.Patches
{
    [HarmonyPatch(typeof(CyclopsDecoyLaunchButton))]
    internal static class DecoyButton
    {
        [HarmonyPatch(nameof(CyclopsDecoyLaunchButton.Update)), HarmonyPostfix]
        private static void Update_Patch(CyclopsDecoyLaunchButton __instance)
        {
            if(!Equipment_Patches.HasModule(__instance.subRoot.upgradeConsole.modules))
            {
                return;
            }

            if (string.IsNullOrEmpty(SubRoot_Patch.inputFields[__instance.subRoot].text))
            {
                return;
            }

            CyclopsDecoyManager manager = __instance.subRoot.GetComponent<CyclopsDecoyManager>();
            bool hasAnyBeacons = manager.decoyLoadingTube.decoySlots.GetCount(TechType.Beacon) > 0;

            if (__instance.mouseHover && hasAnyBeacons)
            {
                HandReticle main = HandReticle.main;
                main.SetText(HandReticle.TextType.Hand, "Launch Beacon", true, GameInput.Button.LeftHand);
                main.SetText(HandReticle.TextType.HandSubscript, string.Empty, false, GameInput.Button.None);
            }
        }
    }
}
