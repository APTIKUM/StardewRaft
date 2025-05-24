namespace StardewRaft.Image.Structs;
public struct SpiteSheetObjectsStruct<T> where T : notnull
{
    public TextureStruct Texture {  get; private set; }

    public Dictionary<T, Rectangle> Frames { get; private set; }
    
    public SpiteSheetObjectsStruct(TextureStruct texture, Dictionary<T, Rectangle> frames)
    {
        Texture = texture;
        Frames = frames;
    }

    public SpiteSheetObjectsStruct(Bitmap textureImage, Dictionary<T, Rectangle> frames) 
        : this(new TextureStruct(textureImage), frames)
    {
    }

    public Rectangle GetFrameRectangle(T nameFrame)
    {
        if (!Frames.ContainsKey(nameFrame))
        {
            throw new ArgumentException($"Frames doesn't contains frame : {nameFrame}");
        }

        return Frames[nameFrame];
    }

    public void AddFrame(T nameFrame, Rectangle rectangleFrame)
    {
        Frames[nameFrame] = rectangleFrame;
    }
}