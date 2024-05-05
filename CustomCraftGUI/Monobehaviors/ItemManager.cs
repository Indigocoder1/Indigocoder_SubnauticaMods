using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Ingredient = CraftData.Ingredient;
using UnityEngine.UI;

namespace CustomCraftGUI.Monobehaviors
{
    public abstract class ItemManager : MonoBehaviour
    {
        [Header("Item Info")]
        public TMP_InputField amountCraftedInputField;
        public uGUI_ItemIcon itemIcon;
        public Toggle unlockAtStartToggle;

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
        }

        public virtual void SetIngredientsActive()
        {
            ingredientsButtonAnimator.SetBool("NotSelected", false);
            linkedItemsButtonAnimator.SetBool("NotSelected", true);
            ingredientsActive = true;
            linkedItemsActive = false;
        }

        public virtual void SetLinkedItemsActive()
        {
            ingredientsButtonAnimator.SetBool("NotSelected", true);
            linkedItemsButtonAnimator.SetBool("NotSelected", false);
            ingredientsActive = false;
            linkedItemsActive = true;
        }

        public virtual void AdjustCurrentList(ItemIcon item, int change)
        {
            List<Ingredient> newList = GetActiveList();

            if(currentItem == null)
            {
                return;
            }

            if(newList == null)
            {
                return;
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
        }

        protected virtual List<Ingredient> GetActiveList()
        {
            if (ingredientsActive && ingredients.ContainsKey(currentItem))
            {
                return ingredients[currentItem];
            }
            else if (linkedItemsActive && linkedItems.ContainsKey(currentItem))
            {
                return linkedItems[currentItem];
            }

            return null;
        }

        public abstract void SetCurrentItem(Item item);

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
                newIngredient.GetComponent<IngredientItem>().SetInfo(SpriteManager.Get(ingredient.techType), ingredient.techType, ingredient.amount);
            }
        }

        protected virtual void UpdateLinkedItemsList()
        {
            foreach (Ingredient linkedItem in linkedItems[currentItem])
            {
                GameObject newIngredient = Instantiate(ingredientPrefab, linkedItemsParent);
                newIngredient.GetComponent<IngredientItem>().SetInfo(SpriteManager.Get(linkedItem.techType), linkedItem.techType, linkedItem.amount);
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

        protected Dictionary<string, string[]> fabricatorPaths = new()
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
