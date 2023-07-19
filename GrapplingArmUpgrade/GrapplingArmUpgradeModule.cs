using System.Collections.Generic;
using ModdedArmsHelper.API;
using System.Collections;
using System.IO;
using System.Reflection;
using Nautilus.Utility;
using SMLExpander;
using SMLHelper.V2.Crafting;
using SMLHelper.V2.Handlers;

namespace GrapplingArmUpgrade_BepInEx
{
    internal class GrapplingArmUpgradeModule : CraftableModdedArm
    {
        public static TechType moduleTechType { get; private set; }

        internal GrapplingArmUpgradeModule()
            : base(
                techTypeName: "UpgradedGrapplingArm",
                friendlyName: "Upgraded Prawn Suit grappling arm",
                description: "This upgraded grappling arm will enhance your swinging capabilities",
                armType: ArmType.ExosuitArm,
                armTemplate: ArmTemplate.GrapplingArm,
                requiredForUnlock: TechType.ExosuitGrapplingArmModule,
                fragment: null
            )
        { }

        protected override RegisterArmRequest RegisterArm()
        {
            moduleTechType = TechType;
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
            string spriteFilePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets") + "/exosuitgrapplingarmmodule_Upgraded.png";
            return ImageUtils.LoadSpriteFromFile(spriteFilePath);
        }

        protected override TechData GetRecipe()
        {
            return new TechData()
            {
                craftAmount = 1,
                Ingredients = new List<Ingredient>(new Ingredient[6]
                {
                    new Ingredient(TechType.ExosuitGrapplingArmModule, 1),
                    new Ingredient(TechType.Polyaniline, 2),
                    new Ingredient(TechType.Lithium, 2),
                    new Ingredient(TechType.AramidFibers, 1),
                    new Ingredient(TechType.AluminumOxide, 1),
                    new Ingredient(TechType.Nickel, 1)
                })
            };
        }

        protected override IEnumerator ModifyGameObjectAsync(IOut<bool> success)
        {
            success.Set(true);
            yield break;
        }
    }
}
