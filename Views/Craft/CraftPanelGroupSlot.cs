using StardewRaft.Core.Factories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardewRaft.Views.Craft
{
    public class CraftPanelGroupSlot(string title, int x, int y)
    {
        public string Title { get; private set; } = title;
        public static int SlotSize { get; private set; } = 64;
        public static int SlotSpacing { get; private set; } = 10;
        public static int SlotBorderWidth { get; private set; } = 5;
        public static int FullSlotSize => SlotSize + SlotSpacing + SlotBorderWidth * 2;
        public Rectangle BorderRect { get; private set; } = new Rectangle(
                        x, y,
                        FullSlotSize - SlotSpacing,
                        FullSlotSize - SlotSpacing);
        public Rectangle SlotRect { get; private set; } = new Rectangle(
                x + SlotBorderWidth,
                y + SlotBorderWidth,
                SlotSize, SlotSize);
    }
}
