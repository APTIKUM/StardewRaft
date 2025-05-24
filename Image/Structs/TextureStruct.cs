namespace StardewRaft.Image.Structs;
public struct TextureStruct
{
    public Bitmap Image {  get; private set; }
    public float Width { get; set; }
    public float Height { get; set; }

    public TextureStruct(Bitmap image, float width, float height )
    {
        Image = image;
        Width = width;
        Height = height;
    }
    public TextureStruct(Bitmap image) 
        : this(image, image.Width, image.Height)
    {
    }
}