using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using Nautilus.Utility;

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

        public List<CustomItem> customItems { get; private set; } = new();

        public override void Start()
        {
            base.Start();
            CreateNewCustomItem();
        }

        public override void SetCurrentItem(Item item)
        {
            currentItem = item;

            itemIcon.SetForegroundSprite(item.itemSprite);

            itemIDInputField.text = item.itemID;
            customItemNameInputField.text = ((CustomItem)item).displayName;

            ClearInstantiatedItems();
            UpdateIngredientsList();
            UpdateLinkedItemsList();
        }

        public void OnItemIDInputChanged()
        {
            ((CustomItem)currentItem).SetItemID(itemIDInputField.text);
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
                itemIcon.SetForegroundSprite(currentItem.itemSprite);
            }
        }

        public void CreateNewCustomItem()
        {
            GameObject newCustomItem = Instantiate(itemPrefab, itemsParent);
            currentItem = newCustomItem.GetComponent<CustomItem>();

            ((CustomItem)currentItem).SetItemID($"myamazingcoolitem{itemsCreated}");
            currentItem.SetNameText($"myamazingcoolitem{itemsCreated}");
            ((CustomItem)currentItem).SetDisplayName($"My amazing cool item {itemsCreated}");
            currentItem.SetItemsManager(this);
            ((CustomItem)currentItem).SetFabricatorPath(fabricatorPaths["Fabricator Basic Materials"]);

            if (itemIcon.foreground != null)
            {
                Destroy(itemIcon.foreground.gameObject);
            }

            itemIDInputField.text = currentItem.itemID;
            customItemNameInputField.text = ((CustomItem)currentItem).displayName;

            customItems.Add((CustomItem)currentItem);

            itemsCreated++;
            itemIcon.SetForegroundSprite(null);

            ingredients.Add(currentItem, new());
            linkedItems.Add(currentItem, new());

            ClearInstantiatedItems();
            UpdateIngredientsList();
            UpdateLinkedItemsList();
        }

        public override void RemoveCurrentItemFromList()
        {
            CustomItem oldItem = (CustomItem)currentItem;
            base.RemoveCurrentItemFromList();

            customItems.Remove(oldItem);

            if (customItems.Count > 0)
            {
                SetCurrentItem(customItems[customItems.Count - 1]);
            }

            itemsCreated--;
        }
    }
}
