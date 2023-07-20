----On adding lifepod textures----

You will need to add a new TextureConfigItem to the TextureConfigList
This can be done by editing the Json file in the BepInEx/Configs/TextureReplacer folder
To generate the config folder you have to run the game first

To add a new item it will need to be formatted in a specefic way
Here is an example of an item that could be added to the list:
{
    "lifepodIndex": 0,
    "materialIndex": 0,
    "fileName": "life_pod_exterior_exploded_01.png",
    "prefabClassID": "66cc5a83-142b-4d8d-8d16-2d6e960f59c3",
    "rendererHierchyPath": "life_pod_exploded_02/life_pod",
    "isVariation": false,
    "variationChance": -1.0
}

lifepodIndex is the index of the lifepod. To see what index a lifepod is, view the enum in the LifepodTextureReplacer class on my github, here: https://github.com/Indigocoder1/Indogocoder_SubnauticaMods/tree/master/Texture%20Replacer
materialIndex is the index of the material on the renderer. To see the full list of index to texture conversion, check the bottom of the ReadMe. (0 Based indexing)
fileName is the name of the texture (Including extension) that is replacing the texture of the specefied materialIndex. This file will have to be placed in the Assets folder for it to work
prefabClassID is the classID of the prefab you're replacing. For lifepods a list can be found on my github in the LifepodTextureReplacer class. For other prefabs use Lee23's runtime editor mod to locate the prefab.
Once you've located the prefab click on the PrefabIdentifier script and copy the ClassID
rendererHierchyPath is the path in the hierchy to the child from the prefab that contains the renderer you're trying to replace
isVariation is if this texture is a variation of another texture. If this is set to true, variationChance must be greater than 0 to do anything
variaionChance is the chance (From 0 to 1) of this texture being chosen instead of another one. Note that this is only impactful if isVariation is true
*Side note, I usually set variationChance to -1 when isVariation is false so I know it isn't set to true by accident


Taking into account this explanation, the above line replaces the texture in materialIndex 0, with "life_pod_exterior_exploded_01.png" for lifepod 2

To actually add this to the Json, you need to put it in like this:
[
	*Item here*
]

So using our above example would look like this:
[
	{
        "lifepodIndex": 0,
        "materialIndex": 0,
        "fileName": "life_pod_exterior_exploded_01.png",
        "prefabClassID": "66cc5a83-142b-4d8d-8d16-2d6e960f59c3",
        "rendererHierchyPath": "life_pod_exploded_02/life_pod",
        "isVariation": false,
        "variationChance": -1.0
    }
]

Keep in mind that when adding multiple textures, you have to put a comma after each texture you add.
Here is an example of multiple textures:

[
    {
        "lifepodIndex": 0,
        "materialIndex": 0,
        "fileName": "life_pod_exterior_exploded_01.png",
        "prefabClassID": "66cc5a83-142b-4d8d-8d16-2d6e960f59c3",
        "rendererHierchyPath": "life_pod_exploded_02/life_pod",
        "isVariation": false,
        "variationChance": -1.0
    }, <-- Note the comma
    {
        "lifepodIndex": 0,
        "materialIndex": 1,
        "fileName": "life_pod_exterior_exploded_02.png",
        "prefabClassID": "66cc5a83-142b-4d8d-8d16-2d6e960f59c3",
        "rendererHierchyPath": "life_pod_exploded_02/life_pod",
        "isVariation": false,
        "variationChance": -1.0
    }
]

Lifepod material index to texture conversion:

Index 0 - life_pod_exterior_exploded_01
Index 1 - life_pod_exterior_exploded_02
Index 2 - life_pod_exterior_exploded_06_decals_01
Index 3 - starship_exploded_exterrior_hull_dirt_decals_03
Index 4 - starship_exploded_exterrior_hull_dirt_decals
Index 5 - starship_exploded_exterrior_damaged_tile_02_01
Index 6 - life_pod_exterior_exploded_02_alpha
Index 7 - life_pod_exterior_exploded_06_decals_02

----On adding custom prefab textures----

The process is the same as replacing lifepod textures, except that you don't need a lifepod index