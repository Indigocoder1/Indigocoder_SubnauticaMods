using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Crafting;
using UnityEngine;
using Ingredient = CraftData.Ingredient;
using IndigocoderLib;
using System.Collections.Generic;
using SuitLib;
using Nautilus.Extensions;

namespace WarpStabilizationSuit.Items
{
    internal static class Suit_Craftable
    {
        public static Color WarpColor = new Color(176 / 255f, 99 / 255f, 213 / 255f);
        public static TechType techType { get; private set; }

        public static List<Ingredient> Ingredients
        {
            get
            {
                bool usingHardRecipe = Main_Plugin.UseHardRecipe.Value;
                return new List<Ingredient>
                {
                    new Ingredient(TechType.ReinforcedDiveSuit, 1),
                            new Ingredient(TechType.ReinforcedGloves, 1),
                            new Ingredient(usingHardRecipe ? TechType.AramidFibers : TechType.Polyaniline, usingHardRecipe ? 4 : 2),
                            new Ingredient(usingHardRecipe ? TechType.Lithium : TechType.AdvancedWiringKit, usingHardRecipe ? 4 : 1),
                            new Ingredient(usingHardRecipe ? TechType.AdvancedWiringKit : TechType.Nickel, 2),
                            new Ingredient(TechType.PrecursorIonCrystal, 2)
                };
            }
            private set
            {

            }
        }

        public static void Patch()
        {
            Atlas.Sprite sprite = ImageHelper.GetSpriteFromAssetsFolder("warpStabilizationSuit.png");

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

            bool usingHardRecipe = Main_Plugin.UseHardRecipe.Value;
            RecipeData recipe = new RecipeData()
            {
                craftAmount = 1,
                Ingredients = Ingredients,
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

            if (Main_Plugin.TabsNeeded)
            {
                prefab.SetRecipe(recipe)
                    .WithFabricatorType(CraftTree.Type.Workbench)
                    .WithStepsToFabricatorTab("Other")
                    .WithCraftingTime(6);
            }

            prefab.SetPdaGroupCategory(TechGroup.Workbench, TechCategory.Workbench);

            prefab.Register();
        }
    }
}
