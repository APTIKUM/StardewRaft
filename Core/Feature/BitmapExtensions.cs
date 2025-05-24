using System.Drawing.Imaging;

namespace StardewRaft.Core.Feature;
public static class BitmapExtensions
{
    public static Bitmap RecolorGrayscaleImage(this Bitmap originalImage, Color newColor)
    {
        Bitmap newBitmap = new Bitmap(originalImage.Width, originalImage.Height);

        float r = newColor.R / 255f;
        float g = newColor.G / 255f;
        float b = newColor.B / 255f;

        ColorMatrix colorMatrix = new ColorMatrix(new float[][]
        {
        new float[] {r, 0, 0, 0, 0}, // R
        new float[] {0, g, 0, 0, 0}, // G
        new float[] {0, 0, b, 0, 0}, // B
        new float[] {0, 0, 0, 1, 0}, // A
        new float[] {0, 0, 0, 0, 1}  // Дополнительный коэффициент
        });

        using (Graphics gfx = Graphics.FromImage(newBitmap))
        using (ImageAttributes attributes = new ImageAttributes())
        {
            attributes.SetColorMatrix(colorMatrix);
            gfx.DrawImage(
                originalImage,
                new Rectangle(0, 0, originalImage.Width, originalImage.Height),
                0, 0, originalImage.Width, originalImage.Height,
                GraphicsUnit.Pixel,
                attributes);
        }

        return newBitmap;
    }
}
