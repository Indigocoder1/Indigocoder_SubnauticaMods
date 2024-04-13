using System.Collections.Generic;
using UnityEngine;

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

        public Animator ingredientsButtonAnimator;
        public Animator linkedItemsButtonAnimator;

        private List<CraftData.Ingredient> ingredients = new();
        private List<CraftData.Ingredient> linkedItems = new();
        private bool ingredientsActive;
        private bool linkedItemsActive;

        private void Start()
        {
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
            List<CraftData.Ingredient> newList = null;
            if(ingredientsActive)
            {
                newList = ingredients;
            }
            else if(linkedItemsActive)
            {
                newList = linkedItems;
            }

            if (newList.Count == 0)
            {
                newList.Add(new CraftData.Ingredient(item.techType, 1));
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
                ingredients = newList;
            }
            else if (linkedItemsActive)
            {
                linkedItems = newList;
            }

            UpdateIngredientsList();
            UpdateLinkedItemsList();
        }

        private void UpdateIngredientsList()
        {

        }

        private void UpdateLinkedItemsList()
        {

        }
    }
}
