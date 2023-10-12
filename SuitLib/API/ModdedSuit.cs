using System.Collections.Generic;
using UnityEngine;
using static SuitLib.ModdedSuitsManager;

namespace SuitLib
{
    public class ModdedSuit
    {
        public Dictionary<string, Texture2D> suitReplacementTexturePropertyPairs;
        public Dictionary<string, Texture2D> armsReplacementTexturePropertyPairs;
        public VanillaModel vanillaModel;
        public TechType itemTechType;
        //public Modifications modifications;

        /// <param name="suitReplacementTexturePropertyPairs">The texture name (like _MainTex) and the texture pairs for the suit (not the arms!)</param>
        /// <param name="vanillaModel">The tech type for the suit you're replacing (like reinforcedSuit)</param>
        /// <param name="itemTechType">The tech type of the moddedsuit</param>
        public ModdedSuit(Dictionary<string, Texture2D> suitReplacementTexturePropertyPairs, Dictionary<string, Texture2D> armsReplacementTexturePropertyPairs, 
            VanillaModel vanillaModel, TechType itemTechType/*, Modifications modifications,*/)
        {
            this.suitReplacementTexturePropertyPairs = suitReplacementTexturePropertyPairs;
            this.armsReplacementTexturePropertyPairs = armsReplacementTexturePropertyPairs;
            this.vanillaModel = vanillaModel;
            this.itemTechType = itemTechType;
            //this.modifications = modifications;
        }
    }
}
