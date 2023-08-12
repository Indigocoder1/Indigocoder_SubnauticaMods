using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ImprovedGravTrap
{
    [BepInPlugin(myGUID, pluginName, versionString)]
    public class Main_Plugin : BaseUnityPlugin
    {
        private const string myGUID = "Indigocoder.ImprovedGravTrap";
        private const string pluginName = "Improved Grav Trap";
        private const string versionString = "1.0.0";

        private static string ConfigFilePath = Path.Combine(Path.GetDirectoryName(Paths.BepInExConfigPath), "EnhancedGravTrap.json");

        public static ConfigEntry<bool> UseScrollWheel;
        public static ConfigEntry<KeyCode> AdvanceKey;
        public static ConfigEntry<int> EnhancedRange;
        public static ConfigEntry<float> EnhancedMaxForce;
        public static ConfigEntry<float> EnhancedMaxMassStable;
        public static ConfigEntry<int> EnhancedMaxObjects;

        public static List<TechTypeList> AllowedTypes;

        public static ManualLogSource logger;

        private static readonly Harmony harmony = new Harmony(myGUID);

        private void Awake()
        {
            logger = Logger;

            SetUpConfigs();
            new GravTrap_ModOptions();

            Trap_Craftable.Patch();

            harmony.PatchAll();

            Logger.LogInfo($"{pluginName} {versionString} Loaded.");
        }

        private void SetUpConfigs()
        {
            UseScrollWheel = Config.Bind("Improved Grav Trap", "Use scroll wheel to advance types", false);

            AdvanceKey = Config.Bind("Improved Grav Trap", "Key used to advance the type", KeyCode.Mouse2);

            EnhancedRange = Config.Bind("Improved Grav Trap", "Enhanced grav trap range", 30,
                new ConfigDescription("The range of the improved grav trap",
                acceptableValues: new AcceptableValueRange<int>(17, 40)));

            EnhancedMaxForce = Config.Bind("Improved Grav Trap", "Enhanced grav trap force", 20f,
                new ConfigDescription("The force of the improved grav trap",
                acceptableValues: new AcceptableValueRange<float>(15f, 30f)));

            EnhancedMaxMassStable = Config.Bind("Improved Grav Trap", "Enhanced max mass stable", 150f,
                new ConfigDescription("The max stable mass of the improved grav trap",
                acceptableValues: new AcceptableValueRange<float>(15f, 200f)));

            EnhancedMaxObjects = Config.Bind("Improved Grav Trap", "Enhanced grav trap max objects", 20,
                new ConfigDescription("The max attracted objects of the improved grav trap",
                acceptableValues: new AcceptableValueRange<int>(12, 30)));

            InitializeAllowedTypes();
        }

        private void InitializeAllowedTypes()
        {
            AllowedTypes = SaveManager.LoadFromJson(ConfigFilePath);
            if (AllowedTypes == null)
            {
                SaveInitialData();
                AllowedTypes = SaveManager.LoadFromJson(ConfigFilePath);
            }
        }

        private void SaveInitialData()
        {
            List<TechTypeList> list = new List<TechTypeList>
            {
                new TechTypeList("All", techTypes:
                new TechType[]
                {
                    TechType.Biter,
                    TechType.Bladderfish,
                    TechType.Bleeder,
                    TechType.Boomerang,
                    TechType.CaveCrawler,
                    TechType.Eyeye,
                    TechType.GarryFish,
                    TechType.GhostRayBlue,
                    TechType.GhostRayRed,
                    TechType.HoleFish,
                    TechType.Hoopfish,
                    TechType.Hoverfish,
                    TechType.Jellyray,
                    TechType.Jumper,
                    TechType.LavaLarva,
                    TechType.Oculus,
                    TechType.Peeper,
                    TechType.PrecursorDroid,
                    TechType.Reginald,
                    TechType.Skyray,
                    TechType.Spadefish,
                    TechType.RabbitRay,
                    TechType.Mesmer,
                    TechType.CrabSquid,
                    TechType.Stalker,
                    TechType.LavaLizard,
                    TechType.GasPod,
                    TechType.Quartz,
                    TechType.ScrapMetal,
                    TechType.FiberMesh,
                    TechType.LimestoneChunk,
                    TechType.Copper,
                    TechType.Lead,
                    TechType.Salt,
                    TechType.MercuryOre,
                    TechType.CalciumChunk,
                    TechType.Glass,
                    TechType.Titanium,
                    TechType.Silicone,
                    TechType.Gold,
                    TechType.Magnesium,
                    TechType.Sulphur,
                    TechType.Lodestone,
                    TechType.Bleach,
                    TechType.Silver,
                    TechType.BatteryAcidOld,
                    TechType.TitaniumIngot,
                    TechType.SandstoneChunk,
                    TechType.CrashPowder,
                    TechType.Diamond,
                    TechType.BasaltChunk,
                    TechType.ShaleChunk,
                    TechType.ObsidianChunk,
                    TechType.Lithium,
                    TechType.PlasteelIngot,
                    TechType.EnameledGlass,
                    TechType.PowerCell,
                    TechType.ComputerChip,
                    TechType.Fiber,
                    TechType.Enamel,
                    TechType.AcidOld,
                    TechType.VesselOld,
                    TechType.CombustibleOld,
                    TechType.OpalGem,
                    TechType.Uranium,
                    TechType.AluminumOxide,
                    TechType.HydrochloricAcid,
                    TechType.Magnetite,
                    TechType.AminoAcids,
                    TechType.Polyaniline,
                    TechType.AramidFibers,
                    TechType.Graphene,
                    TechType.Aerogel,
                    TechType.Nanowires,
                    TechType.Benzene,
                    TechType.Lubricant,
                    TechType.UraniniteCrystal,
                    TechType.ReactorRod,
                    TechType.DepletedReactorRod,
                    TechType.PrecursorIonCrystal,
                    TechType.Kyanite,
                    TechType.Nickel,
                    TechType.KelpForestEgg,
                    TechType.GrassyPlateausEgg,
                    TechType.GrandReefsEgg,
                    TechType.MushroomForestEgg,
                    TechType.KooshZoneEgg,
                    TechType.TwistyBridgesEgg,
                    TechType.LavaZoneEgg,
                    TechType.StalkerEgg,
                    TechType.StalkerEggUndiscovered,
                    TechType.ReefbackEgg,
                    TechType.ReefbackEggUndiscovered,
                    TechType.SpadefishEgg,
                    TechType.SpadefishEggUndiscovered,
                    TechType.RabbitrayEgg,
                    TechType.RabbitrayEggUndiscovered,
                    TechType.MesmerEgg,
                    TechType.MesmerEggUndiscovered,
                    TechType.JumperEgg,
                    TechType.JumperEggUndiscovered,
                    TechType.SandsharkEgg,
                    TechType.SandsharkEggUndiscovered,
                    TechType.JellyrayEgg,
                    TechType.JellyrayEggUndiscovered,
                    TechType.BonesharkEgg,
                    TechType.BonesharkEggUndiscovered,
                    TechType.CrabsnakeEgg,
                    TechType.CrabsnakeEggUndiscovered,
                    TechType.ShockerEgg,
                    TechType.ShockerEggUndiscovered,
                    TechType.GasopodEgg,
                    TechType.GasopodEggUndiscovered,
                    TechType.CrashEgg,
                    TechType.CrashEggUndiscovered,
                    TechType.CrabsquidEgg,
                    TechType.CrabsquidEggUndiscovered,
                    TechType.CutefishEgg,
                    TechType.CutefishEggUndiscovered,
                    TechType.LavaLizardEgg,
                    TechType.LavaLizardEggUndiscovered,
                    TechType.GenericEgg
                }),
                new TechTypeList("Creatures", techTypes:
                new TechType[]
                {
                    TechType.Biter,
                    TechType.Bleeder,
                    TechType.CaveCrawler,
                    TechType.CrabSquid,
                    TechType.Eyeye,
                    TechType.GarryFish,
                    TechType.GhostRayBlue,
                    TechType.GhostRayRed,
                    TechType.HoleFish,
                    TechType.Hoverfish,
                    TechType.Jellyray,
                    TechType.Jumper,
                    TechType.LavaLarva,
                    TechType.LavaLizard,
                    TechType.Mesmer,
                    TechType.Oculus,
                    TechType.Peeper,
                    TechType.RabbitRay,
                    TechType.Reginald,
                    TechType.Spadefish,
                    TechType.Stalker,
                    TechType.Bladderfish,
                    TechType.Boomerang,
                    TechType.Crash,
                    TechType.Hoopfish,
                    TechType.PrecursorDroid,
                    TechType.Skyray
                }),
                new TechTypeList("Resources", techTypes:
                new TechType[]
                {
                    TechType.Bleach,
                    TechType.GasPod,
                    TechType.SandstoneChunk,
                    TechType.ShaleChunk,
                    TechType.JeweledDiskPiece,
                    TechType.Aerogel,
                    TechType.AluminumOxide,
                    TechType.AramidFibers,
                    TechType.Benzene,
                    TechType.Copper,
                    TechType.DepletedReactorRod,
                    TechType.Diamond,
                    TechType.EnameledGlass,
                    TechType.FiberMesh,
                    TechType.Glass,
                    TechType.Gold,
                    TechType.HydrochloricAcid,
                    TechType.Kyanite,
                    TechType.Lead,
                    TechType.LimestoneChunk,
                    TechType.Lithium,
                    TechType.Lubricant,
                    TechType.Magnetite,
                    TechType.Nickel,
                    TechType.PlasteelIngot,
                    TechType.Polyaniline,
                    TechType.PrecursorIonCrystal,
                    TechType.Quartz,
                    TechType.ReactorRod,
                    TechType.Salt,
                    TechType.ScrapMetal,
                    TechType.Silicone,
                    TechType.Silver,
                    TechType.Sulphur,
                    TechType.Titanium,
                    TechType.TitaniumIngot,
                    TechType.UraniniteCrystal,
                    TechType.AcidMushroom
                }),
                new TechTypeList("Eggs", techTypes:
                new TechType[]
                {
                    TechType.BonesharkEgg,
                    TechType.BonesharkEggUndiscovered,
                    TechType.CrabsnakeEgg,
                    TechType.CrabsnakeEggUndiscovered,
                    TechType.CrabsquidEgg,
                    TechType.CrabsquidEggUndiscovered,
                    TechType.CrashEgg,
                    TechType.CrashEggUndiscovered,
                    TechType.CutefishEgg,
                    TechType.CutefishEggUndiscovered,
                    TechType.GasopodEgg,
                    TechType.GasopodEggUndiscovered,
                    TechType.JellyrayEgg,
                    TechType.JellyrayEggUndiscovered,
                    TechType.JumperEgg,
                    TechType.JumperEggUndiscovered,
                    TechType.LavaLizardEgg,
                    TechType.LavaLizardEggUndiscovered,
                    TechType.MesmerEgg,
                    TechType.MesmerEggUndiscovered,
                    TechType.RabbitrayEgg,
                    TechType.RabbitrayEggUndiscovered,
                    TechType.ReefbackEgg,
                    TechType.ReefbackEggUndiscovered,
                    TechType.SandsharkEgg,
                    TechType.SandsharkEggUndiscovered,
                    TechType.ShockerEgg,
                    TechType.ShockerEggUndiscovered,
                    TechType.SpadefishEgg,
                    TechType.SpadefishEggUndiscovered,
                    TechType.StalkerEgg,
                    TechType.StalkerEggUndiscovered,
                    TechType.CrashEgg,
                    TechType.CrashEggUndiscovered
                })
            };

            SaveManager.SaveToJson(list, ConfigFilePath);
        }
    }
}
