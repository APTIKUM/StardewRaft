using StardewRaft.Models;
using StardewRaft.Properties;
using StardewRaft.Image.Structs;

namespace StardewRaft.Views
{
    public class SeaBackgroundView
    {
        private TextureStruct _textureTile = new TextureStruct(Resources.sea_sprite_1);
        private SeaBackgroundModel _model;
        private readonly Form _renderForm;

        public SeaBackgroundView(SeaBackgroundModel model, Form renderForm)
        {
            _model = model;
            _model.ModelUpdated += OnModelUpdated;
            _renderForm = renderForm;

            _textureTile.Width = _model.TileSize.Width;
            _textureTile.Height = _model.TileSize.Height;
        }

        private void OnModelUpdated()
        {
            _renderForm.Invalidate();
        }

        public void Draw(Graphics graphics)
        {
            int tilesCountX = CalculateTilesNeeded(_model.SeaSize.Width, _textureTile.Width);
            int tilesCountY = CalculateTilesNeeded(_model.SeaSize.Height, _textureTile.Height);

            for (int tileX = -2; tileX < tilesCountX; tileX++)
            {
                for (int tileY = -2; tileY < tilesCountY; tileY++)
                {
                    int drawX =(int)(tileX * _textureTile.Width + (int)_model.Position.X - (int)_model.Position.X % _model.TileSize.Width + (int)_model.OffsetPosition.X);
                    int drawY = (int)(tileY * _textureTile.Height + (int)_model.Position.Y - (int)_model.Position.Y % _model.TileSize.Height + (int)_model.OffsetPosition.Y);

                    graphics.DrawImage(_textureTile.Image, drawX, drawY, _textureTile.Width, _textureTile.Height);
                }
            }
        }

        private int CalculateTilesNeeded(float viewportSize, float textureSize)
        {
            return (int)Math.Ceiling(viewportSize / textureSize) + 2;
        }
    }
}
