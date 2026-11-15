using System.Collections.Generic;
using static SuitLib.API.ModdedSuitsManager;

namespace SuitLib.API.JSON;

internal class JsonSuit
{
    public Dictionary<string, string> suitFileNamePairs;
    public Dictionary<string, string> armFileNamePairs;
    public VanillaModel vanillaModel;
    public Modifications modifications;
    public StillsuitValues stillsuitValues;

    public JsonSuit(Dictionary<string, string> suitFileNamePairs, Dictionary<string, string> armFileNamePairs,
        VanillaModel vanillaModel, Modifications modifications, StillsuitValues stillsuitValues)
    {
        this.suitFileNamePairs = suitFileNamePairs;
        this.armFileNamePairs = armFileNamePairs;
        this.vanillaModel = vanillaModel;
        this.modifications = modifications;
        this.stillsuitValues = stillsuitValues;
    }
}