using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Crafting;
using System.Collections.Generic;

namespace SealGrapplingTurret.Items
{
    internal static class GrappleModule
    {
        public static TechType techType;

        public static void Patch()
        {
            PrefabInfo prefabInfo = PrefabInfo.WithTechType("SealGrappleModule", "Seal Grapple Module", "Allows you to use a grappling hook from the Seal sub")
                .WithIcon(SpriteManager.Get(TechType.ExosuitGrapplingArmModule));

            techType = prefabInfo.TechType;

            var prefab = new CustomPrefab(prefabInfo);
            var cloneTemplate = new CloneTemplate(prefabInfo, TechType.ExosuitGrapplingArmModule);

            RecipeData recipe = new RecipeData(new List<Ingredient>
            {
                new Ingredient(TechType.ExosuitGrapplingArmModule, 1)
            });

            prefab.SetGameObject(cloneTemplate);
            prefab.SetUnlock(TechType.ExosuitGrapplingArmModule);
            Main_Plugin.logger.LogInfo(Main_Plugin.recipeFilePath);
            prefab.SetRecipeFromJson(Main_Plugin.recipeFilePath);
            prefab.SetEquipment(Main_Plugin.sealModuleEquipmentType);

            prefab.SetPdaGroupCategory(TechGroup.VehicleUpgrades, TechCategory.VehicleUpgrades);
            prefab.Register();
        }
    }
}
