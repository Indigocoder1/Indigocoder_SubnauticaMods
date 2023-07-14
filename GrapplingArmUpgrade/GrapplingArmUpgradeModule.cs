using System;
using UnityEngine;
using System.Collections.Generic;
using Nautilus.Crafting;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using BepInEx.Logging;
using Nautilus.Handlers;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Utility;
using System.Reflection;
using System.IO;

namespace GrapplingArmUpgrade_BepInEx
{
    internal class GrapplingArmUpgradeModule
    {
        public static TechType TechType { get; private set; }

        public static void RegisterModule()
        {
            Main_Plugin.logger.Log(LogLevel.Info, "Registering module");

            string spriteFilePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets") + "/exosuitgrapplingarmmodule_Upgraded.png";
            var prefabInfo = PrefabInfo.WithTechType("GrapplingArmUpgradeModule", "Prawn suit grappling arm upgrade module", "With this upgrade module all grappling arms on your Prawn will have enhanced capabilities!")
                .WithIcon(ImageUtils.LoadSpriteFromFile(spriteFilePath));

            var customPrefab = new CustomPrefab(prefabInfo);
            customPrefab.SetRecipe(new RecipeData()
            {
                craftAmount = 1,
                Ingredients = new List<CraftData.Ingredient>()
                {
                    new CraftData.Ingredient(TechType.ExosuitGrapplingArmModule),
                    new CraftData.Ingredient(TechType.Polyaniline, 2),
                    new CraftData.Ingredient(TechType.Lithium, 2),
                    new CraftData.Ingredient(TechType.AramidFibers),
                    new CraftData.Ingredient(TechType.AluminumOxide),
                }
            })
                .WithFabricatorType(CraftTree.Type.Workbench)
                .WithCraftingTime(5f);

            CraftDataHandler.SetBackgroundType(prefabInfo.TechType, CraftData.BackgroundType.ExosuitArm);

            int extraFragmentsToScan = 2;
            customPrefab.SetUnlock(TechType.ExosuitGrapplingArmFragment, 2 + extraFragmentsToScan);

            CloneTemplate cloneTemplate = new CloneTemplate(prefabInfo, TechType.ExosuitGrapplingArmModule);
            cloneTemplate.ModifyPrefab += (gameObject) => gameObject.AddComponent<GrapplingArmUpgraded>();
            cloneTemplate.ModifyPrefab += (gameObject) => gameObject.name = "UpgradedGrapplingArm";
            customPrefab.SetGameObject(cloneTemplate);
            customPrefab.SetEquipment(EquipmentType.ExosuitArm);
            customPrefab.Register();

            TechType = prefabInfo.TechType;
        }
    }
}
