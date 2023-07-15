On adding custom textures:

You will need to add a new TextureConfigItem to the TextureConfigList
This can be done by editing the Json file in the Configs folder

To add a new item it will need to be formatted in a specefic way
Here is an example of an item that could be added to the list:
{"materialIndex":0,"fileName":"life_pod_exterior_exploded_01.png","lifepodNumberIndex":2,"isVariation":false,"variationChance":-1}

The materialIndex is the index of the material on the renderer. It is 0 for life_pod_exterior_exploded_01 and 1 for life_pod_exterior_exploded_02, etc. (0 Based indexing)
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