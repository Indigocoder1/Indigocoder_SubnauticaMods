using System.Collections.Generic;
using SuitLib.API;
using UnityEngine;

namespace SuitLib.TextureManagement;

internal struct SuitTextureInfo()
{
    public GameObject suitBodyObject;
    public GameObject suitGlovesObject;

    private readonly ModdedSuitsManager.VanillaModel suitModel;
    private readonly Dictionary<string, Texture> vanillaSuitTextures = new();
    private readonly Dictionary<string, Texture> vanillaGloveTextures = new();
    private readonly bool glovesUseBodyModel;

    public SuitTextureInfo(ModdedSuitsManager.VanillaModel suitModel, bool glovesUseBodyModel = false) : this()
    {
        this.suitModel = suitModel;
        this.glovesUseBodyModel = glovesUseBodyModel;
    }

    public void AddBodyTexture(string textureName)
    {
        var firstIndex = MaterialIndexManager.ModelBodyTexIndices[suitModel][0];
        var suitTex = suitBodyObject.GetComponent<Renderer>().materials[firstIndex].GetTexture(textureName);
        vanillaSuitTextures.Add(textureName, suitTex);
    }
    
    public void AddGlovesTexture(string textureName)
    {
        var materialIndex = MaterialIndexManager.ModelArmTexIndices[suitModel][0];
        
        var glovesTex = suitBodyObject.GetComponent<Renderer>().materials[materialIndex].GetTexture(textureName);
        vanillaGloveTextures.Add(textureName, glovesTex);
    }

    public void ResetBodyTextures()
    {
        var bodyRend = suitBodyObject.GetComponent<Renderer>();
        foreach (var key in vanillaSuitTextures.Keys)
        {
            var bodyTex = (Texture2D)vanillaSuitTextures[key];
            foreach (var index in MaterialIndexManager.ModelBodyTexIndices[suitModel])
            {
                bodyRend.materials[index].SetTexture(key, bodyTex);
            }
        }

        // The arm textures are stored in the gloves texture, not the body one
        foreach (var key in vanillaGloveTextures.Keys)
        {
            var armsTex = (Texture2D)vanillaGloveTextures[key];
            foreach (var index in MaterialIndexManager.ModelArmTexIndices[suitModel])
            {
                bodyRend.materials[index].SetTexture(key, armsTex);
            }
        }
    }

    public void ResetGloveTextures()
    {
        if (suitGlovesObject == null) return;
        
        var rend = suitGlovesObject.GetComponent<Renderer>();
        foreach (var key in vanillaGloveTextures.Keys)
        {
            var tex = (Texture2D)vanillaGloveTextures[key];
            foreach (var index in MaterialIndexManager.ModelGloveTexIndices[suitModel])
            {
                rend.materials[index].SetTexture(key, tex);
            }
        }
    }
}