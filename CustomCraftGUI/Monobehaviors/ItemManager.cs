using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Ingredient = CraftData.Ingredient;
using UnityEngine.UI;
using System;

namespace CustomCraftGUI.Monobehaviors
{
    public abstract class ItemManager : MonoBehaviour
    {
        public static event EventHandler OnActiveManagerChanged;
        public event EventHandler OnActiveListChanged;

        [Header("Item Info")]
        public TMP_InputField amountCraftedInputField;
        public uGUI_ItemIcon itemIcon;
        public Toggle unlockAtStartToggle;
        public InfoPanel infoPanel;

        [Header("Animators")]
        public Animator ingredientsButtonAnimator;
        public Animator linkedItemsButtonAnimator;

        [Header("Prefabs")]
        public GameObject ingredientPrefab;
        public GameObject itemPrefab;
        public Transform itemsParent;
        public Transform ingredientsParent;
        public Transform linkedItemsParent;

        protected Dictionary<Item, List<Ingredient>> ingredients = new();
        protected Dictionary<Item, List<Ingredient>> linkedItems = new();
        protected bool ingredientsActive;
        protected bool linkedItemsActive;
        protected Item currentItem;
        protected int itemsCreated = 1;

        public virtual void Start()
        {
            ClearInstantiatedItems();
            Invoke(nameof(SetIngredientsActive), 0.2f);

            OnActiveManagerChanged?.Invoke(this, EventArgs.Empty);
        }

        public virtual void SetIngredientsActive()
        {
            ingredientsButtonAnimator.SetBool("NotSelected", false);
            linkedItemsButtonAnimator.SetBool("NotSelected", true);
            ingredientsActive = true;
            linkedItemsActive = false;

            OnActiveListChanged?.Invoke(this, EventArgs.Empty);
        }

        public virtual void SetLinkedItemsActive()
        {
            ingredientsButtonAnimator.SetBool("NotSelected", true);
            linkedItemsButtonAnimator.SetBool("NotSelected", false);
            ingredientsActive = false;
            linkedItemsActive = true;

            OnActiveListChanged?.Invoke(this, EventArgs.Empty);
        }

        public virtual List<Ingredient> AdjustCurrentList(ItemIcon item, int change)
        {
            List<Ingredient> newList = GetActiveList(out _);

            if(currentItem == null)
            {
                return null;
            }

            if(newList == null)
            {
                return null;
            }

            if (newList.Find((i) => { return i.techType == item.techType; }) == null && change > 0)
            {
                newList.Add(new Ingredient(item.techType, 1));
            }
            else
            {
                for (int i = newList.Count - 1; i >= 0; i--)
                {
                    if (newList[i].techType != item.techType) continue;

                    newList[i]._amount += change;

                    if (newList[i].amount <= 0)
                    {
                        newList.RemoveAt(i);
                    }
                }
            }

            if (ingredientsActive)
            {
                ingredients[currentItem] = newList;
            }
            else if (linkedItemsActive)
            {
                linkedItems[currentItem] = newList;
            }

            ClearInstantiatedItems();
            UpdateIngredientsList();
            UpdateLinkedItemsList();

            currentItem.SetIngredients(ingredients[currentItem]);
            currentItem.SetLinkedItems(linkedItems[currentItem]);

            return newList;
        }

        public virtual List<Ingredient> GetActiveList(out string listName)
        {
            if ((ingredients == null && linkedItems == null) || currentItem == null)
            {
                listName = "Ingredients";
                return null;
            }

            if (ingredientsActive && ingredients.ContainsKey(currentItem))
            {
                listName = "Ingredients";
                return ingredients[currentItem];
            }
            else if (linkedItemsActive && linkedItems.ContainsKey(currentItem))
            {
                listName = "Linked Items";
                return linkedItems[currentItem];
            }

            listName = "";
            return null;
        }

        public abstract void SetCurrentItem(Item item);

        public virtual void RemoveCurrentItemFromList()
        {
            ingredients.Remove(currentItem);
            linkedItems.Remove(currentItem);

            amountCraftedInputField.text = "1";
            unlockAtStartToggle.isOn = false;

            itemIcon.SetForegroundSprite(null);

            foreach (var item in itemsParent.GetComponentsInChildren<Item>())
            {
                if (item.itemID != currentItem.itemID)
                {
                    continue;
                }

                Destroy(item.gameObject);
            }

            ClearInstantiatedItems();

            currentItem = null;
        }
        public virtual void OnAmountCraftedChanged()
        {
            currentItem.SetAmountCrafted(int.Parse(amountCraftedInputField.text));
        }
        public virtual void OnUnlockAtStartChanged()
        {
            currentItem.SetUnlockAtStart(unlockAtStartToggle.isOn);
        }
        
        protected virtual void UpdateIngredientsList()
        {
            foreach (Ingredient ingredient in ingredients[currentItem])
            {
                GameObject newIngredient = Instantiate(ingredientPrefab, ingredientsParent);
                var ingredientItem = newIngredient.GetComponent<IngredientItem>();
                ingredientItem.SetInfo(SpriteManager.Get(ingredient.techType), ingredient.techType, ingredient.amount, (ModifiedItemsManager)this);
                ingredientItem.SetInfoPanel(infoPanel);
            }
        }

        protected virtual void UpdateLinkedItemsList()
        {
            foreach (Ingredient linkedItem in linkedItems[currentItem])
            {
                GameObject newIngredient = Instantiate(ingredientPrefab, linkedItemsParent);
                var ingredientItem = newIngredient.GetComponent<IngredientItem>();
                ingredientItem.SetInfo(SpriteManager.Get(linkedItem.techType), linkedItem.techType, linkedItem.amount, (ModifiedItemsManager)this);
                ingredientItem.SetInfoPanel(infoPanel);
            }
        }

        protected virtual void ClearInstantiatedItems()
        {
            foreach (Transform child in ingredientsParent)
            {
                Destroy(child.gameObject);
            }

            foreach (Transform child in linkedItemsParent)
            {
                Destroy(child.gameObject);
            }
        }

        protected void InvokeActiveListChanged()
        {
            OnActiveListChanged?.Invoke(this, EventArgs.Empty);
        }

        private void OnEnable()
        {
            OnActiveManagerChanged?.Invoke(this, EventArgs.Empty);
        }

        protected readonly Dictionary<string, string[]> fabricatorPaths = new()
        {
            { "Fabricator Basic Materials", new[] { "Fabricator", "Resources", "BasicMaterials" } },
            { "Fabricator Advanced Materials", new[] { "Fabricator", "Resources", "AdvancedMaterials" } },
            { "Fabricator Electronics", new[] { "Fabricator", "Resources", "Electronics" } },
            { "Fabricator Water", new[] { "Fabricator", "Survival", "Water" } },
            { "Fabricator Cooked Food", new[] { "Fabricator", "Survival", "CookedFood" } },
            { "Fabricator Cured Food", new[] { "Fabricator", "Survival", "CuredFood" } },
            { "Fabricator Equipment", new[] { "Fabricator", "Personal", "Equipment" } },
            { "Fabricator Tools", new[] { "Fabricator", "Personal", "Tools" } },
            { "Fabricator Deployables", new[] { "Fabricator", "Machines" } },
            { "MVB Vehicles", new[] { "Constructor", "Vehicles" } },
            { "MVB Rocket", new[] { "Constructor", "Rocket" } },
            { "Modification Station", new[] { "Workbench" } },
            { "Common Vehicle Modules", new    [] { "Workbench", "CommonModules" } },
            { "Seamoth Vehicle Modules", new[] { "Workbench", "SeamothModules" } },
            { "Prawn Vehicle Modules", new[] { "Workbench", "ExosuitModules" } },
            { "Vehicle Torpedos", new[] { "Workbench", "Torpedos" } },
            { "Cyclops Upgrade Fabricator", new[] { "CyclopsFabricator" } }
        };
    }
}
