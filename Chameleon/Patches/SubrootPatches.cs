﻿using Chameleon.Monobehaviors;
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

            Main_Plugin.logger.LogMessage($"ID = {chameleon.GetComponent<PrefabIdentifier>().Id}");
            chameleon.LoadSaveData(chameleon.GetComponent<PrefabIdentifier>().Id);
            Main_Plugin.logger.LogInfo("Attempting to load Chameleon data");

            return false; //Temp fix until SubNames are properly implemented (idek if this will get done, just copying the Seal's comments)
        }
    }
}
