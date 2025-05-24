using StardewRaft.Core.Factories;
using StardewRaft.Core.Factories.InventoryItem;
using StardewRaft.Core.Feature;
using StardewRaft.Models;
using System.Net.Http.Headers;
using System.Numerics;
using Timer = System.Windows.Forms.Timer;


namespace StardewRaft.Controllers
{
    public class PlayerController
    {
        private PlayerModel _model;

        private Timer _decreaseThirstTimer = new();
        private Timer _decreaseHungerTimer = new();
        private Timer _processStatsEffectsTimer = new();

        public PlayerController(PlayerModel model)
        {
            _model = model;

            _decreaseThirstTimer.Interval = 6 * 1200;
            _decreaseHungerTimer.Interval = 7 * 1100;
            _processStatsEffectsTimer.Interval = 6 * 1000;

            _decreaseThirstTimer.Tick += OnDecreaseThirstTick;
            _decreaseHungerTimer.Tick += OnDecreaseHungerTick;
            _processStatsEffectsTimer.Tick += OnProcessStatsEffectsTimerTick; ;

            _decreaseHungerTimer.Start();
            _decreaseThirstTimer.Start();
            _processStatsEffectsTimer.Start();
        }

        private void OnProcessStatsEffectsTimerTick(object? sender, EventArgs e)
        {
            _model.ProcessStatsEffects();
        }

        private void OnDecreaseThirstTick(object? sender, EventArgs e)
        {
            _model.DecreaseThirst(1);
        }
        private void OnDecreaseHungerTick(object? sender, EventArgs e)
        {
            _model.DecreaseHunger(1);
        }

        public void UpdateModel(bool[] keyPressed)
        {
            if (!_model.IsPlayerOnRaft(_model.Collider))
            {
                _model.GetDamage(1);
            }
            MoveModel(keyPressed);
            
        }

        public void MoveModel(bool[] keyPressed)
        {
            _model.SetZeroSpeed();

            if (keyPressed[(int)Keys.A] || keyPressed[(int)Keys.Left])
            {
                _model.IncreaseSpeed(new Vector2(-_model.MaxSpeed, 0));
            }
            if (keyPressed[(int)Keys.D] || keyPressed[(int)Keys.Right])
            {
                _model.IncreaseSpeed(new Vector2(_model.MaxSpeed, 0));
            }
            if (keyPressed[(int)Keys.W] || keyPressed[(int)Keys.Up])
            {
                _model.IncreaseSpeed(new Vector2(0, -_model.MaxSpeed));
            }
            if (keyPressed[(int)Keys.S] || keyPressed[(int)Keys.Down])
            {
                _model.IncreaseSpeed(new Vector2(0, _model.MaxSpeed));
            }
            

            if (_model.Speed != Vector2.Zero) 
            {
                _model.Move();
            }
        }

        public bool TryUseActiveInventoryItem(CustomMouseEventArgs e)
        {
            return _model.TryUseActiveInventoryItem(e);
        }

        public bool IsInInteractionRange(PointF position)
        {
            return _model.IsInInteractionRange(position);
        }

        public bool TryAddToInventory(IInventoryItem item)
        {
            return _model.TryAddToInventory(item);
        }

        public bool TryUseActiveInventoryItemOnMouseDown(CustomMouseEventArgs e)
        {
            return _model.TryUseActiveInventoryItemOnMouseDown(e);
        }

    }
}
