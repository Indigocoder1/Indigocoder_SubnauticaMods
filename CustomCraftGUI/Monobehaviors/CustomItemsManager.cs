using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using Nautilus.Utility;
using CustomCraftGUI.Utilities;

namespace CustomCraftGUI.Monobehaviors
{
    public class CustomItemsManager : ItemManager
    {
        [Header("Custom Item Info")]
        public TMP_InputField itemIDInputField;
        public TMP_InputField customItemNameInputField;
        public TMP_Dropdown fabricatorPathDropdown;
        public TMP_InputField itemSizeXField;
        public TMP_InputField itemSizeYField;
        public TMP_InputField itemTooltipInputField;

#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
        private CustomItem currentItem
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword
        {
            get
            {
                return base.currentItem as CustomItem;
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
                return _items;
            }
            protected set
            {
                _items = value;
            }
        }

        private List<Item> _items;

        public override void Start()
        {
            base.Start();
            CreateNewCustomItem();
        }

        public override void SetCurrentItem(Item item)
        {
            currentItem = item as CustomItem;

            itemIcon.SetForegroundSprite(currentItem.customItemInfo.itemSprite);
            
            itemIDInputField.text = currentItem.customItemInfo.itemID;
            customItemNameInputField.text = currentItem.displayName;

            ClearInstantiatedItems();
            UpdateIngredientsList();
            UpdateLinkedItemsList();
        }

        public void OnItemIDInputChanged()
        {
            currentItem.customItemInfo.SetItemID(itemIDInputField.text);
        }
        public void OnNameInputChanged()
        {
            string text = string.IsNullOrEmpty(customItemNameInputField.text) ? "My amazing cool item" : customItemNameInputField.text;
            ((CustomItem)currentItem).SetDisplayName(text);
            currentItem.SetNameText(text);
        }
        public void OnFabricatorDropdownChanged()
        {
            string dropdownValue = fabricatorPathDropdown.options[fabricatorPathDropdown.value].text;
            ((CustomItem)currentItem).SetFabricatorPath(fabricatorPaths[dropdownValue]);
        }
        public void OnSizeInputFieldChanged()
        {
            ((CustomItem)currentItem).SetItemSize(new Vector2Int(int.Parse(itemSizeXField.text), int.Parse(itemSizeYField.text)));
        }
        public void OnTooltipInputFieldChanged()
        {
            ((CustomItem)currentItem).SetTooltip(itemTooltipInputField.text);
        }
        public void OnLoadIconPressed()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.InitialDirectory = Assembly.GetExecutingAssembly().Location;
            dialog.Filter = "Png Files (*.png;*)|*.png";
            dialog.Title = "Choose item png image";

            if(dialog.ShowDialog() == DialogResult.OK)
            {
                string imageLocation = dialog.FileName;
                if(!File.Exists(imageLocation))
                {
                    Plugin.logger.LogError($"File doesn't exist at path {imageLocation}!");
                    return;
                }

                currentItem.SetItemSprite(ImageUtils.LoadSpriteFromFile(imageLocation));
                itemIcon.SetForegroundSprite(currentItem.customItemInfo.itemSprite);
                itemIcon.foreground.transform.localScale = SpriteSizeFormatter.GetSpriteShrinkScalar(currentItem.customItemInfo.itemSprite);
            }
        }

        public void CreateNewCustomItem()
        {
            GameObject newCustomItem = Instantiate(itemPrefab, itemsParent);
            currentItem = newCustomItem.GetComponent<CustomItem>();

            currentItem.SetNameText($"MyAmazingCoolItem{itemsCreated}");
            currentItem.SetDisplayName($"My amazing cool item {itemsCreated}");
            currentItem.SetFabricatorPath(fabricatorPaths["Fabricator Basic Materials"]);
            currentItem.customItemInfo.SetItemID($"MyAmazingCoolItem{itemsCreated}");

            if (itemIcon.foreground != null)
            {
                Destroy(itemIcon.foreground.gameObject);
            }

            itemIDInputField.text = currentItem.customItemInfo.itemID;
            customItemNameInputField.text = ((CustomItem)currentItem).displayName;

            Items.Add(currentItem);

            itemsCreated++;
            itemIcon.SetForegroundSprite(null);

            currentItem.customItemInfo.SetIngredients(new());
            currentItem.customItemInfo.SetLinkedItems(new());

            ClearInstantiatedItems();
            UpdateIngredientsList();
            UpdateLinkedItemsList();
        }

        public override void RemoveCurrentItemFromList()
        {
            CustomItem oldItem = currentItem;
            base.RemoveCurrentItemFromList();

            Items.Remove(oldItem);

            if (Items.Count > 0)
            {
                SetCurrentItem(Items[Items.Count - 1]);
            }

            itemsCreated--;
        }
    }
}
