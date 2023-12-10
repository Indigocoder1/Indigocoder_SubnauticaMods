using System.Collections.Generic;
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
        public bool jsonGloves;

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
    }
}
