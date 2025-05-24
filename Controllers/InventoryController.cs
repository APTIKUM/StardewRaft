using StardewRaft.Core.Factories;
using StardewRaft.Models;
using StardewRaft.Views.Inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace StardewRaft.Controllers
{
    public class InventoryController
    {
        private InventoryModel _model;
        private InventoryView _view;

        public InventoryController(InventoryModel model, InventoryView view) 
        {
            _model = model;
            _view = view;

            _view.RequestDeleteItemAt += DeleteItemAt;
            _view.RequestDragAndDropItem += DragAndDropItem;
            _view.RequestSetActiveSlot += SetActiveSlot;
        }

        public void DragAndDropItem(int indexFrom, int indexTo)
        {
            var itemIndexFrom = _model.GetItemAt(indexFrom);
            var itemIndexTo = _model.GetItemAt(indexTo);
         
            if (!TryStackItems(itemIndexFrom, itemIndexTo))
            {
                _model.InsertItem(itemIndexTo, indexFrom);
                _model.InsertItem(itemIndexFrom, indexTo);
            }
        }

        public bool TryStackItems(IInventoryItem itemFrom, IInventoryItem itemTo)
        {
            if (itemFrom != null && itemTo != null
                && itemFrom.Type == itemTo.Type)
            {
                var allowedMoveItemsCount = Math.Min(itemFrom.Count, itemTo.StackCount - itemTo.Count);

                if (allowedMoveItemsCount == 0)
                {
                    return false;
                }

                itemFrom.Count -= allowedMoveItemsCount;
                itemTo.Count += allowedMoveItemsCount;
                _model.Update();

                return true;
            }


            return false;
        }


        public void DeleteItemAt(int indexItem)
        {
            _model.DeleteItemAt(indexItem);
        }

        public void SetActiveSlot(int indexSlot)
        {
            _model.SetActiveSlot(indexSlot);
        }
    }
}
