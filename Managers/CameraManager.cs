using StardewRaft.Models;

namespace StardewRaft.Managers
{
    public class CameraManager
    {
        private CameraModel _model;

        public CameraManager(CameraModel model) 
        {
            _model = model;
        }

        public void UpdateMovel()
        {
            _model.Update();
        }

        public void ChangeScale(bool isBigger)
        {
            _model.ChangeScale(isBigger);
        }
    }
}
