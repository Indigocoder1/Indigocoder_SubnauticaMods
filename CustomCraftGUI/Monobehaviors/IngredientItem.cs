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
        public InfoPanel infoPanel { get; private set; }
        public ModifiedItemsManager modifiedItemsManager { get; private set; }

        public void SetInfo(Atlas.Sprite icon, TechType techType, int amount, ModifiedItemsManager modifiedItemsManager)
        {
            iconImage.SetForegroundSprite(icon);
            iconImage.foreground.transform.localScale = Vector3.one * 0.3f * SpriteSizeFormatter.GetSpriteShrinkScalar(icon);

            nameText.text = Language.main.Get(techType.ToString());
            amountText.text = $"x{amount}";

            this.techType = techType;
            this.modifiedItemsManager = modifiedItemsManager;
        }

        public void SetInfoPanel(InfoPanel infoPanel)
        {
            this.infoPanel = infoPanel;
        }

        public void SetInfoPanelData()
        {
            ItemIcon icon = new();
            icon.SetTechType(techType);
            icon.SetInfoPanel(infoPanel);

            infoPanel.SetCurrentItem(icon);

            if(modifiedItemsManager != null)
            {
                modifiedItemsManager.SetCurrentIcon(icon);
            }
        }
    }
}
