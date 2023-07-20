using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Crafting;
using Nautilus.Handlers;
using Nautilus.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace UpgradedJumpJetModule
{
    internal static class UpgradedJetsModule
    {
        public static TechType moduleTechType { get; private set; }

        public static void RegisterModule()
        {
            Main_Plugin.logger.LogInfo("Registering module");

            string spriteFilePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets") + "/upgradedJumpJets.png";
            Atlas.Sprite sprite = ImageUtils.LoadSpriteFromFile(spriteFilePath);

            var prefabInfo = PrefabInfo.WithTechType("UpgradedJumpJetModule", "Upgraded Prawn jump jet module", "With this upgrade module you can jump even more! Effects do not stack")
                .WithIcon(sprite);

            var customPrefab = new CustomPrefab(prefabInfo);
            customPrefab.SetRecipe(new RecipeData()
            {
                craftAmount = 1,
                Ingredients = new List<CraftData.Ingredient>()
                {
                    new CraftData.Ingredient(TechType.ExosuitJetUpgradeModule),
                    new CraftData.Ingredient(TechType.Sulphur, 2),
                    new CraftData.Ingredient(TechType.Kyanite, 1),
                    new CraftData.Ingredient(TechType.Titanium, 2),
                }
            })
                .WithFabricatorType(CraftTree.Type.Workbench)
                .WithCraftingTime(5f);

            CraftDataHandler.SetBackgroundType(prefabInfo.TechType, CraftData.BackgroundType.Normal);

            int extraFragmentsToScan = 2;
            customPrefab.SetUnlock(TechType.ExosuitJetUpgradeModule, 2 + extraFragmentsToScan);

            CloneTemplate cloneTemplate = new CloneTemplate(prefabInfo, TechType.ExosuitJetUpgradeModule);
            customPrefab.SetGameObject(cloneTemplate);
            customPrefab.SetEquipment(EquipmentType.ExosuitModule);
            customPrefab.Register();

            moduleTechType = prefabInfo.TechType;
        }
    }
}
