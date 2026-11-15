using System.Collections;
using System.Collections.Generic;
using System.IO;
using MirrorMod.Monobehaviors;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Crafting;
using Nautilus.Json.Converters;
using Nautilus.Utility;
using Newtonsoft.Json;
using UnityEngine;

namespace MirrorMod.Craftables
{
    internal static class Basic_Mirror
    {
        public static void Patch(string classID, string name, Sprite sprite, GameObject assetBundlePrefab)
        {
            PrefabInfo prefabInfo = PrefabInfo.WithTechType(classID, name, "It's a Mirror")
                .WithIcon(sprite);

            var prefab = new CustomPrefab(prefabInfo);

            IEnumerator GetGameObject(IOut<GameObject> prefabOut)
            {
                yield return GetMirrorPrefab(prefabInfo, assetBundlePrefab, prefabOut);
            }
            
            prefab.SetGameObject(GetGameObject);
            prefab.SetUnlock(TechType.PictureFrame);

            prefab.SetRecipeFromJson(Path.Combine(Main_Plugin.RecipesFolderPath, $"{classID}.json"));

            prefab.SetPdaGroupCategory(TechGroup.Miscellaneous, TechCategory.Misc);

            prefab.Register();
        }

        private static RecipeData GetRecipeFromJson(string path)
        {
            var content = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<RecipeData>(content, new CustomEnumConverter());
        }
        
        private static IEnumerator GetMirrorPrefab(PrefabInfo prefabInfo, GameObject assetPrefab, IOut<GameObject> prefab)
        {
            yield return new WaitUntil(() => MaterialUtils.IsReady);
            
            var prefabInstance = UWE.Utils.InstantiateDeactivated(assetPrefab);
            
            PrefabUtils.AddBasicComponents(prefabInstance, prefabInfo.ClassID, prefabInfo.TechType, LargeWorldEntity.CellLevel.Near);

            float scale = 0.6f;
            prefabInstance.transform.localScale = new Vector3(scale, scale, scale);

            Transform mirrorCamera = prefabInstance.transform.Find("MirrorCamera");

            Transform outlineParent = prefabInstance.transform.Find("IgnoreOnOtherMirrors/MirrorOutline");
            if(outlineParent)
            {
                MaterialUtils.ApplySNShaders(outlineParent.gameObject,
                specularIntensity: 0.25f,
                shininess: 5
                );
            }
            
            Transform renderPlane = prefabInstance.transform.Find("IgnoreOnOtherMirrors/RenderPlane");
            if(renderPlane)
            {
                MaterialUtils.ApplySNShaders(renderPlane.gameObject,
                specularIntensity: 0f,
                shininess: 2
                );
            }

            Transform handlesParent = prefabInstance.transform.Find("IgnoreOnOtherMirrors/Handles");
            if(handlesParent)
            {
                MaterialUtils.ApplySNShaders(handlesParent.gameObject,
                specularIntensity: 0.25f,
                shininess: 5
                );
            }

            mirrorCamera.gameObject.EnsureComponent<CameraComponentCopier>();

            GameObject model = prefabInstance.transform.Find("IgnoreOnOtherMirrors").gameObject;

            //Constructable
            Constructable constructable = prefabInstance.EnsureComponent<Constructable>();
            constructable.techType = prefabInfo.TechType;
            constructable.model = model;
            constructable.allowedOnWall = true;
            constructable.allowedOnGround = false;
            constructable.allowedInBase = true;
            constructable.allowedInSub = true;
            constructable.allowedUnderwater = true;

            constructable.ghostMaterial = MaterialUtils.GhostMaterial;

            ConstructableFlags flags = ConstructableFlags.Wall | ConstructableFlags.Base | ConstructableFlags.Inside |
            ConstructableFlags.Submarine;

            PrefabUtils.AddConstructable(prefabInstance, prefabInfo.TechType, flags, model);

            //Constructable Bounds
            ConstructableBounds constructableBounds = prefabInstance.EnsureComponent<ConstructableBounds>();
            Bounds bounds = new Bounds();
            OrientedBounds oBounds = new OrientedBounds();
            Renderer[] renderers = prefabInstance.GetComponentsInChildren<Renderer>();
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

            prefab.Set(prefabInstance);
        }
    }
}
