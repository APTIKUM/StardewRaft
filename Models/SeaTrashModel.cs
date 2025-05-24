using StardewRaft.Core.Factories;
using StardewRaft.Models.Raft;
using System.Numerics;

namespace StardewRaft.Models
{
    public class SeaTrashModel
    {
        private Random _rnd = new();
        public static SeaBackgroundModel SeaModel { get; set; }

        private float offSetRatioAreaForTrash = 2f;
        public RectangleF _trashArea => new RectangleF(
            SeaModel.Position.X - SeaModel.SeaSize.Width * (offSetRatioAreaForTrash - 1) / 2,
            SeaModel.Position.Y - SeaModel.SeaSize.Height * (offSetRatioAreaForTrash - 1) / 2,
            SeaModel.SeaSize.Width * offSetRatioAreaForTrash,
            SeaModel.SeaSize.Height * offSetRatioAreaForTrash);

        private Vector2 _offsetSpawn;

        public List<SeaTrash> SeaTrashList { get; private set; } = new();

        public int Count => SeaTrashList.Count;

        private RaftModel _raftModel;

        public event Action ModelUpdated;

        public SeaTrashModel(SeaBackgroundModel seaBackgroundModel, RaftModel raftModel)
        {
            SeaModel = seaBackgroundModel;
            _raftModel = raftModel;

            _offsetSpawn = new((SeaModel.Position.X - _trashArea.Left) - 100, (SeaModel.Position.Y - _trashArea.Top) - 100);
        }

        public void Move()
        {
            if (SeaModel.Wind != null)
            {
                Parallel.ForEach(SeaTrashList, MoveItemWithCollider);
                Update();
            }
        }

        public void MoveItemSimply(SeaTrash item)
        {
            item.Position += SeaModel.Wind?.Speed ?? Vector2.Zero;
        }


        public void MoveItemWithCollider(SeaTrash item)
        {
            var newX = item.Position.X;
            var newY = item.Position.Y;

            if (!_raftModel.IsIntersectsWithRaft(new RectangleF(item.Position.X + SeaModel.Wind.Speed.X, item.Position.Y, item.Collider.Width, item.Collider.Height)))
            {
                newX += SeaModel.Wind.Speed.X;
            }

            if (!_raftModel.IsIntersectsWithRaft(new RectangleF(newX, item.Position.Y + SeaModel.Wind.Speed.Y, item.Collider.Width, item.Collider.Height)))
            {
                newY += SeaModel.Wind.Speed.Y;
            }

            var newPosition = new Vector2(newX, newY);
            item.Position = newPosition;

        }

        public void Clear()
        {
            var localTrashArea = _trashArea;
            SeaTrashList = SeaTrashList.Where(model => localTrashArea.Contains(model.Collider)).ToList();

            Update();
        }

        public void SpawnRandomTrash(int count = 1)
        {
            for(var i = 0; i < count; i++)
            {
                SeaTrashList.Add(SeaTrashFactory.CreateRandom(GetTrashStartPosition()));
            }

            Update();
        }

        public void SpawnTrashInPos(SeaTrash trash, Vector2 position)
        {
            trash.Position = position;
            SeaTrashList.Add(trash);
            Update();
        }

        public Vector2 GetTrashStartPosition()
        {
            var spawnZone = _trashArea;

            var startX = SeaModel.Wind?.Speed.X > 0 ? spawnZone.Left + _rnd.Next((int)_offsetSpawn.X) : spawnZone.Right - _rnd.Next((int)_offsetSpawn.X);
            var startY = SeaModel.Wind?.Speed.Y > 0 ? spawnZone.Top + _rnd.Next((int)_offsetSpawn.Y) : spawnZone.Bottom - _rnd.Next((int)_offsetSpawn.Y);

            if (_rnd.Next(2) == 1)
            {
                startX = _rnd.Next((int)spawnZone.Left, (int)spawnZone.Right);
            }
            else
            {
                startY = _rnd.Next((int)spawnZone.Top, (int)spawnZone.Bottom);
            }

            return new Vector2(startX, startY);
        }

        public SeaTrash? TryGetTrashByPosition(PointF position)
        {
            return SeaTrashList.FirstOrDefault(t => t.Collider.Contains(position));
        }

        public void RemoveTrash(SeaTrash seaTrash)
        {
            SeaTrashList.Remove(seaTrash);
            Update();
        }

        public void Update()
        {
            ModelUpdated?.Invoke();
        }
    }
}
