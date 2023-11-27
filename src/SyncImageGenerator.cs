namespace MandelbrotGenerator
{
    public class SyncImageGenerator : IImageGenerator
    {
        public Image<Rgba32> GenerateImage(Area area)
        {
            Image<Rgba32> bitmap = new(area.Width, area.Height);

            bitmap.ProcessPixelRows(accessor => {
                for (var y = 0; y < accessor.Height; ++y)
                {
                    Span<Rgba32> pixelRow = accessor.GetRowSpan(y);

                    for (var x = 0; x < pixelRow.Length; ++x) {
                        // implement drawing code for each pixel in the Mandelbrot set
                    }
                }
            });

            //end insert
            return bitmap;
        }
    }
}