using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
            scrollRect.enabled = true;

            scrollCanvas = scrollRect.transform.Find("Viewport/ScrollCanvas");
            var verticalLayout = scrollCanvas.GetComponent<VerticalLayoutGroup>();
            verticalLayout.padding.top = 0;
            verticalLayout.spacing = 5;
            verticalLayout.childControlWidth = false;
            verticalLayout.childForceExpandWidth = false;
            verticalLayout.childScaleWidth = false;
            //verticalLayout.childAlignment = TextAnchor.UpperCenter;
            verticalLayout.enabled = false;
            verticalLayout.enabled = true;

            GameObject label = transform.Find("Content/LogLabel").gameObject;
            label.name = "TodoLabel";
            label.GetComponent<TextMeshProUGUI>().text = "TODO LIST";

            GetComponentInChildren<RectMask2D>().enabled = true;

            var viewportRect = scrollRect.transform.Find("Viewport").GetComponent<RectTransform>();
            viewportRect.offsetMax = new Vector2(-30f, -80f);
            viewportRect.offsetMin = new Vector2(17.25f, 0f);
            viewportRect.sizeDelta = new Vector2(0, -100);
            //viewportRect.pivot = new Vector2(0, -1);

            GameObject newItemButton = Main_Plugin.AssetBundle.LoadAsset<GameObject>("AddNewItemButton");
            RectTransform rect = Instantiate(newItemButton, content.transform).GetComponent<RectTransform>();
            rect.localPosition = new Vector3(-386f, 241f, 5.94f);
            rect.GetComponent<Button>().onClick.AddListener(() => CreateNewItem());
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
    }
}
