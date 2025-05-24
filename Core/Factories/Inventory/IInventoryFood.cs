namespace StardewRaft.Core.Factories.InventoryItem
{
    public interface IInventoryFood : IInventoryItem, IUseableOnMouseClick
    {
        public int HungerIncrease { get; }
        public int ThirstIncrease { get; }
    }
}
