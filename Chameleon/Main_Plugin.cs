using BepInEx;
using BepInEx.Logging;
using Chameleon.Attributes;
using Chameleon.Craftables;
using Chameleon.Monobehaviors;
using HarmonyLib;
using IndigocoderLib;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Crafting;
using Nautilus.Handlers;
using System;
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

        public static AssetBundle AssetBundle { get; } = AssetBundle.LoadFromFile(Path.Combine(AssetsFolderPath, "chameleon"));

        public static EquipmentType ChameleonUpgradeModuleType { get; } = EnumHandler.AddEntry<EquipmentType>("ChameleonModule");
        public static CraftTree.Type ChameleonFabricatorTree { get; } = EnumHandler.AddEntry<CraftTree.Type>("ChameleonFabricator").CreateCraftTreeRoot(out _);

        public static TechGroup ChameleonGroup { get; } = EnumHandler.AddEntry<TechGroup>("Chameleon").WithPdaInfo("Chameleon");

        public static TechCategory ChameleonCategory { get; } = EnumHandler.AddEntry<TechCategory>("Chameleon")
            .RegisterToTechGroup(ChameleonGroup).WithPdaInfo("Chameleon");

        public static TechCategory ChameleonModuleCategory { get; } = EnumHandler.AddEntry<TechCategory>("ChameleonModules")
            .RegisterToTechGroup(ChameleonGroup).WithPdaInfo("ChameleonModules");

        internal static PingType ChameleonPingType { get; private set; }

        private void Awake()
        {
            logger = Logger;

            PiracyDetector.TryFindPiracy();
            harmony.PatchAll();

            LanguageHandler.RegisterLocalizationFolder();

            ChameleonPingType = EnumHandler.AddEntry<PingType>("ChameleonSub")
            .WithIcon(new Atlas.Sprite(AssetBundle.LoadAsset<Sprite>("Ping_Chameleon")));

            InitializeSlotMapping();
            RegisterUpgradeModules();
            RegisterUpgradeModuleFunctionalities(Assembly.GetExecutingAssembly());

            logger.LogInfo($"{pluginName} {versionString} Loaded.");
        }

        private void InitializeSlotMapping()
        {
            Equipment.slotMapping.Add("ChameleonUpgrade1", ChameleonUpgradeModuleType);
            Equipment.slotMapping.Add("ChameleonUpgrade2", ChameleonUpgradeModuleType);
            Equipment.slotMapping.Add("ChameleonUpgrade3", ChameleonUpgradeModuleType);
            Equipment.slotMapping.Add("ChameleonUpgrade4", ChameleonUpgradeModuleType);
        }

        private void RegisterUpgradeModules()
        {
            PrefabInfo depthModuleMk1Info = PrefabInfo.WithTechType("ChameleonHullModule1", null, null)
                .WithIcon(SpriteManager.Get(TechType.CyclopsHullModule1));

            PrefabInfo depthModuleMk2Info = PrefabInfo.WithTechType("ChameleonHullModule2", null, null)
                .WithIcon(SpriteManager.Get(TechType.CyclopsHullModule1));

            PrefabInfo depthModuleMk3Info = PrefabInfo.WithTechType("ChameleonHullModule3", null, null)
                .WithIcon(SpriteManager.Get(TechType.CyclopsHullModule1));

            CreateUpgradeModulePrefab(depthModuleMk1Info);
            CreateUpgradeModulePrefab(depthModuleMk2Info);
            CreateUpgradeModulePrefab(depthModuleMk3Info);

            Chameleon_Craftable.Patch();
        }

        private void CreateUpgradeModulePrefab(PrefabInfo info, RecipeData recipe = null)
        {
            CustomPrefab prefab = new(info);
            prefab.SetGameObject(new CloneTemplate(info, TechType.CyclopsHullModule1));
            prefab.SetPdaGroupCategory(ChameleonGroup, ChameleonModuleCategory);
            prefab.SetUnlock(Chameleon_Craftable.techType);

            if (recipe == null) recipe = RecipeUtils.TryGetRecipeFromJson(info.TechType);
            prefab.SetRecipe(recipe).WithFabricatorType(ChameleonFabricatorTree);

            prefab.SetEquipment(ChameleonUpgradeModuleType);
            prefab.Register();
        }

        private void RegisterUpgradeModuleFunctionalities(Assembly assemly)
        {
            foreach (var type in assemly.GetTypes())
            {
                ChameleonUpgradeModuleAttribute attribute = type.GetCustomAttribute<ChameleonUpgradeModuleAttribute>();
                if(attribute is null || !type.IsClass || type.IsAbstract)
                {
                    return;
                }

                TechType techType;
                if (Enum.TryParse<TechType>(attribute.ModuleTechType, out techType) || EnumHandler.TryGetValue<TechType>(attribute.ModuleTechType, out techType))
                {
                    ChameleonSubRoot.moduleFunctions.Add(techType, type);
                }
            }
        }
    }
}
