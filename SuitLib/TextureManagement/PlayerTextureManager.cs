using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SuitLib.API;
using SuitLib.Patches;
using UnityEngine;

namespace SuitLib.TextureManagement;

internal static class PlayerTextureManager
{
    private static SuitTextureInfo _diveSuitInfo;
    private static SuitTextureInfo _radiationSuitInfo;
    private static SuitTextureInfo _reinforcedSuitInfo;
    private static SuitTextureInfo _filtrationSuitInfo;

    #region Initialization

    internal static void InitializeAllSuitInfosAsync()
    {
        if (Player.main == null)
        {
            // This error state should only happen when called via the API, which exposes this method as the "Texture cache"
            Main.logger.LogError("Attempting to initialize texture cache when Player.main is null! Doing nothing.");
            return;
        }
        
        _diveSuitInfo = new SuitTextureInfo();
        _radiationSuitInfo = new SuitTextureInfo();
        _reinforcedSuitInfo = new SuitTextureInfo();
        _filtrationSuitInfo = new SuitTextureInfo(true);
        InitializeModelGOs();
        
        UWE.CoroutineHost.StartCoroutine(SetupTexturesAfterHolds());
    }

    private static IEnumerator SetupTexturesAfterHolds()
    {
        yield return CacheHoldManager.WaitForHoldsAsync();

        InitializeVanillaTextures();
    }

    private static void InitializeModelGOs()
    {
        var geoRoot = Player.main.transform.Find("body/player_view/male_geo");

        var diveSuit = geoRoot.Find("diveSuit");
        _diveSuitInfo.suitBodyObject = diveSuit.Find("diveSuit_body_geo").gameObject;
        _diveSuitInfo.suitGlovesObject = diveSuit.Find("diveSuit_hands_geo").gameObject;

        var radiationSuit = geoRoot.Find("radiationSuit");
        _radiationSuitInfo.suitBodyObject = radiationSuit.Find("radiationSuit_body_geo").gameObject;
        _radiationSuitInfo.suitGlovesObject = radiationSuit.Find("radiationSuit_gloves_geo").gameObject;

        var reinforcedSuit = geoRoot.Find("reinforcedSuit");
        _reinforcedSuitInfo.suitBodyObject = reinforcedSuit.Find("reinforced_suit_01_body_geo").gameObject;
        _reinforcedSuitInfo.suitGlovesObject = reinforcedSuit.Find("reinforced_suit_01_glove_geo").gameObject;

        _filtrationSuitInfo.suitBodyObject = geoRoot.Find("stillSuit/still_suit_01_body_geo").gameObject;
    }

    internal static void InitializeVanillaTextures()
    {
        // Use hash sets to get built-in ignoring of duplicates
        var suitTextureNamesToCache = new HashSet<string>();
        var armsTextureNamesToCache = new HashSet<string>();
        foreach (var suit in ModdedSuitsManager.ModdedSuitsList)
        {
            var keys = suit.suitReplacementTexturePropertyPairs.Keys.ToList();
            foreach (var key in keys)
            {
                if (!suitTextureNamesToCache.Add(key)) continue;
                
                _diveSuitInfo.AddBodyTexture(key);
                _radiationSuitInfo.AddBodyTexture(key);
                _reinforcedSuitInfo.AddBodyTexture(key);
                _filtrationSuitInfo.AddBodyTexture(key);
            }

            var armsKeys = suit.armsReplacementTexturePropertyPairs.Keys.ToList();
            foreach (var key in armsKeys)
            {
                if (!armsTextureNamesToCache.Add(key)) continue;
                
                _filtrationSuitInfo.AddGlovesTexture(key);
            }
        }

        var gloveTextureNamesToCache = new HashSet<string>();
        foreach (var gloves in ModdedSuitsManager.ModdedGlovesList)
        {
            var textureNames = gloves.replacementTexturePropertyPairs.Keys.ToList();
            foreach (string name in textureNames)
            {
                if (!gloveTextureNamesToCache.Add(name)) continue;
                
                _diveSuitInfo.AddGlovesTexture(name);
                _radiationSuitInfo.AddGlovesTexture(name);
                _reinforcedSuitInfo.AddGlovesTexture(name);
            }
        }
    }
    
    #endregion
    
    internal static void UpdateActiveObjects(TechType typeInBodySlot, TechType typeInGlovesSlot, bool wearingAnySuit,
        bool wearingAnyGloves)
    {
        _diveSuitInfo.suitBodyObject.SetActive(!wearingAnySuit);
        _diveSuitInfo.suitGlovesObject.SetActive(!wearingAnyGloves);
        if (wearingAnySuit)
        {
            foreach (var suit in ModdedSuitsManager.ModdedSuitsList)
            {
                if (suit.itemTechType != typeInBodySlot) continue;

                var model = GetModel(suit.vanillaModel, true);
                model.SetActive(true);
            }
        }

        if (!wearingAnyGloves) return;
        
        foreach (var gloves in ModdedSuitsManager.ModdedGlovesList)
        {
            if (gloves.itemTechType != typeInGlovesSlot) continue;

            var model = GetModel(gloves.vanillaModel, false);
            model.SetActive(true);
        }
    }

    internal static void UpdateSuitTextures(bool wearingModdedSuit)
    {
        foreach (var suit in ModdedSuitsManager.ModdedSuitsList)
        {
            if (!suit.jsonSuit) continue;

            wearingModdedSuit = true;
        }

        if (!wearingModdedSuit)
        {
            _diveSuitInfo.ResetBodyTextures();
            _radiationSuitInfo.ResetBodyTextures();
            _reinforcedSuitInfo.ResetBodyTextures();
            _filtrationSuitInfo.ResetBodyTextures();
            return;
        }

        foreach (var suit in ModdedSuitsManager.ModdedSuitsList)
        {
            if (!Player_Patches.WearingItem(suit.itemTechType, "Body")) continue;

            var model = GetModel(suit.vanillaModel, true);
            var modelRenderer = model.GetComponent<Renderer>();

            foreach (var key in suit.suitReplacementTexturePropertyPairs.Keys)
            {
                var moddedTex = suit.suitReplacementTexturePropertyPairs[key];
                modelRenderer.materials[0].SetTexture(key, moddedTex);
            }

            foreach (var key in suit.armsReplacementTexturePropertyPairs.Keys)
            {
                var armsTex = suit.armsReplacementTexturePropertyPairs[key];
                modelRenderer.materials[1].SetTexture(key, armsTex);
            }
        }
    }

    internal static void UpdateGloveTextures(bool wearingModdedGloves)
    {
        foreach (var suit in ModdedSuitsManager.ModdedSuitsList)
        {
            if (!suit.jsonSuit) continue;
            
            wearingModdedGloves = true;
        }

        if (!wearingModdedGloves)
        {
            _diveSuitInfo.ResetGloveTextures();
            _radiationSuitInfo.ResetGloveTextures();
            _reinforcedSuitInfo.ResetGloveTextures();
            return;
        }

        foreach (var gloves in ModdedSuitsManager.ModdedGlovesList)
        {
            if (!Player_Patches.WearingItem(gloves.itemTechType, "Gloves")) continue;

            var glovesModelRenderer = GetModel(gloves.vanillaModel, false).GetComponent<Renderer>();

            foreach (var key in gloves.replacementTexturePropertyPairs.Keys)
            {
                var moddedTex = gloves.replacementTexturePropertyPairs[key];
                glovesModelRenderer.material.SetTexture(key, moddedTex);
            }
        }
    }
    
    private static GameObject GetModel(ModdedSuitsManager.VanillaModel type, bool isSuit)
    {
        return type switch
        {
            ModdedSuitsManager.VanillaModel.Dive => isSuit
                ? _diveSuitInfo.suitBodyObject
                : _diveSuitInfo.suitGlovesObject,
            ModdedSuitsManager.VanillaModel.Radiation => isSuit
                ? _radiationSuitInfo.suitBodyObject
                : _radiationSuitInfo.suitGlovesObject,
            ModdedSuitsManager.VanillaModel.Reinforced => isSuit
                ? _reinforcedSuitInfo.suitBodyObject
                : _reinforcedSuitInfo.suitGlovesObject,
            ModdedSuitsManager.VanillaModel.WaterFiltration => isSuit
                ? _filtrationSuitInfo.suitBodyObject
                : _filtrationSuitInfo.suitGlovesObject,
            _ => null
        };
    }
}