using UnityEngine;

namespace CustomCraftGUI.Monobehaviors
{
    public class ItemIcon : MonoBehaviour
    {
        public string itemName
        {
            get
            {
                return techType.ToString();
            }
        }

        public TechType techType { get; private set; }
        private InfoPanel infoPanel;
        private ModifiedItemsManager manager;

        public void SetTechType(TechType techType)
        {
            this.techType = techType;
        }

        public void SetInfoPanel(InfoPanel infoPanel)
        {
            this.infoPanel = infoPanel;
        }
        public void SetItemManager(ModifiedItemsManager manager)
        {
            this.manager = manager;
        }

        public void SetInfoPanelData()
        {
            infoPanel.SetCurrentItem(this);
        }
        public void SetModifiedItemsManagerData()
        {
            manager.SetCurrentIcon(this);
        }
    }
}