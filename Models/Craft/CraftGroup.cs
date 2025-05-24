using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardewRaft.Models.Craft
{
    public class CraftGroup(string title, List<CraftRecipe> craftRecipes)
    {
        public string Title { get; } = title;
        public List<CraftRecipe> CraftRecipes { get; } = craftRecipes;
    }
}
