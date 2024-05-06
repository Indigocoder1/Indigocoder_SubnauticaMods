using CustomCraftGUI.Utilities;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace CustomCraftGUI.Monobehaviors
{
    public class Item : MonoBehaviour
    {
        public List<CraftData.Ingredient> ingredients { get; protected set; }
        public List<CraftData.Ingredient> linkedItems { get; protected set; }

        public TextMeshProUGUI nameText;
        public uGUI_ItemIcon icon;
        public float iconScalar;

        protected ItemManager manager;
        public string itemID { get; private set; }
        public Atlas.Sprite itemSprite { get; protected set; }
        public int amountCrafted { get; protected set; } = 1;
        public bool unlockAtStart { get; protected set; }

        public virtual void SetItemID(string itemID)
        {
            this.itemID = itemID;
        }
        public virtual void SetNameText(string text)
        {
            nameText.text = text;
        }
        public virtual void SetItemSprite(Atlas.Sprite sprite)
        {
            itemSprite = sprite;
            icon.SetForegroundSprite(sprite);
            icon.foreground.transform.localScale = Vector3.one * iconScalar * SpriteSizeFormatter.GetSpriteShrinkScalar(sprite);
        }
        public virtual void SetItemsManager(ItemManager manager)
        {
            this.manager = manager;
        }
        public virtual void SetAmountCrafted(int amountCrafted)
        {
            this.amountCrafted = amountCrafted;
        }
        public virtual void SetUnlockAtStart(bool unlockAtStart)
        {
            this.unlockAtStart = unlockAtStart;
        }
        public virtual void SetIngredients(List<CraftData.Ingredient> ingredients)
        {
            this.ingredients = ingredients;
        }
        public virtual void SetLinkedItems(List<CraftData.Ingredient> linkedItems)
        {
            this.linkedItems = linkedItems;
        }
        public virtual void SetCurrentItem()
        {
            manager.SetCurrentItem(this);
        }
    }
}
