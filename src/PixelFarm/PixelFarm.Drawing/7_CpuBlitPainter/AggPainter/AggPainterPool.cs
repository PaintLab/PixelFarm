//MIT, 2016-present, WinterDev

using System;
using System.Collections.Generic;
using PixelFarm.Drawing;
using PixelFarm.CpuBlit.VertexProcessing;
using PixelFarm.CpuBlit.PixelProcessing;

using BitmapBufferEx;
namespace PixelFarm.CpuBlit
{
    public static class AggPainterPool
    {
        public static TempContext<AggPainter> Borrow(MemBitmap bmp, out AggPainter painter)
        {

            if (!Temp<AggPainter>.IsInit())
            {
                Temp<AggPainter>.SetNewHandler(
                    () => new AggPainter(new AggRenderSurface()),
                    p =>
                    {
                        p.RenderSurface.DetachDstBitmap();
                    }
                    );
            }

            var tmpPainter = Temp<AggPainter>.Borrow(out painter);
            painter.DestBitmapBlender.Attach(bmp);
            return tmpPainter;
        }
    }

}

