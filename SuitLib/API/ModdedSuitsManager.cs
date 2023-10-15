using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuitLib
{
    public static class ModdedSuitsManager
    {
        internal static List<ModdedSuit> moddedSuitsList = new List<ModdedSuit>();
        internal static List<ModdedGloves> moddedGlovesList = new List<ModdedGloves>();

        public static void AddModdedSuit(ModdedSuit moddedSuit)
        {
            moddedSuitsList.Add(moddedSuit);
        }

        public static void AddModdedGloves(ModdedGloves moddedGloves)
        {
            moddedGlovesList.Add(moddedGloves);
        }

        public enum VanillaModel
        {
            None,
            Dive,
            Radiation,
            Reinforced,
            WaterFiltration
        }

        public enum Modifications
        {
            None,
            Reinforced,
            Filtration
        }

        public class ModificationValues
        {
            public float stillsuitDeltaTimeReduction;
            public float stillsuitWaterIncreaseMultiplier;

            public ModificationValues(float stillsuitDeltaTimeReduction, float stillsuitWaterIncreaseMultiplier)
            {
                this.stillsuitDeltaTimeReduction = stillsuitDeltaTimeReduction;
                this.stillsuitWaterIncreaseMultiplier = stillsuitWaterIncreaseMultiplier;
            }
        }
    }
}
