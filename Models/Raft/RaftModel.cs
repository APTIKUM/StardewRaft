using System.Numerics;

namespace StardewRaft.Models.Raft
{
    public class RaftModel
    {
        
        public List<RaftTile> Tiles { get; private set; } = new List<RaftTile>() { 
            new(0, 0), new(0, 1), new(1, 0), new(1, 1), };
        public List<RaftTile> BottomTiles { get; private set; }
        public List<RaftTile> BorderTiles { get; private set; }
        public Vector2 Position { get; private set; }

        public event Action ModelUpdated;

        public RaftModel(int x, int y)
        {
            RaftTile.BaseRaft = this;
            Position = new Vector2(x, y);
            SelectBottomTiles();
            SelectBorderTiles();
        }

        public RaftTile? GetTileByPosition(Point tileRaftPosition)
        {
            return Tiles.FirstOrDefault(t => t.PositionInRaft == tileRaftPosition);
        }

        public Point GetTileOnRaftPosition(int screenX, int screenY)
        {
            var formattedX = screenX - Position.X;
            var formattedY = screenY - Position.Y;

            var tileRaftX = (int)(formattedX / RaftTile.ColliderSize.Width + (formattedX < 0 ? -1 : 0));
            var tileRaftY = (int)(formattedY / RaftTile.ColliderSize.Height + (formattedY < 0 ? -1 : 0));

            return new Point(tileRaftX, tileRaftY);
        }

        public Point GetTileOnRaftPosition(Point location)
        {
            return GetTileOnRaftPosition(location.X, location.Y);
        }

        public Point GetTileOnRaftPosition(Vector2 location)
        {
            return GetTileOnRaftPosition((int)location.X, (int)location.Y);
        }
        public void AddTile(RaftTile newTile)
        {
            Tiles.Add(newTile);
            Update();
        }

        public void RemoveTile(RaftTile tile)
        {
            Tiles.Remove(tile);
            Update();
        }

        public void Update()
        {
            SelectBottomTiles();
            SelectBorderTiles();
            ModelUpdated?.Invoke();
        }

        private void SelectBottomTiles()
        {
            BottomTiles = Tiles
                .GroupBy(t => t.PositionInRaft.X)
                .SelectMany(g =>
                {
                    var sortedGroup = g.OrderBy(t => t.PositionInRaft.Y).ToList();
                    return sortedGroup.Where((t, index) =>
                                            index == sortedGroup.Count - 1 ||
                                            sortedGroup[index + 1].PositionInRaft.Y != t.PositionInRaft.Y + 1);
                })
                .ToList();
        }

        private void SelectBorderTiles()
        {
            BorderTiles = Tiles
                .GroupBy(t => t.PositionInRaft.Y)
                .SelectMany(g =>
                {
                    var sortedRow = g.OrderBy(t => t.PositionInRaft.X).ToList();
                    return new[] { sortedRow.First(), sortedRow.Last() };
                })
                .Concat(Tiles
                    .GroupBy(t => t.PositionInRaft.X)
                    .SelectMany(g =>
                    {
                        var sortedCol = g.OrderBy(t => t.PositionInRaft.Y).ToList();
                        return new[] { sortedCol.First(), sortedCol.Last() };
                    }))
                .Distinct()
                .ToList();
        }

        public bool IsIntersectsWithRaft(RectangleF collider)
        {
            return Tiles.Any(tile => tile.Collider.IntersectsWith(collider));
        }
    }
}
