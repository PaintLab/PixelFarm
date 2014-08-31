namespace MatterHackers.Agg
{
    public interface ISpanGenerator
    {
        void Prepare();
        void Generate(ColorRGBA[] span, int spanIndex,
            int x, int y, int len);
    }
    public interface IPatternFilter
    {
        int Dilation { get; }
        void SetPixelHighRes(ImageBase sourceImage,
            ColorRGBA[] destBuffer, 
            int destBufferOffset, 
            int x, int y);
    }
}