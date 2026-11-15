using System.Collections.Generic;
using UnityEngine;

namespace SuitLib.TextureManagement;

internal struct SuitTextureInfo()
{
    public GameObject suitBodyObject;
    public GameObject suitGlovesObject;
    
    private readonly Dictionary<string, Texture> vanillaSuitTextures = new();
    private readonly Dictionary<string, Texture> vanillaGloveTextures = new();
    private readonly bool glovesUseBodyIndex1;

    public SuitTextureInfo(bool glovesUseBodyIndex1) : this()
    {
        this.glovesUseBodyIndex1 = glovesUseBodyIndex1;
    }

    public void AddBodyTexture(string textureName)
    {
        var suitTex = suitBodyObject.GetComponent<Renderer>().material.GetTexture(textureName);
        vanillaSuitTextures.Add(textureName, suitTex);
    }
    
    public void AddGlovesTexture(string textureName)
    {
        var obj = glovesUseBodyIndex1 ? suitBodyObject : suitGlovesObject;
        var materialIndex = glovesUseBodyIndex1 ? 1 : 0;
        
        var glovesTex = obj.GetComponent<Renderer>().materials[materialIndex].GetTexture(textureName);
        vanillaGloveTextures.Add(textureName, glovesTex);
    }

    public void ResetBodyTextures()
    {
        foreach (var key in vanillaSuitTextures.Keys)
        {
            var bodyTex = (Texture2D)vanillaSuitTextures[key];
            suitBodyObject.GetComponent<Renderer>().material.SetTexture(key, bodyTex);
        }

        // The arm textures are stored in the gloves texture, not the body one
        foreach (var key in vanillaGloveTextures.Keys)
        {
            var armsTex = (Texture2D)vanillaGloveTextures[key];
            suitBodyObject.GetComponent<Renderer>().materials[1].SetTexture(key, armsTex);
        }
    }

    public void ResetGloveTextures()
    {
        if (suitGlovesObject == null) return;
        
        foreach (var key in vanillaGloveTextures.Keys)
        {
            var tex = (Texture2D)vanillaGloveTextures[key];
            suitGlovesObject.GetComponent<Renderer>().material.SetTexture(key, tex);
        }
    }
}