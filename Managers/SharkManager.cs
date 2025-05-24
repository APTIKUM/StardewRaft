using StardewRaft.Models;
using StardewRaft.Core.Feature;
using StardewRaft.Models.Raft;
using System.Numerics;


namespace StardewRaft.Managers
{
    public class SharkManager
    {
        private SharkModel _sharkModel;
        private RaftModel _raftModel;

        private RandomTimer _timerSetTarget = new(500, 7 * 1000);

        private RandomTimer _timerSetBaseSpeed = new(500, 15 * 1000);

        private RandomTimer _timerEatRaftTile = new(500, 1000);

        private Random _rnd = new Random();

        public SharkManager(SharkModel sharkModel, RaftModel raftModel)
        {
            _sharkModel = sharkModel;
            _raftModel = raftModel;
            SetTarget();

            _timerEatRaftTile.Tick += _timerEatRaftTile_Tick;

            _timerSetBaseSpeed.Tick += _sharkTimerSetBaseSpeed_Tick;
            _timerSetBaseSpeed.Start();

            _timerSetTarget.Tick += _sharkTimerSetTarget_Tick;
            _timerSetTarget.Start();
        }

        private void _timerEatRaftTile_Tick(object? sender, EventArgs e)
        {
            if(_sharkModel.TargetRaftTile is not null)
            {
                if (_sharkModel.TargetRaftTile.TryDestroy(_rnd.Next(15, 25)))
                {
                    _sharkModel.SetRandomSideTarget();
                    _sharkModel.IsEatingRaft = false;
                    _timerEatRaftTile.Stop();
                }
            }
        }

        private void _sharkTimerSetBaseSpeed_Tick(object? sender, EventArgs e)
        {
            SetBaseSpeed();
        }

        private void SetBaseSpeed()
        {
            _sharkModel.SetBaseSpeed(Math.Clamp(_sharkModel.NowBaseSpeed + (_rnd.Next(2) == 1 ? 1 : -1) * 0.5f, _sharkModel.MinBaseSpeed, _sharkModel.MaxBaseSpeed));
        }

        private void _sharkTimerSetTarget_Tick(object? sender, EventArgs e)
        {
            SetTarget();
        }

        private void SetTarget()
        {
            if (_sharkModel.Target == null && !_sharkModel.IsEatingRaft)
            {
                if (_rnd.Next(0, 5) == 0 && _raftModel.BorderTiles.Count != 0)
                {
                    SetRaftTarget();
                }
                else
                {
                    _sharkModel.SetRandomSideTarget();
                }
            }
        }

        private void SetRaftTarget()
        {
            _sharkModel.TargetRaftTile = _raftModel.BorderTiles
                .MinBy(t => Vector2.Distance(_sharkModel.Position, t.Collider.Location.ToVector2()));

            var tileCollider = _sharkModel.TargetRaftTile.Collider;
            var vectorPos = new Vector2(tileCollider.X + (_sharkModel.Speed.X >= 0 ? tileCollider.Width : 0),
                tileCollider.Y + tileCollider.Height / 2);

            _sharkModel.SetTarget(vectorPos);
            _sharkModel.GotTarget += OnSharkGotTarget;
        }

        private void OnSharkGotTarget()
        {
            _sharkModel.GotTarget -= OnSharkGotTarget;
            if (_raftModel.BorderTiles.Contains(_sharkModel.TargetRaftTile))
            {
                _sharkModel.IsEatingRaft = true;
                _timerEatRaftTile.Start();
            }
        }

        public void UpdateModel()
        {
            if (!_sharkModel.IsEatingRaft)
            {
                _sharkModel.MoveToTarget();
            }
            else if (!_raftModel.BorderTiles.Contains(_sharkModel.TargetRaftTile))
            {
                _sharkModel.IsEatingRaft = false;
            }
        }
    }
}
