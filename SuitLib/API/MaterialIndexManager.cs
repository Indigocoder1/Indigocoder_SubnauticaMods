using System.Collections.Generic;

namespace SuitLib.API;

public static class MaterialIndexManager
{
    internal static readonly Dictionary<ModdedSuitsManager.VanillaModel, int[]> ModelBodyTexIndices = new()
    {
        { ModdedSuitsManager.VanillaModel.Dive, [0] },
        { ModdedSuitsManager.VanillaModel.Radiation, [0] },
        { ModdedSuitsManager.VanillaModel.Reinforced, [0] },
        { ModdedSuitsManager.VanillaModel.WaterFiltration, [0] },
    };
    
    internal static readonly Dictionary<ModdedSuitsManager.VanillaModel, int[]> ModelArmTexIndices = new()
    {
        { ModdedSuitsManager.VanillaModel.Dive, [1] },
        { ModdedSuitsManager.VanillaModel.Radiation, [1] },
        { ModdedSuitsManager.VanillaModel.Reinforced, [1] },
        { ModdedSuitsManager.VanillaModel.WaterFiltration, [1] },
    };
    
    internal static readonly Dictionary<ModdedSuitsManager.VanillaModel, int[]> ModelGloveTexIndices = new()
    {
        { ModdedSuitsManager.VanillaModel.Dive, [0] },
        { ModdedSuitsManager.VanillaModel.Radiation, [0] },
        { ModdedSuitsManager.VanillaModel.Reinforced, [0] },
    };

    public static void SetBodyTextureIndexRemap(ModdedSuitsManager.VanillaModel model, int[] newIndices)
    {
        ModelBodyTexIndices[model] = newIndices;
    }
    
    public static void SetArmTextureIndexRemap(ModdedSuitsManager.VanillaModel model, int[] newIndices)
    {
        ModelArmTexIndices[model] = newIndices;
    }
    
    public static void SetGloveTextureIndexRemap(ModdedSuitsManager.VanillaModel model, int[] newIndices)
    {
        ModelGloveTexIndices[model] = newIndices;
    }
}