using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Crafting;
using UnityEngine;
using Ingredient = CraftData.Ingredient;
using IndigocoderLib;
using static VFXParticlesPool;
using Nautilus.Extensions;
using static GameObjectPoolPrefabMap;
using Nautilus.Utility;
using System.Collections;

namespace WarpStabilizationSuit
{
    internal static class Gloves_Craftable
    {
        public static TechType glovesTechType { get; private set; }

        public static void Patch()
        {
            Atlas.Sprite sprite = SpriteHelper.GetSpriteFromAssetsFolder("WarpStabilizationGloves.png");

            PrefabInfo prefabInfo = PrefabInfo.WithTechType("WarpStabilizationGloves", "Warp Stabilization Gloves", "Protects you from being displaced by teleportation technology. Works with the Warp Stabilization Suit")
                .WithIcon(sprite)
                .WithSizeInInventory(new Vector2int(2, 2));

            glovesTechType = prefabInfo.TechType;

            var prefab = new CustomPrefab(prefabInfo);

            Texture armsDiffuse = ImageUtils.LoadTextureFromFile(Main_Plugin.AssetsFolderPath + "/Textures/player_02_reinforced_suit_01_arms_WARP.png");
            Texture armsSpecular = ImageUtils.LoadTextureFromFile(Main_Plugin.AssetsFolderPath + "/Textures/player_02_reinforced_suit_01_arms_spec_WARP.png");

            var cloneTemplate = new CloneTemplate(prefabInfo, TechType.ReinforcedGloves);
            cloneTemplate.ModifyPrefab += gameObject =>
            {
                Renderer renderer = gameObject.GetComponentInChildren<Renderer>();
                renderer.material.SetTexture("_MainTex", armsDiffuse);
                renderer.material.SetTexture(ShaderPropertyID._SpecTex, armsSpecular);
            };

            prefab.SetGameObject(cloneTemplate);
            prefab.SetUnlock(TechType.PrecursorPrisonIonGenerator);
            prefab.SetEquipment(EquipmentType.Gloves);

            prefab.Register();
        }
    }
}
