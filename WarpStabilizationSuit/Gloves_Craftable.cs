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

            var cloneTemplate = new CloneTemplate(prefabInfo, TechType.ReinforcedGloves);
            cloneTemplate.ModifyPrefab += gameObject =>
            {
                /*
                var renderer = gameObject.GetComponentInChildren<Renderer>();
                //The dividing by 255 is needed to normalize the color values
                renderer.materials[0].color = new Color(176 / 255f, 99 / 255f, 213 / 255f);
                renderer.materials[1].color = new Color(176 / 255f, 99 / 255f, 213 / 255f);
                */
            };

            prefab.SetGameObject(cloneTemplate);
            prefab.SetUnlock(TechType.PrecursorPrisonIonGenerator);
            prefab.SetEquipment(EquipmentType.Gloves);

            prefab.Register();
        }
    }
}
