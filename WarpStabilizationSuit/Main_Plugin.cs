using HarmonyLib;
using BepInEx;
using BepInEx.Logging;
using System.Reflection;
using System.IO;
using WarpStabilizationSuit.Items;
using BepInEx.Configuration;
using IndigocoderLib;
using BepInEx.Bootstrap;
using Nautilus.Handlers;
using SuitLib;
using Nautilus.Utility;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace WarpStabilizationSuit
{
    [BepInPlugin(myGUID, pluginName, versionString)]
    [BepInDependency("com.snmodding.nautilus", BepInDependency.DependencyFlags.HardDependency)]
    //[BepInDependency("Indigocoder.SuitLib", BepInDependency.DependencyFlags.HardDependency)]
    public class Main_Plugin : BaseUnityPlugin
    {
        private const string myGUID = "Indigocoder.WarpStabilizationSuit";
        private const string pluginName = "Warp Stabilization Suit";
        private const string versionString = "1.3.2";
        public static ManualLogSource logger;

        public static string AssetsFolderPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets");

        private static readonly Harmony harmony = new Harmony(myGUID);

        public static ConfigEntry<bool> UseHardRecipe;
        public static bool TabsNeeded;

        private IEnumerator Start()
        {
            logger = Logger;

            if (PiracyDetector.TryFindPiracy())
            {
                yield break;
            }

            harmony.PatchAll();
            UseHardRecipe = Config.Bind("Warp Stabilization Suit", "Use harder recipe", false);

            new Suit_ModOptions();

            Dictionary<string, PluginInfo> keys = Chainloader.PluginInfos;
            if (keys.ContainsKey("com.ramune.SeaglideUpgrades") || keys.ContainsKey("com.ramune.OrganizedWorkbench") || keys.ContainsKey("com.ramune.LithiumBatteries"))
            {
                CraftTreeHandler.AddTabNode(CraftTree.Type.Workbench, "Other", "Other", SpriteManager.Get(TechType.Titanium));
                TabsNeeded = true;
            }

            Gloves_Craftable.Patch();
            Suit_Craftable.Patch();
            //InitializeSuits();

            Logger.LogInfo($"{pluginName} {versionString} Loaded.");
        }

        private void InitializeSuits()
        {
            string suitFilePath = AssetsFolderPath + "/Textures/player_02_reinforced_suit_01_body_WARP.png";
            Texture2D warpSuitTexture = ImageUtils.LoadTextureFromFile(suitFilePath);

            string suitSpecFilePath = AssetsFolderPath + "/Textures/player_02_reinforced_suit_01_body_spec_WARP.png";
            Texture2D warpSuitSpec = ImageUtils.LoadTextureFromFile(suitSpecFilePath);

            string armsFilePath = AssetsFolderPath + "/Textures/player_02_reinforced_suit_01_arms_WARP.png";
            Texture2D warpArmsTexture = ImageUtils.LoadTextureFromFile(armsFilePath);

            string armsSpecFilePath = AssetsFolderPath + "/Textures/player_02_reinforced_suit_01_arms_spec_WARP.png";
            Texture2D warpArmsSpec = ImageUtils.LoadTextureFromFile(armsSpecFilePath);

            Dictionary<string, Texture2D> suitKeyValuePairs = new Dictionary<string, Texture2D> { { "_MainTex", warpSuitTexture }, { "_SpecTex", warpSuitSpec } };
            Dictionary<string, Texture2D> armKeyValuePairs = new Dictionary<string, Texture2D> { { "_MainTex", warpArmsTexture }, { "_SpecTex", warpArmsSpec } };

            ModdedSuit warpSuit = new ModdedSuit(suitKeyValuePairs, armKeyValuePairs, ModdedSuitsManager.VanillaModel.Reinforced, Suit_Craftable.techType,
                ModdedSuitsManager.Modifications.Reinforced);
            ModdedGloves warpGloves = new ModdedGloves(armKeyValuePairs, ModdedSuitsManager.VanillaModel.Reinforced, Gloves_Craftable.techType,
                ModdedSuitsManager.Modifications.Reinforced);

            ModdedSuitsManager.AddModdedSuit(warpSuit);
            ModdedSuitsManager.AddModdedGloves(warpGloves);
        }
    }
}
