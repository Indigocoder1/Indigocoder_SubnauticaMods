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
        /// <param name="vanillaModel">The tech type for the suit you're replacing (like reinforcedSuit)</param>
        /// <param name="itemTechType">The tech type of the moddedsuit</param>
        public ModdedSuit(Dictionary<string, Texture2D> replacementTexturePropertyPairs, VanillaModel vanillaModel, TechType itemTechType)
        {
            this.replacementTexturePropertyPairs = replacementTexturePropertyPairs;
            this.vanillaModel = vanillaModel;
            this.itemTechType = itemTechType;
        }
    }
}
