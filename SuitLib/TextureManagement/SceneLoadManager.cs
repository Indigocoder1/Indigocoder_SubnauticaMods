using SuitLib.API;
using UnityEngine.SceneManagement;

namespace SuitLib.TextureManagement;

public static class SceneLoadManager
{
    private static bool _hasLoadedInitialMainMenu;
    
    public static void OnSceneLoaded(Scene scene, LoadSceneMode loadMode)
    {
        if (scene.name != "StartScreen") return;

        if (!_hasLoadedInitialMainMenu)
        {
            _hasLoadedInitialMainMenu = true;
            return;
        }

        CacheHoldManager.ClearAllHolds();
    }
}