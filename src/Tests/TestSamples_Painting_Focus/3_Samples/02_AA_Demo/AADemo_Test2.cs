//BSD, 2014-present, WinterDev
//MatterHackers

using System;
using PixelFarm.Drawing;
using PixelFarm.CpuBlit.Imaging;
using PixelFarm.CpuBlit.VertexProcessing;
using PixelFarm.CpuBlit.PixelProcessing;
using PixelFarm.CpuBlit.Rasterization;
using PixelFarm.CpuBlit.FragmentProcessing;

using Mini;
namespace PixelFarm.CpuBlit.Sample_AADemoTest2
{
    class CustomRas_EnlargeV2 : CustomDestBitmapRasterizer
    {
        double _size;
        Square _square;
        ScanlineUnpacked8 _sl = new ScanlineUnpacked8();
        AggRenderSurface _gfx;
        public CustomRas_EnlargeV2(double size, MemBitmap dstBmp)
        {
            this.ScanlineRenderMode = ScanlineRenderMode.Custom;
            _size = size;
            _square = new Square(size);
            _gfx = new AggRenderSurface();
            _gfx.AttachDstBitmap(dstBmp);
        }
        protected override void CustomRenderSingleScanLine(IBitmapBlender destImage, Scanline scanline, Color color)
        {
            int y = scanline.Y;
            int num_spans = scanline.SpanCount;
            byte[] covers = scanline.GetCovers();
            int spanCount = scanline.SpanCount;
            var ras = _gfx.ScanlineRasterizer;
            var rasToBmp = _gfx.BitmapRasterizer;
            for (int i = 1; i <= num_spans; ++i)
            {
                var span2 = scanline.GetSpan(i);
                int x = span2.x;
                int num_pix = span2.len;
                int coverIndex = span2.cover_index;
                do
                {
                    int a = (covers[coverIndex++] * color.A) >> 8;
                    _square.Draw(rasToBmp,
                             ras, _sl, destImage,
                             Color.FromArgb(a, color),
                            x, y);
                    ++x;
                }
                while (--num_pix > 0);
            }
        }
    }

    //---------------------------
    [Info(OrderCode = "02", AvailableOn = AvailableOn.Agg)]
    [Info("Demonstration of the Anti-Aliasing principle with Subpixel Accuracy. The triangle "
                + "is rendered two times, with its “natural” size (at the bottom-left) and enlarged. "
                + "To draw the enlarged version there is a special scanline renderer written (see "
                + "class renderer_enlarged in the source code). You can drag the whole triangle as well "
                + "as each vertex of it. Also change “Gamma” to see how it affects the quality of Anti-Aliasing.")]
    public class aa_demo_test2 : DemoBase
    {
        double[] _x = new double[3];
        double[] _y = new double[3];
        double _dx;
        double _dy;
        int _idx;
        public aa_demo_test2()
        {
            _idx = -1;
            _x[0] = 57; _y[0] = 100;
            _x[1] = 369; _y[1] = 170;
            _x[2] = 143; _y[2] = 310;
            //init value
            this.PixelSize = 32;
            this.GammaValue = 1;
        }

        [DemoConfig(MinValue = 8, MaxValue = 100)]
        public int PixelSize
        {
            get;
            set;
        }
        [DemoConfig(MinValue = 0, MaxValue = 3)]
        public float GammaValue
        {
            get;
            set;
        }

        public override void Draw(Painter p)
        {
            //this specific for agg
            if (p is AggPainter)
            {
                AggPainter p2 = (AggPainter)p;
                AggRenderSurface aggsx = p2.RenderSurface;
                PixelProcessing.SubBitmapBlender subImg = PixelProcessing.BitmapBlenderExtension.CreateSubBitmapBlender(aggsx.DestBitmapBlender, aggsx.GetClippingRect());

                //TODO: review here again
                PixelBlenderBGRA blenderWithGamma = new PixelProcessing.PixelBlenderBGRA();

                SubBitmapBlender rasterGamma = new SubBitmapBlender(subImg, blenderWithGamma);
                ClipProxyImage clippingProxyNormal = new ClipProxyImage(subImg);
                ClipProxyImage clippingProxyGamma = new ClipProxyImage(rasterGamma);
                clippingProxyNormal.Clear(Color.White);
                ScanlineRasterizer rasterizer = aggsx.ScanlineRasterizer;
                var sl = new ScanlineUnpacked8();
                int size_mul = this.PixelSize;
                var sclineToBmpEn2 = new CustomRas_EnlargeV2(size_mul, aggsx.DestBitmap);
                rasterizer.Reset();
                rasterizer.MoveTo(_x[0] / size_mul, _y[0] / size_mul);
                rasterizer.LineTo(_x[1] / size_mul, _y[1] / size_mul);
                rasterizer.LineTo(_x[2] / size_mul, _y[2] / size_mul);
                sclineToBmpEn2.RenderWithColor(clippingProxyGamma, rasterizer, sl, Color.Black);
                DestBitmapRasterizer bmpRas = aggsx.BitmapRasterizer;
                bmpRas.RenderWithColor(clippingProxyGamma, rasterizer, sl, Color.Black);
                //-----------------------------------------------------------------------------------------------------------
                rasterizer.ResetGamma(new GammaNone());

                using (Tools.BorrowVxs(out var v1, out var v2))
                using (Tools.BorrowPathWriter(v1, out PathWriter ps))
                {
                    ps.Clear();
                    ps.MoveTo(_x[0], _y[0]);
                    ps.LineTo(_x[1], _y[1]);
                    ps.LineTo(_x[2], _y[2]);
                    ps.LineTo(_x[0], _y[0]);
                    rasterizer.AddPath((new Stroke(2)).MakeVxs(v1, v2));
                    bmpRas.RenderWithColor(clippingProxyNormal, rasterizer, sl, new Color(200, 0, 150, 160));
                }
            }
        }

        public override void MouseDown(int mx, int my, bool isRightButton)
        {
            double x = mx;
            double y = my;
            int i;
            for (i = 0; i < 3; i++)
            {
                if (Math.Sqrt((x - _x[i]) * (x - _x[i]) + (y - _y[i]) * (y - _y[i])) < 5.0)
                {
                    _dx = x - _x[i];
                    _dy = y - _y[i];
                    _idx = i;
                    break;
                }
            }
            if (i == 3)
            {
                if (AggMath.point_in_triangle(_x[0], _y[0],
                                      _x[1], _y[1],
                                      _x[2], _y[2],
                                      x, y))
                {
                    _dx = x - _x[0];
                    _dy = y - _y[0];
                    _idx = 3;
                }
            }
        }
        public override void MouseDrag(int mx, int my)
        {
            double x = mx;
            double y = my;
            if (_idx == 3)
            {
                double dx = x - _dx;
                double dy = y - _dy;
                _x[1] -= _x[0] - dx;
                _y[1] -= _y[0] - dy;
                _x[2] -= _x[0] - dx;
                _y[2] -= _y[0] - dy;
                _x[0] = dx;
                _y[0] = dy;
                return;
            }

            if (_idx >= 0)
            {
                _x[_idx] = x - _dx;
                _y[_idx] = y - _dy;
            }
        }

        public override void MouseUp(int x, int y)
        {
            _idx = -1;
            base.MouseUp(x, y);
        }
    }
}
