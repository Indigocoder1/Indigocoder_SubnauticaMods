using UnityEngine;

namespace CustomCraftGUI.Monobehaviors
{
    public class CustomItem : Item
    {
        public string displayName { get; protected set; }
        public string[] fabricatorPath { get; private set; }
        public Vector2Int itemSize { get; private set; }
        public string tooltip { get; private set; }

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
    }
}
