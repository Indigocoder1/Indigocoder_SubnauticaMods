using System;
using System.Collections.Generic;
using SuitLib.Patches;

namespace SuitLib.API;

public static class ModdedSuitsManager
{
    public static event Action<TechType> onSuitEquippedChanged;
    public static event Action<TechType> onGlovesEquippedChanged;
    
    internal static readonly List<ModdedSuit> ModdedSuitsList = new();
    internal static readonly List<ModdedGloves> ModdedGlovesList = new();

    public static void AddModdedSuit(ModdedSuit moddedSuit)
    {
        ModdedSuitsList.Add(moddedSuit);
    }

    public static void AddModdedSuits(List<ModdedSuit> moddedSuit)
    {
        ModdedSuitsList.AddRange(moddedSuit);
    }

    public static void AddModdedGloves(ModdedGloves moddedGloves)
    {
        ModdedGlovesList.Add(moddedGloves);
    }

    public static void AddModdedGloves(List<ModdedGloves> moddedGloves)
    {
        ModdedGlovesList.AddRange(moddedGloves);
    }

    /// <summary>
    /// Resets the vanilla texture cache for all suits.
    /// All textures currently set on those suit models will be set as the new vanilla textures.
    /// Make sure you do not have incorrect textures on disabled models when you call this method.
    ///
    /// Note: This will do nothing if not called in the main game scene (When Player.main is not equal to null)
    /// </summary>
    public static void ResetVanillaTextureCache()
    {
        Player_Patches.InitializeAllSuitInfos();
    }

    public enum VanillaModel
    {
        None,
        Dive,
        Radiation,
        Reinforced,
        WaterFiltration
    }

    [Flags]
    public enum Modifications
    {
        None,
        Reinforced,
        Filtration
    }

    public class StillsuitValues
    {
        public readonly float stillsuitDeltaTimeReduction;
        public float stillsuitWaterIncreaseMultiplier;

        public StillsuitValues(float stillsuitDeltaTimeReduction, float stillsuitWaterIncreaseMultiplier)
        {
            this.stillsuitDeltaTimeReduction = stillsuitDeltaTimeReduction;
            this.stillsuitWaterIncreaseMultiplier = stillsuitWaterIncreaseMultiplier;
        }
    }

    public static Dictionary<VanillaModel, TechType> suitModelTechTypes = new()
    {
        { VanillaModel.None, TechType.None },
        { VanillaModel.Dive, TechType.None },
        { VanillaModel.Radiation, TechType.RadiationSuit },
        { VanillaModel.Reinforced, TechType.ReinforcedDiveSuit },
        { VanillaModel.WaterFiltration, TechType.WaterFiltrationSuit},
    };

    public static Dictionary<VanillaModel, TechType> gloveModelTechTypes = new()
    {
        { VanillaModel.None, TechType.None },
        { VanillaModel.Dive, TechType.None },
        { VanillaModel.Radiation, TechType.RadiationGloves },
        { VanillaModel.Reinforced, TechType.ReinforcedGloves }
    };

    internal static void OnSuitEquippedChanged(TechType techType)
    {
        onSuitEquippedChanged?.Invoke(techType);
    }
    
    internal static void OnGlovesEquippedChanged(TechType techType)
    {
        onGlovesEquippedChanged?.Invoke(techType);
    }
}
