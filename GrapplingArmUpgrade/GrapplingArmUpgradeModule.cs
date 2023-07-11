using System;
using UnityEngine;
using System.Collections.Generic;
using Nautilus.Crafting;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using BepInEx.Logging;
using Nautilus.Handlers;
using Nautilus.Assets.PrefabTemplates;

namespace GrapplingArmUpgrade_BepInEx
{
    internal class GrapplingArmUpgradeModule
    {
        public static TechType TechType { get; private set; }

        public static void RegisterModule()
        {
            Main_Plugin.logger.Log(LogLevel.Info, "Registering module");

            var prefabInfo = PrefabInfo.WithTechType("GrapplingArmUpgradeModule", "Prawn suit upgraded grappling arm", "With this upgrade module all grappling arms on your Prawn will have enhance capabilities!")
                .WithIcon(SpriteManager.Get(TechType.ExosuitGrapplingArmModule));

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

            CraftDataHandler.SetBackgroundType(prefabInfo.TechType, CraftData.BackgroundType.Normal);

            int extraFragmentsToScan = 2;
            customPrefab.SetUnlock(TechType.ExosuitGrapplingArmFragment, 2 + extraFragmentsToScan);

            CloneTemplate cloneTemplate = new CloneTemplate(prefabInfo, TechType.ExosuitGrapplingArmModule);
            customPrefab.SetGameObject(cloneTemplate);
            customPrefab.SetEquipment(EquipmentType.ExosuitModule);
            customPrefab.Register();

            Main_Plugin.logger.Log(LogLevel.Info, $"Prefab module type = {customPrefab.GetType()}") ;

            TechType = prefabInfo.TechType;
        }
    }
}
