using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardewRaft.Core.Feature
{
    public static class PointFExtensions
    {
        public static PointF RotatePoint(this PointF point, PointF pivot, float angle)
        {
            float cos = (float)Math.Cos(angle);
            float sin = (float)Math.Sin(angle);

            float x = point.X - pivot.X;
            float y = point.Y - pivot.Y;

            float rotatedX = x * cos - y * sin;
            float rotatedY = x * sin + y * cos;

            return new PointF(rotatedX + pivot.X, rotatedY + pivot.Y);
        }
    }
}
