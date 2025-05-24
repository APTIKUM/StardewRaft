using StardewRaft.Models;

namespace StardewRaft.Controllers
{
    public static class HookController
    {
        public static void UpdateModel()
        {
            if (HookModel.IsActive)
            {
                HookModel.HookSeaTrash();
            }
        }

        public static void MoveToPlayer()
        {
            if (HookModel.IsActive)
            {
                HookModel.MoveToPlayer();
            }
        }

    }
}
