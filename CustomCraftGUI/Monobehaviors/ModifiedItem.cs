using System.Collections.Generic;

namespace CustomCraftGUI.Monobehaviors
{
    public class ModifiedItem : Item
    {
        public TechType techType { get; private set; }
        public List<TechType> unlockedItems { get; private set; } = new();

        public void SetTechType(TechType techType)
        {
            this.techType = techType;
        }
        public void SetUnlocks(List<TechType> unlockedItems)
        {
            this.unlockedItems = unlockedItems;
        }
    }
}
