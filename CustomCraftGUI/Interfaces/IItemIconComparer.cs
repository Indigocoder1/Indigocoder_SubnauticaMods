using CustomCraftGUI.Monobehaviors;
using System;
using System.Collections.Generic;

namespace CustomCraftGUI.Interfaces
{
    internal class IItemIconComparer : IComparer<ItemIcon>
    {
        public int Compare(ItemIcon x, ItemIcon y)
        {
            return x.itemName.CompareTo(y.itemName);
        }
    }
}
