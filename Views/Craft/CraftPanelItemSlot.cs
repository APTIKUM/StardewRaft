using StardewRaft.Core.Factories;
using StardewRaft.Models.Craft;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardewRaft.Views.Craft
{
    public class CraftPanelItemSlot(CraftRecipe recipe, int x, int y)
    {
        public CraftRecipe Recipe { get; private set; } = recipe;
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
