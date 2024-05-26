using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System.Collections.Generic;

namespace TodoList.Monobehaviors
{
    internal class uGUI_TodoTab : uGUI_PDATab, uGUI_IScrollReceiver
    {
        private CanvasGroup content;
        private ScrollRect scrollRect;
        private Transform scrollCanvas;

        private void Start()
        {
            content = GetComponentInChildren<CanvasGroup>();
            scrollRect = transform.Find("Content/ScrollView").GetComponent<ScrollRect>();
            scrollCanvas = scrollRect.transform.Find("Viewport/ScrollCanvas");
            scrollRect.enabled = true;
            
            InitVerticalLayoutGroup();

            GameObject label = transform.Find("Content/LogLabel").gameObject;
            label.name = "TodoLabel";
            label.GetComponent<TextMeshProUGUI>().text = "TODO LIST";

            GetComponentInChildren<RectMask2D>().enabled = true;

            UpdateViewportSize();
            SpawnNewItemButton();
            SpawnClearItemsButton();

            //USE THIS FOR THE SAVE SYSTEM \/
            Main_Plugin.logger.LogInfo($"Current save slot = {SaveLoadManager.main.GetCurrentSlot()}");
        }

        private void InitVerticalLayoutGroup()
        {
            var verticalLayout = scrollCanvas.GetComponent<VerticalLayoutGroup>();
            verticalLayout.padding.top = 0;
            verticalLayout.spacing = 5;
            verticalLayout.childControlWidth = false;
            verticalLayout.childForceExpandWidth = false;
            verticalLayout.childScaleWidth = false;
            verticalLayout.enabled = false;
            verticalLayout.enabled = true;
        }

        private void UpdateViewportSize()
        {
            var viewportRect = scrollRect.transform.Find("Viewport").GetComponent<RectTransform>();
            viewportRect.offsetMax = new Vector2(-30f, -80f);
            viewportRect.offsetMin = new Vector2(17.25f, 0f);
            viewportRect.sizeDelta = new Vector2(0, -100);
        }

        private void SpawnNewItemButton()
        {
            GameObject newItemButton = Main_Plugin.AssetBundle.LoadAsset<GameObject>("AddNewItemButton");
            RectTransform newItemButtonRect = Instantiate(newItemButton, content.transform).GetComponent<RectTransform>();
            newItemButtonRect.localPosition = new Vector3(-386f, 241f, 5.94f);
            newItemButtonRect.GetComponent<Button>().onClick.AddListener(() => CreateNewItem());
        }

        private void SpawnClearItemsButton()
        {
            GameObject clearItemsButton = Main_Plugin.AssetBundle.LoadAsset<GameObject>("ClearCompletedItemsButton");
            RectTransform clearItemsButtonRect = Instantiate(clearItemsButton, content.transform).GetComponent<RectTransform>();
            clearItemsButtonRect.localPosition = new Vector3(350f, 241f, 5.94f);
            clearItemsButtonRect.GetComponent<Button>().onClick.AddListener(() => ClearCompletedItems());
        }

        public override void Open()
        {
            content.SetVisible(true);
        }

        public override void Close()
        {
            scrollRect.velocity = Vector2.zero;
            content.SetVisible(false);
        }

        public bool OnScroll(float scrollDelta, float speedMultiplier)
        {
            scrollRect.Scroll(scrollDelta, speedMultiplier);
            return true;
        }

        private void CreateNewItem()
        {
            Instantiate(Main_Plugin.NewItemPrefab, scrollCanvas);
        }

        private void ClearCompletedItems()
        {
            List<TodoInputField> checkedInputFields = TodoInputField.inputFields.Where(i => i.IsChecked).ToList();
            for (int i = checkedInputFields.Count - 1; i >= 0; i--)
            {
                Destroy(checkedInputFields[i].transform.parent.gameObject);
            }
        }
    }
}
