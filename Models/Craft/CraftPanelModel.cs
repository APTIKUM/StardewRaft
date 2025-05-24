using StardewRaft.Core.Factories;

namespace StardewRaft.Models.Craft
{
    public static class CraftPanelModel
    {
        public static PlayerModel Player { get; set; }

        public static List<CraftGroup> CraftGroups = [];
        public static bool IsOpen { get; set; }

        static CraftPanelModel()
        {
            InitializeCraftData();
        }

        private static void InitializeCraftData()
        {
            CraftGroups.Add(new("Инструменты",
            [
                new (InventoryItemType.Hook, [new(InventoryItemType.Plastic, 2), new(InventoryItemType.Wood, 1), new(InventoryItemType.Rope, 2)], 1),
                new (InventoryItemType.BuildingHammer, [new(InventoryItemType.Wood, 2), new(InventoryItemType.Rope, 1)], 1),
            ]));

            CraftGroups.Add(new("Расходники",
            [
                new (InventoryItemType.Rope, [new(InventoryItemType.Leaf, 2)], 1)
            ]));

            //CraftGroups.Add(new("Оружие", []));
        }


        public static bool TryCraftItem(CraftRecipe recipe)
        {
            if (!IsPossibleToCraft(recipe))
            {
                return false;
            }

            foreach (var ingredient in recipe.Ingredients)
            {
                RemoveIngredientFromInventory(ingredient);
            }

            Player.InventoryModel.ClearInventory();

            Player.TryAddToInventory(InventoryItemFactory.CreateInventoryCraftableItem(recipe.CraftItem));


            return true;
        }

        public static void RemoveIngredientFromInventory(CraftIngredient ingredient)
        {
            var leftAmount = ingredient.Amount;

            foreach (var item in Player.InventoryModel.Items.Where(item => item is not null && item.Type == ingredient.ItemType).ToList())
            {

                if (item.Count >= leftAmount)
                {
                    item.Count -= leftAmount;
                    leftAmount = 0;
                    return;
                }
                else
                {
                    leftAmount -= item.Count;
                    item.Count = 0;
                }
            }
        }

        public static bool IsPossibleToCraft(CraftRecipe recipe)
        {
            if (Player is null
                || Player.InventoryModel is null
                || Player.InventoryModel.FreeSlotCount == 0)
            {
                return false;
            }

            foreach (var ingredient in recipe.Ingredients)
            {
                if (!IsEnoughForIngredient(ingredient))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool IsEnoughForIngredient(CraftIngredient ingredient)
        {
            return ingredient.Amount <= Player.InventoryModel.Items
                .Where(item => item is not null && item.Type == ingredient.ItemType)
                .Sum(item => item.Count);
        }
    }
}
