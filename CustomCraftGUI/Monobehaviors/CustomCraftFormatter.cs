using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using UnityEngine;
using Ingredient = CraftData.Ingredient;

namespace CustomCraftGUI.Monobehaviors
{
    public class CustomCraftFormatter : MonoBehaviour
    {
        public ModifiedItemsManager modifiedItemsManager;
        public CustomItemsManager customItemsManager;

        public void ExportFile()
        {
            SaveFileDialog sfd = new();
            sfd.Filter = "Text Files|*.txt";
            sfd.Title = "Choose export location";
            sfd.ShowDialog();

            if (string.IsNullOrEmpty(sfd.FileName))
            {
                Plugin.logger.LogInfo("Aborting save due to invalid file location");
                return;
            }

            if (customItemsManager.customItems.Count > 0)
            {
                ExportCustomItemsFile(sfd);
            }

            if (modifiedItemsManager.modifiedItems.Count > 0)
            {
                ExportModifiedItemsFile(sfd);
            }
        }

        private void ExportCustomItemsFile(SaveFileDialog sfd)
        {
            string customItemsString = "# AUTOGENERATED BY CUSTOM CRAFT GUI #\n";
            customItemsString += "# Created by Indigocoder #\n\n";

            customItemsString += "# If you're creating new items: #\n";
            customItemsString += "# -You need to place the images for your custom items in the CustomCraft(2/3)/Assets folder #\n";
            customItemsString += "# -The image names need to be the same as the ItemID for each item #\n\n";

            customItemsString += "AliasRecipes:\n(\n";
            int itemsIndex = 0;
            foreach (CustomItem customItem in customItemsManager.customItems)
            {
                customItemsString += GetFormattedAliasItem(customItem, itemsIndex == customItemsManager.customItems.Count - 1);
                itemsIndex++;
            }

            FileStream customItemsFileStream = new FileStream(sfd.FileName.Remove(sfd.FileName.Length - 4, 4) + "_AliasRecipes.txt", FileMode.Create, FileAccess.Write);
            StreamWriter customItemsWriter = new StreamWriter(customItemsFileStream);
            customItemsWriter.Write(customItemsString);
            customItemsWriter.Flush();
            customItemsWriter.Close();
        }
        private void ExportModifiedItemsFile(SaveFileDialog sfd)
        {
            string modifiedItemsString = "# AUTOGENERATED BY CUSTOM CRAFT GUI #\n";
            modifiedItemsString += "# Created by Indigocoder #\n\n";

            modifiedItemsString += "ModifiedRecipes:\n(\n";
            int itemsIndex = 0;
            foreach (ModifiedItem modifiedItem in modifiedItemsManager.modifiedItems)
            {
                modifiedItemsString += GetFormattedModifiedItem(modifiedItem, itemsIndex == modifiedItemsManager.modifiedItems.Count - 1);
                itemsIndex++;
            }

            FileStream modifiedItemsFileStream = new FileStream(sfd.FileName.Remove(sfd.FileName.Length - 4, 4) + "_ModifiedRecipes.txt", FileMode.Create, FileAccess.Write);
            StreamWriter modifiedItemsWriter = new StreamWriter(modifiedItemsFileStream);
            modifiedItemsWriter.Write(modifiedItemsString);
            modifiedItemsWriter.Flush();
            modifiedItemsWriter.Close();
        }

        private string GetFormattedAliasItem(CustomItem item, bool endOfFile)
        {
            VerifyImageInAssetsFolder(item);

            string formattedString = string.Empty;

            formattedString += $"    ItemID: {item.itemID};\n";
            formattedString += $"    DisplayName: \"{item.displayName}\";\n";
            formattedString += $"    Tooltip: \"{item.tooltip}\";\n";
            formattedString += $"    AmountCrafted: {item.amountCrafted};\n";
            formattedString += $"    Ingredients:\n";
            formattedString += GetFormattedIngredients(item.ingredients);

            if (item.linkedItems.Count > 0)
            {
                formattedString += "    LinkedItemIDs: ";
                formattedString += GetFormattedLinkedItems(item.linkedItems);
            }

            formattedString += $"    Path: ";
            for (int i = 0; i < item.fabricatorPath.Length; i++)
            {
                string currentPathPortion = item.fabricatorPath[i];
                string endChar = i == item.fabricatorPath.Length - 1 ? ";\n" : "/";
                formattedString += $"{currentPathPortion}{endChar}";
            }

            string unlockAtStart = item.unlockAtStart ? "YES" : "NO";
            formattedString += $"    ForceUnlockAtStart: {unlockAtStart};\n";

            string endCharacter = endOfFile ? ";" : ",";
            formattedString += $"){endCharacter}";

            return formattedString;
        }

        private string GetFormattedModifiedItem(ModifiedItem item, bool endOfFile)
        {
            string formattedString = string.Empty;

            formattedString += $"    ItemID: {item.itemID};\n";
            formattedString += $"    AmountCrafted: {item.amountCrafted};\n";
            formattedString += $"    Ingredients:\n";
            formattedString += GetFormattedIngredients(item.ingredients);

            if(item.linkedItems.Count > 0)
            {
                formattedString += $"    LinkedItemIDs:\n";
                formattedString += GetFormattedLinkedItems(item.linkedItems);
            }

            if(item.unlockedItems != null && item.unlockedItems.Count > 0)
            {
                formattedString += $"    Unlocks: ";

                for (int i = 0; i < item.unlockedItems.Count; i++)
                {
                    string endChar = i == item.unlockedItems.Count ? ";\n" : ", ";
                    formattedString += $"{item.unlockedItems[i]}{endChar}";
                }
            }

            string unlockAtStart = item.unlockAtStart ? "YES" : "NO";
            formattedString += $"    ForceUnlockAtStart: {unlockAtStart};\n";

            string endCharacter = endOfFile ? ";" : ",\n";
            formattedString += $"){endCharacter}";

            return formattedString;
        }

        private string GetFormattedIngredients(List<Ingredient> ingredients)
        {
            string result = string.Empty;
            int ingredientIndex = 0;
            foreach (Ingredient ingredient in ingredients)
            {
                string endChar = ingredientIndex != ingredients.Count - 1 ? "," : ";";
                result += $"        ( ItemID: {ingredient.techType}; Required: {ingredient.amount}; ){endChar}\n";
                ingredientIndex++;
            }

            return result;
        }

        private string GetFormattedLinkedItems(List<Ingredient> linkedItems)
        {
            string result = string.Empty;
            int linkedItemIndex = 0;
            foreach (Ingredient linkedItem in linkedItems)
            {
                string endChar = linkedItemIndex == linkedItems.Count - 1 ? ";\n" : ", ";
                string originalEndChar = endChar;

                for (int i = 0; i < linkedItem.amount; i++)
                {
                    if (linkedItemIndex == linkedItems.Count - 1 && i < linkedItem.amount - 1)
                    {
                        endChar = ", ";
                    }
                    else
                    {
                        endChar = originalEndChar;
                    }

                    result += $"{linkedItem.techType}{endChar}";
                }
                linkedItemIndex++;
            }

            return result;
        }

        private void VerifyImageInAssetsFolder(CustomItem item)
        {
            string dllFolder = Assembly.GetExecutingAssembly().Location;
            string pluginsFolder = Directory.GetParent(Directory.GetParent(dllFolder).FullName).FullName;
            if(!pluginsFolder.EndsWith("plugins"))
            {
                Plugin.logger.LogError($"Plugins folder not found! Where the hell am I??");
                return;
            }

            string cc2Path = Path.Combine(pluginsFolder, "CustomCraft2SML");
            string cc3Path = Path.Combine(pluginsFolder, "CustomCraft3");

            bool cc2Exists = Directory.Exists(cc2Path);
            bool cc3Exists = Directory.Exists(cc3Path);

            if (!cc2Exists && !cc3Exists)
            {
                //CC2 and CC3 not installed. Why are you even using this?
                Plugin.logger.LogInfo("Aborting image file verification due to CC2 and/or CC3 not being installed");
                return;
            }

            if(cc2Exists && !File.Exists(Path.Combine(cc2Path, $"Assets/{item.itemID}.png")))
            {
                Plugin.logger.LogWarning($"There is no image in the Custom Craft 2 assets folder named \"{item.itemID}.png\"! This item will not work correctly.");
            }

            if(cc3Exists && !File.Exists(Path.Combine(cc3Path, $"Assets/{item.itemID}.png")))
            {
                ErrorMessage.AddError($"There is no image in the Custom Craft 3 assets folder named \"{item.itemID}.png\"! This item will not work correctly.");
                Plugin.logger.LogWarning($"There is no image in the Custom Craft 3 assets folder named \"{item.itemID}.png\"! This item will not work correctly.");
            }
        }
    }
}
