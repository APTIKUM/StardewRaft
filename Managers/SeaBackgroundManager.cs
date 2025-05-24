using StardewRaft.Models;

namespace StardewRaft.Managers
{
    public class SeaBackgroundManager
    {
        private SeaBackgroundModel _model;

        public SeaBackgroundManager(SeaBackgroundModel model) 
        {
            _model = model;
        }

        public void UpdateModel()
        {
            _model.Move();
        }

        public void ResizeModel(int newWidth, int newHeight)
        {
            _model.Resize(newWidth, newHeight);
        }
    }
}
