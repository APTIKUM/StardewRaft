using StardewRaft.Models.Craft;
using StardewRaft.Views.Inventory;
using StardewRaft.Core.Factories;

namespace StardewRaft.Controllers
{
    public static class CraftPanelController
    {
        private static CraftPanelView _view;
        public static CraftPanelView View
        {
            get => _view;
            set
            {
                _view = value;
                View.RequestCraftItem += OnRequestCraftItem;
            }
        }


        public static void ChangeOpen(bool isOpen)
        {
            View.IsOpen = isOpen;

        }

        private static void OnRequestCraftItem(CraftRecipe recipe)
        {
            CraftPanelModel.TryCraftItem(recipe);
        }

    }
}
