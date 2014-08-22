namespace MatterHackers.Agg
{
    public interface ISpanGenerator
    {
        void prepare();
        void generate(ColorRGBA[] span, int spanIndex, int x, int y, int len);
    }
    public interface IPatternFilter
    {
        int dilation();
        void pixel_high_res(ImageBase sourceImage, ColorRGBA[] destBuffer, int destBufferOffset, int x, int y);
    }
}