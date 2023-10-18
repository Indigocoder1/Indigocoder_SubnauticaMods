using MirrorMod.Monobehaviors;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Crafting;
using Nautilus.Utility;
using UnityEngine;
using Ingredient = CraftData.Ingredient;

namespace MirrorMod.Craftables
{
    internal static class Basic_Mirror
    {
        public static TechType techType;

        public static void Patch(string classID, string name, Atlas.Sprite sprite, GameObject assetBundlePrefab)
        {
            PrefabInfo prefabInfo = PrefabInfo.WithTechType(classID, name, "It's a Mirror")
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

            prefab.SetGameObject(GetMirrorPrefab(classID, assetBundlePrefab));
            prefab.SetUnlock(TechType.PictureFrame);

            prefab.SetRecipe(recipe)
                .WithCraftingTime(6f);

            prefab.SetPdaGroupCategory(TechGroup.Miscellaneous, TechCategory.Misc);

            prefab.Register();
        }

        private static GameObject GetMirrorPrefab(string classID, GameObject mirrorPrefab)
        {
            PrefabUtils.AddBasicComponents(mirrorPrefab, classID, techType, LargeWorldEntity.CellLevel.Near);

            float scale = 0.6f;
            mirrorPrefab.transform.localScale = new Vector3(scale, scale, scale);

            Transform mirrorCamera = mirrorPrefab.transform.Find("MirrorCamera");

            Transform outlineParent = mirrorPrefab.transform.Find("IgnoreOnOtherMirrors/MirrorOutline");
            if(outlineParent)
            {
                MaterialUtils.ApplySNShaders(outlineParent.gameObject,
                specularIntensity: 0.25f,
                shininess: 5
                );
            }
            
            Transform renderPlane = mirrorPrefab.transform.Find("IgnoreOnOtherMirrors/RenderPlane");
            if(renderPlane)
            {
                MaterialUtils.ApplySNShaders(renderPlane.gameObject,
                specularIntensity: 0f,
                shininess: 2
                );
            }

            Transform handlesParent = mirrorPrefab.transform.Find("IgnoreOnOtherMirrors/Handles");
            if(handlesParent)
            {
                MaterialUtils.ApplySNShaders(handlesParent.gameObject,
                specularIntensity: 0.25f,
                shininess: 5
                );
            }

            mirrorCamera.gameObject.EnsureComponent<CameraComponentCopier>();

            GameObject model = mirrorPrefab.transform.Find("IgnoreOnOtherMirrors").gameObject;

            //Constructable
            Constructable constructable = mirrorPrefab.EnsureComponent<Constructable>();
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
            ConstructableBounds constructableBounds = mirrorPrefab.EnsureComponent<ConstructableBounds>();
            Bounds bounds = new Bounds();
            OrientedBounds oBounds = new OrientedBounds();
            Renderer[] renderers = mirrorPrefab.GetComponentsInChildren<Renderer>();
            int index = 0;
            foreach (Renderer renderer in renderers)
            {
                if (index == 0)
                {
                    bounds = renderer.bounds;
                    index++;
                    continue;
                }
                bounds.Encapsulate(renderer.bounds);
                index++;
            }
            oBounds.position = model.transform.position;
            oBounds.rotation = model.transform.rotation;
            oBounds.extents = bounds.extents;
            oBounds.size = bounds.size;
            constructableBounds.bounds = oBounds;

            return mirrorPrefab;
        }
    }
}
