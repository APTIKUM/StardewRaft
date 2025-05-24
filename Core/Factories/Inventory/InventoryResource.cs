using StardewRaft.Models;

namespace StardewRaft.Core.Factories
{
    public class InventoryResource(InventoryItemType type, int count = 1) : IInventoryItem
    {
        
        public string Title => InventoryItemFactory.ItemsTitles[Type];
        public int StackCount => InventoryItemFactory.StackCounts[Type];
        public string Description { get; set ; }
        public InventoryItemType Type { get; set; } = type;

        public int Count { get; set; } = count;
        public static InventoryModel Inventory { get; }
    }
}
