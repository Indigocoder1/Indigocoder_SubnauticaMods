using HarmonyLib;
using Nautilus.Utility;
using UnityEngine;
using static Player;

namespace WarpStabilizationSuit
{
    [HarmonyPatch(typeof(Player))]
    internal static class Player_Patches
    {
        private static Texture defaultSuitTexture;
        private static Texture defaultSuitSpec;
        private static Texture defaultArmsTexture;
        private static Texture defaultArmsSpec;

        private static Texture warpSuitTexture;
        private static Texture warpSuitSpec;
        private static Texture warpArmsTexture;
        private static Texture warpArmsSpec;

        [HarmonyPatch(nameof(Player.Start)), HarmonyPostfix]
        private static void Start_Patch()
        {
            string suitFilePath = Main_Plugin.AssetsFolderPath + "/Textures/player_02_reinforced_suit_01_body_WARP.png";
            warpSuitTexture = ImageUtils.LoadTextureFromFile(suitFilePath);

            string suitSpecFilePath = Main_Plugin.AssetsFolderPath + "/Textures/player_02_reinforced_suit_01_body_spec_WARP.png";
            warpSuitSpec = ImageUtils.LoadTextureFromFile(suitSpecFilePath);

            string armsFilePath = Main_Plugin.AssetsFolderPath + "/Textures/player_02_reinforced_suit_01_arms_WARP.png";
            warpArmsTexture = ImageUtils.LoadTextureFromFile(armsFilePath);

            string armsSpecFilePath = Main_Plugin.AssetsFolderPath + "/Textures/player_02_reinforced_suit_01_arms_spec_WARP.png";
            warpArmsSpec = ImageUtils.LoadTextureFromFile(armsSpecFilePath);
        }

        [HarmonyPatch(nameof(Player.EquipmentChanged)), HarmonyPostfix]
        private static void EquiupmentChanged_Patch()
        {
            Player player = main;
            Equipment equipment = Inventory.main.equipment;

            for (int i = 0; i < player.equipmentModels.Length; i++)
            {
                Player.EquipmentType equipmentType = player.equipmentModels[i];
                TechType techTypeInSlot = equipment.GetTechTypeInSlot(equipmentType.slot);
                bool flag = false;

                bool hasSuit = false;
                bool hasGloves = false;
                GameObject suitModel = null;
                GameObject glovesModel = null;

                for (int j = 0; j < equipmentType.equipment.Length; j++)
                {
                    EquipmentModel equipmentModel = equipmentType.equipment[j];

                    if (equipmentModel.techType == TechType.ReinforcedDiveSuit)
                    {
                        defaultSuitTexture = equipmentModel.model.GetComponent<Renderer>().materials[0].GetTexture("_MainTex");
                        defaultSuitSpec = equipmentModel.model.GetComponent<Renderer>().materials[0].GetTexture(ShaderPropertyID._SpecTex);

                        defaultArmsTexture = equipmentModel.model.GetComponent<Renderer>().materials[1].GetTexture("_MainTex");
                        defaultArmsSpec = equipmentModel.model.GetComponent<Renderer>().materials[1].GetTexture(ShaderPropertyID._SpecTex);
                    }
 
                    bool hasWarpSuit = equipmentModel.techType == TechType.ReinforcedDiveSuit && techTypeInSlot == Suit_Craftable.techType;
                    bool hasWarpGloves = equipmentModel.techType == TechType.ReinforcedGloves && techTypeInSlot == Gloves_Craftable.techType;

                    if (hasWarpSuit) hasSuit = true;
                    if (hasWarpGloves) hasGloves = true;

                    flag = (flag || hasWarpSuit || hasWarpGloves);
                    if (hasWarpSuit)
                    {
                        if (equipmentModel.model)
                        {
                            suitModel = equipmentModel.model;
                            equipmentModel.model.SetActive(hasWarpSuit);
                        }
                    }
                    else if(equipmentModel.techType == TechType.ReinforcedDiveSuit)
                    {
                        suitModel = equipmentModel.model;
                    }

                    if(hasWarpGloves)
                    {
                        if (equipmentModel.model)
                        {
                            glovesModel = equipmentModel.model;
                            equipmentModel.model.SetActive(hasWarpGloves);
                        }
                    }
                    else if (equipmentModel.techType == TechType.ReinforcedGloves)
                    {
                        glovesModel = equipmentModel.model;
                    }

                    if (techTypeInSlot == TechType.ReinforcedDiveSuit || techTypeInSlot == TechType.ReinforcedGloves)
                    {
                        flag = true;
                    }
                }

                if (equipmentType.defaultModel)
                {
                    equipmentType.defaultModel.SetActive(!flag);
                }

                SetWarpColors(suitModel, glovesModel, hasSuit, hasGloves);
            }
        }

        private static void SetWarpColors(GameObject suitModel, GameObject glovesModel, bool hasWarpSuit, bool hasWarpGloves)
        {
            if (!hasWarpSuit && !hasWarpGloves)
            {
                return;
            }

            if (suitModel != null)
            {
                Renderer suitRenderer = suitModel.GetComponent<Renderer>();

                if (hasWarpSuit)
                {
                    suitRenderer.materials[0].SetTexture("_MainTex", warpSuitTexture);
                    suitRenderer.materials[0].SetTexture(ShaderPropertyID._SpecTex, warpSuitSpec);

                    suitRenderer.materials[1].SetTexture("_MainTex", warpArmsTexture);
                    suitRenderer.materials[1].SetTexture(ShaderPropertyID._SpecTex, warpArmsSpec);
                }
                else
                {
                    suitRenderer.materials[0].SetTexture("_MainTex", defaultSuitTexture);
                    suitRenderer.materials[0].SetTexture(ShaderPropertyID._SpecTex, defaultSuitSpec);

                    suitRenderer.materials[1].SetTexture("_MainTex", defaultArmsTexture);
                    suitRenderer.materials[1].SetTexture(ShaderPropertyID._SpecTex, defaultArmsSpec);
                }
            }

            if (glovesModel != null)
            {
                Renderer glovesRenderer = glovesModel.GetComponent<Renderer>();

                if (hasWarpGloves)
                {
                    glovesRenderer.material.SetTexture("_MainTex", warpArmsTexture);
                    glovesRenderer.materials[0].SetTexture(ShaderPropertyID._SpecTex, warpArmsSpec);
                }
                else
                {
                    glovesRenderer.material.SetTexture("_MainTex", defaultArmsTexture);
                    glovesRenderer.materials[0].SetTexture(ShaderPropertyID._SpecTex, defaultArmsSpec);
                }
            }
        }

        [HarmonyPatch(nameof(Player.HasReinforcedSuit)), HarmonyPostfix]
        private static void Suit_Patch(ref bool __result)
        {
            if(Inventory.main.equipment.GetTechTypeInSlot("Body") == Suit_Craftable.techType)
            {
                __result = true;
            }
        }

        [HarmonyPatch(nameof(Player.HasReinforcedGloves)), HarmonyPostfix]
        private static void Gloves_Patch(ref bool __result)
        {
            if(Inventory.main.equipment.GetTechTypeInSlot("Gloves") == Gloves_Craftable.techType)
            {
                __result = true;
            }
        }
    }
}