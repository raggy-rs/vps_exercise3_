using SixLabors.ImageSharp;

namespace MandelbrotGenerator
{
    public static class ColorScheme
    {
        public static Rgba32 GetColor(int iterations)
        {
            if (iterations == Settings.DefaultSettings.MaxIterations)
            {
                return Color.Black;
            }
            else
            {
                int red = (iterations % 32) * 3;
                if (red > 255)
                    red = 255;

                int green = (iterations % 16) * 2;
                if (green > 255)
                    green = 255;

                int blue = (iterations % 128) * 14;
                if (blue > 255)
                    blue = 255;

                return new Rgba32((byte)red, (byte)green, (byte)blue);
            }
        }
    }
}