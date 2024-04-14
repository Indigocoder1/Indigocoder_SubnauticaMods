using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace CustomCraftGUI.Monobehaviors
{
    public class CustomItem : MonoBehaviour
    {
        public List<CraftData.Ingredient> ingredients { get; private set; }
        public List<CraftData.Ingredient> linkedItems { get; private set; }

        public TextMeshProUGUI nameText;
        public uGUI_ItemIcon icon;
        public float iconScalar;

        private CustomItemsManager manager;
        public string itemID { get; private set; }
        public string displayName { get; private set; }
        public Atlas.Sprite itemSprite { get; private set; }
        public int amountCrafted { get; private set; }
        public bool unlockAtStart { get; private set; }
        public string[] fabricatorPath { get; private set; }
        public Vector2Int itemSize { get; private set; }
        public string tooltip { get; private set; }

        public void SetItemID(string itemID)
        {
            this.itemID = itemID;
            nameText.text = itemID;
        }
        public void SetDisplayName(string name)
        {
            displayName = name;
        }
        public void SetItemSprite(Atlas.Sprite sprite)
        {
            itemSprite = sprite;
            icon.SetForegroundSprite(sprite);
            icon.foreground.transform.localScale = Vector3.one * iconScalar;
        }
        public void SetItemsManager(CustomItemsManager manager)
        {
            this.manager = manager;
        }
        public void SetCurrentItem()
        {
            manager.SetCurrentItem(this);
        }
        public void SetAmountCrafted(int amountCrafted)
        {
            this.amountCrafted = amountCrafted;
        }
        public void SetUnlockAtStart(bool unlockAtStart)
        {
            this.unlockAtStart = unlockAtStart;
        }
        public void SetFabricatorPath(string[] fabricatorPath)
        {
            this.fabricatorPath = fabricatorPath;
        }
        public void SetItemSize(Vector2Int size)
        {
            this.itemSize = size;
        }
        public void SetTooltip(string tooltip)
        {
            this.tooltip = tooltip;
        }
        public void SetIngredients(List<CraftData.Ingredient> ingredients)
        {
            this.ingredients = ingredients;
        }
        public void SetLinkedItems(List<CraftData.Ingredient> linkedItems)
        {
            this.linkedItems = linkedItems;
        }
    }
}
