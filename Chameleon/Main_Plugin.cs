using BepInEx;
using BepInEx.Logging;
using Chameleon.Craftables;
using HarmonyLib;
using IndigocoderLib;
using Nautilus.Handlers;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace Chameleon
{
    [BepInPlugin(myGUID, pluginName, versionString)]
    [BepInDependency("com.snmodding.nautilus", BepInDependency.DependencyFlags.HardDependency)]
    public class Main_Plugin : BaseUnityPlugin
    {
        private const string myGUID = "Indigocoder.Chameleon";
        private const string pluginName = "Chameleon Sub";
        private const string versionString = "1.0.0";

        public static ManualLogSource logger;

        private static readonly Harmony harmony = new Harmony(myGUID);

        internal static ChameleonSaveCache SaveCache { get; } = SaveDataHandler.RegisterSaveDataCache<ChameleonSaveCache>();

        public static string AssetsFolderPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets");
        public static string RecipesFolderPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Recipes");
        public static string LocalizationFolderPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Localization");

        public static AssetBundle assetBundle { get; private set; }

        private void Awake()
        {
            logger = Logger;

            PiracyDetector.TryFindPiracy();

            LanguageHandler.RegisterLocalizationFolder();
            assetBundle = AssetBundle.LoadFromFile(Path.Combine(AssetsFolderPath, "chameleon"));
            InitializeSlotMapping();

            Chameleon_Craftable.Patch();

            harmony.PatchAll();

            logger.LogInfo($"{pluginName} {versionString} Loaded.");
        }

        private void InitializeSlotMapping()
        {
            Equipment.slotMapping.Add("ChameleonUpgrade1", EquipmentType.VehicleModule);
            Equipment.slotMapping.Add("ChameleonUpgrade2", EquipmentType.VehicleModule);
            Equipment.slotMapping.Add("ChameleonUpgrade3", EquipmentType.VehicleModule);
            Equipment.slotMapping.Add("ChameleonUpgrade4", EquipmentType.VehicleModule);
        }
    }
}
