using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Crafting;
using Nautilus.Handlers;
using Nautilus.Utility;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace UpgradedJumpJetModule
{
    internal static class UpgradedJetsModule
    {
        public static TechType techType { get; private set; }

        public static void RegisterModule()
        {
            Main_Plugin.logger.LogInfo("Registering module");

            string spriteFilePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets") + "/upgradedJumpJets.png";
            Atlas.Sprite sprite = ImageUtils.LoadSpriteFromFile(spriteFilePath);

            var prefabInfo = PrefabInfo.WithTechType("UpgradedJumpJetModule", "Upgraded Prawn jump jet module", "With this upgrade module you can jump even more! Effects do not stack")
                .WithIcon(sprite);

            techType = prefabInfo.TechType;

            var customPrefab = new CustomPrefab(prefabInfo);
            RecipeData recipe = new RecipeData()
            {
                craftAmount = 1,
                Ingredients = new List<CraftData.Ingredient>()
                {
                    new CraftData.Ingredient(TechType.ExosuitJetUpgradeModule),
                    new CraftData.Ingredient(TechType.Sulphur, 2),
                    new CraftData.Ingredient(TechType.Kyanite, 1),
                    new CraftData.Ingredient(TechType.Titanium, 2),
                }
            };

            customPrefab.SetRecipe(recipe)
                .WithFabricatorType(CraftTree.Type.Workbench)
                .WithCraftingTime(5f);

            if (Main_Plugin.TabsNeeded)
            {
                customPrefab.SetRecipe(recipe)
                    .WithFabricatorType(CraftTree.Type.Workbench)
                    .WithStepsToFabricatorTab("Other")
                    .WithCraftingTime(5f);
            }

            CraftDataHandler.SetBackgroundType(prefabInfo.TechType, CraftData.BackgroundType.Normal);

            customPrefab.SetUnlock(TechType.Exosuit);

            CloneTemplate cloneTemplate = new CloneTemplate(prefabInfo, TechType.ExosuitJetUpgradeModule);
            customPrefab.SetGameObject(cloneTemplate);
            customPrefab.SetEquipment(EquipmentType.ExosuitModule);


            customPrefab.Register();

            CraftDataHandler.RemoveFromGroup(TechGroup.Resources, TechCategory.BasicMaterials, techType);
            CraftDataHandler.AddToGroup(TechGroup.Workbench, TechCategory.Workbench, techType);
        }
    }
}
