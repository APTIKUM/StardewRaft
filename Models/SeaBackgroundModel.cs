using System.Numerics;

namespace StardewRaft.Models
{
    public class SeaBackgroundModel
    {
        private CameraModel _cameraModel;
        private Form _renderForm;
        public Size TileSize { get; private set; } = new Size(128, 128);
        public Size SeaSize => _renderForm.ClientSize;
        public Vector2 Position => _cameraModel.ScreenToWorld(Vector2.Zero);
        public Vector2 OffsetPosition { get; private set; }
        public WindModel? Wind { get; private set; }
        
        public event Action ModelUpdated;

        public SeaBackgroundModel(Form renderForm, CameraModel cameraModel, WindModel? wind = null)
        {
            _cameraModel = cameraModel;
            _renderForm = renderForm;
            Wind = wind;
        }

        public void Move()
        {
            if(Wind != null)
            {
                OffsetPosition += Wind.Speed;
                LoopOffSetPosition();
                Update();
            }
        }

        public void Update()
        {
            ModelUpdated?.Invoke();

        }

        private void LoopOffSetPosition()
        {
            OffsetPosition = new Vector2(OffsetPosition.X % TileSize.Width, OffsetPosition.Y % TileSize.Height); 
        }

        public void Resize(int newWidth, int newHeight)
        {
            ModelUpdated?.Invoke();
        }
    }
}
