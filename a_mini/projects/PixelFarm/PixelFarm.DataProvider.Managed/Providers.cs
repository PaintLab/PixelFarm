//MIT, 2016-2017, WinterDev
 

namespace PixelFarm.Drawing
{
    public interface IImageProvider
    {
        byte[] LoadImageBufferFromFile(string filename);
    }
} 