using IndigocoderLib;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Crafting;
using Nautilus.Handlers;
using System;
using System.Collections.Generic;
using Ingredient = CraftData.Ingredient;

namespace HullReinforcementFix.Craftables
{
    internal static class UpgradedHullReinfocement
    {
        public static TechType mk2TechType { get; private set; }
        public static TechType mk3TechType { get; private set; }

        public static void Patch(int markNumber)
        {
            Atlas.Sprite sprite = ImageHelper.GetSpriteFromAssetsFolder($"hullReinforcementMK{markNumber}.png");

            PrefabInfo prefabInfo = PrefabInfo.WithTechType($"HullReinforcementMK{markNumber}", $"Hull Reinforcement MK{markNumber}",
                "Gives even further damage reduction")
                .WithIcon(sprite);

            if(markNumber == 2)
            {
                mk2TechType = prefabInfo.TechType;
            }
            else
            {
                mk3TechType = prefabInfo.TechType;
            }

            var prefab = new CustomPrefab(prefabInfo);

            var cloneTemplate = new CloneTemplate(prefabInfo, TechType.VehicleArmorPlating);

            TechType previousTechType = TechType.None;
            if(markNumber <= 2)
            {
                previousTechType = TechType.VehicleArmorPlating;
            }
            else
            {
                previousTechType = (TechType)Enum.Parse(typeof(TechType), $"HullReinforcementMK{markNumber - 1}"); 
            }

            bool isMark2 = markNumber == 2;

            RecipeData recipe = new RecipeData()
            {
                Ingredients = new List<Ingredient>()
                {
                    new Ingredient(previousTechType, 1),
                    new Ingredient(isMark2 ? TechType.Nickel : TechType.Kyanite, isMark2 ? 2 : 1),
                    new Ingredient(isMark2 ? TechType.AluminumOxide : TechType.EnameledGlass, 3),
                    new Ingredient(isMark2 ? TechType.Lithium : TechType.Diamond, 2)
                },
                craftAmount = 1
            };

            prefab.SetGameObject(cloneTemplate);
            prefab.SetUnlock(TechType.CyclopsHullModule1);
            prefab.SetEquipment(EquipmentType.VehicleModule).WithQuickSlotType(QuickSlotType.Passive);
                  
            prefab.SetRecipe(recipe)
                .WithFabricatorType(CraftTree.Type.SeamothUpgrades)
                .WithStepsToFabricatorTab("CommonModules")
                .WithCraftingTime(5f);

            prefab.Register();

            CraftDataHandler.RemoveFromGroup(TechGroup.Resources, TechCategory.BasicMaterials, prefabInfo.TechType);
            CraftDataHandler.AddToGroup(TechGroup.VehicleUpgrades, TechCategory.VehicleUpgrades, prefabInfo.TechType);
        }
    }
}
