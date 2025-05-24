using StardewRaft.Core.Factories;
using StardewRaft.Core.Feature;
using StardewRaft.Image.Structs;
using StardewRaft.Models;
using StardewRaft.Properties;

namespace StardewRaft.Views
{
    public static class HookView
    {
        private static Pen _penRope = new Pen(Color.FromArgb(140, 80, 42), 5f);

        private static readonly SpiteSheetObjectsStruct<InventoryItemType> _spriteSheetHook = new(Resources.hook,
new Dictionary<InventoryItemType, Rectangle>()
{
             { InventoryItemType.Hook , new Rectangle(0, 0, 44, 53)},
});

        public static void Draw(Graphics g)
        {
            if (HookModel.IsActive)
            {
                var hookCollider = HookModel.Collider;
                var playerCollider = HookModel.PlayerModel.Collider;
                //g.DrawRectangle(new Pen(Brushes.Green, 4), hookCollider);
                var state = g.Save();

                var pivot = new PointF(hookCollider.X + hookCollider.Width / 2, hookCollider.Y + hookCollider.Height / 2);

                g.TranslateTransform(pivot.X, pivot.Y);


                g.RotateTransform((float)(HookModel.Angle / Math.PI * 180) + 45);

                var hookColliderTransform = hookCollider;
                hookColliderTransform.X = -hookCollider.Width / 2;
                hookColliderTransform.Y = -hookCollider.Height / 2;

                g.DrawImage(_spriteSheetHook.Texture.Image, hookColliderTransform, _spriteSheetHook.GetFrameRectangle(InventoryItemType.Hook), GraphicsUnit.Pixel);
                //g.DrawRectangle(new Pen(Brushes.Red, 4), hookColliderTransform.X, hookColliderTransform.Y, hookCollider.Width, hookCollider.Height);

                g.Restore(state);

                PointF startPoint = playerCollider.Location;
                startPoint.X += playerCollider.Width / 2;
                startPoint.Y -= 30;

                var endPoint = new PointF(hookCollider.X + 3, hookCollider.Bottom - 3).RotatePoint(pivot, HookModel.Angle + (float)Math.PI / 4);

                g.DrawLine(_penRope, startPoint, endPoint);

                DrawTrash(g);
            }
        }

        

        public static void DrawTrash(Graphics g)
        {
            var hookCollider = HookModel.Collider;

            foreach (var trash in HookModel.SeaTrashHooked)
            {
                var drawRect = new RectangleF(HookModel.Position.X + (hookCollider.Width - trash.Collider.Width) / 2,
                    HookModel.Position.Y + (hookCollider.Height - trash.Collider.Height) / 2,
                    trash.Collider.Width,
                    trash.Collider.Height);

                g.DrawImage(SeaTrashView.SpriteSheetSeaTrash.Texture.Image,
                    drawRect,
                    SeaTrashView.SpriteSheetSeaTrash.GetFrameRectangle(trash.Skin),
                    GraphicsUnit.Pixel);
            }
        }
    }
}
