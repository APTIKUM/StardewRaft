using StardewRaft.Core.Feature;
using StardewRaft.Models;
using System.Numerics;

namespace StardewRaft.Managers
{
    public class WindManager
    {
        private RandomTimer _timerChangeSpeed = new RandomTimer(3 * 1000, 100 * 1000);
        private WindModel _model;


        public WindManager(WindModel model) 
        {
            _model = model;

            _timerChangeSpeed.Tick += _timerChangeSpeed_Tick;
            _timerChangeSpeed.Start();
        }

        private void _timerChangeSpeed_Tick(object? sender, EventArgs e)
        {
            _model.ChangeSpeedRandom();
        }

        public void ChangeSpeedModel(Vector2 newSpeed)
        {
            _model.CnangeSpeed(newSpeed);
        }
        
        public void UpdateModel()
        {
            
        }
    }
}
