using Nautilus.Assets;
using UnityEngine;
using Nautilus.Assets.Gadgets;
using System.IO;
using System.Collections;
using Chameleon.Monobehaviors.Cyclops;
using Nautilus.Utility;
using Chameleon.Interfaces;
using Chameleon.Monobehaviors.Abstract;

namespace Chameleon.Craftables
{
    internal static class Chameleon_Craftable
    {
        public static TechType techType;

        public static void Patch()
        {
            Texture2D sprite = Main_Plugin.AssetBundle.LoadAsset<Texture2D>("ChameleonIcon");

            PrefabInfo prefabInfo = PrefabInfo.WithTechType("Chameleon", null, null, "English")
                .WithIcon(new Atlas.Sprite(sprite));

            techType = prefabInfo.TechType;

            var prefab = new CustomPrefab(prefabInfo);

            prefab.RemoveFromCache();
            prefab.SetGameObject(GetSubPrefab);
            prefab.SetUnlock(TechType.PictureFrame);

            prefab.SetRecipeFromJson(Path.Combine(Main_Plugin.RecipesFolderPath, "Chameleon.json"))
                .WithFabricatorType(CraftTree.Type.Constructor)
                .WithStepsToFabricatorTab("Vehicles")
                .WithCraftingTime(20f);

            prefab.SetUnlock(TechType.Constructor).WithAnalysisTech(null); //ADD FRAGMENTS LATER
            prefab.SetPdaGroupCategory(TechGroup.Constructor, TechCategory.Constructor);

            prefab.Register();
        }

        private static IEnumerator GetSubPrefab(IOut<GameObject> prefabOut)
        {
            GameObject model = Main_Plugin.AssetBundle.LoadAsset<GameObject>("ChameleonSub");

            model.SetActive(false);
            GameObject chameleon = GameObject.Instantiate(model);

            yield return new WaitUntil(() => MaterialUtils.IsReady);

            //Apply shaders first so you don't mess with the instantiated stuff
            MaterialUtils.ApplySNShaders(chameleon, shininess: 1f);

            MaterialUtils.ApplySNShaders(chameleon.transform.Find("Model/Interior/Int_Interior").gameObject, shininess: 0f, specularIntensity: 0f);
            Material floorMat = chameleon.transform.Find("Model/Interior/Int_Interior").GetComponent<Renderer>().materials[1];
            floorMat.EnableKeyword("MARMO_SPECMAP");

            yield return CyclopsReferenceManager.EnsureCyclopsReference();

            foreach (ICyclopsReferencer referencer in chameleon.GetComponentsInChildren<ICyclopsReferencer>())
            {
                referencer.OnCyclopsReferenceFinished(CyclopsReferenceManager.CyclopsReference);
            }

            foreach (PrefabModifier modifier in chameleon.GetComponentsInChildren<PrefabModifier>())
            {
                modifier.OnAsyncPrefabTasksCompleted();
                modifier.OnLateMaterialOperation();
            }

            chameleon.transform.Find("Model/Exterior/Sub_Canopy").GetComponent<MeshRenderer>().material.color = new Color(1f, 1f, 1f, 0.3f);
            chameleon.GetComponent<PingInstance>().pingType = Main_Plugin.ChameleonPingType;

            PrefabUtils.AddBasicComponents(chameleon, "chameleon", techType, LargeWorldEntity.CellLevel.Global);

            chameleon.SetActive(true);
            prefabOut.Set(chameleon);
        }
    }
}
