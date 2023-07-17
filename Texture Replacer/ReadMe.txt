On adding custom textures:

You will need to add a new TextureConfigItem to the TextureConfigList
This can be done by editing the Json file in the Configs folder

To add a new item it will need to be formatted in a specefic way
Here is an example of an item that could be added to the list:
{"materialIndex":0,"fileName":"life_pod_exterior_exploded_01.png","lifepodNumberIndex":2,"isVariation":false,"variationChance":-1}

The materialIndex is the index of the material on the renderer. To see the full list of index to texture conversion, check the bottom of the ReadMe. (0 Based indexing)
fileName is the name of the texture (Including extension) that is replacing the texture of the specefied materialIndex. This file will have to be placed in the Assets folder for it to work
lifepodNumberIndex is the index of the lifepod. To see what index a lifepod is, view the dictionary in the Main class on my github, here: https://github.com/Indigocoder1/Indogocoder_SubnauticaMods/tree/master/Texture%20Replacer
isVariation is if this texture is a variation of another texture. If this is set to true, variationChance must be greater than 1
variaionChance is the chance (From 0 to 1) of this texture being chosen instead of another one. Note that this is only useful if isVariation is true
*Side note, I usually set variationChance to -1 when isVariation is false so I know it isn't set to true by accident


Taking into account this explanation, the above line replaces the texture in materialIndex 0, with "life_pod_exterior_exploded_01.png" for lifepod 4

To actually add this to the Json, you need to put it in like this:

{
	"textureConfigs":
	[
		*Item here*
	]
}

So using our above example would look like this:

{
	"textureConfigs":
	[
		{"materialIndex":0,"fileName":"life_pod_exterior_exploded_01.png","lifepodNumberIndex":2,"isVariation":false,"variationChance":-1}
	]
}

Keep in mind that when adding multiple textures, you have to put a comma after each texture you add.
Here is an example of multiple textures:

{
	"textureConfigs":
	[
		{"materialIndex":0,"fileName":"life_pod_exterior_exploded_01.png","lifepodNumberIndex":2,"isVariation":false,"variationChance":0.75}, <-- Note the comma
		{"materialIndex":0,"fileName":"life_pod_exterior_exploded_01.png","lifepodNumberIndex":2,"isVariation":false,"variationChance":0.75}
	]
}

Lifepod material index to texture conversion:

Index 0 - life_pod_exterior_exploded_01
Index 1 - life_pod_exterior_exploded_02
Index 2 - life_pod_exterior_exploded_06_decals_01
Index 3 - starship_exploded_exterrior_hull_dirt_decals_03
Index 4 - starship_exploded_exterrior_hull_dirt_decals
Index 5 - starship_exploded_exterrior_damaged_tile_02_01
Index 6 - life_pod_exterior_exploded_02_alpha
Index 7 - life_pod_exterior_exploded_06_decals_02