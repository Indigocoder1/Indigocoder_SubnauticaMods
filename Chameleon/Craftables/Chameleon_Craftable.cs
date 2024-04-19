using Nautilus.Assets;
using UnityEngine;
using Nautilus.Assets.Gadgets;
using System.IO;
using System.Collections;
using Chameleon.Monobehaviors.Cyclops;
using Nautilus.Utility;
using Chameleon.Interfaces;

namespace Chameleon.Craftables
{
    internal static class Chameleon_Craftable
    {
        public static TechType techType;

        public static void Patch()
        {
            PrefabInfo prefabInfo = PrefabInfo.WithTechType("Chameleon", "Chameleon Sub", "An advanced sub that utilizes cloaking technology for enhanced stealth")
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
            GameObject model = Main_Plugin.assetBundle.LoadAsset<GameObject>("ChameleonSub");

            model.SetActive(false);
            GameObject chameleon = GameObject.Instantiate(model);

            yield return new WaitUntil(() => MaterialUtils.IsReady);

            //Apply shaders first so you don't mess with the instantiated stuff
            MaterialUtils.ApplySNShaders(chameleon, shininess: 1f);

            yield return CyclopsReferenceManager.EnsureCyclopsReference();

            foreach (ICyclopsReferencer referencer in chameleon.GetComponentsInChildren<ICyclopsReferencer>())
            {
                referencer.OnCyclopsReferenceFinished(CyclopsReferenceManager.CyclopsReference);
            }

            chameleon.transform.Find("Model/Exterior/Sub_Canopy").GetComponent<MeshRenderer>().material.color = new Color(1f, 1f, 1f, 0.3f);

            PrefabUtils.AddBasicComponents(chameleon, "chameleon", techType, LargeWorldEntity.CellLevel.Global);

            prefabOut.Set(chameleon);
        }
    }
}
