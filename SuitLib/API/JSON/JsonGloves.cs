using System.Collections.Generic;
using static SuitLib.API.ModdedSuitsManager;

namespace SuitLib.API.JSON;

internal class JsonGloves
{
    public Dictionary<string, string> gloveTexturePairs;
    public VanillaModel vanillaModel;
    public Modifications modifications;
    public StillsuitValues stillsuitValues;

    public JsonGloves(Dictionary<string, string> gloveFileNamePairs, VanillaModel vanillaModel, Modifications modifications, StillsuitValues stillsuitValues)
    {
        this.gloveTexturePairs = gloveFileNamePairs;
        this.vanillaModel = vanillaModel;
        this.modifications = modifications;
        this.stillsuitValues = stillsuitValues;
    }
}