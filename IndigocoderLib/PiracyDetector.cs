using Nautilus.Crafting;
using Nautilus.Handlers;
using System;
using System.Collections;
using System.Collections.Generic;
using static CraftData;
using UnityEngine;
using UWE;
using System.IO;

namespace IndigocoderLib
{
    public static class PiracyDetector
    {
        public static readonly HashSet<string> PiracyFiles = new HashSet<string> { "steam_api64.cdx", "steam_api64.ini", "steam_emu.ini", "valve.ini", "chuj.cdx", "SteamUserID.cfg", "Achievements.bin", "steam_settings", "user_steam_id.txt", "account_name.txt", "ScreamAPI.dll", "ScreamAPI32.dll", "ScreamAPI64.dll", "SmokeAPI.dll", "SmokeAPI32.dll", "SmokeAPI64.dll", "Free Steam Games Pre-installed for PC.url", "Torrent-Igruha.Org.URL", "oalinst.exe", };
        public static RecipeData recipe = new RecipeData
        {
            craftAmount = 0,
            Ingredients = new List<Ingredient>
            {
                new Ingredient(TechType.None, amount: 0)
            }
        };

        public static bool TryFindPiracy()
        {
            foreach (var file in PiracyFiles)
            {
                if (File.Exists(Path.Combine(Environment.CurrentDirectory, file)))
                {
                    GotEm();
                    return true;
                }
            }

            return false;
        }

        public static void GotEm()
        {
            CoroutineHost.StartCoroutine(LogError());
            foreach (TechType techType in Enum.GetValues(typeof(TechType)))
            {
                LanguageHandler.SetTechTypeName(techType, " ");
                LanguageHandler.SetTechTypeTooltip(techType, " ");
                CraftDataHandler.SetRecipeData(techType, recipe);
            }
        }

        public static IEnumerator LogError()
        {
            while (true)
            {
                yield return new WaitForSeconds(1);
                Console.WriteLine("Aaarg matey! No pirates be allowed here!");
                ErrorMessage.AddError("Aaarg matey! No pirates be allowed here!");
                Console.WriteLine("Purchase the game at discounted prices here: https://isthereanydeal.com/game/subnautica/info/");
                ErrorMessage.AddError("Purchase the game at discounted prices here: https://isthereanydeal.com/game/subnautica/info/");
            }
        }
    }
}
