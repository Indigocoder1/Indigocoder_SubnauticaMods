using TMPro;
using UnityEngine;
using static RootMotion.FinalIK.RagdollUtility;

namespace TextureReplacerEditor.Monobehaviors
{
    internal class PrefabInfoWindow : MonoBehaviour
    {
        public TextMeshProUGUI prefabNameText;
        public GameObject childItemPrefab;
        public Transform childHierarchyParent;

        public void CreateChildHierarchy(Transform parent)
        {
            foreach (Transform child in childHierarchyParent)
            {
                Destroy(child.gameObject);
            }

            RecursivelyGenChildren(parent, 0);
        }

        private void RecursivelyGenChildren(Transform parent, int previousSiblingIndex)
        {
            foreach (Transform child in parent)
            {
                int siblingIndex = previousSiblingIndex + child.GetSiblingIndex();

                GameObject childItem = Instantiate(childItemPrefab, childHierarchyParent);
                childItem.GetComponent<ChildItem>().SetChildInfo(child.name, siblingIndex);

                if(childItem.transform.childCount > 0)
                {
                    RecursivelyGenChildren(childItem.transform, childItem.transform.GetSiblingIndex());
                }
            }
        }

        public void CloseWindow()
        {
            GetComponentInParent<uGUI_InputGroup>().Deselect();
        }
    }
}
