using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nautilus.Handlers;
using Nautilus.Options;

namespace GrappleItemPickup_BepInEx
{
    public class GrappleItemPickupModOptions : ModOptions
    {
        public static ModToggleOption WriteLogs;
        public static ModSliderOption PickupDistance;

        public GrappleItemPickupModOptions() : base("Grapple Item Pickup Options")
        {
            OptionsPanelHandler.RegisterModOptions(this);

            WriteLogs = ModToggleOption.Create("writeLogs", "Write Logs", false);
            AddItem(WriteLogs);

            PickupDistance = ModSliderOption.Create("pickupDistance", "Item Pickup Distance", 1f, 2f, 1f, 1f, "{0:F2}", 0.1f);
            AddItem(PickupDistance);
        }
    }
}