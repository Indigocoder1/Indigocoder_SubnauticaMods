using IndigocoderLib;
using ModdedArmsHelper.API;
using Nautilus.Utility;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace GrapplingArmUpgrade_BepInEx
{
    internal class GrapplingArmUpgrade_Fragment : SpawnableArmFragment
    {
        internal GrapplingArmUpgrade_Fragment()
            : base(
                    techTypeName: "UpgradedGrapplingArmFragment",
                    friendlyName: "Upgraded Prawn suit grappling arm fragment",
                    fragmentTemplate: ArmTemplate.GrapplingArm,
                    prefabFilePath: null,
                    scanTime: 5,
                    totalFragments: 4
                    )
        { }

        protected override List<LootDistributionData.BiomeData> GetBiomeDatas()
        {
            return new List<LootDistributionData.BiomeData>()
            {
                new LootDistributionData.BiomeData() { biome = BiomeType.BloodKelp_TechSite, count = 1, probability = 0.15f },
                new LootDistributionData.BiomeData() { biome = BiomeType.Dunes_TechSite, count = 2, probability = 0.2f },
                new LootDistributionData.BiomeData() { biome = BiomeType.GrandReef_TechSite, count = 1, probability = 0.275f },
                new LootDistributionData.BiomeData() { biome = BiomeType.Mountains_TechSite, count = 2, probability = 0.2f },
                new LootDistributionData.BiomeData() { biome = BiomeType.SparseReef_Techsite, count = 2, probability = 0.1f }
            };
        }

        protected override Sprite GetUnlockSprite()
        {
            string spriteFilePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets") + "/exosuitgrapplingarmmodule_Upgraded.png";
            return SpriteHelper.SpriteFromAtlasSprite(ImageUtils.LoadSpriteFromFile(spriteFilePath));
        }

        protected override IEnumerator PostModifyGameObjectAsync(IOut<bool> success)
        {
            success.Set(true);
            yield break;
        }
    }
}
