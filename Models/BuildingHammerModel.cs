using StardewRaft.Controllers;
using StardewRaft.Core.Factories;
using StardewRaft.Models.Craft;
using StardewRaft.Models.Raft;
using System.Linq;
using System.Numerics;

namespace StardewRaft.Models
{
    public static class BuildingHammerModel
    {
        public static List<CraftIngredient> RaftTileIngredients { get; set; } = [new(InventoryItemType.Wood, 2), new(InventoryItemType.Plastic, 2)];
        public static PlayerModel PlayerModel { get; set; }
        public static RaftController RaftController { get; set; }

        public static event Action ModelUpdated;

        private static void Update()
        {
            ModelUpdated?.Invoke();
        }
        
        public static bool TryRemoveTile(Point clickPos)
        {
            if (!PlayerModel.IsInInteractionRange(clickPos)) 
            {
                return false;
            }

            return RaftController.TryRemoveTile(RaftController.GetTileOnRaftPosition(clickPos));
        }

        public static bool TryAddTile(Point clickPos)
        {
            if (!PlayerModel.IsInInteractionRange(clickPos))
            {
                return false;
            }

            if (!RaftTileIngredients.All(i => CraftPanelModel.IsEnoughForIngredient(i)))
            {
                return false;
            }
            
            if (RaftController.TryAddTile(RaftController.GetTileOnRaftPosition(clickPos))) 
            {
                foreach (var ingredient in RaftTileIngredients)
                {
                    CraftPanelModel.RemoveIngredientFromInventory(ingredient);
                }

                return true;
            }

            return false;
        }

    }
}
