using BepInEx;
using BepInEx.Logging;

namespace TextureReplacer
{
    [BepInPlugin(myGUID, pluginName, versionString)]
    public class Main : BaseUnityPlugin
    {
        private const string myGUID = "Indigocoder.TextureReplacer";
        private const string pluginName = "Texture Replacer";
        private const string versionString = "1.0.0";

        public static ManualLogSource logger;

        private void Awake()
        {
            logger = Logger;

            Logger.LogInfo($"{pluginName} {versionString} Loaded.");

            LifepodTextureReplacer.Initialize();
            CustomTextureReplacer.Initialize();
        }

        public class TexturePatchConfigData
        {
            public int materialIndex;
            public string fileName;
            public string prefabClassID;
            public string rendererHierchyPath;

            public bool isVariation;
            public float variationChance;

            public TexturePatchConfigData(int materialIndex, string fileName, bool isVariation, float variationChance, string prefabClassID, string rendererHierchyPath)
            {
                this.materialIndex = materialIndex;
                this.fileName = fileName;
                this.prefabClassID = prefabClassID;
                this.rendererHierchyPath = rendererHierchyPath;
                this.isVariation = isVariation;
                this.variationChance = variationChance;
            }
        }
    }
}
