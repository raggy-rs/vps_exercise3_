namespace MandelbrotGenerator
{
    public interface IImageGenerator
    {
        Image<Rgba32> GenerateImage(Area area);
    }
}