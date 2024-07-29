using Nautilus.Json.Converters;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using TextureReplacerEditor.Monobehaviors.Items;
using TextureReplacerEditor.Monobehaviors.Windows;
using UnityEngine;

using ConfigInfo = TextureReplacer.CustomTextureReplacer.ConfigInfo;

namespace TextureReplacerEditor.Monobehaviors
{
    internal class ConfigExporter : MonoBehaviour
    {
        private const string NO_ITEMS_ERROR = "You do not have any items to save!";
        private const string DUPLICATE_PREFAB_ERRROR = "Configs with shared prefab detected!\r\nDelete all but one config per prefab and retry.";
        private bool textureSavedToAssets;

        public void TryExportFiles()
        {
            textureSavedToAssets = false;
            InfoMessageWindow messageWindow = TextureReplacerEditorWindow.Instance.messageWindow;
            List<CustomConfigItem> items = TextureReplacerEditorWindow.Instance.configViewerWindow.addedItems;
            if(items.Count <= 0)
            {
                messageWindow.OpenMessage(NO_ITEMS_ERROR, Color.white);
            }

            List<ConfigInfo> configInfos = new();

            foreach (var configItem in items)
            {
                if(configInfos.Any(i => i.prefabClassID == configItem.configInfo.prefabClassID))
                {
                    messageWindow.OpenMessage(DUPLICATE_PREFAB_ERRROR, Color.red);
                    return;
                }

                configInfos.Add(configItem.configInfo);

                foreach (var propertyEdit in configItem.propertyEdits)
                {
                    if (propertyEdit.type != UnityEngine.Rendering.ShaderPropertyType.Texture) continue;

                    if (propertyEdit.newValue is not Texture2D) continue;

                    Texture2D tex = propertyEdit.newValue as Texture2D;
                    string path = Path.Combine(Main_Plugin.TextureReplacerAssetsFolderPath, $"{tex.name}.png");
                    File.WriteAllBytes(path, tex.EncodeToPNG());
                    textureSavedToAssets = true;
                }
            }

            var sfd = new SaveFileDialog();
            sfd.Title = "Choose a save location";
            sfd.Filter = "JSON|*.json";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                string path = sfd.FileName;
                string jsonData = JsonConvert.SerializeObject(configInfos, Formatting.Indented, new CustomEnumConverter());
                File.WriteAllText(path, jsonData);

                string message = $"Saved config to {path}";
                if(textureSavedToAssets)
                {
                    message += "\nEdited textures saved to TextureReplacer/Assets";
                }

                messageWindow.OpenMessage(message, Color.white);
                TextureReplacerEditorWindow.Instance.tutorialHandler.TriggerTutorialItem("OnConfigSaved");
            }
        }
    }
}
