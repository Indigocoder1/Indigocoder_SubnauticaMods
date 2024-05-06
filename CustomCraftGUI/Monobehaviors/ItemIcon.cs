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
        private ItemManager manager;

        public void SetTechType(TechType techType)
        {
            this.techType = techType;
        }

        public void SetInfoPanel(InfoPanel infoPanel)
        {
            this.infoPanel = infoPanel;
        }
        public void SetItemManager(ItemManager manager)
        {
            this.manager = manager;
        }
        public void SetInfoPanelData()
        {
            infoPanel.SetCurrentItem(this);
        }
        public void SetModifiedItemsManagerData()
        {
            if(manager is ModifiedItemsManager)
            {
                ((ModifiedItemsManager)manager).SetCurrentIcon(this);
            }
        }
    }
}