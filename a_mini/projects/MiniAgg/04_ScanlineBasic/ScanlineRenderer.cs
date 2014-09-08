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
        ArrayList<ColorRGBA> tempSpanColors = new ArrayList<ColorRGBA>();

        public void RenderScanlineSolidAA(IImage destImage,
            ScanlineRasterizer rasterizer,
            IScanline scline, ColorRGBA color)
        {
            if (rasterizer.RewindScanlines())
            {
                scline.ResetSpans(rasterizer.MinX, rasterizer.MaxX);
                while (rasterizer.SweepScanline(scline))
                {
                    RenderSolidSingleScanLine(destImage, scline, color);
                }
            }
        }

        protected virtual void RenderSolidSingleScanLine(IImage destImage, IScanline scline, ColorRGBA color)
        {
            int y = scline.Y;
            int num_spans = scline.SpanCount;
            byte[] covers = scline.GetCovers();
            for (int i = 1; i <= num_spans; ++i)
            {
                ScanlineSpan span = scline.GetSpan(i);
                int x = span.x;
                if (span.len > 0)
                {
                    destImage.blend_solid_hspan(x, y, span.len, color, covers, span.cover_index);
                }
                else
                {
                    int x2 = (x - (int)span.len - 1);
                    destImage.blend_hline(x, y, x2, color, covers[span.cover_index]);
                }

            }
        }


        public void RenderSolidAllPaths(IImage destImage,
            ScanlineRasterizer ras,
            IScanline sl,
            VertexStorage vx,
            ColorRGBA[] color_storage,
            int[] path_id,
            int num_paths)
        {
            for (int i = 0; i < num_paths; i++)
            {
                ras.Reset();
                ras.AddPath(new VertexSnap(vx, path_id[i]));
                RenderScanlineSolidAA(destImage, ras, sl, color_storage[i]);
            }
        }

        void GenerateAndRenderSingleScanline(IScanline scline, IImage destImage, ISpanGenerator span_gen)
        {
            int y = scline.Y;
            int num_spans = scline.SpanCount;
            byte[] covers = scline.GetCovers();
            for (int i = 1; i <= num_spans; ++i)
            {
                ScanlineSpan span = scline.GetSpan(i);
                int x = span.x;
                int len = span.len;
                if (len < 0) len = -len;

                if (tempSpanColors.AllocatedSize < len)
                {
                    tempSpanColors.Clear(len);
                }

                span_gen.Generate(tempSpanColors.Array, 0, x, y, len);

                bool useFirstCoverForAll = span.len < 0;

                destImage.blend_color_hspan(x, y, len,
                    tempSpanColors.Array, 0,
                    covers, span.cover_index, useFirstCoverForAll);
            }
        }


        public void GenerateAndRender(ScanlineRasterizer rasterizer,
             IScanline scline, IImage destImage,
             ISpanGenerator spanGenerator)
        {
            if (rasterizer.RewindScanlines())
            {
                scline.ResetSpans(rasterizer.MinX, rasterizer.MaxX);
                spanGenerator.Prepare();
                while (rasterizer.SweepScanline(scline))
                {
                    GenerateAndRenderSingleScanline(scline, destImage, spanGenerator);
                }
            }
        }




    }
}
