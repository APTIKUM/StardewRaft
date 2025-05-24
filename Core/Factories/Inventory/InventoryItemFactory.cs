using StardewRaft.Core.Factories.Inventory;
using StardewRaft.Core.Factories.InventoryItem;
using StardewRaft.Core.Feature;
using StardewRaft.Properties;
using System.Net.Http.Headers;

namespace StardewRaft.Core.Factories
{
    public static class InventoryItemFactory
    {
        private static Random _rnd = new Random();
        private static Dictionary<SeaTrashType, InventoryItemType> _seaTrashToItemType = new()
        {
            { SeaTrashType.Leaf, InventoryItemType.Leaf},
            { SeaTrashType.Plastic, InventoryItemType.Plastic},
            { SeaTrashType.Wood, InventoryItemType.Wood},
        };

        private static Dictionary<InventoryItemType, double> _itemsBarrelWeights = new()
        {
            {InventoryItemType.Leaf, 0.20 },
            {InventoryItemType.Plastic, 0.20 },
            {InventoryItemType.Wood, 0.20 },
            {InventoryItemType.Apple, 0.10 },
            {InventoryItemType.Potato, 0.30 },
            {InventoryItemType.Banana, 0.15 },
        };

        public static readonly Dictionary<InventoryItemType, string> ItemsTitles = new()
        {
            {InventoryItemType.Leaf, "Пальмовый лист" },
            {InventoryItemType.Plastic, "Пластик" },
            {InventoryItemType.Wood, "Доска" },
            {InventoryItemType.Apple, "Яблоко" },
            {InventoryItemType.Potato, "Картофель" },
            {InventoryItemType.Banana, "Банан" },
            {InventoryItemType.Hook, "Крюк" },
            {InventoryItemType.BuildingHammer, "Строительный молоток" },
            {InventoryItemType.Rope, "Веревка" },
        };

        public static readonly Dictionary<InventoryItemType, int> StackCounts = new()
        {
            {InventoryItemType.Leaf, 20 },
            {InventoryItemType.Plastic, 20},
            {InventoryItemType.Wood, 20 },
            {InventoryItemType.Rope, 20 },
            {InventoryItemType.Apple, 20 },
            {InventoryItemType.Potato, 20},
            {InventoryItemType.Banana, 20},
        };

        static InventoryItemFactory()
        {
        }

        public static IInventoryItem CreateInventoryCraftableItem(InventoryItemType craftableItem)
        {
            switch (craftableItem)
            {
                case InventoryItemType.Hook:
                    return new InventoryHook();
                case InventoryItemType.BuildingHammer:
                    return new InventoryBuildingHammer();
                case InventoryItemType.Rope:
                    return new InventoryResource(InventoryItemType.Rope);
                default:
                    throw new ArgumentException($"Unknown craftableItem type: {craftableItem}");
            }
        }

        public static List<IInventoryItem> CreateResourcesBySeaTrash(SeaTrash seaTrash)
        {
            var resultResources = new List<IInventoryItem>();

            if (seaTrash.Type == SeaTrashType.Barrel)
            {
                return CreateResourcesByBarrel();
            }

            resultResources.Add(new InventoryResource(_seaTrashToItemType[seaTrash.Type]));
            return resultResources;
        }

        private static List<IInventoryItem> CreateResourcesByBarrel()
        {
            var resultResources = new List<IInventoryItem>();

            var countResources = _rnd.Next(1, 4);

            for(int i = 0; i < countResources; i++)
            {
                var resourceType = _rnd.RandomWithWeight(_itemsBarrelWeights);
                var resourceCount = _rnd.Next(1, 4);


                if (resourceType is InventoryItemType.Potato 
                    || resourceType is InventoryItemType.Apple
                    || resourceType is InventoryItemType.Banana)
                {
                    resultResources.Add(new InventoryFood(resourceType, resourceCount));
                }
                else
                {
                    resultResources.Add(new InventoryResource(resourceType, resourceCount));
                }
            }

            return resultResources;

        }

    }
}
