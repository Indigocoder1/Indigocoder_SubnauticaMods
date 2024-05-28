using CustomCraftGUI.Structs;
using CustomCraftGUI.Utilities;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace CustomCraftGUI.Monobehaviors
{
    public class Item : MonoBehaviour
    {
        public TextMeshProUGUI nameText;
        public uGUI_ItemIcon icon;
        public float iconScalar;
        public CustomItemInfo customItemInfo;

        public void SetNameText(string text)
        {
            nameText.text = text;
        }

        public void SetItemSprite(Atlas.Sprite sprite)
        {
            customItemInfo.SetItemSprite(sprite);
            icon.SetForegroundSprite(sprite);
            icon.foreground.transform.localScale = Vector3.one * iconScalar * SpriteSizeFormatter.GetSpriteShrinkScalar(sprite);
        }
    }
}
