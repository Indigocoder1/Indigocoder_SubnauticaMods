using Nautilus.Json.Converters;
using Newtonsoft.Json;
using System.Collections.Generic;
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

        public void TryExportFiles()
        {
            InfoMessageWindow messageWindow = TextureReplacerEditorWindow.Instance.messageWindow;
            List<CustomConfigItem> items = TextureReplacerEditorWindow.Instance.configViewerWindow.addedItems;
            if(items.Count <= 0)
            {
                messageWindow.OpenWindow();
                messageWindow.SetMessage(NO_ITEMS_ERROR, Color.black);
            }

            List<ConfigInfo> configInfos = new();

            foreach (var configItem in items)
            {
                if(!configInfos.Any(i => i.prefabClassID == configItem.configInfo.prefabClassID))
                {
                    configInfos.Add(configItem.configInfo);
                }
                else
                {
                    messageWindow.OpenWindow();
                    messageWindow.SetMessage(DUPLICATE_PREFAB_ERRROR, Color.red);
                    return;
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

                messageWindow.OpenWindow();
                messageWindow.SetMessage($"Saved config to {path}", Color.black);
            }
        }
    }
}
