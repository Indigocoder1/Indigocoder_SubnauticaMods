using UnityEngine;
using TMPro;
using CustomCraftGUI.Utilities;

namespace CustomCraftGUI.Monobehaviors
{
    internal class IngredientItem : MonoBehaviour
    {
        public uGUI_ItemIcon iconImage;
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI amountText;
        public TechType techType { get; private set; }

        public void SetInfo(Atlas.Sprite icon, TechType techType, int amount)
        {
            iconImage.SetForegroundSprite(icon);
            iconImage.foreground.transform.localScale = Vector3.one * 0.3f * SpriteSizeFormatter.GetSpriteShrinkScalar(icon);

            nameText.text = Language.main.Get(techType.ToString());
            amountText.text = $"x{amount.ToString()}";

            this.techType = techType;
        }
    }
}
