using StardewRaft.Core.Factories;
using StardewRaft.Core.Factories.InventoryItem;

namespace StardewRaft.Models
{
    public class InventoryModel
    {
        public List<IInventoryItem?> Items;

        public int FreeSlotCount => Items.Count(item => item is null);
        public int IndexActiveSlot { get; private set; } = 0;
        public IInventoryItem? ActiveSlot => Items[IndexActiveSlot];
        public int MaxCapacity { get; private set; } = 20;

        public event Action ModelUpdated;
        public void Update()
        {
            ClearInventory();
            ModelUpdated?.Invoke();
        }

        public InventoryModel()
        {
            Items = Enumerable.Repeat<IInventoryItem?>(null, MaxCapacity).ToList();

            Items[0] = InventoryItemFactory.CreateInventoryCraftableItem(InventoryItemType.Hook);
            
        }

        public IInventoryItem? AddItemWithStack(IInventoryItem itemToAdd)
        {
            foreach(var item in Items.Where(x => x != null 
                                                && x.Type == itemToAdd.Type 
                                                && x.Count < x.StackCount))
            {
                var allowedMoveItemsCount = Math.Min(itemToAdd.Count, item.StackCount - item.Count);

                itemToAdd.Count -= allowedMoveItemsCount;
                item.Count += allowedMoveItemsCount;

                if (itemToAdd.Count == 0)
                {
                    Update();
                    return null;
                }
            }

            var firstNullIndex = Items.IndexOf(null);

            if (firstNullIndex != -1) 
            {
                Items[firstNullIndex] = itemToAdd;
                Update();
                return null;
            }

            return itemToAdd;
        }

        public void SetActiveSlot(int index)
        {
            IndexActiveSlot = index;
            Update();
        }

        public void InsertItem(IInventoryItem item, int index)
        {
            if (0 <= index && index < MaxCapacity)
            {
                Items[index] = item;
            }
            Update();
        }

        public IInventoryItem? GetItemAt(int index)
        {
            if (index >= 0 && index < Items.Count)
            {
                return Items[index];
            }

            return null;
        }

        public void DeleteItemAt(int index)
        {
            if (0 <= index && index < Items.Count)
            {
                Items[index] = null;
            }
            Update();
        }


        public void ClearInventory()
        {
            Items = Items.Select(item =>
            {
                if (item == null || item.Count == 0 || (item is IInvetoryTool tool && tool.NowDurability <= 0))
                {
                    return null;
                }
                else
                {
                    return item;
                }
            }).ToList();
        }

    }
}
