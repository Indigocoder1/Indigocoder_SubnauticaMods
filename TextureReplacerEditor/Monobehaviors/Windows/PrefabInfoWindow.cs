using IndigocoderLib;
using System.Collections;
using System.Linq;
using TextureReplacerEditor.Monobehaviors.Items;
using TMPro;
using UnityEngine;
using UWE;

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
        private TutorialHandler tutorialHandler;
        private ComponentItem currentTutorialComponentItem;
        private MaterialItem currentTutorialMaterialItem;
        private bool initialized;
        private bool tutorialStarted;
        private int currentTutorialChildIndex;

        private void Start()
        {
            tutorialHandler = TextureReplacerEditorWindow.Instance.tutorialHandler;
            tutorialStarted = false;
        }

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
                string pathToChild = pathToParent + child.name;
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

            tutorialHandler.TriggerTutorialItem("OnChildSelected");
        }

        #region Tutorial

        public void StartTutorial()
        {
            if(childHierarchyParent.childCount == 0)
            {
                TextureReplacerEditorWindow.Instance.messageWindow.OpenMessage("This object has no children to edit. Select a different object and try again.", 
                    Color.white);
                return;
            }

            tutorialStarted = true;
            currentTutorialChildIndex = 0;

            var childItem = childHierarchyParent.GetChild(currentTutorialChildIndex).GetComponent<ChildItem>();
            childItem.SetTutorialHighlightActive(true);
            TextureReplacerEditorWindow.Instance.tutorialWindow.SetMessage("Welcome to the tutorial! Click on a child to view its components. " +
                "You can also drag around these windows by the white bar at the top.");
            TextureReplacerEditorWindow.Instance.tutorialWindow.SetLRTarget(childItem.tutorialLRTarget);

            tutorialHandler.AddTutorialItem("OnChildSelected", OnChildSelected);
            tutorialHandler.AddTutorialItem("OnRendererSelected", OnRendererSelected);
            tutorialHandler.AddTutorialItem("OnMaterialWindowOpened", OnMaterialWindowOpened);
            tutorialHandler.AddTutorialItem("OnConfigWindowOpened", OnConfigWindowOpened);
            tutorialHandler.AddTutorialItem("OnConfigSaved", OnConfigSaved);
        }

        private void OnChildSelected()
        {
            childHierarchyParent.GetChild(0).GetComponent<ChildItem>().SetTutorialHighlightActive(false);

            ComponentItem componentItem = componentItemsParent.GetComponentsInChildren<ComponentItem>(true).FirstOrDefault(i => i.component is Renderer);
            if(componentItem == null)
            {
                currentTutorialChildIndex++;
                ChildItem childItem = childHierarchyParent.GetChild(currentTutorialChildIndex).GetComponent<ChildItem>();
                childItem.SetTutorialHighlightActive(true);

                TextureReplacerEditorWindow.Instance.tutorialWindow.SetMessage("This object doesn't have any renderers on it, so you can't change anything on it.\n" +
                    "Try selecting a different object.");
                TextureReplacerEditorWindow.Instance.tutorialWindow.SetLRTarget(childItem.tutorialLRTarget);

                return;
            }

            componentItem.SetTutorialHighlightActive(true);
            currentTutorialComponentItem = componentItem;

            TextureReplacerEditorWindow.Instance.tutorialWindow.SetMessage("Click on a renderer component to open it in a separate window and edit its materials.");
            TextureReplacerEditorWindow.Instance.tutorialWindow.SetLRTarget(componentItem.tutorialLineTarget);
        }

        private void OnRendererSelected()
        {
            currentTutorialComponentItem.SetTutorialHighlightActive(false);
            TextureReplacerEditorWindow.Instance.tutorialWindow.SetMessage("You can edit the main color of materials here, and preview their main textures. \n" +
                "Click on the popup icon to view a material in more detail.");

            RendererWindow rendWindow = TextureReplacerEditorWindow.Instance.rendererWindow;
            currentTutorialMaterialItem = rendWindow.materialItemsParent.GetChild(0).GetComponent<MaterialItem>();

            currentTutorialMaterialItem.SetHighlightActive(true);
            TextureReplacerEditorWindow.Instance.tutorialWindow.SetLRTarget(currentTutorialMaterialItem.tutorialLineTarget);
        }

        private void OnMaterialWindowOpened()
        {
            MaterialWindow matWindow = TextureReplacerEditorWindow.Instance.materialWindow;
            currentTutorialMaterialItem.SetHighlightActive(false);

            TextureReplacerEditorWindow.Instance.tutorialWindow.SetMessage("Here, you can edit any material property you want. " +
                "Mess around a bit if you'd like, then click the popup button to create a texture replacer config for your changes.");
            TextureReplacerEditorWindow.Instance.tutorialWindow.SetLRTarget(matWindow.tutorialLineTarget);
            matWindow.SetTutorialHighlightActive(true);
        }

        private void OnConfigWindowOpened()
        {
            TextureReplacerEditorWindow.Instance.materialWindow.SetTutorialHighlightActive(false);
            TextureReplacerEditorWindow.Instance.tutorialWindow.SetMessage("Here you can view all your configs and view their changes. " +
                "If you have multiple configs, you can click on their names to swap between them. Click the trash icons to delete changes or configs. " +
                "Once you're done, click the save icon to save your configs.");

            ConfigViewerWindow configWindow = TextureReplacerEditorWindow.Instance.configViewerWindow;
            configWindow.SetTutorialHighlightActive(true);

            TextureReplacerEditorWindow.Instance.tutorialWindow.SetLRTarget(null);
        }

        private void OnConfigSaved()
        {
            TextureReplacerEditorWindow.Instance.tutorialWindow.SetMessage("Congrats, you've finished the tutorial! Here's a few extra tips on things not covered:\n" +
                "- You can click on texture previews to view a larger version and save them to disk.\n" +
                "- You can click the open file button to replace textures with ones you've saved to your computer.\n" +
                "- You can click the popup button on configs to open the material window with their changes.");

            TextureReplacerEditorWindow.Instance.configViewerWindow.SetTutorialHighlightActive(false);
        }

        public void EndTutorial()
        {
            tutorialStarted = false;
        }

        #endregion

        private void SpawnComponentItems()
        {
            ClearComponentItems();

            foreach (var component in currentItem.originalChild.GetComponents<Component>())
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

        public override void OpenWindow()
        {
            base.OpenWindow();
            if (!initialized)
            {
                gameObject.SetActive(false);
                CoroutineHost.StartCoroutine(ReEnableWindow());
            }
        }

        public override void CloseWindow()
        {
            if(tutorialStarted)
            {
                TextureReplacerEditorWindow.Instance.messageWindow.OpenPrompt("Are you sure you want to close this window? The tutorial will reset.", Color.red,
                    "Yes", "No", base.CloseWindow, null);
                return;
            }

            base.CloseWindow();
        }

        private IEnumerator ReEnableWindow()
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();

            gameObject.SetActive(true);
            initialized = true;
        }
    }
}
