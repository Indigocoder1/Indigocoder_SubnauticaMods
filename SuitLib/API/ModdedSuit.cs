using System.Collections.Generic;
using UnityEngine;
using static SuitLib.API.ModdedSuitsManager;

namespace SuitLib.API;

public class ModdedSuit
{
    public Dictionary<string, Texture2D> suitReplacementTexturePropertyPairs;
    public Dictionary<string, Texture2D> armsReplacementTexturePropertyPairs;
    public VanillaModel vanillaModel;
    public TechType itemTechType;
    public Modifications modifications;
    public StillsuitValues stillsuitValues;
    public float tempValue;
    public bool jsonSuit;

    /// <summary>
    /// Creates a new modded suit
    /// </summary>
    /// <param name="suitReplacementTexturePropertyPairs">The texture name (like _MainTex) and the texture pairs for the suit (not the arms!)</param>
    /// <param name="armReplacementTexturePropertyPairs">The texture name (like _MainTex) and the texture pairs for the arms of the suit.</param>
    /// <param name="vanillaModel">The model of the suit you're replacing</param>
    /// <param name="itemTechType">The tech type of the modded suit</param>
    /// <param name="tempValue">The temperature damage reduction for this suit</param>
    public ModdedSuit(Dictionary<string, Texture2D> suitReplacementTexturePropertyPairs, Dictionary<string, Texture2D> armReplacementTexturePropertyPairs, 
        VanillaModel vanillaModel, TechType itemTechType, float tempValue = 15)
    {
        this.suitReplacementTexturePropertyPairs = suitReplacementTexturePropertyPairs;
        armsReplacementTexturePropertyPairs = armReplacementTexturePropertyPairs;
        this.vanillaModel = vanillaModel;
        this.itemTechType = itemTechType;
        this.tempValue = tempValue;
    }

    /// <summary>
    /// Creates a new modded suit
    /// </summary>
    /// <param name="suitReplacementTexturePropertyPairs">The texture name (like _MainTex) and the texture pairs for the suit (not the arms!)</param>
    /// <param name="armReplacementTexturePropertyPairs">The texture name (like _MainTex) and the texture pairs for the arms and gloves.
    /// Assumes the arm and glove textures are the same (This is the case for the default player model)</param>
    /// <param name="vanillaModel">The model of the suit you're replacing</param>
    /// <param name="itemTechType">The tech type of the modded suit</param>
    /// <param name="modifications">The modification flags to apply to the suit</param>
    /// <param name="stillsuitValues">The stillsuit values to apply if the "Filtration" flag is set in the modifications/></param>
    /// <param name="tempValue">The temperature damage reduction for this suit</param>
    public ModdedSuit(Dictionary<string, Texture2D> suitReplacementTexturePropertyPairs, Dictionary<string, Texture2D> armReplacementTexturePropertyPairs,
        VanillaModel vanillaModel, TechType itemTechType, Modifications modifications, StillsuitValues stillsuitValues = null, float tempValue = 15)
    {
        this.suitReplacementTexturePropertyPairs = suitReplacementTexturePropertyPairs;
        armsReplacementTexturePropertyPairs = armReplacementTexturePropertyPairs;
        this.vanillaModel = vanillaModel;
        this.itemTechType = itemTechType;
        this.modifications = modifications;
        this.stillsuitValues = stillsuitValues;
        this.tempValue = tempValue;
    }

    /// <summary>
    /// Creates a new modded suit
    /// </summary>
    /// <param name="copyFrom">The suit to copy dataFrom</param>
    public ModdedSuit(ModdedSuit copyFrom)
    {
        suitReplacementTexturePropertyPairs = copyFrom.suitReplacementTexturePropertyPairs;
        armsReplacementTexturePropertyPairs = copyFrom.armsReplacementTexturePropertyPairs;
        vanillaModel = copyFrom.vanillaModel;
        itemTechType = copyFrom.itemTechType;
        modifications = copyFrom.modifications;
        stillsuitValues = copyFrom.stillsuitValues;
        tempValue = copyFrom.tempValue;
        jsonSuit = copyFrom.jsonSuit;
    }
}