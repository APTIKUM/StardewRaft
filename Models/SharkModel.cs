using StardewRaft.Models.Raft;
using System.Numerics;

namespace StardewRaft.Models
{
    public class SharkModel
    {
        public RectangleF Collider => new RectangleF(Position.X, Position.Y, 80, 60);
        public Vector2 Position { get; private set; }
        public Vector2? Target { get; private set; }
        public RaftTile? TargetRaftTile { get; set; } 
        public bool IsEatingRaft { get; set; } = false;
        public Vector2 Speed { get; private set; }
        public SeaBackgroundModel SeaModel { get; private set; }
        public int Health { get; private set; } = 100;
        public float NowBaseSpeed { get; private set; }
        public float MinBaseSpeed { get; private set; } = 0.5f;
        public float MaxBaseSpeed { get; private set; } = 4f;
        public int MaxDeltaY { get; private set; } = 350; // насколько акула может подниматься , когда двигается на расслабоне
        public int MaxOffsetSea { get; private set; } = 100; // на сколько она может заплывать в невидимое игроку пространство

        private Random _random = new Random();


        public event Action GotTarget;
        public event Action ModelUpdated;

        public SharkModel(SeaBackgroundModel seaModel)
        {
            SeaModel = seaModel;
            Position = new Vector2(SeaModel.SeaSize.Width, SeaModel.SeaSize.Height); // акула появляется посередине  справа относительно моря
            SetRandomBaseSpeed();
        }

        public void SetBaseSpeed(float newBaseSpeed)
        {
            NowBaseSpeed = newBaseSpeed;
        }

        public void SetRandomBaseSpeed()
        {
            SetBaseSpeed(_random.Next((int)(MinBaseSpeed * 10), (int)(MaxBaseSpeed * 10)) / 10f);
        }

        public void SetTarget(Vector2? newTarget)
        {
            Target = newTarget;
        }

        public void SetRandomSideTarget()
        {
            float newTargetX = Position.X < SeaModel.Position.X ? SeaModel.SeaSize.Width + Collider.Width : -Collider.Width;
            newTargetX += SeaModel.Position.X;
            newTargetX += _random.Next(MaxOffsetSea) * Math.Sign(newTargetX);


            float newTargetY = Position.Y + _random.Next(-MaxDeltaY, MaxDeltaY);
            newTargetY = Math.Clamp(newTargetY, SeaModel.Position.Y - MaxOffsetSea, SeaModel.Position.Y + SeaModel.SeaSize.Height + MaxOffsetSea);

            SetTarget(new Vector2(newTargetX, newTargetY));
        }

        public Vector2 CulcSpeedToTarget()
        {
            if (Target != null)
            {
                float dx = Target.Value.X - (Position.X + Collider.Width / 2);
                float dy = Target.Value.Y - (Position.Y + Collider.Height / 2);
                float distance = (float)Math.Sqrt(dx * dx + dy * dy);

                if (distance > NowBaseSpeed * 20)
                {
                    return new Vector2(dx / distance * NowBaseSpeed, dy / distance * NowBaseSpeed);
                }

                Target = null;
                GotTarget?.Invoke();
            }
            return Vector2.Zero;
        }

        public void MoveToTarget()
        {
            if (Target != null)
            {
                var speedToTarget = CulcSpeedToTarget();
                if (speedToTarget != Vector2.Zero)
                {
                    Speed = speedToTarget;
                }
            }

            Move();
        }

        public void Move()
        {
            Position += Speed;
            Update();
        }

        private void Update() 
        {
            ModelUpdated?.Invoke();
        }
    }
}
