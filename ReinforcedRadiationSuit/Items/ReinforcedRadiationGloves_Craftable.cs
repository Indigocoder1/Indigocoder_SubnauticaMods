using Nautilus.Assets.PrefabTemplates;
using Nautilus.Assets;
using UnityEngine;
using Nautilus.Assets.Gadgets;
using IndigocoderLib;

namespace ReinforcedRadiationSuit.Items
{
    internal static class ReinforcedRadiationGloves_Craftable
    {
        public static TechType techType { get; private set; }

        public static void Patch()
        {
            Atlas.Sprite sprite = ImageHelper.GetSpriteFromAssetsFolder("reinforcedRadiation_GlovesIcon.png", Main_Plugin.AssetsFolderPath);

            PrefabInfo prefabInfo = PrefabInfo.WithTechType("ReinforcedRadiationGloves", "Reinforced Radiation Gloves",
                "Lead and-ruby-lined gloves to protect against radiation and physical trauma")
                .WithIcon(sprite)
                .WithSizeInInventory(new Vector2int(2, 2));

            techType = prefabInfo.TechType;

            var prefab = new CustomPrefab(prefabInfo);

            var cloneTemplate = new CloneTemplate(prefabInfo, TechType.RadiationGloves);
            cloneTemplate.ModifyPrefab += gameObject =>
            {
                var renderer = gameObject.GetComponentInChildren<Renderer>();

            };

            prefab.SetGameObject(cloneTemplate);
            prefab.SetEquipment(EquipmentType.Gloves);

            prefab.SetPdaGroupCategory(TechGroup.Workbench, TechCategory.Workbench);

            prefab.Register();
        }
    }
}
