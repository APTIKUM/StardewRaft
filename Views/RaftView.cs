using StardewRaft.Properties;
using StardewRaft.Models.Raft;
using StardewRaft.Image.Structs;

namespace StardewRaft.Views
{
    public class RaftView
    {
        private Bitmap _imageTileBottom = Resources.raft_tile_sprite_0_plactic_bottom;
        private Bitmap _imageTile = Resources.raft_tile_sprite_0;

        private RaftModel _model;
        private Form _renderForm;

        private TextureStruct _textureTile;
        private TextureStruct _textureTileBottom;        

        public RaftView(RaftModel model, Form renderForm)
        {
            _model = model;
            _model.ModelUpdated += OnModelUpdated;

            _textureTile = new TextureStruct(_imageTile, RaftTile.ColliderSize.Width, RaftTile.ColliderSize.Height);
            _textureTileBottom = new TextureStruct(_imageTileBottom, RaftTile.ColliderSize.Width, 15);

            _renderForm = renderForm;
        }

        private void OnModelUpdated()
        {
            _renderForm.Invalidate();
        }

        public void Draw(Graphics graphics)
        {
            foreach (var tile in _model.Tiles)
            {
                graphics.DrawImage(_textureTile.Image, tile.Collider);
            }

            //DrawBorder(graphics);
        }

        public void DrawBottom(Graphics graphics)
        {
            //отрисовка пластика для объема , шобы все афигели как могут на Руси
            foreach (var bottomTile in _model.BottomTiles)
            {
                var renderRect = bottomTile.Collider;
                renderRect.Y += _textureTile.Height;
                renderRect.Height = _textureTileBottom.Height;

                graphics.DrawImage(_textureTileBottom.Image, renderRect);
            }
        }
    }
}
