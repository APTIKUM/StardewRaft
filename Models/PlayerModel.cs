using StardewRaft.Core.Factories;
using StardewRaft.Core.Factories.InventoryItem;
using StardewRaft.Core.Feature;
using StardewRaft.Models.Raft;
using System.Numerics;

namespace StardewRaft.Models
{
    public class PlayerModel
    {
        private RaftModel _raftModel;
        public InventoryModel InventoryModel { get; }
        public Vector2 Position { get; private set; }
        public RectangleF Collider => new RectangleF(Position.X, Position.Y, 30, 10);
        public Vector2 Speed { get; private set; } = Vector2.Zero;
        public float MaxSpeed { get; private set; } = 4f;
        public int Health { get; private set; }
        public int Hunger { get; private set; }
        public int Thirst { get; private set; }
        public bool IsStartUseItem { get; private set; } = false;
        public int MaxHealth => 100;
        public int MaxHunger => 100;
        public int MaxThirst => 100;
        public int DangerHunger => 20;
        public int DangerThirst => 20;

        public RectangleF InteractionRange => new RectangleF(
            Position.X - 95, // заранее просчитал значения
            Position.Y - 140, // todo  убрать в отдельный метод
            _interactionRangeSize,
            _interactionRangeSize);

        public int _interactionRangeSize = 220;

        public event Action ModelUpdated;
        public event Action StatsUpdated;
        public PlayerModel(float x, float y, RaftModel raftModel, InventoryModel inventoryModel)
        {
            Position = new Vector2(x, y);

            _raftModel = raftModel;
            InventoryModel = inventoryModel;

            Health = MaxHealth;
            Hunger = MaxHunger;
            Thirst = MaxThirst;
        }

        public void DecreaseHunger(int amount = 1)
        {
            Hunger = UpdateStat(Hunger, -amount, MaxHunger);
        }

        public void IncreaseHunger(int amount)
        {
            Hunger = UpdateStat(Hunger, amount, MaxHunger);
        }

        public void DecreaseThirst(int amount = 1)
        {
            Thirst = UpdateStat(Thirst, -amount, MaxThirst);
        }

        public void IncreaseThirst(int amount)
        {
            Thirst = UpdateStat(Thirst, amount, MaxThirst);
        }

        private int UpdateStat(int stat, int change, int maxValue)
        {
            StatsUpdated?.Invoke();
            return Math.Clamp(stat + change, 0, maxValue);
        }

        public void ProcessHungerEffects()
        {
            if (Hunger <= DangerHunger)
            {
                GetDamage(1);
            }
        }

        public void ProcessThirstEffects()
        {
            if (Thirst <= DangerThirst)
            {
                GetDamage(1);
            }
        }

        public void ProcessStatsEffects()
        {
            ProcessHungerEffects();
            ProcessThirstEffects();
        }

        public void GetDamage(int damage)
        {
            Health -= damage;
        }

        public void Move()
        {
            MormalizeSpeed();

            if (CheckNextPosition())
            {
                Position += Speed;
            }
            else
            {
                Speed = Vector2.Zero;
            }
            
            Update();
        }

        private bool CheckNextPosition() 
        {
            var nextPosition = Position + Speed;
            nextPosition += new Vector2(Collider.Width * Math.Sign(Speed.X),
                                        Collider.Height * Math.Sign(Speed.Y));

            if (IsPlayerOnRaft(new RectangleF(nextPosition.X, nextPosition.Y,
                                               Collider.Width, Collider.Height)))
            {
                return true;
            }

            return false;
        }

        public bool IsPlayerOnRaft(RectangleF playerCollider)
        {
            return _raftModel.IsIntersectsWithRaft(playerCollider);
        }

        private void MormalizeSpeed()
        {
            if (Speed.X != 0 && Speed.Y != 0)
            {
                Speed *= 0.7f;
            }
        }

        public void SetZeroSpeed()
        {
            Speed = Vector2.Zero;
        }

        private void LimitSpeed()
        {
            Speed = new Vector2(Math.Clamp(Speed.X, -MaxSpeed, MaxSpeed),
                                Math.Clamp(Speed.Y, -MaxSpeed, MaxSpeed));
        }

        public void IncreaseSpeed(Vector2 increaseSpeed)
        {
            Speed += increaseSpeed;
            LimitSpeed();
        }

        public void Update()
        {
            ModelUpdated?.Invoke();
        }

        public bool IsInInteractionRange(PointF position)
        {
            return InteractionRange.Contains(position);
        }

        public bool TryAddToInventory(IInventoryItem item)
        {
            return InventoryModel.AddItemWithStack(item) == null;
        }


        public bool TryUseActiveInventoryItem(CustomMouseEventArgs e)
        {
            var activeItem = InventoryModel.ActiveSlot;

            if (activeItem == null || activeItem is not IUseableOnMouseClick)
            {
                return false;
            }

            if (activeItem is IInventoryFood)
            {
                EatFood((IInventoryFood)activeItem);
                ((IUseableOnMouseClick)activeItem).OnMouseClick(e);
                InventoryModel.Update();
                return true;
            }

            if (activeItem is IInvetoryTool)
            {
                ((IInvetoryTool)activeItem).OnMouseClick(e);
                InventoryModel.Update();
                return true;
            }

            return false;
        }

        public bool TryUseActiveInventoryItemOnMouseDown(CustomMouseEventArgs e)
        {
            var activeItem = InventoryModel.ActiveSlot;

            if (activeItem == null || activeItem is not IUseableOnMouseDown)
            {
                return false;
            }

            ((IUseableOnMouseDown)activeItem).OnMouseDown(e);
            InventoryModel.Update();
            return true;

        }

        public void EatFood(IInventoryFood food)
        {
            IncreaseHunger(food.HungerIncrease);
            IncreaseThirst(food.ThirstIncrease);
        }

    }
}
