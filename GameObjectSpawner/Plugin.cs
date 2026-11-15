using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace GameObjectSpawner
{
    [BepInPlugin(guid, pluginName, version)]
    [BepInDependency("com.snmodding.nautilus")]
    public class Plugin : BaseUnityPlugin
    {
        private const string guid = "com.indigocoder.gameobjectspawner";
        private const string pluginName = "GameObject Spawner";
        private const string version = "1.0.0";

        public new static ManualLogSource Logger { get; private set; }

        private static Assembly Assembly { get; } = Assembly.GetExecutingAssembly();

        public readonly static string modFolderPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public static ConfigEntry<KeyCode> spawnKey;
        public static ConfigEntry<int> spawnDistance;
        public static ConfigEntry<float> spawnScaleMultiplier;

        private void Awake()
        {
            // set project-scoped logger instance
            Logger = base.Logger;
   
            BindConfigs();
            new Configs();

            // register harmony patches, if there are any
            Harmony.CreateAndPatchAll(Assembly, $"{guid}");
            Logger.LogInfo($"Plugin {guid} is loaded!");
        }

        private void BindConfigs()
        {
            spawnKey = Config.Bind("GameObject Spawner", "Spawn Key", KeyCode.RightAlt);
            spawnDistance = Config.Bind("GameObject Spawner", "Spawn Distance", 10);
            spawnScaleMultiplier = Config.Bind("GameObject Spawner", "Spawn Scale Multiplier", 1f);
        }
    }
}