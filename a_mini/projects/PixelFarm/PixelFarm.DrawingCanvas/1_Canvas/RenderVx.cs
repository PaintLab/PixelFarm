//MIT, 2014-2017, WinterDev


namespace PixelFarm.Drawing
{
    public abstract class RenderVx
    {
    }
    public abstract class RenderVxFormattedString
    {
        public string OriginalString { get; set; }
        public int[] glyphList { get; set; }
    }
}