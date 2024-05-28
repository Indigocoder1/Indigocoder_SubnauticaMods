using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Ingredient = CraftData.Ingredient;

namespace CustomCraftGUI.Monobehaviors
{
    public class ModifiedItemsManager : ItemManager
    {
        private const string CACHE_NOT_SET_WARNING =
            "<color=ff0000>The default tech has not been cached; " +
            "the unlock at start toggle and unlocks may have incorrect values.\n" +
            "To fix this, open a saved game, then return to the Custom Craft Editor</color>";

        [Header("Modified Items Manager")]
        public TextMeshProUGUI itemText;
        public Animator unlocksButtonAnimator;
        public Transform unlocksParent;

        private ItemIcon currentItemIcon;
        private bool unlocksActive;

#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
        private ModifiedItem currentItem
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword
        {
            get
            {
                return base.currentItem as ModifiedItem;
            }
            set
            {
                base.currentItem = value;
            }
        }

        public override List<Item> Items 
        { 
            get
            {
                return _modifiedItems;
            }
            protected set
            {
                _modifiedItems = value;
            }
        }

        private List<Item> _modifiedItems = new();

        public override void SetCurrentItem(Item item)
        {
            currentItem = item as ModifiedItem;

            itemText.text = Language.main.Get(currentItem.customItemInfo.itemID);

            Atlas.Sprite sprite = SpriteManager.Get(((ModifiedItem)currentItem).techType);
            itemIcon.SetForegroundSprite(sprite);

            UpdateAllLists();
        }

        public void SetCurrentIcon(ItemIcon itemIcon)
        {
            currentItemIcon = itemIcon;
        }

        public void AddCurrentItemToList()
        {
            if(currentItemIcon == null)
            {
                return;
            }

            foreach (var item in Items)
            {
                //Don't add duplicates
                if((item as ModifiedItem).techType == currentItemIcon.techType)
                {
                    return;
                }
            }

            GameObject newCustomItem = Instantiate(itemPrefab, itemsParent);
            currentItem = newCustomItem.GetComponent<ModifiedItem>();
            Items.Add(currentItem);

            SetNewItemInfo();

            ITechData techData = CraftData.Get(currentItemIcon.techType, true);
            if (techData == null)
            {
                //No ingredients (Not meant to be crafted)
                currentItem.SetUnlocks(new());
                UpdateAllLists();

                return;
            }

            SetIngredientsData(techData);
            SetLinkedItemData(techData);

            currentItem.customItemInfo.SetAmountCrafted(techData.craftAmount);
            amountCraftedInputField.text = techData.craftAmount.ToString();

            TrySetCachedTechData();

            amountCraftedInputField.text = techData.craftAmount.ToString();

            UpdateAllLists();
        }

        private void SetNewItemInfo()
        {
            Atlas.Sprite sprite = SpriteManager.Get(currentItemIcon.techType);
            currentItem.SetItemSprite(sprite);
            currentItem.SetTechType(currentItemIcon.techType);
            currentItem.SetNameText(Language.main.Get(currentItemIcon.techType));
            currentItem.customItemInfo.SetItemID(currentItemIcon.itemName);

            itemText.text = Language.main.Get(currentItem.customItemInfo.itemID);
            itemIcon.SetForegroundSprite(sprite);
        }
        private void TrySetCachedTechData()
        {
            if (Plugin.cacheData.defaultTech == null)
            {
                ErrorMessage.AddError(CACHE_NOT_SET_WARNING);

                Invoke(nameof(ReAddWarningMessage), 5f);
                return;
            }

            bool unlocksAtStart = Plugin.cacheData.defaultTech.Contains(currentItemIcon.techType);
            unlockAtStartToggle.isOn = unlocksAtStart;
            currentItem.customItemInfo.SetUnlockAtStart(unlocksAtStart);

            Plugin.SlimAnalysisTech tech = Plugin.cacheData.analysisTech.FirstOrDefault(i => i.techType == currentItem.techType);
            List<Ingredient> unlocks = null;

            if (tech != null)
            {
                foreach (var techType in tech.unlockTechTypes)
                {
                    unlocks.Add(new(techType));
                }
            }

            currentItem.SetUnlocks(unlocks);
        }
        private void SetIngredientsData(ITechData techData)
        {
            List<Ingredient> ingredients = new();

            for (int i = 0; i < techData.ingredientCount; i++)
            {
                IIngredient ingredient = techData.GetIngredient(i);
                ingredients.Add(new Ingredient(ingredient.techType, ingredient.amount));
            }

            currentItem.customItemInfo.SetIngredients(ingredients);
        }
        private void SetLinkedItemData(ITechData techData)
        {
            Dictionary<TechType, int> linkedItemsDictionary = new();
            List<Ingredient> linkedItems = new();

            for (int i = 0; i < techData.linkedItemCount; i++)
            {
                TechType techType = techData.GetLinkedItem(i);
                if (linkedItemsDictionary.ContainsKey(techType))
                {
                    linkedItemsDictionary[techType]++;
                }
                else
                {
                    linkedItemsDictionary.Add(techType, 1);
                }
            }

            if (linkedItemsDictionary.Count != 0)
            {
                foreach (var key in linkedItemsDictionary.Keys)
                {
                    linkedItems.Add(new Ingredient(key, linkedItemsDictionary[key]));
                }
            }

            currentItem.customItemInfo.SetLinkedItems(linkedItems);
        }

        public override void RemoveCurrentItemFromList()
        {
            base.RemoveCurrentItemFromList();

            itemText.text = "N/A";
            currentItemIcon = null;
        }

        private void UpdateUnlockedItemsList()
        {
            foreach (Ingredient ingredient in currentItem.unlockedItems)
            {
                GameObject newUnlock = Instantiate(ingredientPrefab, unlocksParent);
                var ingredientItem = newUnlock.GetComponent<IngredientItem>();
                ingredientItem.SetInfo(SpriteManager.Get(ingredient.techType), ingredient.techType, ingredient.amount, this);
                ingredientItem.SetInfoPanel(infoPanel);
            }
        }

        public void SetUnlocksActive()
        {
            ingredientsButtonAnimator.SetBool("NotSelected", true);
            linkedItemsButtonAnimator.SetBool("NotSelected", true);
            unlocksButtonAnimator.SetBool("NotSelected", false);

            ingredientsActive = false;
            linkedItemsActive = false;
            unlocksActive = true;

            InvokeActiveListChanged();
        }

        public override void SetIngredientsActive()
        {
            base.SetIngredientsActive();
            unlocksButtonAnimator.SetBool("NotSelected", true);
            unlocksActive = false;
        }

        public override void SetLinkedItemsActive()
        {
            base.SetLinkedItemsActive();
            unlocksButtonAnimator.SetBool("NotSelected", true);
            unlocksActive = false;
        }

        protected override void ClearAllIngredientItems()
        {
            base.ClearAllIngredientItems();

            foreach (Transform child in unlocksParent)
            {
                Destroy(child.gameObject);
            }
        }

        protected override void UpdateAllLists()
        {
            base.UpdateAllLists();
            UpdateUnlockedItemsList();
        }

        public override List<Ingredient> GetActiveList(out string listName)
        {
            if(currentItem.unlockedItems == null)
            {
                return base.GetActiveList(out listName);
            }

            if(unlocksActive)
            {
                listName = "Unlocked Items";
                return currentItem.unlockedItems;
            }

            return base.GetActiveList(out listName);
        }

        public override List<Ingredient> AdjustCurrentList(ItemIcon item, int change)
        {
            var newList = base.AdjustCurrentList(item, change);

            if(unlocksActive)
            {
                currentItem.SetUnlocks(newList);
            }
            
            UpdateUnlockedItemsList();

            return newList;
        }

        private void ReAddWarningMessage()
        {
            ErrorMessage.AddError(CACHE_NOT_SET_WARNING);
        }
    }
}
