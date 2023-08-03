using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Crafting;
using UnityEngine;
using Ingredient = CraftData.Ingredient;
using IndigocoderLib;
using Nautilus.Handlers;

namespace WarpStabilizationSuit.Items
{
    internal static class Suit_Craftable
    {
        public static Color WarpColor = new Color(176 / 255f, 99 / 255f, 213 / 255f);

        public static TechType techType { get; private set; }

        public static void Patch()
        {
            Atlas.Sprite sprite = SpriteHelper.GetSpriteFromAssetsFolder("warpStabilizationSuit.png");

            PrefabInfo prefabInfo = PrefabInfo.WithTechType("WarpStabilizationSuit", "Warp Stabilization Suit", "Protects you from being displaced by teleportation technology")
                .WithIcon(sprite)
                .WithSizeInInventory(new Vector2int(2, 2));

            techType = prefabInfo.TechType;

            var prefab = new CustomPrefab(prefabInfo);

            var cloneTemplate = new CloneTemplate(prefabInfo, TechType.ReinforcedDiveSuit);
            cloneTemplate.ModifyPrefab += gameObject =>
            {
                var renderer = gameObject.GetComponentInChildren<Renderer>();

                renderer.materials[0].color = WarpColor;
                renderer.materials[1].color = WarpColor;
            };

            RecipeData recipe = new RecipeData
            {
                craftAmount = 1,
                Ingredients =
                {
                    new Ingredient(TechType.ReinforcedDiveSuit, 1),
                    new Ingredient(TechType.ReinforcedGloves, 1),
                    new Ingredient(TechType.Polyaniline, 2),
                    new Ingredient(TechType.AdvancedWiringKit, 1),
                    new Ingredient(TechType.ComputerChip, 1),
                    new Ingredient(TechType.PrecursorIonCrystal, 2)
                },
                LinkedItems =
                {
                    Gloves_Craftable.techType
                }
            };

            prefab.SetGameObject(cloneTemplate);
            prefab.SetUnlock(TechType.PrecursorPrisonIonGenerator);
            prefab.SetEquipment(EquipmentType.Body);

            prefab.SetRecipe(recipe)
                .WithFabricatorType(CraftTree.Type.Workbench)
                .WithCraftingTime(6f);

            prefab.Register();
        }
    }
}
