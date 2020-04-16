//MIT, 2020, WinterDev
using System;
using System.Collections.Generic;
using PixelFarm.CpuBlit;
using PaintLab.ChromaJs;
using PaintLab.Colourful;
using PaintLab.Colourful.Conversion;

namespace PixelFarm.Drawing
{
    public static class ColorToolExtensions
    {
        public static PixelFarm.TempContext<Chroma> BorrowChromaTool(
            this PixelFarm.CpuBlit.Tools tools,
            out Chroma chroma)
        {
            return BorrowChromaTool(out chroma);
        }
        internal static PixelFarm.TempContext<Chroma> BorrowChromaTool(
           out Chroma chroma)
        {
            if (!PixelFarm.Temp<Chroma>.IsInit())
            {
                PixelFarm.Temp<Chroma>.SetNewHandler(
                    () => new Chroma(),
                    p => p.Reset());//when relese back
            }

            return PixelFarm.Temp<Chroma>.Borrow(out chroma);
        }

        public static PixelFarm.TempContext<ColourfulConverter> BorrowColourfulConverter(
          this PixelFarm.CpuBlit.Tools tools,
          out ColourfulConverter conveter)
        {
            return BorrowColourfulConverter(out conveter);
        }

        internal static PixelFarm.TempContext<ColourfulConverter> BorrowColourfulConverter(
            out ColourfulConverter conveter)
        {
            if (!PixelFarm.Temp<ColourfulConverter>.IsInit())
            {
                PixelFarm.Temp<ColourfulConverter>.SetNewHandler(
                    () => new ColourfulConverter(),
                    p => { });//when relese back
            }

            return PixelFarm.Temp<ColourfulConverter>.Borrow(out conveter);
        }
    }
}