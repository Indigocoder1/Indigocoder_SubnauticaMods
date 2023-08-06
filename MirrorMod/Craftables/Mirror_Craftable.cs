using Nautilus.Assets;
using Nautilus.Crafting;
using static CraftData;
using UnityEngine;
using Nautilus.Assets.Gadgets;
using System.IO;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Utility;
using static Nautilus.Assets.PrefabTemplates.FabricatorTemplate;
using static UWE.FreezeTime;
using System.Collections;
using System.Collections.Generic;
using MirrorMod.Monobehaviors;
using static ICSharpCode.SharpZipLib.Zip.ExtendedUnixData;

namespace MirrorMod.Craftables
{
    internal static class Mirror_Craftable
    {
        public static TechType techType;

        public static void Patch()
        {
            PrefabInfo prefabInfo = PrefabInfo.WithTechType("Mirror", "Mirror", "It's a Mirror")
                .WithIcon(SpriteManager.Get(TechType.PictureFrame));

            techType = prefabInfo.TechType;

            var prefab = new CustomPrefab(prefabInfo);

            RecipeData recipe = new RecipeData
            {
                craftAmount = 1,
                Ingredients =
                {
                    new Ingredient(TechType.CopperWire, 1),
                    new Ingredient(TechType.Glass, 4),
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
            var myAssetBundle = AssetBundle.LoadFromFile(Path.Combine(Main_Plugin.AssetsFolderPath, "mirrorassetbundle"));
            GameObject mirrorPrefab = myAssetBundle.LoadAsset<GameObject>("Mirror");
            PrefabUtils.AddBasicComponents(mirrorPrefab, "modded_mirror", techType, LargeWorldEntity.CellLevel.Near);

            float scale = 0.6f;
            mirrorPrefab.transform.localScale = new Vector3(scale, scale, scale);

            Transform mirrorCamera = mirrorPrefab.transform.Find("MirrorCameraHolder/MirrorCamera");

            DisableWhenNotInView disable = mirrorPrefab.GetComponent<DisableWhenNotInView>();
            disable.mirrorCamera = mirrorCamera.GetComponent<Camera>();

            Transform outlineParent = mirrorPrefab.transform.Find("IgnoreOnOtherMirrors/MirrorOutline");
            MaterialUtils.ApplySNShaders(outlineParent.gameObject,
                specularIntensity: 1.5f,
                shininess: 5
                );

            mirrorCamera.gameObject.AddComponent<CameraComponentCopier>();

            GameObject model = mirrorPrefab.transform.Find("IgnoreOnOtherMirrors").gameObject;

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

            return mirrorPrefab;
        }
    }
}
