using CustomCraftGUI.Utilities;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

        [Header("Ingredients and Linked Items")]
        public GameObject ingredientItemPrefab;
        public Transform ingredientsPrefabParent;
        public Transform linkedItemsPrefabsParent;

        [Header("Button Text")]
        public TextMeshProUGUI addItemButtonText;
        public TextMeshProUGUI removeItemButtonText;

        public List<ItemManager> ItemManagers { get; private set; } = new();
        private ItemIcon currentIcon;
        private List<IngredientItem> linkedItems = new();

        private ModifiedItemsManager ModifiedItemsManager
        {
            get
            {
                if(_modifiedItemsManager == null)
                {
                    _modifiedItemsManager = ItemManagers.FirstOrDefault(i => i is ModifiedItemsManager) as ModifiedItemsManager;
                }

                return _modifiedItemsManager;
            }
        }

        private ModifiedItemsManager _modifiedItemsManager;

        private void Awake()
        {
            ItemManager.OnActiveManagerChanged += (_, __) => ItemManager_OnActiveManagerChanged();
        }

        private void Start()
        {
            ClearItemsLists();
            UpdateButtonText();
        }
        
        public void AddItemToCurrentList()
        {
            if (!currentIcon) return;

            GetActiveManager().AdjustCurrentList(currentIcon, 1);
            UpdateButtonText();
        }

        public void RemoveItemFromCurrentList()
        {
            if (!currentIcon) return;

            GetActiveManager().AdjustCurrentList(currentIcon, -1);
            UpdateButtonText();
        }

        public void SetCurrentItem(ItemIcon icon)
        {
            ClearItemsLists();
            currentIcon = icon;

            Atlas.Sprite sprite = SpriteManager.Get(icon.techType);

            itemNameText.text = Language.main.Get(icon.itemName);
            itemIcon.SetForegroundSprite(sprite);
            itemIcon.foreground.transform.localScale = SpriteSizeFormatter.GetSpriteShrinkScalar(sprite);

            ModifiedItemsManager.SetCurrentIcon(currentIcon);

            ITechData techData = CraftData.Get(icon.techType, true);
            if (techData == null) return;

            SpawnItemIngredients(techData);
            SpawnLinkedItems(techData);

            TryCollapseLinkedItems();
        }

        private void SpawnItemIngredients(ITechData techData)
        {
            for (int i = 0; i < techData.ingredientCount; i++)
            {
                IIngredient ingredient = techData.GetIngredient(i);
                IngredientItem ingredientItem = Instantiate(ingredientItemPrefab, ingredientsPrefabParent).GetComponent<IngredientItem>();
                var item = ingredientItem.GetComponent<IngredientItem>();
                item.SetInfo(SpriteManager.Get(ingredient.techType), ingredient.techType, ingredient.amount);
                item.SetInfoPanel(this);
            }
        }

        private void SpawnLinkedItems(ITechData techData)
        {
            for (int i = 0; i < techData.linkedItemCount; i++)
            {
                TechType techType = techData.GetLinkedItem(i);
                IngredientItem ingredientItem = Instantiate(ingredientItemPrefab, linkedItemsPrefabsParent).GetComponent<IngredientItem>();
                var linkedItem = ingredientItem.GetComponent<IngredientItem>();
                linkedItem.SetInfo(SpriteManager.Get(techType), techType, 1);
                linkedItem.SetInfoPanel(this);

                linkedItems.Add(ingredientItem);
            }
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
                ingredientItem.SetInfo(SpriteManager.Get(key), key, linkedItemValues[key]);
                ingredientItem.SetInfoPanel(this);

                linkedItems.Add(ingredientItem);
            }
        }

        private ItemManager GetActiveManager()
        {
            if (ItemManagers == null) return null;

            foreach (var manager in ItemManagers)
            {
                if (manager == null) continue;

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

            if (activeManager == null) return;

            activeManager.GetActiveList(out string listName);
            addItemButtonText.text = $"Add to {listName}";
            removeItemButtonText.text = $"Remove from {listName}";
        }

        private void ItemManager_OnActiveManagerChanged()
        {
            UpdateButtonText();
            if (GetActiveManager() != null)
            {
                GetActiveManager().OnActiveListChanged += (_, __) => UpdateButtonText();
            }
        }

        private void OnDestroy()
        {
            ItemManager.OnActiveManagerChanged -= (_, __) => ItemManager_OnActiveManagerChanged();
        }
    }
}