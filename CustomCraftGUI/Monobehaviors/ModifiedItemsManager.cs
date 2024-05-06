using CustomCraftGUI.Utilities;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Ingredient = CraftData.Ingredient;

namespace CustomCraftGUI.Monobehaviors
{
    public class ModifiedItemsManager : ItemManager
    {
        [Header("Other")]
        public TextMeshProUGUI itemText;
        public Animator unlocksButtonAnimator;
        public Transform unlocksParent;

        public List<ModifiedItem> modifiedItems { get; private set; }  = new();
        public Dictionary<Item, List<Ingredient>> unlockedItems { get; private set; } = new();
        private ItemIcon currentItemIcon;
        private bool unlocksActive;

        private const string CACHE_NOT_SET_WARNING = 
            "The default tech has not been cached; " +
            "the unlock at start toggle and unlocks may have incorrect values.\n" +
            "To fix this, open a saved game, then return to the Custom Craft Editor";

        public override void SetCurrentItem(Item item)
        {
            currentItem = item;

            itemText.text = Language.main.Get(currentItem.itemID);

            Atlas.Sprite sprite = SpriteManager.Get(((ModifiedItem)currentItem).techType);
            itemIcon.SetForegroundSprite(sprite);

            ClearInstantiatedItems();
            UpdateIngredientsList();
            UpdateLinkedItemsList();
        }

        public void SetCurrentIcon(ItemIcon itemIcon)
        {
            currentItemIcon = itemIcon;
        }

        public void AddCurrentItemToList()
        {
            foreach (var item in modifiedItems)
            {
                //Don't add duplicates
                if(item.techType == currentItemIcon.techType)
                {
                    return;
                }
            }

            GameObject newCustomItem = Instantiate(itemPrefab, itemsParent);
            currentItem = newCustomItem.GetComponent<ModifiedItem>();

            Atlas.Sprite sprite = SpriteManager.Get(currentItemIcon.techType);
            currentItem.SetItemSprite(sprite);
            ((ModifiedItem)currentItem).SetTechType(currentItemIcon.techType);
            currentItem.SetItemID(currentItemIcon.itemName);
            currentItem.SetNameText(Language.main.Get(currentItemIcon.techType));
            currentItem.SetItemsManager(this);

            itemText.text = Language.main.Get(currentItem.itemID);
            itemIcon.SetForegroundSprite(sprite);

            ITechData techData = CraftData.Get(currentItemIcon.techType, true);
            if (techData == null)
            {
                base.ingredients.Add(currentItem, new());
                base.linkedItems.Add(currentItem, new());

                modifiedItems.Add((ModifiedItem)currentItem);

                ClearInstantiatedItems();
                UpdateIngredientsList();
                UpdateLinkedItemsList();

                return;
            }

            List<Ingredient> ingredients = new();
            List<Ingredient> linkedItems = new();
            Dictionary<TechType, int> linkedItemsDictionary = new();

            for (int i = 0; i < techData.ingredientCount; i++)
            {
                IIngredient ingredient = techData.GetIngredient(i);
                ingredients.Add(new Ingredient(ingredient.techType, ingredient.amount));
            }

            for (int i = 0; i < techData.linkedItemCount; i++)
            {
                TechType techType = techData.GetLinkedItem(i);
                if(linkedItemsDictionary.ContainsKey(techType))
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

            amountCraftedInputField.text = techData.craftAmount.ToString();

            List<Ingredient> unlocks = new();
            if (Plugin.cacheData.defaultTech != null)
            {
                bool unlocksAtStart = Plugin.cacheData.defaultTech.Contains(currentItemIcon.techType);
                unlockAtStartToggle.isOn = unlocksAtStart;
                currentItem.SetUnlockAtStart(unlocksAtStart);
            }

            if(Plugin.cacheData.analysisTech != null)
            {
                Plugin.SlimAnalysisTech tech = null;
                foreach (var item in Plugin.cacheData.analysisTech)
                {
                    if (item.techType != currentItemIcon.techType)
                    {
                        continue;
                    }

                    tech = item;
                    break;
                }

                if (tech == null)
                {
                    //No unlocks
                    ((ModifiedItem)currentItem).SetUnlocks(null);
                }
                else
                {
                    ((ModifiedItem)currentItem).SetUnlocks(tech.unlockTechTypes);

                    //I dislike this looping but idk how else to convert the 2
                    foreach (var techType in tech.unlockTechTypes)
                    {
                        unlocks.Add(new(techType, 1));
                    }
                }    
            }

            bool cacheNotSet = Plugin.cacheData.analysisTech == null && Plugin.cacheData.defaultTech == null;

            if(cacheNotSet)
            {
                ErrorMessage.AddError(CACHE_NOT_SET_WARNING);

                Invoke(nameof(ReAddWarningMessage), 5f);
            }

            currentItem.SetAmountCrafted(techData.craftAmount);
            currentItem.SetIngredients(ingredients);
            currentItem.SetLinkedItems(linkedItems);

            modifiedItems.Add((ModifiedItem)currentItem);

            base.ingredients.Add(currentItem, ingredients);
            base.linkedItems.Add(currentItem, linkedItems);
            unlockedItems.Add(currentItem, unlocks);

            ClearInstantiatedItems();
            UpdateIngredientsList();
            UpdateLinkedItemsList();
            UpdateUnlockedItemsList();
        }

        public void RemoveCurrentItemFromList()
        {
            ingredients.Remove(currentItem);
            linkedItems.Remove(currentItem);

            itemText.text = "N/A";
            amountCraftedInputField.text = "1";
            unlockAtStartToggle.isOn = false;

            modifiedItems.Remove((ModifiedItem)currentItem);
            unlockedItems.Remove(currentItem);
            itemIcon.SetForegroundSprite(null);

            foreach (var item in itemsParent.GetComponentsInChildren<Item>())
            {
                if(item.itemID != currentItem.itemID)
                {
                    continue;
                }

                Destroy(item.gameObject);
            }

            ClearInstantiatedItems();

            if (modifiedItems.Count > 0)
            {
                currentItem = modifiedItems[0];

                SetCurrentItem(currentItem);
            }
            else
            {
                currentItem = null;
            }
            
            currentItemIcon = null;
        }

        private void UpdateUnlockedItemsList()
        {
            foreach (Ingredient ingredient in unlockedItems[currentItem])
            {
                GameObject newUnlock = Instantiate(ingredientPrefab, ingredientsParent);
                newUnlock.GetComponent<IngredientItem>().SetInfo(SpriteManager.Get(ingredient.techType), ingredient.techType, 1);
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

        protected override void ClearInstantiatedItems()
        {
            base.ClearInstantiatedItems();

            foreach (Transform child in unlocksParent)
            {
                Destroy(child.gameObject);
            }
        }

        protected override List<Ingredient> GetActiveList()
        {
            if(unlocksActive && unlockedItems.ContainsKey(currentItem))
            {
                return unlockedItems[currentItem];
            }

            return base.GetActiveList();
        }

        private void ReAddWarningMessage()
        {
            ErrorMessage.AddError(CACHE_NOT_SET_WARNING);
        }
    }
}
