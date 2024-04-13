using System;
using UnityEngine;
using UnityEngine.UI;

namespace CustomCraftGUI.Monobehaviors
{
    public class ItemIcon : MonoBehaviour
    {
        public string itemName
        {
            get
            {
                return Language.main.Get(techType.ToString());
            }
        }

        public TechType techType { get; private set; }
        private InfoPanel infoPanel;

        public void SetTechType(TechType techType)
        {
            this.techType = techType;
        }

        public void SetInfoPanel(InfoPanel infoPanel)
        {
            this.infoPanel = infoPanel;
        }

        public void SetInfoPanelData()
        {
            infoPanel.SetCurrentItem(this);
        }
    }
}