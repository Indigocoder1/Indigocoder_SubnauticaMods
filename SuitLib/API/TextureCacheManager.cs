using SuitLib.TextureManagement;

namespace SuitLib.API;

public static class TextureCacheManager
{
    /// <summary>
    /// Resets the vanilla texture cache for all suits.
    /// All textures currently set on those suit models will be set as the new vanilla textures.
    /// Make sure you do not have incorrect textures on disabled models when you call this method.
    ///
    /// Note: This will do nothing if not called in the main game scene (When Player.main is not equal to null)
    /// </summary>
    public static void ResetVanillaTextureCache()
    {
        PlayerTextureManager.InitializeAllSuitInfos();
    }
}