using HarmonyLib;
using System.Collections;
using UnityEngine;

namespace WarpStabilizationSuit
{
    [HarmonyPatch(typeof(Player))]
    internal static class Player_Patches
    {
        [HarmonyPatch(nameof(Player.Start)), HarmonyPostfix]
        private static void Start_Patch()
        {
            //UWE.CoroutineHost.StartCoroutine(SetupGlovesVisual());
        }

        [HarmonyPatch(nameof(Player.HasReinforcedSuit)), HarmonyPostfix]
        private static void Suit_Patch(ref bool __result)
        {
            if(Inventory.main.equipment.GetTechTypeInSlot("Body") == Suit_Craftable.suitTechType)
            {
                __result = true;
            }
        }

        [HarmonyPatch(nameof(Player.HasReinforcedGloves)), HarmonyPostfix]
        private static void Gloves_Patch(ref bool __result)
        {
            if(Inventory.main.equipment.GetTechTypeInSlot("Gloves") == Gloves_Craftable.glovesTechType)
            {
                __result = true;
            }
        }

        private static IEnumerator SetupGlovesVisual()
        {
            Player player = Player.main;

            CoroutineTask<GameObject> task = CraftData.GetPrefabForTechTypeAsync(TechType.Player);
            yield return task;

            GameObject prefabModel = task.GetResult().transform.Find("body/player_view/male_geo/reinforcedSuit").gameObject;

            Player.EquipmentModel[] model = new Player.EquipmentModel[]
            {
                new Player.EquipmentModel
                {
                    techType = Gloves_Craftable.glovesTechType,
                    model = prefabModel
                }
            };
        }
    }
}
