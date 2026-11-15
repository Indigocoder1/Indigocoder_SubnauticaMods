using System;
using System.Collections;
using System.Collections.Generic;

namespace SuitLib.API;

public static class CacheHoldManager
{
    private static readonly List<Func<IEnumerator>> AsyncWaitTasks = new();
    private static readonly List<Func<bool>> SynchronousWaitTasks = new();
    private static readonly HashSet<object> GenericWaitTasks = new();
    
    internal static void OnHoldRequested(object[] input)
    {
        if (input.Length != 1) return;

        if (input[0] == null) return;
        
        switch (input[0])
        {
            case Func<IEnumerator> asyncTask:
                RegisterCacheHold(asyncTask);
                break;
            case Func<bool> syncTask:
                RegisterCacheHold(syncTask);
                break;
            default:
                RegisterCacheHold(input[0]);
                break;
        }
    }
    
    internal static void OnHoldFreed(object[] input)
    {
        if (input.Length != 1) return;
        
        if (input[0] == null) return;

        FreeCacheHold(input[0]);
    }

    internal static IEnumerator WaitForHoldsAsync()
    {
        foreach (var task in AsyncWaitTasks)
        { 
            yield return task;
        }

        foreach (var task in SynchronousWaitTasks)
        {
            while (!task())
            {
                yield return null;
            }
        }

        while (GenericWaitTasks.Count > 0)
        {
            yield return null;
        }
    }

    /// <summary>
    /// The passed task will be run before caching the suits' vanilla textures
    /// </summary>
    /// <remarks>
    /// Holds must be registered before the cache initialization and will be reset after exiting to the main menu
    /// </remarks>
    /// <param name="waitTask">The task to wait for</param>
    public static void RegisterCacheHold(Func<IEnumerator> waitTask)
    {
        AsyncWaitTasks.Add(waitTask);
    }
    
    /// <summary>
    /// Suit library will wait for the passed function to return true before caching the suits' vanilla textures 
    /// </summary>
    /// <remarks>
    /// Holds must be registered before the cache initialization and will be reset after exiting to the main menu
    /// </remarks>
    /// <param name="waitTask">The task to wait for</param>
    public static void RegisterCacheHold(Func<bool> waitTask)
    {
        SynchronousWaitTasks.Add(waitTask);
    }
    
    /// <summary>
    /// Suit library will wait until <see cref="FreeCacheHold"/> is called with the same
    /// owner before caching the suits' vanilla textures 
    /// </summary>
    /// <remarks>
    /// Holds must be registered before the cache initialization and will be reset after exiting to the main menu
    /// </remarks>
    /// <param name="owner">The owner of the cache hold. Must also be passed into <see cref="FreeCacheHold"/> to free the hold</param>
    public static void RegisterCacheHold(object owner)
    {
        GenericWaitTasks.Add(owner);
    }
    
    /// <summary>
    /// Frees a texture hold registered with <see cref="RegisterCacheHold(object)"/>. The passed owner must be the same
    /// one used to register the hold.
    /// </summary>
    /// <param name="owner">The owner of the cache hold</param>
    public static void FreeCacheHold(object owner)
    {
        GenericWaitTasks.Remove(owner);
    }
}