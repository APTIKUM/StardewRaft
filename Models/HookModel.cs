using StardewRaft.Core.Factories;
using StardewRaft.Models.Raft;
using System.Linq;
using System.Numerics;

namespace StardewRaft.Models
{
    public static class HookModel
    {
        private static Random _rnd = new();
        public static PlayerModel PlayerModel { get; set; }
        public static RaftModel RaftModel { get; set; }
        public static SeaTrashModel SeaTrashModel { get; set; }
        public static float Angle { get; private set; }
        public static Vector2 Position { get; private set; }
        public static RectangleF Collider => new(Position.X, Position.Y, 40, 40);
        public static List<SeaTrash> SeaTrashHooked = new();
        public static float BaseSpeed { get; private set; } = 4f;

        public static float MaxRopeLength = 500;
        public static bool IsIntersectsWithPlayer => Collider.IntersectsWith(PlayerModel.InteractionRange);


        public static bool IsActive = false;

        public static event Action ModelUpdated;

        private static void Update()
        {
            ModelUpdated?.Invoke();
        }


        public static void MoveToPlayer()
        {
            var shiftPos = CulcSpeedToPlayer();
            Position += shiftPos;

            if (SeaTrashHooked.Count == 0 || !RaftModel.IsIntersectsWithRaft(Collider))
            {
                UpdateAngle(true);
                Update();
                return;
            }

            Position -= shiftPos;
        }

        public static void HookSeaTrash()
        {
            var colliderHook = Collider;

            var newHookedTrash = SeaTrashModel.SeaTrashList
                        .Where(t => t.Collider.IntersectsWith(colliderHook))
                        .ToList();

            SeaTrashHooked = SeaTrashHooked
                .Concat(newHookedTrash)
                .ToList();

            foreach (var trash in newHookedTrash)
            {
                SeaTrashModel.RemoveTrash(trash);
            }

            Update();
        }

        public static Vector2 CulcSpeedToPlayer()
        {
            if (PlayerModel != null)
            {
                float dx = PlayerModel.Position.X - (Position.X + Collider.Width / 2);
                float dy = PlayerModel.Position.Y - (Position.Y + Collider.Height / 2);
                float distance = (float)Math.Sqrt(dx * dx + dy * dy);

                if (distance > BaseSpeed)
                {
                    return new Vector2(dx / distance * BaseSpeed, dy / distance * BaseSpeed);
                }    
            }

            return Vector2.Zero;
        }

        public static void GiveTrashToPlayer()
        {

            foreach(var trash in SeaTrashHooked)
            {
                var inventoryItems = InventoryItemFactory.CreateResourcesBySeaTrash(trash);

                var successTakeSeaTrash = false;

                foreach (var item in inventoryItems)
                {
                    if (PlayerModel.TryAddToInventory(item))
                    {
                        successTakeSeaTrash = true;
                    }
                }

                if (!successTakeSeaTrash)
                {
                    SeaTrashModel.SpawnTrashInPos(trash, Position);
                }
            }

            SeaTrashHooked = new List<SeaTrash>();
            IsActive = false;
            Update();
        }

        public static void ThrowTo(Vector2 position, double power)
        {

            if (!IsActive)
            {
                Vector2 playerPosition = PlayerModel.Collider.Location.ToVector2();

                Vector2 direction = position - playerPosition;

                direction = Vector2.Normalize(direction);

                float deviation = 0.20f;
                direction.X += (float)(_rnd.NextDouble() * deviation * 2 - deviation); 
                direction.Y += (float)(_rnd.NextDouble() * deviation * 2 - deviation); 


                float maxLength = MaxRopeLength * (float)power;

                Vector2 hookPosition = playerPosition + direction * maxLength;

                Position = hookPosition;

                UpdateAngle();
                Update();
                IsActive = true;
            }
        }

        public static void ThrowTo(Point position, double power)
        {
            ThrowTo(new Vector2(position.X, position.Y), power);    
        }

        public static void CancelHook()
        {
            if (SeaTrashHooked.Count == 0)
            {
                IsActive = false;
            }
        }

        public static void UpdateAngle(bool smooth = false)
        {
            Vector2 playerPosition = PlayerModel.Collider.Location.ToVector2();

            Vector2 direction = Position - playerPosition;

            var corectAngle = (float)Math.Atan2(direction.Y, direction.X);


            if (smooth)
            {
                var different = Angle - corectAngle;
               


                if (Math.Abs(different) > 0.05 && Math.Abs(different) < 4)
                {
                    Angle += Math.Sign(different) * -0.05f;
                }
                else
                {
                    Angle = corectAngle;
                }
            }
            else
            {
                Angle = corectAngle;
            }
        }
    }
}
