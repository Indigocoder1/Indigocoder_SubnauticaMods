using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using SuitLib.API;
using SuitLib.TextureManagement;

namespace SuitLib.Patches;

[HarmonyPatch(typeof(Player))]
internal static class Player_Patches
{
    private static Equipment GetPlayerEquipment() => Inventory.main.equipment;
    
    [HarmonyPatch(nameof(Player.Start)), HarmonyPrefix]
    private static void Start()
    {
        PlayerTextureManager.InitializeAllSuitInfos();
    }

    [HarmonyPatch(nameof(Player.EquipmentChanged)), HarmonyPostfix]
    private static void EquipmentChanged_Patch()
    {
        var typeInBodySlot = GetPlayerEquipment().GetTechTypeInSlot("Body");
        var typeInGlovesSlot = GetPlayerEquipment().GetTechTypeInSlot("Gloves");

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

        PlayerTextureManager.UpdateActiveObjects(typeInBodySlot, typeInGlovesSlot, wearingAnySuit, wearingAnyGloves);
        ModdedSuitsManager.OnSuitEquippedChanged(typeInBodySlot);
        ModdedSuitsManager.OnGlovesEquippedChanged(typeInGlovesSlot);
        
        PlayerTextureManager.UpdateGloveTextures(wearingModdedGloves);
        PlayerTextureManager.UpdateSuitTextures(wearingModdedSuit);
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
        var typeInBodySlot = GetPlayerEquipment().GetTechTypeInSlot("Body");
        var suitInSlot = ModdedSuitsManager.ModdedSuitsList.FirstOrDefault(suit => suit.itemTechType == typeInBodySlot);
        if (suitInSlot == null)
        {
            return 15f;
        }

        return suitInSlot.tempValue;
    }

    private static float GetGlovesTemp()
    {
        var typeInGlovesSlot = GetPlayerEquipment().GetTechTypeInSlot("Gloves");
        var glovesInSlot = ModdedSuitsManager.ModdedGlovesList.FirstOrDefault(gloves => gloves.itemTechType == typeInGlovesSlot);
        if (glovesInSlot == null)
        {
            return 6f;
        }

        return glovesInSlot.tempValue;
    }
    
    public static bool WearingItem(TechType item, string slot)
    {
        return item == GetPlayerEquipment().GetTechTypeInSlot(slot);
    }
}

