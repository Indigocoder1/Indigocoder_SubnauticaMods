
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Crafting;
using Ingredient = CraftData.Ingredient;

namespace CyclopsBeaconDeployer.Items
{
    internal static class BeaconDeployModule
    {
        public static TechType techType;

        public static void Patch()
        {
            PrefabInfo prefabInfo = PrefabInfo.WithTechType("BeaconDeployModule", "Beacon Deploy Module", "Allows beacons to be deoployed " +
                "from the cyclops decoy tube")
                .WithIcon(SpriteManager.Get(TechType.CyclopsDecoyModule));

            techType = prefabInfo.TechType;

            var prefab = new CustomPrefab(prefabInfo);

            var cloneTemplate = new CloneTemplate(prefabInfo, TechType.CyclopsDecoyModule);

            RecipeData recipe = new RecipeData
            {
                craftAmount = 1,
                Ingredients =
                {
                    new Ingredient(TechType.Gravsphere, 1),
                    new Ingredient(TechType.Polyaniline, 2),
                    new Ingredient(TechType.AdvancedWiringKit, 2),
                    new Ingredient(TechType.ComputerChip, 1),
                },
            };

            prefab.SetGameObject(cloneTemplate);
            prefab.SetUnlock(TechType.CyclopsDecoyModule);
            prefab.SetEquipment(EquipmentType.CyclopsModule);

            prefab.SetRecipe(recipe)
                .WithFabricatorType(CraftTree.Type.CyclopsFabricator)
                .WithCraftingTime(5f);

            prefab.SetPdaGroupCategory(TechGroup.VehicleUpgrades, TechCategory.VehicleUpgrades);

            prefab.Register();
        }
    }
}
