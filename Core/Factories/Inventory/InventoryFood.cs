using StardewRaft.Core.Feature;
using StardewRaft.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardewRaft.Core.Factories.InventoryItem
{
    public class InventoryFood(InventoryItemType type, int count) : IInventoryFood
    {
        private static readonly Dictionary<InventoryItemType, int> _hungerIncrease = new()
        {
            {InventoryItemType.Apple, 5 },
            {InventoryItemType.Banana, 7 },
            {InventoryItemType.Potato, 15},
        };

        private static readonly Dictionary<InventoryItemType, int> _thirstIncrease = new()
        {
            {InventoryItemType.Apple, 10 },
            {InventoryItemType.Banana, 10 },
            {InventoryItemType.Potato, 0 },
        };

        public string Title => InventoryItemFactory.ItemsTitles[Type];
        public int StackCount => InventoryItemFactory.StackCounts[Type];
        public string Description { get; }
        public InventoryItemType Type { get; } = type;
        public int Count { get; set; } = count;
        public int HungerIncrease => _hungerIncrease[Type];
        public int ThirstIncrease => _thirstIncrease[Type];

        public static InventoryModel Inventory { get; }

        public void OnMouseClick(CustomMouseEventArgs e)
        {
            Count--;
        }
    }
}
