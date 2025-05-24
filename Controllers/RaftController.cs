using StardewRaft.Core.Feature;
using StardewRaft.Models.Raft;

namespace StardewRaft.Controllers
{
    public class RaftController
    {
        private RaftModel _model;
        public RaftController(RaftModel model)
        {
            _model = model;
        }

        public void UpdateModel()
        {
            _model.Update();
        }

        public void RequestChangeRaft(CustomMouseEventArgs e)
        {
            var tileRaftPosition = GetTileOnRaftPosition(e.Location);

            if (e.Button == MouseButtons.Left)
            {
                TryAddTile(tileRaftPosition);
            }
            else if (e.Button == MouseButtons.Right)
            {
                TryRemoveTile(tileRaftPosition);
            }
        }

        public Point GetTileOnRaftPosition(Point location)
        {
            return _model.GetTileOnRaftPosition(location);
        }

        public bool TryAddTile(Point tileRaftPosition)
        {   
            if (_model.GetTileByPosition(tileRaftPosition) == null)
            {
                if (_model.Tiles.Any(t =>
                    Math.Abs(t.PositionInRaft.X - tileRaftPosition.X) + Math.Abs(t.PositionInRaft.Y - tileRaftPosition.Y) == 1))
                {
                    _model.AddTile(new RaftTile(tileRaftPosition));

                    return true;
                }
            }

            return false;
            
        }

        public bool TryRemoveTile(Point tileRaftPosition)
        {
            var tile = _model.GetTileByPosition(tileRaftPosition);
            if (tile != null)
            {
                _model.RemoveTile(tile);
                return true;
            }

            return false;
        }
    }
}
