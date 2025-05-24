using StardewRaft.Core.Factories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardewRaft.Views.Inventory
{
    public class InventorySlot
    {
        public static InventoryView BaseInventory { get; set; }

        public IInventoryItem? InventoryItem => BaseInventory.Inventory.GetItemAt(IndexItem);
        public static int SlotSize { get; private set; } = 64;
        public static int SlotSpacing { get; private set; } = 10;
        public static int SlotBorderWidth { get; private set; } = 5;
        public static int FullSlotSize => SlotSize + SlotSpacing + SlotBorderWidth * 2;

        public int IndexItem { get; private set; }
        public Rectangle BorderRect { get; private set; }
        public Rectangle SlotRect { get; private set; }


        public InventorySlot(int indexItem, int x, int y) 
        {
            IndexItem = indexItem;

            BorderRect = new Rectangle(
                        x, y,
                        FullSlotSize - SlotSpacing,
                        FullSlotSize - SlotSpacing);

            SlotRect = new Rectangle(
                x + SlotBorderWidth,
                y + SlotBorderWidth,
                SlotSize, SlotSize);
        }
    }
}
