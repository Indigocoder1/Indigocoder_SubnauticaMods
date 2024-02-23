using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using UnityEngine;

namespace ReinforcedRadiationSuit.Items
{
    internal class ReinforcedRadiationSuit_Craftable
    {
        public static TechType techType { get; private set; }

        public static void Patch()
        {
            //Atlas.Sprite sprite = ImageHelper.GetSpriteFromAssetsFolder("warpStabilizationSuit.png");

            PrefabInfo prefabInfo = PrefabInfo.WithTechType("ReinforcedRadiationSuit", "Warp Stabilization Suit", "Protects you from being displaced by teleportation technology")
                .WithIcon(SpriteManager.Get(TechType.RadiationSuit))
                .WithSizeInInventory(new Vector2int(2, 2));

            techType = prefabInfo.TechType;

            var prefab = new CustomPrefab(prefabInfo);

            var cloneTemplate = new CloneTemplate(prefabInfo, TechType.RadiationSuit);
            cloneTemplate.ModifyPrefab += gameObject =>
            {
                var renderer = gameObject.GetComponentInChildren<Renderer>();

            };

            prefab.SetGameObject(cloneTemplate);
            prefab.SetUnlock(TechType.RadiationSuit);
            prefab.SetEquipment(EquipmentType.Body);

            prefab.SetRecipe(Main_Plugin.SuitRecipe)
                .WithFabricatorType(CraftTree.Type.Workbench)
                .WithStepsToFabricatorTab("ModdedWorkbench")
                .WithCraftingTime(6f);

            prefab.SetPdaGroupCategory(TechGroup.Workbench, TechCategory.Workbench);

            prefab.Register();
        }
    }
}
