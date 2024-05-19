using Nautilus.Assets;
using UnityEngine;
using Nautilus.Assets.Gadgets;
using System.IO;
using System.Collections;
using Chameleon.Monobehaviors.Cyclops;
using Nautilus.Utility;
using Chameleon.Interfaces;
using Chameleon.Monobehaviors.Abstract;
using Chameleon.Prefabs;
using Nautilus.Handlers;

namespace Chameleon.Craftables
{
    internal static class Chameleon_Craftable
    {
        public static PrefabInfo PrefabInfo { get; private set; }

        public static void Register()
        {
            Texture2D sprite = Main_Plugin.AssetBundle.LoadAsset<Texture2D>("ChameleonIcon");

            PrefabInfo prefabInfo = PrefabInfo.WithTechType("Chameleon", null, null, "English")
                .WithIcon(new Atlas.Sprite(sprite));

            PrefabInfo = prefabInfo;

            var prefab = new CustomPrefab(prefabInfo);

            Sprite popupSprite = Main_Plugin.AssetBundle.LoadAsset<Sprite>("chameleonPopup");

            prefab.RemoveFromCache();
            prefab.SetGameObject(GetSubPrefab);
            prefab.SetUnlock(PrefabInfo.TechType).WithAnalysisTech(popupSprite, PDAHandler.UnlockImportant);

            prefab.SetRecipeFromJson(Path.Combine(Main_Plugin.RecipesFolderPath, "Chameleon.json"))
                .WithFabricatorType(CraftTree.Type.Constructor)
                .WithStepsToFabricatorTab("Vehicles")
                .WithCraftingTime(20f);

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

            GameObject normalInterior = chameleon.transform.Find("Model/Normal/Interior/Int_Interior").gameObject;
            Material floorMat = normalInterior.GetComponent<Renderer>().materials[1];
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

            chameleon.GetComponent<PingInstance>().pingType = Main_Plugin.ChameleonPingType;

            PrefabUtils.AddBasicComponents(chameleon, PrefabInfo.ClassID, PrefabInfo.TechType, LargeWorldEntity.CellLevel.Global);

            prefabOut.Set(chameleon);
        }
    }
}
