using Nautilus.Assets.PrefabTemplates;
using Nautilus.Assets;
using Nautilus.Crafting;
using Nautilus.Handlers;
using System.Collections.Generic;
using Nautilus.Assets.Gadgets;
using GrapplingArmUpgrade_BepInEx.Monobehaviours;
using IndigocoderLib;
using static HandReticle;

namespace GrapplingArmUpgrade_BepInEx
{
    internal class UpgradedArm_Craftable
    {
        public static TechType TechType { get; private set; }

        public static void RegisterModule()
        {
            Atlas.Sprite sprite = ImageHelper.GetSpriteFromAssetsFolder("exosuitgrapplingarmmodule_Upgraded.png");
            var prefabInfo = PrefabInfo.WithTechType("GrapplingArmUpgradeModule", "Prawn suit upgraded grappling arm", 
                "With this upgrade module all grappling arms on your Prawn will have enhanced capabilities!")
                .WithIcon(sprite);

            TechType = prefabInfo.TechType;

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

            CraftDataHandler.SetBackgroundType(prefabInfo.TechType, CraftData.BackgroundType.ExosuitArm);
            customPrefab.SetUnlock(TechType.ExosuitGrapplingArmModule);

            CloneTemplate cloneTemplate = new CloneTemplate(prefabInfo, TechType.ExosuitGrapplingArmModule);
            cloneTemplate.ModifyPrefab += (gameObject) =>
            {
                gameObject.EnsureComponent<UpgradedArm_Identifier>();
            };

            customPrefab.SetGameObject(cloneTemplate);
            customPrefab.SetEquipment(EquipmentType.ExosuitArm).WithQuickSlotType(QuickSlotType.Selectable);
            customPrefab.Register();

            CraftDataHandler.RemoveFromGroup(TechGroup.Resources, TechCategory.BasicMaterials, TechType);
            CraftDataHandler.AddToGroup(TechGroup.Workbench, TechCategory.Workbench, TechType);
        }
    }
}
