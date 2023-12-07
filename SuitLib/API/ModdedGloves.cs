using Nautilus.Utility;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static SuitLib.ModdedSuitsManager;

namespace SuitLib
{
    public class ModdedGloves
    {
        public Dictionary<string, Texture2D> replacementTexturePropertyPairs;
        public VanillaModel vanillaModel;
        public TechType itemTechType;
        public Modifications modifications;
        public StillsuitValues stillsuitValues;

        /// <param name="replacementTexturePropertyPairs">The texture name (like _MainTex) and the texture pairs</param>
        /// <param name="vanillaModel">The tech type for the gloves model you're replacing (like reinforcedGloves)</param>
        /// <param name="itemTechType">The tech type of the moddedsuit</param>
        public ModdedGloves(Dictionary<string, Texture2D> replacementTexturePropertyPairs, VanillaModel vanillaModel, TechType itemTechType)
        {
            this.replacementTexturePropertyPairs = replacementTexturePropertyPairs;
            this.vanillaModel = vanillaModel;
            this.itemTechType = itemTechType;
        }

        public ModdedGloves(Dictionary<string, Texture2D> replacementTexturePropertyPairs, VanillaModel vanillaModel, TechType itemTechType,
            Modifications modifications, StillsuitValues stillsuitValues = null)
        {
            this.replacementTexturePropertyPairs = replacementTexturePropertyPairs;
            this.vanillaModel = vanillaModel;
            this.itemTechType = itemTechType;
            this.modifications = modifications;
            this.stillsuitValues = stillsuitValues;
        }

        [JsonConstructor]
        public ModdedGloves(Dictionary<string, string> gloveFileNamePairs, VanillaModel vanillaModel, Modifications modifications)
        {
            Dictionary<string, Texture2D> gloveTexturePairs = new Dictionary<string, Texture2D>();
            foreach (string key in gloveFileNamePairs.Keys)
            {
                string path = Path.Combine(Main.jsonTexturesFolder, gloveFileNamePairs[key]);
                gloveTexturePairs.Add(key, ImageUtils.LoadTextureFromFile(path));
            }

            this.replacementTexturePropertyPairs = gloveTexturePairs;
            this.vanillaModel = vanillaModel;
            this.modifications = modifications;
            this.itemTechType = TechType.None;
        }
    }
}
