using System.Collections.Generic;
using UnityEngine;

namespace CustomCraftGUI.Structs
{
    public class CustomItemInfo
    {
        public List<Ingredient> ingredients { get; private set; } = null;
        public List<Ingredient> linkedItems { get; private set; } = null;

        public string itemID { get; private set; }
        public Sprite itemSprite { get; private set; }
        public int amountCrafted { get; private set; } = 1;
        public bool unlockAtStart { get; private set; }

        public CustomItemInfo()
        {

        }

        public CustomItemInfo(string itemID, Sprite itemSprite, int amountCrafted, bool unlockAtStart)
        {
            this.itemID = itemID;
            this.itemSprite = itemSprite;
            this.amountCrafted = amountCrafted;
            this.unlockAtStart = unlockAtStart;
        }

        public void SetItemID(string itemID) => this.itemID = itemID;
        public void SetAmountCrafted(int amountCrafted) => this.amountCrafted = amountCrafted;
        public void SetUnlockAtStart(bool unlockAtStart) => this.unlockAtStart = unlockAtStart;
        public void SetItemSprite(Sprite sprite) => itemSprite = sprite;
        public void SetIngredients(List<Ingredient> ingredients) => this.ingredients = ingredients;
        public void SetLinkedItems(List<Ingredient> linkedItems) => this.linkedItems = linkedItems;
    }
}
