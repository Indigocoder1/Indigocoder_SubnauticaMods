using System.Collections.Generic;
using UnityEngine;
using static SuitLib.ModdedSuitsManager;

namespace SuitLib
{
    public class ModdedSuit
    {
        public Dictionary<string, Texture2D> replacementTexturePropertyPairs;
        public VanillaModel vanillaModel;
        public TechType itemTechType;

        /// <param name="replacementTexturePropertyPairs">The texture name (like _MainTex) and the texture pairs</param>
        /// <param name="vanillaSuitModel">The tech type for the suit you're replacing (like reinforcedSuit)</param>
        /// <param name="suitTechType">The tech type of the moddedsuit</param>
        public ModdedSuit(Dictionary<string, Texture2D> replacementTexturePropertyPairs, VanillaModel vanillaSuitModel, TechType suitTechType)
        {
            this.replacementTexturePropertyPairs = replacementTexturePropertyPairs;
            this.vanillaModel = vanillaSuitModel;
            this.itemTechType = suitTechType;
        }
    }
}
