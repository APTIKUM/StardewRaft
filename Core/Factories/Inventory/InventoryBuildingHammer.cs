using StardewRaft.Core.Factories.InventoryItem;
using StardewRaft.Core.Feature;
using StardewRaft.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardewRaft.Core.Factories.Inventory
{
    internal class InventoryBuildingHammer : IInvetoryTool
    {
        public string Title => InventoryItemFactory.ItemsTitles[Type];
        public string Description { get; set; }
        public InventoryItemType Type => InventoryItemType.BuildingHammer;
        public int StackCount => 1;
        public int Count { get; set; } = 1;
        public int NowDurability { get; set; } = 40;
        public int MaxDurability => 40;

        public void OnMouseClick(CustomMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (BuildingHammerModel.TryAddTile(e.Location))
                {
                    NowDurability--;
                }
            }
            else
            {
                if (BuildingHammerModel.TryRemoveTile(e.Location))
                {
                    NowDurability--;
                }
            }
        }

    }
}
