﻿using IndigocoderLib;
using TextureReplacerEditor.Monobehaviors.Items;
using TMPro;
using UnityEngine;

namespace TextureReplacerEditor.Monobehaviors.Windows
{
    internal class PrefabInfoWindow : DraggableWindow
    {
        public PrefabIdentifier currentPrefabIdentifier { get; private set; }
        public TextMeshProUGUI prefabNameText;
        public GameObject childItemPrefab;
        public Transform childHierarchyParent;
        public GameObject componentItemPrefab;
        public Transform componentItemsParent;

        private ChildItem currentItem;

        public void CreateChildHierarchy(Transform parent)
        {
            ClearChildHierarchy();
            ClearComponentItems();
            RecursivelyGenChildren(parent, 0, "");
        }

        public void SetPrefabIdentifier(PrefabIdentifier prefabIdentifier)
        {
            this.currentPrefabIdentifier = prefabIdentifier;
            prefabNameText.text = Utilities.GetNameWithCloneRemoved(prefabIdentifier.name);
        }

        private void RecursivelyGenChildren(Transform parent, int previousSiblingIndex, string pathToParent)
        {
            foreach (Transform child in parent)
            {
                GameObject childItem = Instantiate(childItemPrefab, childHierarchyParent);
                string pathToChild = pathToParent + childItem.name;
                childItem.GetComponent<ChildItem>().SetChildInfo(child.name, previousSiblingIndex, child.gameObject, pathToChild);

                if (child.childCount > 0)
                {
                    RecursivelyGenChildren(child, previousSiblingIndex + 1, pathToChild + "/");
                }
            }
        }

        public void SetCurrentItem(ChildItem item)
        {
            currentItem = item;
            SpawnComponentItems();
        }

        private void SpawnComponentItems()
        {
            ClearComponentItems();

            foreach (var component in currentItem.originalChild.GetComponentsInChildren<Component>())
            {
                ComponentItem componentItem = Instantiate(componentItemPrefab, componentItemsParent).GetComponent<ComponentItem>();
                componentItem.SetInfo(component, currentItem.pathToChild);
            }
        }

        private void ClearChildHierarchy()
        {
            foreach (Transform child in childHierarchyParent)
            {
                Destroy(child.gameObject);
            }
        }

        private void ClearComponentItems()
        {
            foreach (Transform child in componentItemsParent)
            {
                Destroy(child.gameObject);
            }
        }
    }
}
