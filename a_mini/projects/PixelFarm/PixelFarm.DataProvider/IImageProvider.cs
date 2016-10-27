//MIT,  2016, WinterDev 
using System;
using System.Collections.Generic;

namespace PixelFarm.Drawing
{
    public interface IImageProvider
    {
        byte[] LoadImageBufferFromFile(string filename);
    }
}