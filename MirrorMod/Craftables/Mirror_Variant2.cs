﻿using Nautilus.Assets;
using Nautilus.Crafting;
using static CraftData;
using UnityEngine;
using Nautilus.Assets.Gadgets;
using System.IO;
using Nautilus.Utility;
using MirrorMod.Monobehaviors;
using IndigocoderLib;

namespace MirrorMod.Craftables
{
    internal static class Mirror_Variant2
    {
        public static TechType techType;

        public static void Patch()
        {
            Atlas.Sprite sprite = ImageHelper.GetSpriteFromAssetsFolder("MirrorVariant2.png");

            PrefabInfo prefabInfo = PrefabInfo.WithTechType("MirrorVariant2", "Mirror (Variant 2)", "It's a Mirror")
                .WithIcon(sprite);

            techType = prefabInfo.TechType;

            var prefab = new CustomPrefab(prefabInfo);

            RecipeData recipe = new RecipeData
            {
                craftAmount = 1,
                Ingredients =
                {
                    new Ingredient(TechType.Titanium, 1),
                    new Ingredient(TechType.Glass, 2),
                },
            };

            prefab.SetGameObject(InitializeMirrorPrefab());
            prefab.SetUnlock(TechType.PictureFrame);
            prefab.SetEquipment(EquipmentType.Body);

            prefab.SetRecipe(recipe)
                .WithCraftingTime(6f);

            prefab.SetPdaGroupCategory(TechGroup.Miscellaneous, TechCategory.Misc);

            prefab.Register();
        }

        private static GameObject InitializeMirrorPrefab()
        {
            GameObject mirrorPrefab = Main_Plugin.assetBundle.LoadAsset<GameObject>("Mirror_Variant2");

            PrefabUtils.AddBasicComponents(mirrorPrefab, "modded_mirror2", techType, LargeWorldEntity.CellLevel.Near);

            float scale = 0.6f;
            mirrorPrefab.transform.localScale = new Vector3(scale, scale, scale);

            Transform mirrorCamera = mirrorPrefab.transform.Find("MirrorCameraHolder/MirrorCamera");

            DisableWhenNotInView disable = mirrorPrefab.GetComponent<DisableWhenNotInView>();
            disable.mirrorCamera = mirrorCamera.GetComponent<Camera>();

            Transform outlineParent = mirrorPrefab.transform.Find("IgnoreOnOtherMirrors/MirrorOutline");
            MaterialUtils.ApplySNShaders(outlineParent.gameObject,
                specularIntensity: 0.25f,
                shininess: 5
                );

            Transform renderPlaneParent = mirrorPrefab.transform.Find("IgnoreOnOtherMirrors/TargetPlane");
            MaterialUtils.ApplySNShaders(renderPlaneParent.gameObject,
                specularIntensity: 0f,
                shininess: 2
                );

            mirrorCamera.gameObject.AddComponent<CameraComponentCopier>();

            GameObject model = mirrorPrefab.transform.Find("IgnoreOnOtherMirrors").gameObject;

            //Constructable
            Constructable constructable = mirrorPrefab.AddComponent<Constructable>();
            constructable.techType = techType;
            constructable.model = model;
            constructable.allowedOnWall = true;
            constructable.allowedOnGround = false;
            constructable.allowedInBase = true;
            constructable.allowedInSub = true;
            constructable.allowedUnderwater = true;

            constructable.ghostMaterial = MaterialUtils.GhostMaterial;

            ConstructableFlags flags = ConstructableFlags.Wall | ConstructableFlags.Base | ConstructableFlags.Inside |
            ConstructableFlags.Submarine;

            PrefabUtils.AddConstructable(mirrorPrefab, techType, flags, model);

            //Constructable Bounds
            Transform colliderParent = mirrorPrefab.transform.Find("IgnoreOnOtherMirrors/MirrorOutline");

            ConstructableBounds constructableBounds = mirrorPrefab.AddComponent<ConstructableBounds>();
            Bounds bounds = new Bounds(outlineParent.position, Vector3.one);
            OrientedBounds oBounds = new OrientedBounds();
            oBounds.position = outlineParent.position;
            oBounds.rotation = outlineParent.rotation;
            Renderer[] renderers = colliderParent.GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in renderers)
            {
                bounds.Encapsulate(renderer.bounds.extents);
            }
            oBounds.extents = bounds.extents;
            oBounds.size = bounds.size;
            constructableBounds.bounds = oBounds;

            return mirrorPrefab;
        }
    }
}
