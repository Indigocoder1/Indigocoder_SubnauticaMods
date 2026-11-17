using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Nautilus.Utility;
using SuitLib.API;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Nautilus.Utility.ModMessages;
using SuitLib.API.JSON;
using SuitLib.TextureManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SuitLib;

[BepInPlugin(Guid, PluginName, VersionString)]
[BepInDependency("com.snmodding.nautilus")]
internal class Main : BaseUnityPlugin
{
    private const string Guid = "Indigocoder.SuitLib";
    private const string PluginName = "SuitLib";
    private const string VersionString = "2.1.0";

    public static ManualLogSource logger;
    private static readonly Harmony Harmony = new(Guid);
    private static readonly string JsonFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Json Files");
    private static readonly string JsonTexturesFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Json Textures");
    private static readonly ModInbox ModInbox = new("SuitLibrary");
    
    internal static Eatable WaterPrefab;

    private IEnumerator Start()
    {
        logger = Logger;
        
        Harmony.PatchAll();

        ModInbox.AddMessageReader(new BasicModMessageReader("RequestCacheHold", CacheHoldManager.OnHoldRequested));
        ModInbox.AddMessageReader(new BasicModMessageReader("FreeCacheHold", CacheHoldManager.OnHoldFreed));
        ModInbox.AddMessageReader(new BasicModMessageReader("RequestCacheReset", _ => TextureCacheManager.RequestTextureCacheReset()));
        ModInbox.AddMessageReader(new BasicModMessageReader("ForceCacheReset", _ => TextureCacheManager.ForceTextureCacheReset()));
        
        SceneManager.sceneLoaded += SceneLoadManager.OnSceneLoaded;
        
        ModMessageSystem.RegisterInbox(ModInbox);
        ModInbox.ReadAnyHeldMessages();
        
        CoroutineTask<GameObject> stillsuitTask = CraftData.GetPrefabForTechTypeAsync(TechType.WaterFiltrationSuit);
        yield return stillsuitTask;
        WaterPrefab = stillsuitTask.result.value.GetComponent<Stillsuit>().waterPrefab;
        
        LoadJsons();

        Logger.LogInfo($"{PluginName} {VersionString} Loaded.");
    }

    private void LoadJsons()
    {
        if (!Directory.Exists(JsonFolder)) Directory.CreateDirectory(JsonFolder);
        if (!Directory.Exists(JsonTexturesFolder)) Directory.CreateDirectory(JsonTexturesFolder);

        AddModdedSuits();
        logger.LogInfo("Finished loading suit Jsons");

        AddModdedGloves();
        logger.LogInfo("Finished loading gloves Jsons");
    }

    private void AddModdedSuits()
    {
        List<JsonSuit> suits = JsonLoader<JsonSuit>.LoadJsons(JsonFolder);

        foreach (JsonSuit suit in suits)
        {
            if (suit == null) continue;

            Dictionary<string, Texture2D> suitKVPs = new Dictionary<string, Texture2D>();
            Dictionary<string, Texture2D> armKVPs = new Dictionary<string, Texture2D>();
            bool example = false;

            if (suit.suitFileNamePairs == null || suit.armFileNamePairs == null)
            {
                continue;
            }

            foreach (var suitKey in suit.suitFileNamePairs.Keys)
            {
                if (suit.suitFileNamePairs[suitKey].ToLower().Contains("example"))
                {
                    example = true;
                    break;
                }

                string path = Path.Combine(JsonTexturesFolder, suit.suitFileNamePairs[suitKey]);
                suitKVPs.Add(suitKey, ImageUtils.LoadTextureFromFile(path));
            }
            foreach (var armsKey in suit.armFileNamePairs.Keys)
            {
                string path = Path.Combine(JsonTexturesFolder, suit.armFileNamePairs[armsKey]);
                armKVPs.Add(armsKey, ImageUtils.LoadTextureFromFile(path));
            }

            if (example)
            {
                logger.LogWarning($"Skipping a json suit because it's example data");
                break;
            }

            ModdedSuit moddedSuit = new ModdedSuit(suitKVPs, armKVPs, suit.vanillaModel, ModdedSuitsManager.suitModelTechTypes[suit.vanillaModel], suit.modifications, suit.stillsuitValues);
            moddedSuit.jsonSuit = true;
            ModdedSuitsManager.AddModdedSuit(moddedSuit);
        }
    }

    private void AddModdedGloves()
    {
        List<JsonGloves> gloves = JsonLoader<JsonGloves>.LoadJsons(JsonFolder);

        foreach (JsonGloves item in gloves)
        {
            if (item == null) continue;

            if (item.gloveTexturePairs == null)
            {
                continue;
            }

            Dictionary<string, Texture2D> armKVPs = new Dictionary<string, Texture2D>();
            bool example = false;

            foreach (var glovesKey in item.gloveTexturePairs.Keys)
            {
                if (item.gloveTexturePairs[glovesKey].ToLower().Contains("example"))
                {
                    example = true;
                    break;
                }

                string path = Path.Combine(JsonTexturesFolder, item.gloveTexturePairs[glovesKey]);
                armKVPs.Add(glovesKey, ImageUtils.LoadTextureFromFile(path));
            }

            if (example)
            {
                logger.LogWarning($"Skipping a json suit because it's example data");
                break;
            }

            ModdedGloves moddedGloves = new ModdedGloves(armKVPs, item.vanillaModel, ModdedSuitsManager.gloveModelTechTypes[item.vanillaModel], item.modifications, item.stillsuitValues);
            moddedGloves.jsonGloves = true;
            ModdedSuitsManager.AddModdedGloves(moddedGloves);
        }
    }
}