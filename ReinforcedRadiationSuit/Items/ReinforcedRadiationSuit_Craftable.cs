using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using UnityEngine;
using IndigocoderLib;

namespace ReinforcedRadiationSuit.Items
{
    internal static class ReinforcedRadiationSuit_Craftable
    {
        public static TechType techType { get; private set; }

        public static void Patch()
        {
            Atlas.Sprite sprite = ImageHelper.GetSpriteFromAssetsFolder("reinforcedRadiation_SuitIcon.png", Main_Plugin.AssetsFolderPath);

            PrefabInfo prefabInfo = PrefabInfo.WithTechType("ReinforcedRadiationSuit", "Reinforced Radiation Suit", 
                "Lead and-ruby-lined suit to protect against radiation and physical trauma")
                .WithIcon(sprite)
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
                .WithCraftingTime(6f);

            prefab.SetPdaGroupCategory(TechGroup.Workbench, TechCategory.Workbench);

            prefab.Register();
        }
    }
}
