using StardewRaft.Core.Factories;

namespace StardewRaft.Models.Craft
{
    public class CraftRecipe(InventoryItemType craftItem, List<CraftIngredient> ingredients, int craftCount)
    {
        public InventoryItemType CraftItem { get; } = craftItem;
        public List<CraftIngredient> Ingredients { get; } = ingredients;
        public int CraftCount { get; } = craftCount;
    }
}
