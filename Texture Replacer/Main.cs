using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UWE;

namespace TextureReplacer
{
    [BepInPlugin(myGUID, pluginName, versionString)]
    public class Main : BaseUnityPlugin
    {
        private const string myGUID = "Indigocoder.TextureReplacer";
        private const string pluginName = "Texture Replacer";
        private const string versionString = "1.0.0";

        public static ManualLogSource logger;

        public static LifepodTextureConfigList textureConfig;

        private void Awake()
        {
            logger = Logger;

            Logger.LogInfo($"{pluginName} {versionString} Loaded.");

            textureConfig = SaveManager.LoadFromJson();

            LifepodTextureReplacer.LoadAllTextures();
        }

        public class LifepodTextureConfigList
        {
            public List<LifepodConfigItem> textureConfigs;

            public LifepodTextureConfigList(List<LifepodConfigItem> textureConfigs)
            {
                this.textureConfigs = textureConfigs;
            }
        }

        public class LifepodConfigItem
        {
            public int materialIndex;
            public string fileName;
            public int lifepodNumberIndex;
            public bool isVariation;
            public float variationChance;

            public LifepodConfigItem(int materialIndex, string fileName, int lifepodNumberIndex, bool isVariation, float variationChance)
            {
                this.materialIndex = materialIndex;
                this.fileName = fileName;
                this.lifepodNumberIndex = lifepodNumberIndex;
                this.isVariation = isVariation;
                this.variationChance = variationChance;
            }
        }
    }
}
