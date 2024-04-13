using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace CustomCraftGUI.Monobehaviors
{
    internal class ItemSearcher : MonoBehaviour
    {
        public ItemIconSpawner itemSpawner;
        public TMP_InputField inputField;
        public float keyPressSearchDelay;

        private float timeSinceLastInput;
        private int textLengthLastFrame;
        private bool searched = true;

        private void Update()
        {
            if(inputField.text.Length != textLengthLastFrame)
            {
                searched = false;
                timeSinceLastInput = 0;
            }

            if(timeSinceLastInput > keyPressSearchDelay && !searched && !string.IsNullOrEmpty(inputField.text))
            {
                TrySearch();
            }

            if(string.IsNullOrEmpty(inputField.text) && !searched)
            {
                ActivateAllItems();
                searched = true;
            }

            timeSinceLastInput += Time.deltaTime;
            textLengthLastFrame = inputField.text.Length;
        }

        private void ActivateAllItems()
        {
            foreach (ItemIcon item in itemSpawner.itemIconKVPs.Values)
            {
                item.gameObject.SetActive(true);
            }
        }

        private bool TrySearch()
        {
            int matches = 0;

            foreach (ItemIcon item in itemSpawner.itemIconKVPs.Values)
            {
                if(!item.itemName.ToLower().Contains(inputField.text.ToLower()))
                {
                    item.gameObject.SetActive(false);
                    continue;
                }

                item.gameObject.SetActive(true);
                matches++;
            } 

            searched = true;
            if(matches == 0)
            {
                return false;
            }

            return true;
        }
    }
}
