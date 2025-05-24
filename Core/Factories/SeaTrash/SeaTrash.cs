using System.Numerics;

namespace StardewRaft.Core.Factories;

public class SeaTrash
{
    public Size TrashSize { get; }
    public Vector2 Position { get; set; }
    public RectangleF Collider => new RectangleF(Position.X, Position.Y, TrashSize.Width, TrashSize.Height);
    public string Skin { get; }
    public SeaTrashType Type { get; }

    public SeaTrash(SeaTrashType type, string skin, Vector2 position, Size trashSize)
    {
        Type = type;
        Skin = skin;
        Position = position;
        TrashSize = trashSize;
    }
}