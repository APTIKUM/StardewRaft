using StardewRaft.Core.Factories;
using StardewRaft.Core.Feature;
using StardewRaft.Models;
using System.Numerics;

namespace StardewRaft.Managers
{
    public class SeaTrashManager
    {
        private Random _rnd = new();
        private RandomTimer _seaTrashTimerSpawnTrash = new(100, 5000);

        private SeaTrashModel _model;
        private int _maxCountTrash = 30;
        
        public void UpdateModel()
        {
            _model.Move();
        }

        public void ClearModel()
        {
            _model.Clear();
        }

        public SeaTrashManager(SeaTrashModel model)
        {
            _model = model;

            _seaTrashTimerSpawnTrash.Tick += _seaTrashTimerSpawnTrash_Tick;
            _seaTrashTimerSpawnTrash.Start();
        }

        public void SpawnTrash()
        {
            if (_model.Count < _maxCountTrash)
            {
                if (_rnd.Next(10) >= 5)
                {
                    _model.SpawnRandomTrash(Math.Clamp(_rnd.Next(1, 6), 1, _maxCountTrash - _model.Count));
                }
                else 
                {
                    _model.SpawnRandomTrash();
                }
            }
        }

        private void _seaTrashTimerSpawnTrash_Tick(object? sender, EventArgs e)
        {
            ClearModel();
            SpawnTrash();
        }


        public SeaTrash? TryGetTrashByPosition(PointF position)
        {
            return _model.TryGetTrashByPosition(position);
        }

        public void RemoveTrash(SeaTrash seaTrash)
        {
            _model.RemoveTrash(seaTrash);
        }

    }
}
