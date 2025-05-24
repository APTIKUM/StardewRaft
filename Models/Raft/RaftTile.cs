using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardewRaft.Models.Raft
{
    public class RaftTile
    {
        public static RaftModel BaseRaft { get; set; }
        public static Size ColliderSize { get; } = new Size(80, 80);

        public Point PositionInRaft { get; }
        public RectangleF Collider => new RectangleF(PositionInRaft.X * ColliderSize.Width + BaseRaft.Position.X,
                                                    PositionInRaft.Y * ColliderSize.Height + BaseRaft.Position.Y,
                                                    ColliderSize.Width, ColliderSize.Height);
        public int Health { get; private set; } = 100;

        public RaftTile(int x, int y)
        {
            PositionInRaft = new Point(x, y);
        }

        public RaftTile(Point position)
        {
            PositionInRaft = position;
        }

        public bool TryDestroy(int damage)
        {
            Health -= damage;

            if (Health <= 0)
            {
                BaseRaft.RemoveTile(this);
                return true;
            }
            return false;
        }
    }
}
