using CustomCraftGUI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using static CraftData;
namespace CustomCraftGUI.Monobehaviors
{
    public class InfoPanel : MonoBehaviour
    {
        [Header("Header Info")]
        public TextMeshProUGUI itemNameText;
        public uGUI_ItemIcon itemIcon;

        [Header("Item Managers")]
        public ItemManager[] itemManagers;

        [Header("Ingredients and Linked Items")]
        public GameObject ingredientItemPrefab;
        public Transform ingredientsPrefabParent;
        public Transform linkedItemsPrefabsParent;

        [Header("Button Text")]
        public TextMeshProUGUI addItemButtonText;
        public TextMeshProUGUI removeItemButtonText;

        private ItemIcon currentItem;
        private List<IngredientItem> linkedItems = new();
        private ModifiedItemsManager modifiedItemsManager;

        private void Awake()
        {
            ItemManager.OnActiveManagerChanged += (_, __) => ItemManager_OnActiveManagerChanged();
        }

        private void Start()
        {
            ClearItemsLists();

            foreach (var manager in itemManagers)
            {
                if(manager is ModifiedItemsManager)
                {
                    modifiedItemsManager = (ModifiedItemsManager)manager;
                    break;
                }
            }

            UpdateButtonText();
        }

        public void AddItemToCurrentList()
        {
            if (!currentItem) return;

            GetActiveManager().AdjustCurrentList(currentItem, 1);
            UpdateButtonText();
        }

        public void RemoveItemFromCurrentList()
        {
            if (!currentItem) return;

            GetActiveManager().AdjustCurrentList(currentItem, -1);
            UpdateButtonText();
        }

        public void SetCurrentItem(ItemIcon icon)
        {
            ClearItemsLists();
            currentItem = icon;

            Atlas.Sprite sprite = SpriteManager.Get(icon.techType);

            itemNameText.text = Language.main.Get(icon.itemName);
            itemIcon.SetForegroundSprite(sprite);
            itemIcon.foreground.transform.localScale = SpriteSizeFormatter.GetSpriteShrinkScalar(sprite);

            ITechData techData = CraftData.Get(icon.techType, true);
            if (techData == null) return;

            for (int i = 0; i < techData.ingredientCount; i++)
            {
                IIngredient ingredient = techData.GetIngredient(i);
                IngredientItem ingredientItem = Instantiate(ingredientItemPrefab, ingredientsPrefabParent).GetComponent<IngredientItem>();
                var item = ingredientItem.GetComponent<IngredientItem>();
                item.SetInfo(SpriteManager.Get(ingredient.techType), ingredient.techType, ingredient.amount, modifiedItemsManager);
                item.SetInfoPanel(this);
            }

            for (int i = 0; i < techData.linkedItemCount; i++)
            {
                TechType techType = techData.GetLinkedItem(i);
                IngredientItem ingredientItem = Instantiate(ingredientItemPrefab, linkedItemsPrefabsParent).GetComponent<IngredientItem>();
                var linkedItem = ingredientItem.GetComponent<IngredientItem>();
                linkedItem.SetInfo(SpriteManager.Get(techType), techType, 1, modifiedItemsManager);
                linkedItem.SetInfoPanel(this);

                linkedItems.Add(ingredientItem);
            }
            TryCollapseLinkedItems();
        }

        private void ClearItemsLists()
        {
            foreach (Transform child in ingredientsPrefabParent)
            {
                Destroy(child.gameObject);
            }

            foreach (Transform child in linkedItemsPrefabsParent)
            {
                Destroy(child.gameObject);
            }

            linkedItems.Clear();
        }

        private void TryCollapseLinkedItems()
        {
            Dictionary<TechType, int> linkedItemValues = new();

            foreach (IngredientItem item in linkedItems)
            {
                if(!linkedItemValues.ContainsKey(item.techType))
                {
                    linkedItemValues.Add(item.techType, 1);
                }
                else
                {
                    linkedItemValues[item.techType]++;
                }
            }

            foreach (Transform child in linkedItemsPrefabsParent)
            {
                Destroy(child.gameObject);
            }

            linkedItems.Clear();

            foreach (TechType key in linkedItemValues.Keys)
            {
                IngredientItem ingredientItem = Instantiate(ingredientItemPrefab, linkedItemsPrefabsParent).GetComponent<IngredientItem>();
                ingredientItem.SetInfo(SpriteManager.Get(key), key, linkedItemValues[key], modifiedItemsManager);
                ingredientItem.SetInfoPanel(this);

                linkedItems.Add(ingredientItem);
            }
        }

        private ItemManager GetActiveManager()
        {
            foreach (var manager in itemManagers)
            {
                if(manager.gameObject.activeSelf)
                {
                    return manager;
                }
            }

            return null;
        }

        private void UpdateButtonText()
        {
            ItemManager activeManager = GetActiveManager();

            activeManager.GetActiveList(out string listName);
            addItemButtonText.text = $"Add to {listName}";
            removeItemButtonText.text = $"Remove from {listName}";
        }

        private void ItemManager_OnActiveManagerChanged()
        {
            UpdateButtonText();
            GetActiveManager().OnActiveListChanged += (_, __) => UpdateButtonText();
        }
    }
}