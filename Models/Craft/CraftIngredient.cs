using StardewRaft.Core.Factories;

namespace StardewRaft.Models.Craft
{
    public class CraftIngredient(InventoryItemType itemType, int count)
    {
        public InventoryItemType ItemType { get; } = itemType;
        public int Amount { get; } = count;
    }
}
