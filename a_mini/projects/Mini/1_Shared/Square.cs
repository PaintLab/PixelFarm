//BSD, 2014-2016, WinterDev
//MattersHackers
//AGG 2.4


using System;
using PixelFarm.Drawing;
using System.Globalization;
using PixelFarm.Agg.Transform;
using PixelFarm.Agg.Image;
using PixelFarm.Agg.VertexSource;
using PixelFarm.Agg;
using PixelFarm.VectorMath;
namespace PixelFarm.Agg
{
    public class Square
    {
        double m_size;
        public Square(double size)
        {
            m_size = size;
        }

        public void Draw(
            ScanlineRasToDestBitmapRenderer sclineRasToBmp,
            ScanlineRasterizer ras,
            Scanline sl,
            IImageReaderWriter destImage, Color color,
            double x, double y)
        {
            ras.Reset();
            ras.MoveTo(x * m_size, y * m_size);
            ras.LineTo(x * m_size + m_size, y * m_size);
            ras.LineTo(x * m_size + m_size, y * m_size + m_size);
            ras.LineTo(x * m_size, y * m_size + m_size);
            sclineRasToBmp.RenderWithColor(destImage, ras, sl, color);
        }
    }
}