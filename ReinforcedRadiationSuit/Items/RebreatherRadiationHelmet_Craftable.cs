using Nautilus.Assets.PrefabTemplates;
using Nautilus.Assets;
using UnityEngine;
using Nautilus.Assets.Gadgets;
using IndigocoderLib;

namespace ReinforcedRadiationSuit.Items
{
    internal static class RebreatherRadiationHelmet_Craftable
    {
        public static TechType techType { get; private set; }

        public static void Patch()
        {
            Atlas.Sprite sprite = ImageHelper.GetSpriteFromAssetsFolder("reinforcedRadiation_HelmetIcon.png", Main_Plugin.AssetsFolderPath);

            PrefabInfo prefabInfo = PrefabInfo.WithTechType("RebreatherRadiationHelmet", "Rebreather Radiation Helmet",
                "Lead lined helmet to protect against radiation with CO₂ filters to enhance diving time at deep depths")
                .WithIcon(sprite)
                .WithSizeInInventory(new Vector2int(2, 2));

            techType = prefabInfo.TechType;

            var prefab = new CustomPrefab(prefabInfo);

            var cloneTemplate = new CloneTemplate(prefabInfo, TechType.Rebreather);
            cloneTemplate.ModifyPrefab += gameObject =>
            {
                var renderer = gameObject.GetComponentInChildren<Renderer>();

            };

            prefab.SetGameObject(cloneTemplate);
            prefab.SetEquipment(EquipmentType.Head);

            prefab.SetPdaGroupCategory(TechGroup.Workbench, TechCategory.Workbench);

            prefab.Register();
        }
    }
}
