using StardewRaft.Models;

namespace StardewRaft.Core.Factories
{
    public interface IInventoryItem
    {
        public string Title { get; }
        public string Description { get; }
        public InventoryItemType Type { get; }
        public int StackCount { get; }
        public int Count { get; set; }
        public static InventoryModel Inventory { get;}
    }
}
