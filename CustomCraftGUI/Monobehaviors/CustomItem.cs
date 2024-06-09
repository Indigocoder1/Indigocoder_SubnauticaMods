using UnityEngine;

namespace CustomCraftGUI.Monobehaviors
{
    public class CustomItem : Item
    {
        public string displayName { get; protected set; }
        public string[] fabricatorPath { get; private set; }
        public Vector2Int itemSize { get; private set; }
        public string tooltip { get; private set; }
        private CustomItemsManager manager;

        public void SetDisplayName(string displayName)
        {
            this.displayName = displayName;
        }
        public void SetFabricatorPath(string[] fabricatorPath)
        {
            this.fabricatorPath = fabricatorPath;
        }
        public void SetItemSize(Vector2Int size)
        {
            this.itemSize = size;
        }
        public void SetTooltip(string tooltip)
        {
            this.tooltip = tooltip;
        }
        public void SetManager(CustomItemsManager manager)
        {
            this.manager = manager;
        }
        public void SetCurrentItem()
        {
            manager.SetCurrentItem(this);
        }
    }
}
