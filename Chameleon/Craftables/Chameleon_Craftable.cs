using Nautilus.Assets;
using Nautilus.Crafting;
using static CraftData;
using UnityEngine;
using Nautilus.Assets.Gadgets;
using System.IO;
using System.Collections;
using Chameleon.Monobehaviors.Cyclops;
using System;

namespace Chameleon.Craftables
{
    internal static class Chameleon_Craftable
    {
        public static TechType techType;

        public static void Patch()
        {
            PrefabInfo prefabInfo = PrefabInfo.WithTechType("chameleon", "Chameleon Sub", "An advanced sub that utilizes cloaking technology for enhanced stealth")
                .WithIcon(SpriteManager.Get(TechType.PosterAurora));

            techType = prefabInfo.TechType;

            var prefab = new CustomPrefab(prefabInfo);

            prefab.SetGameObject(GetSubPrefab);
            prefab.SetUnlock(TechType.PictureFrame);

            prefab.SetRecipeFromJson(Path.Combine(Main_Plugin.RecipesFolderPath, "ChameleonRecipe.json"))
                .WithFabricatorType(CraftTree.Type.Constructor)
                .WithStepsToFabricatorTab("Vehicles")
                .WithCraftingTime(25f);

            prefab.SetUnlock(TechType.Constructor).WithAnalysisTech(null); //ADD FRAGMENTS LATER
            prefab.SetPdaGroupCategory(TechGroup.Constructor, TechCategory.Constructor);

            prefab.Register();
        }

        private static IEnumerator GetSubPrefab(IOut<GameObject> prefabOut)
        {
            Main_Plugin.logger.LogInfo("Retrieving sub prefab");

            GameObject model = Main_Plugin.assetBundle.LoadAsset<GameObject>("Chameleon");
            model.SetActive(false);
            GameObject chameleon = GameObject.Instantiate(model);

            yield return CyclopsReferenceManager.EnsureCyclopsReference();

            foreach (ICyclopsReferencer referencer in chameleon.GetComponentsInChildren<ICyclopsReferencer>())
            {
                referencer.OnCyclopsReferenceFinished(CyclopsReferenceManager.CyclopsReference);
            }

            prefabOut.Set(chameleon);
        }
    }
}
