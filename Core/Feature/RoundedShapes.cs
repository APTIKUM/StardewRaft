using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardewRaft.Core.Feature
{
    public static class RoundedShapes
    {
        public static void DrawRoundedRect(Rectangle sourceRect, int corderRadius, Graphics g, Color color )
        {
            Pen pen = new Pen(color);
            Brush brush = new SolidBrush(color);

            int x = sourceRect.X;
            int y = sourceRect.Y;
            int width = sourceRect.Width;
            int height = sourceRect.Height;

            GraphicsPath path = new GraphicsPath();


            path.AddArc(x, y, corderRadius, corderRadius, 180, 90);

            path.AddArc(x + width - corderRadius, y, corderRadius, corderRadius, 270, 90);

            path.AddArc(x + width - corderRadius, y + height - corderRadius, corderRadius, corderRadius, 0, 90);

            path.AddArc(x, y + height - corderRadius, corderRadius, corderRadius, 90, 90);
            path.CloseFigure();

            g.FillPath(brush, path);
            g.DrawPath(pen, path);

            pen.Dispose();
            brush.Dispose();
            path.Dispose();
        }

    }
}
