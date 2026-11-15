using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using UnityEngine;
using SuitLib.API;

namespace SuitLib.Patches;

internal struct SuitTextureInfo()
{
    public GameObject suitBodyObject;
    public GameObject suitGlovesObject;
    
    private readonly Dictionary<string, Texture> vanillaSuitTextures = new();
    private readonly Dictionary<string, Texture> vanillaGloveTextures = new();
    private readonly bool glovesUseBodyIndex1;

    public SuitTextureInfo(bool glovesUseBodyIndex1) : this()
    {
        this.glovesUseBodyIndex1 = glovesUseBodyIndex1;
    }

    public void AddBodyTexture(string textureName)
    {
        var suitTex = suitBodyObject.GetComponent<Renderer>().material.GetTexture(textureName);
        vanillaSuitTextures.Add(textureName, suitTex);
    }
    
    public void AddGlovesTexture(string textureName)
    {
        var obj = glovesUseBodyIndex1 ? suitBodyObject : suitGlovesObject;
        var materialIndex = glovesUseBodyIndex1 ? 1 : 0;
        
        var glovesTex = obj.GetComponent<Renderer>().materials[materialIndex].GetTexture(textureName);
        vanillaGloveTextures.Add(textureName, glovesTex);
    }

    public void ResetBodyTextures()
    {
        foreach (var key in vanillaSuitTextures.Keys)
        {
            var bodyTex = (Texture2D)vanillaSuitTextures[key];
            suitBodyObject.GetComponent<Renderer>().material.SetTexture(key, bodyTex);
        }

        foreach (var key in vanillaGloveTextures.Keys)
        {
            var armsTex = (Texture2D)vanillaGloveTextures[key];
            suitBodyObject.GetComponent<Renderer>().materials[1].SetTexture(key, armsTex);
        }
    }

    public void ResetGloveTextures()
    {
        if (suitGlovesObject == null) return;
        
        foreach (var key in vanillaGloveTextures.Keys)
        {
            var tex = (Texture2D)vanillaGloveTextures[key];
            suitGlovesObject.GetComponent<Renderer>().material.SetTexture(key, tex);
        }
    }
}

[HarmonyPatch(typeof(Player))]
internal static class Player_Patches
{
    private static SuitTextureInfo _diveSuitInfo;
    private static SuitTextureInfo _radiationSuitInfo;
    private static SuitTextureInfo _reinforcedSuitInfo;

    private static SuitTextureInfo _filtrationSuitInfo;
    private static Equipment _playerEquipment;

    [HarmonyPatch(nameof(Player.Start)), HarmonyPrefix]
    private static void Start()
    {
        _playerEquipment = Inventory.main.equipment;
        InitializeAllSuitInfos();
    }

    internal static void InitializeAllSuitInfos()
    {
        if (Player.main == null)
        {
            Main.logger.LogError("Attempting to initialize suit infos when Player.main is null!");
            return;
        }
        
        _diveSuitInfo = new SuitTextureInfo();
        _radiationSuitInfo = new SuitTextureInfo();
        _reinforcedSuitInfo = new SuitTextureInfo();
        _filtrationSuitInfo = new SuitTextureInfo(true);

        InitializeModelGOs();
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

    private static void InitializeVanillaTextures()
    {
        var suitTextureNamesToCache = new HashSet<string>();
        var armsTextureNamesToCache = new HashSet<string>();
        foreach (var suit in ModdedSuitsManager.ModdedSuitsList)
        {
            var keys = suit.suitReplacementTexturePropertyPairs.Keys.ToList();
            foreach (string key in keys)
            {
                suitTextureNamesToCache.Add(key);
            }

            var armsKeys = suit.armsReplacementTexturePropertyPairs.Keys.ToList();
            foreach (var key in armsKeys)
            {
                armsTextureNamesToCache.Add(key);
            }
        }

        var gloveTextureNamesToCache = new HashSet<string>();
        foreach (var gloves in ModdedSuitsManager.ModdedGlovesList)
        {
            var textureNames = gloves.replacementTexturePropertyPairs.Keys.ToList();
            foreach (string name in textureNames)
            {
                gloveTextureNamesToCache.Add(name);
            }
        }

        foreach (var textureName in suitTextureNamesToCache)
        {
            _diveSuitInfo.AddBodyTexture(textureName);
            _radiationSuitInfo.AddBodyTexture(textureName);
            _reinforcedSuitInfo.AddBodyTexture(textureName);
            _filtrationSuitInfo.AddBodyTexture(textureName);
        }

        foreach (var textureName in gloveTextureNamesToCache)
        {
            _diveSuitInfo.AddGlovesTexture(textureName);
            _radiationSuitInfo.AddGlovesTexture(textureName);
            _reinforcedSuitInfo.AddGlovesTexture(textureName);
        }

        foreach (var textureName in armsTextureNamesToCache)
        {
            _filtrationSuitInfo.AddGlovesTexture(textureName);
        }
    }

    [HarmonyPatch(nameof(Player.EquipmentChanged)), HarmonyPostfix]
    private static void EquipmentChanged_Patch()
    {
        var typeInBodySlot = _playerEquipment.GetTechTypeInSlot("Body");
        var typeInGlovesSlot = _playerEquipment.GetTechTypeInSlot("Gloves");

        var wearingModdedSuit =
            (typeInBodySlot != TechType.None) &&
            (typeInBodySlot != TechType.RadiationSuit) &&
            (typeInBodySlot != TechType.ReinforcedDiveSuit) &&
            (typeInBodySlot != TechType.WaterFiltrationSuit);

        var wearingModdedGloves =
            (typeInGlovesSlot != TechType.None) &&
            (typeInGlovesSlot != TechType.RadiationGloves) &&
            (typeInGlovesSlot != TechType.ReinforcedGloves);

        var wearingAnySuit = typeInBodySlot != TechType.None;
        var wearingAnyGloves = typeInGlovesSlot != TechType.None;

        SetGOsActive(typeInBodySlot, typeInGlovesSlot, wearingAnySuit, wearingAnyGloves);
        ModdedSuitsManager.OnSuitEquippedChanged(typeInBodySlot);
        ModdedSuitsManager.OnGlovesEquippedChanged(typeInGlovesSlot);

        SetGloveTextures(typeInGlovesSlot, wearingModdedGloves);
        SetSuitTextures(typeInBodySlot, wearingModdedSuit);
    }

    [HarmonyPatch(nameof(Player.HasReinforcedSuit)), HarmonyPostfix]
    private static void HasReinforcedSuit_Patch(ref bool __result)
    {
        foreach (ModdedSuit suit in ModdedSuitsManager.ModdedSuitsList)
        {
            if (!WearingItem(suit.itemTechType, "Body"))
            {
                continue;
            }

            if ((suit.modifications & ModdedSuitsManager.Modifications.Reinforced) != 0)
            {
                __result = true;
            }
        }
    }

    [HarmonyPatch(nameof(Player.HasReinforcedGloves)), HarmonyPostfix]
    private static void HasReinforcedGloves_Patch(ref bool __result)
    {
        foreach (var gloves in ModdedSuitsManager.ModdedGlovesList)
        {
            if (!WearingItem(gloves.itemTechType, "Gloves"))
            {
                continue;
            }

            if ((gloves.modifications & ModdedSuitsManager.Modifications.Reinforced) != 0)
            {
                __result = true;
            }
        }
    }

    [HarmonyPatch(nameof(Player.UpdateReinforcedSuit)), HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> UpdateReinforcedSuit_Transpiler(
        IEnumerable<CodeInstruction> instructions)
    {
        CodeMatch suitMatch = new CodeMatch(i => i.opcode == OpCodes.Ldc_R4 && ((float)i.operand == 15f));
        CodeMatch glovesMatch = new CodeMatch(i => i.opcode == OpCodes.Ldc_R4 && ((float)i.operand == 6f));

        var newInstructions = new CodeMatcher(instructions)
            .MatchForward(false, suitMatch)
            .SetInstruction(Transpilers.EmitDelegate(GetSuitTemp))
            .MatchForward(false, glovesMatch)
            .SetInstruction(Transpilers.EmitDelegate(GetGlovesTemp));


        return newInstructions.InstructionEnumeration();
    }

    private static float GetSuitTemp()
    {
        var typeInBodySlot = _playerEquipment.GetTechTypeInSlot("Body");
        var suitInSlot = ModdedSuitsManager.ModdedSuitsList.FirstOrDefault(suit => suit.itemTechType == typeInBodySlot);
        if (suitInSlot == null)
        {
            return 15f;
        }

        return suitInSlot.tempValue;
    }

    private static float GetGlovesTemp()
    {
        var typeInGlovesSlot = _playerEquipment.GetTechTypeInSlot("Gloves");
        var glovesInSlot = ModdedSuitsManager.ModdedGlovesList.FirstOrDefault(gloves => gloves.itemTechType == typeInGlovesSlot);
        if (glovesInSlot == null)
        {
            return 6f;
        }

        return glovesInSlot.tempValue;
    }

    private static void SetGOsActive(TechType typeInBodySlot, TechType typeInGlovesSlot, bool wearingAnySuit,
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

    private static void SetSuitTextures(TechType typeInSuitSlot, bool wearingModdedSuit)
    {
        foreach (var suit in ModdedSuitsManager.ModdedSuitsList)
        {
            if (!suit.jsonSuit) continue;

            wearingModdedSuit = true;
        }

        if (!wearingModdedSuit)
        {
            ResetBodyTextures();
            return;
        }

        foreach (var suit in ModdedSuitsManager.ModdedSuitsList)
        {
            if (!WearingItem(suit.itemTechType, "Body")) continue;

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

    private static void ResetBodyTextures()
    {
        _diveSuitInfo.ResetBodyTextures();
        _radiationSuitInfo.ResetBodyTextures();
        _reinforcedSuitInfo.ResetBodyTextures();
        _filtrationSuitInfo.ResetBodyTextures();
    }

    private static void SetGloveTextures(TechType typeInGlovesSlot, bool wearingModdedGloves)
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
            if (!WearingItem(gloves.itemTechType, "Gloves")) continue;

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

    public static bool WearingItem(TechType item, string slot)
    {
        return item == _playerEquipment.GetTechTypeInSlot(slot);
    }
}

