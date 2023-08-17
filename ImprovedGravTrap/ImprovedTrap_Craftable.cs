using Nautilus.Assets.PrefabTemplates;
using Nautilus.Assets;
using Nautilus.Crafting;
using Ingredient = CraftData.Ingredient;
using Nautilus.Assets.Gadgets;
using UnityEngine;
using IndigocoderLib;

namespace ImprovedGravTrap
{
    internal static class ImprovedTrap_Craftable
    {
        public static TechType techType;

        public static void Patch()
        {
            Atlas.Sprite sprite = ImageHelper.GetSpriteFromAssetsFolder("EnhancedGravTrap.png");

            PrefabInfo prefabInfo = PrefabInfo.WithTechType("EnhancedGravSphere", "Enhanced Grav Trap", "It's a better grav trap!")
                .WithIcon(sprite)
                .WithSizeInInventory(new Vector2int(2, 2));

            techType = prefabInfo.TechType;

            var prefab = new CustomPrefab(prefabInfo);

            var cloneTemplate = new CloneTemplate(prefabInfo, TechType.Gravsphere);

            cloneTemplate.ModifyPrefab += (gameObject) =>
            {
                Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();
                foreach (Renderer rend in renderers)
                {
                    rend.material.color = new Color(55 / 255f, 178 / 255f, 212 / 255f);
                }
                gameObject.EnsureComponent<EnhancedGravSphere>();
            };

            RecipeData recipe = new RecipeData
            {
                craftAmount = 1,
                Ingredients =
                {
                    new Ingredient(TechType.Gravsphere, 1),
                    new Ingredient(TechType.Polyaniline, 2),
                    new Ingredient(TechType.AdvancedWiringKit, 1),
                    new Ingredient(TechType.ComputerChip, 1),
                },
            };

            prefab.SetGameObject(cloneTemplate);
            prefab.SetUnlock(TechType.Gravsphere);
            prefab.SetEquipment(EquipmentType.Hand);

            prefab.SetRecipe(recipe)
                .WithFabricatorType(CraftTree.Type.Workbench)
                .WithCraftingTime(5f);

            prefab.SetPdaGroupCategory(TechGroup.Workbench, TechCategory.Workbench);

            prefab.Register();
        }
    }
}
