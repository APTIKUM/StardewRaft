using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardewRaft.Core.Feature
{
    public class CustomMouseEventArgs : MouseEventArgs
    {
        public double Power { get; set; }
        public CustomMouseEventArgs(MouseButtons button, int clicks, int x, int y, int delta, double power = 0) : base(button, clicks, x, y, delta)
        {
            Power = power;
        }

        public static CustomMouseEventArgs CreateByParent(MouseEventArgs e, double power, Point worldLocation)
        {
            return new(e.Button, e.Clicks, worldLocation.X, worldLocation.Y, e.Delta, power);
        }


    }
}
