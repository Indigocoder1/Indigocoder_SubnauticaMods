using CustomCraftGUI.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomCraftGUI.Monobehaviors
{
    public class ItemIconSpawner : MonoBehaviour
    {
        public GameObject iconPrefab;
        public Transform iconsParent;
        public InfoPanel infoPanel;
        
        private List<ItemIcon> itemIcons = new();

        public IEnumerator SpawnIcons()
        {
            foreach (TechType techType in Enum.GetValues(typeof(TechType)))
            {
                Atlas.Sprite atlasSprite = SpriteManager.Get(techType);
                if (atlasSprite == SpriteManager.defaultSprite) continue;

                GameObject newIconPrefab = Instantiate(iconPrefab, iconsParent);
                ItemIcon icon = newIconPrefab.GetComponent<ItemIcon>();

                icon.SetTechType(techType);
                icon.SetInfoPanel(infoPanel);
                itemIcons.Add(icon);

                uGUI_ItemIcon itemIcon = newIconPrefab.GetComponentInChildren<uGUI_ItemIcon>();
                itemIcon.SetForegroundSprite(atlasSprite);
                itemIcon.foreground.transform.localScale = Vector3.one * 0.8f * SpriteSizeFormatter.GetSpriteShrinkScalar(atlasSprite);
            }

            SortIcons();
            yield break;
        }

        private void SortIcons()
        {
            
        }

        public List<ItemIcon> TryFindItemIcon(string targetName)
        {

        }
    }
}