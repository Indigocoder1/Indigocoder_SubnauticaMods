using System.Collections.Generic;
using ModdedArmsHelper.API;
using SMLExpander;
using SMLHelper.V2.Crafting;
using System.Collections;

namespace GrapplingArmUpgrade_BepInEx
{
    internal class GrapplingArmUpgradeModule : CraftableModdedArm
    {
        public static TechType TechType { get; private set; }

        internal GrapplingArmUpgradeModule()
            : base(
                  techTypeName: "UpgradedGrapplingArm",
                  friendlyName: "Upgraded Prawn suit grappling arm",
                  description: "This upgraded grappling arm will enhance your swinging capabilities",
                  armType: ArmType.ExosuitArm,
                  armTemplate: ArmTemplate.GrapplingArm,
                  requiredForUnlock: TechType.Exosuit,
                  fragment: null
                  )
        { }

        protected override RegisterArmRequest RegisterArm()
        {
            TechType = GrapplingArmUpgradeModule.TechType;
            return new RegisterArmRequest(this, new GrapplingArmUpgrade_ModdingRequest());
        }

        protected override EncyData GetEncyclopediaData()
        {
            return null;
        }

        protected override void SetCustomLanguageText()
        {
            
        }

        protected override Atlas.Sprite GetItemSprite()
        {
            /*
            string spriteFilePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets") + "/exosuitgrapplingarmmodule_Upgraded.png";
            return ImageUtils.LoadSpriteFromFile(spriteFilePath);
            */
            return SpriteManager.Get(TechType.ExosuitGrapplingArmModule);
        }

        protected override TechData GetRecipe()
        {
            return new TechData
            {
                craftAmount = 1,
                Ingredients = new List<Ingredient>(new Ingredient[5])
                {
                    new Ingredient(TechType.ExosuitGrapplingArmModule, 1),
                    new Ingredient(TechType.Polyaniline, 2),
                    new Ingredient(TechType.Lithium, 2),
                    new Ingredient(TechType.AramidFibers, 1),
                    new Ingredient(TechType.AluminumOxide, 1)
                }
            };
        }

        protected override IEnumerator ModifyGameObjectAsync(IOut<bool> success)
        {
            success.Set(true);
            yield break;
        }
    }

    #region Legacy Code
    /*
             public static TechType TechType { get; private set; }

        public static void RegisterModule()
        {
            Main_Plugin.logger.Log(LogLevel.Info, "Registering module");

            string spriteFilePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets") + "/exosuitgrapplingarmmodule_Upgraded.png";
            var prefabInfo = PrefabInfo.WithTechType("GrapplingArmUpgradeModule", "Prawn suit grappling arm upgrade module", "With this upgrade module all grappling arms on your Prawn will have enhanced capabilities!")
                .WithIcon(ImageUtils.LoadSpriteFromFile(spriteFilePath));

            var customPrefab = new CustomPrefab(prefabInfo);
            customPrefab.SetRecipe(new RecipeData()
            {
                craftAmount = 1,
                Ingredients = new List<CraftData.Ingredient>()
                {
                    new CraftData.Ingredient(TechType.ExosuitGrapplingArmModule),
                    new CraftData.Ingredient(TechType.Polyaniline, 2),
                    new CraftData.Ingredient(TechType.Lithium, 2),
                    new CraftData.Ingredient(TechType.AramidFibers),
                    new CraftData.Ingredient(TechType.AluminumOxide),
                }
            })
                .WithFabricatorType(CraftTree.Type.Workbench)
                .WithCraftingTime(5f);

            CraftDataHandler.SetBackgroundType(prefabInfo.TechType, CraftData.BackgroundType.Normal);

            int extraFragmentsToScan = 2;
            customPrefab.SetUnlock(TechType.ExosuitGrapplingArmFragment, 2 + extraFragmentsToScan);

            CloneTemplate cloneTemplate = new CloneTemplate(prefabInfo, TechType.ExosuitGrapplingArmModule);
            cloneTemplate.ModifyPrefab += (gameObject) => gameObject.AddComponent<GrapplingArmUpgraded>();
            cloneTemplate.ModifyPrefab += (gameObject) => gameObject.name = "UpgradedGrapplingArm";
            customPrefab.SetGameObject(cloneTemplate);
            customPrefab.SetEquipment(EquipmentType.ExosuitModule);
            customPrefab.Register();

            TechType = prefabInfo.TechType;
        }
    */
    #endregion
}
