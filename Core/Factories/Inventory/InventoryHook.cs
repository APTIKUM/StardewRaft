using StardewRaft.Core.Feature;
using StardewRaft.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardewRaft.Core.Factories.InventoryItem
{
    internal class InventoryHook : IInvetoryTool , IUseableOnMouseDown
    {
        public string Title => InventoryItemFactory.ItemsTitles[Type];
        public string Description { get; set; }
        public InventoryItemType Type => InventoryItemType.Hook;
        public int StackCount => 1;
        public int Count { get; set; } = 1;
        public int NowDurability { get; set; } = 20;
        public int MaxDurability => 20;

        public void OnMouseClick(CustomMouseEventArgs e)
        {
            if (HookModel.IsActive && e.Button == MouseButtons.Left && HookModel.IsIntersectsWithPlayer)
            {
                NowDurability--;
                HookModel.GiveTrashToPlayer();
            }
            else if (e.Button == MouseButtons.Left && e.Power > 0.1)
            {
                HookModel.ThrowTo(e.Location, e.Power);
            }
            else if (e.Button == MouseButtons.Right)
            {
                HookModel.CancelHook();
            }
        }

        public void OnMouseDown(CustomMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                HookModel.MoveToPlayer();
            }
        }
    }
}
