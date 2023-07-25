using IndigocoderLib.SpriteHelper;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Crafting;
using Nautilus.Utility;
using System.IO;
using System.Reflection;
using UnityEngine;
using Ingredient = CraftData.Ingredient;

namespace WarpStabilizationSuit
{
    internal static class Suit_Craftable
    {
        public static TechType itemTechType { get; private set; }

        public static void Patch()
        {
            string spriteFilePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets") + "/warpStabilizationSuit.png";
            Atlas.Sprite sprite = ImageUtils.LoadSpriteFromFile(spriteFilePath);

            PrefabInfo prefabInfo = PrefabInfo.WithTechType("WarpStabilizationSuit", "Warp Stabilization Suit", "Protects you from being displaced by teleportation technology")
                .WithIcon(sprite)
                .WithSizeInInventory(new Vector2int(2, 2));

            itemTechType = prefabInfo.TechType;

            var prefab = new CustomPrefab(prefabInfo);

            var cloneTemplate = new CloneTemplate(prefabInfo, TechType.ReinforcedDiveSuit);
            cloneTemplate.ModifyPrefab += gameObject =>
            {
                var renderer = gameObject.GetComponentInChildren<Renderer>();
                renderer.materials[0].color = new Color(176 / 255f, 99 / 255f, 213 / 255f);
                renderer.materials[1].color = new Color(176 / 255f, 99 / 255f, 213 / 255f);
            };

            RecipeData recipe = new RecipeData
            {
                craftAmount = 1,
                Ingredients =
                {
                    new Ingredient(TechType.ReinforcedDiveSuit, 1),
                    new Ingredient(TechType.Polyaniline, 2),
                    new Ingredient(TechType.AdvancedWiringKit, 1),
                    new Ingredient(TechType.ComputerChip, 1),
                    new Ingredient(TechType.Aerogel, 2)
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
