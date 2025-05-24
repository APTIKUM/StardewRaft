using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace StardewRaft.Models
{
    public class WindModel
    {
        private Random _rnd = new Random();
        public Vector2 Speed { get; private set; }

        public Vector2 MaxSpeed => new Vector2(3, 3);
        public float MinSpeedLenght => 0.5f;

        public WindModel(Vector2 speed) 
        {
            Speed = speed;
        }

        public void CnangeSpeed(Vector2 newSpeed)
        {
            Speed = newSpeed;
        }

        public void ChangeSpeedRandom()
        {
            var newSpeedX = Math.Clamp(Speed.X + _rnd.Next(-5, 5) / 10f, -MaxSpeed.X, MaxSpeed.X);
            var newSpeedY = Math.Clamp(Speed.Y + _rnd.Next(-5, 5) / 10f, -MaxSpeed.Y, MaxSpeed.Y);

            var newSpeed = new Vector2(newSpeedX, newSpeedY);

            if (newSpeed.Length() >= MinSpeedLenght)
            {
                CnangeSpeed(newSpeed);
            }

        }
    }
}
