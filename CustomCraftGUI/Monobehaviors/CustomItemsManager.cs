using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Ingredient = CraftData.Ingredient;

namespace CustomCraftGUI.Monobehaviors
{
    public class CustomItemsManager : MonoBehaviour
    {
        [Header("Item Info")]
        public TMP_InputField itemIDInputField;
        public TMP_InputField customItemNameInputField;
        public TMP_InputField amountCraftedInputField;
        public uGUI_ItemIcon customItemIcon;
        public Toggle unlockAtStartToggle;
        public TMP_Dropdown fabricatorPathDropdown;
        public TMP_InputField itemSizeXField;
        public TMP_InputField itemSizeYField;
        public TMP_InputField itemTooltipInputField;

        [Header("Animators")]
        public Animator ingredientsButtonAnimator;
        public Animator linkedItemsButtonAnimator;

        [Header("Prefabs")]
        public GameObject ingredientPrefab;
        public GameObject customItemPrefab;
        public Transform ingredientsParent;
        public Transform linkedItemsParent;
        public Transform customItemsParent;

        private Dictionary<CustomItem, List<Ingredient>> ingredients = new();
        private Dictionary<CustomItem, List<Ingredient>> linkedItems = new();
        private bool ingredientsActive;
        private bool linkedItemsActive;
        private CustomItem currentItem;
        private int itemsCreated = 1;

        private void Start()
        {
            CreateNewCustomItem();

            ClearInstantiatedItems();
            SetIngredientsActive();
        }

        public void SetIngredientsActive()
        {
            ingredientsButtonAnimator.SetBool("NotSelected", false);
            linkedItemsButtonAnimator.SetBool("NotSelected", true);
            ingredientsActive = true;
            linkedItemsActive = false;
        }

        public void SetLinkedItemsActive()
        {
            ingredientsButtonAnimator.SetBool("NotSelected", true);
            linkedItemsButtonAnimator.SetBool("NotSelected", false);
            ingredientsActive = false;
            linkedItemsActive = true;
        }

        public void AdjustCurrentList(ItemIcon item, int change)
        {
            List<Ingredient> newList = null;
            if(ingredientsActive)
            {
                newList = ingredients[currentItem];
            }
            else if(linkedItemsActive)
            {
                newList = linkedItems[currentItem];
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

        public void SetCurrentItem(CustomItem item)
        {
            currentItem = item;
            customItemNameInputField.text = item.nameText.text;

            ClearInstantiatedItems();
            UpdateIngredientsList();
            UpdateLinkedItemsList();
        }

        public void OnItemIDInputChanged()
        {
            currentItem.SetItemID(itemIDInputField.text);
        }
        public void OnNameInputChanged()
        {
            string text = string.IsNullOrEmpty(customItemNameInputField.text) ? "My amazing cool item" : customItemNameInputField.text;
            currentItem.SetDisplayName(text);
        }
        public void OnAmountCraftedChanged()
        {
            currentItem.SetAmountCrafted(int.Parse(amountCraftedInputField.text));
        }
        public void OnUnlockAtStartChanged()
        {
            currentItem.SetUnlockAtStart(unlockAtStartToggle.isOn);
        }
        public void OnFabricatorDropdownChanged()
        {
            string dropdownValue = fabricatorPathDropdown.options[fabricatorPathDropdown.value].text;
            currentItem.SetFabricatorPath(fabricatorPaths[dropdownValue]);
        }
        public void OnSizeInputFieldChanged()
        {
            currentItem.SetItemSize(new Vector2Int(int.Parse(itemSizeXField.text), int.Parse(itemSizeYField.text)));
        }
        public void OnTooltipInputFieldChanged()
        {
            currentItem.SetTooltip(itemTooltipInputField.text);
        }

        public void CreateNewCustomItem()
        {
            GameObject newCustomItem = Instantiate(customItemPrefab, customItemsParent);

            currentItem = newCustomItem.GetComponent<CustomItem>();
            currentItem.SetDisplayName($"My amazing cool item {itemsCreated}");
            currentItem.SetItemsManager(this);

            customItemNameInputField.text = $"My amazing cool item {itemsCreated}";

            itemsCreated++;

            foreach (Transform child in customItemIcon.transform)
            {
                Destroy(child);
            }

            ingredients.Add(currentItem, new());
            linkedItems.Add(currentItem, new());

            ClearInstantiatedItems();
            UpdateIngredientsList();
            UpdateLinkedItemsList();
        }

        private void UpdateIngredientsList()
        {
            foreach (Ingredient ingredient in ingredients[currentItem])
            {
                GameObject newIngredient = Instantiate(ingredientPrefab, ingredientsParent);
                newIngredient.GetComponent<IngredientItem>().SetInfo(SpriteManager.Get(ingredient.techType), ingredient.techType, ingredient.amount);
            }
        }

        private void UpdateLinkedItemsList()
        {
            foreach (Ingredient linkedItem in linkedItems[currentItem])
            {
                GameObject newIngredient = Instantiate(ingredientPrefab, linkedItemsParent);
                newIngredient.GetComponent<IngredientItem>().SetInfo(SpriteManager.Get(linkedItem.techType), linkedItem.techType, linkedItem.amount);
            }
        }

        private void ClearInstantiatedItems()
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

        private Dictionary<string, string[]> fabricatorPaths = new()
        {
            { "Fabricator Basic Materials", new string[] { "Fabricator", "Resources", "BasicMaterials" } },
            { "Fabricator Advanced Materials", new string[] { "Fabricator",  "Resources", "AdvancedMaterials" } },
            { "Fabricator Electronics", new string[] { "Fabricator",  "Resources", "Electronics" } },
            { "Fabricator Water", new string[] { "Fabricator", "Survival", "Water" } },
            { "Fabricator Cooked Food", new string[] { "Fabricator",  "Survival", "CookedFood" } },
            { "Fabricator Cured Food", new string[] { "Fabricator",  "Survival", "CuredFood" } },
            { "Fabricator Equipment", new string[] { "Fabricator", "Personal", "Equipment" } },
            { "Fabricator Tools", new string[] { "Fabricator", "Personal", "Tools" } },
            { "Fabricator Deployables", new string[] { "Fabricator", "Machines" } },
            { "MVB Vehicles", new string[] { "Constructor", "Vehicles" } },
            { "MVB Rocket", new string[] { "Constructor", "Rocket" } },
            { "Common Vehicle Upgrades", new string[] { "Workbench", "CommonModules" } },
            { "Seamoth Vehicle Upgrades", new string[] { "Workbench", "SeamothModules" } },
            { "Prawn Vehicle Upgrades", new string[] { "Workbench", "ExosuitModules" } },
            { "Vehicle Torpedos", new string[] { "Workbench", "Torpedos" } },
            { "Cyclops Upgrade Fabricator", new string[] { "CyclopsFabricator" } }
        };
    }
}
