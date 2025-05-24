namespace StardewRaft.Image.Structs;
public struct SpriteSheetAnimateStruct
{
    public TextureStruct Texture { get; private set; }
    public int FrameWidth { get; private set; }
    public int FrameHeight { get; private set; }
    public int CurrentAnimation { get; set; } = 0;
    public int CurrentFrame { get; set; } = 0;
    public Dictionary<int, int> CountFrameAnimation { get; private set; }
    public SpriteSheetAnimateStruct(TextureStruct texture, int frameWidth, int frameheight,
                             Dictionary<int, int> countFrameAnimation)
    {
        Texture = texture;
        FrameWidth = frameWidth;
        FrameHeight = frameheight;
        CountFrameAnimation = countFrameAnimation;
    }

    public SpriteSheetAnimateStruct(Bitmap textureImage, int frameWidth, int frameheight, Dictionary<int, int> countFrameAnimation)
        : this(new TextureStruct(textureImage), frameWidth, frameheight, countFrameAnimation)
    {
    }

    public Rectangle GetNextFrameRectangle()
    {
        CurrentFrame++;
        return GetFrameRectangle(CurrentFrame % CountFrameAnimation[CurrentAnimation], CurrentAnimation);
    }

    public Rectangle GetFrameRectangle(int frame, int animation = 0)
    {
        return new Rectangle(frame * FrameWidth, animation * FrameHeight, FrameWidth, FrameHeight);
    }
}