using System.Collections.Generic;
using Ingredient = CraftData.Ingredient;

namespace CustomCraftGUI.Monobehaviors
{
    public class ModifiedItem : Item
    {
        public TechType techType { get; private set; }
        public List<Ingredient> unlockedItems { get; private set; } = new();

        private List<Ingredient> _unlockedItemsAsIngredients;

        public void SetTechType(TechType techType)
        {
            this.techType = techType;
        }
        public void SetUnlocks(List<Ingredient> unlockedItems)
        {
            this.unlockedItems = unlockedItems;
        }
    }
}
