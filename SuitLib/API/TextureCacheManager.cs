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
    /// <remarks>
    /// Cache holds will be respected when calling this method, so the textures may not instantly be updated
    /// </remarks>
    public static void RequestTextureCacheReset()
    {
        PlayerTextureManager.InitializeAllSuitInfosAsync();
    }
    
    /// <summary>
    /// Resets the vanilla texture cache for all suits.
    /// All textures currently set on those suit models will be set as the new vanilla textures.
    /// Make sure you do not have incorrect textures on disabled models when you call this method.
    ///
    /// Note: This will do nothing if not called in the main game scene (When Player.main is not equal to null)
    /// </summary>
    /// <remarks>
    /// Cache holds will NOT be respected when calling this method.
    /// All previous holds will be canceled, so async holds may be only partially completed. Use caution when using this method
    /// </remarks>
    public static void ForceTextureCacheReset()
    {
        CacheHoldManager.ClearAllHolds();
        PlayerTextureManager.CancelAsyncTextureSetup();
        PlayerTextureManager.InitializeVanillaTextures();
    }
}