This mod makes it easy for modders to add modded suits that have the correct models on the player. You can also recolor/retexture them easily as well. This can produce suits such as the one seen in the Warp Stabilization Suit mod (Disclaimer: I made that mod)

Json Info
You can also create suits through Jsons. These suits will always be loaded when you are wearing the suit defined in the vanillaModel input.
vanillaModel numbers:

    1. Dive Suit
    2. Radiation Suit
    3. Reinforced Suit
    4. Stillsuit


Modification numbers:

    1. Reinforced
    2. Filtration
    3. Reinforced & Filtration

(0 is none)

Tutorial for creating JSON suits: https://youtu.be/9YZoy6IrxXE

Join the [SN Modding Discord](https://discord.gg/UpWuWwq) and @Gaming For Fun for more info

To add a suit you simply need to reference SuitLib by doing

`using SuitLib`

and then add your suits (or gloves!) by doing

```cs
Dictionary<string, Texture2D> suitKeyValuePairs = new Dictionary<string, Texture2D> { { "_MainTex", mySuitTexture } };
ModdedSuit mySuit = new ModdedSuit(suitKeyValuePairs, ModdedSuitsManager.VanillaModel.Reinforced, Suit_Craftable.techType);
ModdedSuitsManager.AddModdedSuit(mySuit);
```


All of these values can of course be tweaked to your liking.
The library currently supports the vanilla suit models but may support custom models in the future.

Source code: https://github.com/Indigocoder1/Indigocoder_SubnauticaMods/tree/master/SuitLib