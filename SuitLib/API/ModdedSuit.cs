using System.Collections.Generic;
using UnityEngine;
using static SuitLib.ModdedSuitsManager;
using Newtonsoft.Json;
using Nautilus.Utility;
using System.IO;

namespace SuitLib
{
    public class ModdedSuit
    {
        public Dictionary<string, Texture2D> suitReplacementTexturePropertyPairs;
        public Dictionary<string, Texture2D> armsReplacementTexturePropertyPairs;
        public VanillaModel vanillaModel;
        public TechType itemTechType;
        public Modifications modifications;
        public StillsuitValues modificationValues;

        /// <param name="suitReplacementTexturePropertyPairs">The texture name (like _MainTex) and the texture pairs for the suit (not the arms!)</param>
        /// <param name="vanillaModel">The tech type for the suit you're replacing (like reinforcedSuit)</param>
        /// <param name="itemTechType">The tech type of the moddedsuit</param>
        public ModdedSuit(Dictionary<string, Texture2D> suitReplacementTexturePropertyPairs, Dictionary<string, Texture2D> armsReplacementTexturePropertyPairs, 
            VanillaModel vanillaModel, TechType itemTechType)
        {
            this.suitReplacementTexturePropertyPairs = suitReplacementTexturePropertyPairs;
            this.armsReplacementTexturePropertyPairs = armsReplacementTexturePropertyPairs;
            this.vanillaModel = vanillaModel;
            this.itemTechType = itemTechType;
        }

        public ModdedSuit(Dictionary<string, Texture2D> suitReplacementTexturePropertyPairs, Dictionary<string, Texture2D> armsReplacementTexturePropertyPairs,
            VanillaModel vanillaModel, TechType itemTechType, Modifications modifications, StillsuitValues modificationValues = null)
        {
            this.suitReplacementTexturePropertyPairs = suitReplacementTexturePropertyPairs;
            this.armsReplacementTexturePropertyPairs = armsReplacementTexturePropertyPairs;
            this.vanillaModel = vanillaModel;
            this.itemTechType = itemTechType;
            this.modifications = modifications;
            this.modificationValues = modificationValues;
        }

        [JsonConstructor]
        public ModdedSuit(Dictionary<string, string> suitFileNamePairs, Dictionary<string, string> armFileNamePairs,
            VanillaModel vanillaModel, Modifications modifications, StillsuitValues modificationValues = null)
        {
            Dictionary<string, Texture2D> suitTexturePairs = new Dictionary<string, Texture2D>();
            Dictionary<string, Texture2D> armTexturePairs = new Dictionary<string, Texture2D>();

            foreach (string key in suitFileNamePairs.Keys)
            {
                string path = Path.Combine(Main.jsonTexturesFolder, suitFileNamePairs[key]);
                suitTexturePairs.Add(key, ImageUtils.LoadTextureFromFile(path));
            }
            foreach (string key in armFileNamePairs.Keys)
            {
                string path = Path.Combine(Main.jsonTexturesFolder, armFileNamePairs[key]);
                armTexturePairs.Add(key, ImageUtils.LoadTextureFromFile(path));
            }

            this.suitReplacementTexturePropertyPairs = suitTexturePairs;
            this.armsReplacementTexturePropertyPairs = armTexturePairs;
            this.vanillaModel = vanillaModel;
            this.modifications = modifications;
            this.modificationValues = modificationValues;
            itemTechType = TechType.None;
        }
    }
}
