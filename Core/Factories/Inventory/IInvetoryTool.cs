using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardewRaft.Core.Factories.InventoryItem
{
    internal interface IInvetoryTool : IInventoryItem , IUseableOnMouseClick 
    {
        public int NowDurability { get; set; }
        public int MaxDurability { get; }
    }
}
