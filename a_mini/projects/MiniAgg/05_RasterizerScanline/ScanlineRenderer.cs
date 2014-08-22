using System;
using System.Collections.Generic;
using System.Text;

using MatterHackers.Agg.Image;
using MatterHackers.Agg.VertexSource;

namespace MatterHackers.Agg
{
    /// <summary>
    /// to bitmap
    /// </summary>

    public class ScanlineRenderer
    {
        VectorPOD<RGBA_Bytes> tempSpanColors = new VectorPOD<RGBA_Bytes>();
        public void render_scanlines_aa_solid(IImage destImage, IRasterizer rasterizer, IScanline scanLine, RGBA_Bytes color)
        {
            if (rasterizer.rewind_scanlines())
            {
                scanLine.reset(rasterizer.min_x(), rasterizer.max_x());
                while (rasterizer.sweep_scanline(scanLine))
                {
                    RenderSolidSingleScanLine(destImage, scanLine, color);
                }
            }
        }

        protected virtual void RenderSolidSingleScanLine(IImage destImage, IScanline scanLine, RGBA_Bytes color)
        {
            int y = scanLine.y();
            int num_spans = scanLine.num_spans();
            ScanlineSpan scanlineSpan = scanLine.begin();

            byte[] coversArray = scanLine.GetCovers();
            for (; ; )
            {
                int x = scanlineSpan.x;
                if (scanlineSpan.len > 0)
                {
                    destImage.blend_solid_hspan(x, y, scanlineSpan.len, color, coversArray, scanlineSpan.cover_index);
                }
                else
                {
                    int x2 = (x - (int)scanlineSpan.len - 1);
                    destImage.blend_hline(x, y, x2, color, coversArray[scanlineSpan.cover_index]);
                }
                if (--num_spans == 0)
                {   
                    break;
                }
                scanlineSpan = scanLine.GetNextScanlineSpan();
            }
        }


        public void RenderSolidAllPaths(IImage destImage,
            IRasterizer ras,
            IScanline sl,
            IVertexSource vs,
            RGBA_Bytes[] color_storage,
            int[] path_id,
            int num_paths)
        {
            for (int i = 0; i < num_paths; i++)
            {
                ras.reset();

                ras.add_path(vs, path_id[i]);

                render_scanlines_aa_solid(destImage, ras, sl, color_storage[i]);
            }
        }

        void GenerateAndRenderSingleScanline(IScanline scanLineCache, IImage destImage, ISpanGenerator span_gen)
        {
            int y = scanLineCache.y();
            int num_spans = scanLineCache.num_spans();
            ScanlineSpan scanlineSpan = scanLineCache.begin();

            byte[] ManagedCoversArray = scanLineCache.GetCovers();
            for (; ; )
            {
                int x = scanlineSpan.x;
                int len = scanlineSpan.len;
                if (len < 0) len = -len;

                if (tempSpanColors.Capacity() < len)
                {
                    tempSpanColors.Capacity(len);
                }

                span_gen.generate(tempSpanColors.Array, 0, x, y, len);
                bool useFirstCoverForAll = scanlineSpan.len < 0;
                destImage.blend_color_hspan(x, y, len, tempSpanColors.Array, 0, ManagedCoversArray, scanlineSpan.cover_index, useFirstCoverForAll);

                if (--num_spans == 0) break;
                scanlineSpan = scanLineCache.GetNextScanlineSpan();
            }
        }


        public void GenerateAndRender(IRasterizer rasterizer,
             IScanline scanlineCache, IImage destImage,
             ISpanGenerator spanGenerator)
        {
            if (rasterizer.rewind_scanlines())
            {
                scanlineCache.reset(rasterizer.min_x(), rasterizer.max_x());
                spanGenerator.prepare();
                while (rasterizer.sweep_scanline(scanlineCache))
                {
                    GenerateAndRenderSingleScanline(scanlineCache, destImage, spanGenerator);
                }
            }
        }




    }
}
