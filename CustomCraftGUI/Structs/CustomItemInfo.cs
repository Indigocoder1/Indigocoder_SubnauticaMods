using CustomCraftGUI.Monobehaviors;
using CustomCraftGUI.Utilities;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace CustomCraftGUI.Structs
{
    public class CustomItemInfo
    {
        public List<CraftData.Ingredient> ingredients { get; private set; } = null;
        public List<CraftData.Ingredient> linkedItems { get; private set; } = null;

        public string itemID { get; private set; }
        public Atlas.Sprite itemSprite { get; private set; }
        public int amountCrafted { get; private set; } = 1;
        public bool unlockAtStart { get; private set; }

        public CustomItemInfo()
        {

        }

        public CustomItemInfo(string itemID, Atlas.Sprite itemSprite, int amountCrafted, bool unlockAtStart)
        {
            this.itemID = itemID;
            this.itemSprite = itemSprite;
            this.amountCrafted = amountCrafted;
            this.unlockAtStart = unlockAtStart;
        }

        public void SetItemID(string itemID) => this.itemID = itemID;
        public void SetAmountCrafted(int amountCrafted) => this.amountCrafted = amountCrafted;
        public void SetUnlockAtStart(bool unlockAtStart) => this.unlockAtStart = unlockAtStart;
        public void SetItemSprite(Atlas.Sprite sprite) => this.itemSprite = sprite;
        public void SetIngredients(List<CraftData.Ingredient> ingredients) => this.ingredients = ingredients;
        public void SetLinkedItems(List<CraftData.Ingredient> linkedItems) => this.linkedItems = linkedItems;
    }
}
