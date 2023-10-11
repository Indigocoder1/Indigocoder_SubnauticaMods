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

        /// <param name="replacementTexturePropertyPairs">The texture name (like _MainTex) and the texture pairs</param>
        /// <param name="vanillaGlovesModel">The tech type for the gloves model you're replacing (like reinforcedGloves)</param>
        /// <param name="suitTechType">The tech type of the moddedsuit</param>
        public ModdedGloves(Dictionary<string, Texture2D> replacementTexturePropertyPairs, VanillaModel vanillaGlovesModel, TechType suitTechType)
        {
            this.replacementTexturePropertyPairs = replacementTexturePropertyPairs;
            this.vanillaModel = vanillaGlovesModel;
            this.itemTechType = suitTechType;
        }
    }
}
