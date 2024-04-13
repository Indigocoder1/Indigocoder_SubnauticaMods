using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Ingredient = CraftData.Ingredient;

namespace CustomCraftGUI.Monobehaviors
{
    public class CustomItemsManager : MonoBehaviour
    {
        public static CustomItemsManager Instance
        {
            get
            {
                return Instance;
            }
            set
            {
                if(Instance != null)
                {
                    Debug.LogError($"More than one CustomItemsManager in the scene! Attempting to set {value}");
                    return;
                }

                Instance = value;
            }
        }

        [Header("Item Info")]
        public TMP_InputField customItemNameInputField;
        public uGUI_ItemIcon customItemIcon;

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

        private void CreateNewCustomItem()
        {
            GameObject newCustomItem = Instantiate(customItemPrefab, customItemsParent);
            currentItem = newCustomItem.GetComponent<CustomItem>();
            customItemNameInputField.text = "";

            foreach (Transform child in customItemIcon.transform)
            {
                Destroy(child);
            } 

            ClearInstantiatedItems();
            UpdateIngredientsList();
            UpdateLinkedItemsList();
        }
    }
}
