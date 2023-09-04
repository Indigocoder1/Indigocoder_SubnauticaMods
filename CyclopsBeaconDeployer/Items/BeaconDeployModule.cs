using IndigocoderLib;
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
            Atlas.Sprite sprite = ImageHelper.GetSpriteFromAssetsFolder("DeployableBeaconModule.png");

            PrefabInfo prefabInfo = PrefabInfo.WithTechType("DeployableBeaconModule", "Deployable Beacon Module", "Allows beacons to be deployed " +
                "from the cyclops decoy tube")
                .WithIcon(sprite);

            techType = prefabInfo.TechType;

            var prefab = new CustomPrefab(prefabInfo);

            var cloneTemplate = new CloneTemplate(prefabInfo, TechType.CyclopsDecoyModule);

            RecipeData recipe = new RecipeData
            {
                craftAmount = 1,
                Ingredients =
                {
                    new Ingredient(TechType.Benzene, 1),
                    new Ingredient(TechType.Titanium, 2),
                    new Ingredient(TechType.Lithium, 2),
                    new Ingredient(TechType.ComputerChip, 1),
                },
            };

            prefab.SetGameObject(cloneTemplate);
            prefab.SetUnlock(TechType.CyclopsDecoyModule);
            prefab.SetEquipment(EquipmentType.CyclopsModule);

            prefab.SetRecipe(recipe)
                .WithFabricatorType(CraftTree.Type.CyclopsFabricator)
                .WithCraftingTime(5f);

            prefab.SetPdaGroupCategory(TechGroup.Cyclops, TechCategory.CyclopsUpgrades);

            prefab.Register();
        }
    }
}
