//BSD, 2014-present, WinterDev
//MatterHackers

using System;
using PixelFarm.Drawing;
using PixelFarm.CpuBlit.Imaging;
using PixelFarm.CpuBlit.VertexProcessing;
using PixelFarm.CpuBlit.Rasterization;
using PixelFarm.CpuBlit.FragmentProcessing;
using Mini;
namespace PixelFarm.CpuBlit.Sample_AADemoTest3
{
    class CustomScanlineRasToBmp_EnlargedSubPixelRendering : CustomDestBitmapRasterizer
    {
        //old idea not corrrect

        double _size;
        Square _square;
        ScanlineUnpacked8 _sl = new ScanlineUnpacked8();
        AggRenderSurface _gfx;
        AggLcdDistributionLookupTable _lcdLut;
        double _primary = 1;
        public CustomScanlineRasToBmp_EnlargedSubPixelRendering(double size, MemBitmap destImage)
        {
            this.ScanlineRenderMode = ScanlineRenderMode.Custom;
            _size = size;
            _square = new Square(size);
            _gfx = new AggRenderSurface(destImage);
            _lcdLut = new Sample_AADemoTest3.AggLcdDistributionLookupTable(_primary, 2.0 / 9, 1.0 / 9);
        }
        static float mix(float farColor, float nearColor, float weight)
        {
            //from ...
            //opengl es2 mix function              
            return farColor * (1f - weight) + (nearColor * weight);
        }

        const float _cover_1_3 = 255f / 3f;
        const float _cover_2_3 = _cover_1_3 * 2f;
        protected override void CustomRenderSingleScanLine(PixelProcessing.IBitmapBlender destImage, Scanline scanline, Color color)
        {
            SubPixRender(destImage, scanline, color);
        }

        void SubPixRender(PixelProcessing.IBitmapBlender destImage, Scanline scanline, Color color)
        {
            int y = scanline.Y;
            int num_spans = scanline.SpanCount;
            byte[] covers = scanline.GetCovers();
            ScanlineRasterizer ras = _gfx.ScanlineRasterizer;
            var rasToBmp = _gfx.BitmapRasterizer;
            //------------------------------------------
            Color bgColor = Color.White;
            float cb_R = bgColor.R / 255f;
            float cb_G = bgColor.G / 255f;
            float cb_B = bgColor.B / 255f;
            float cf_R = color.R / 255f;
            float cf_G = color.G / 255f;
            float cf_B = color.B / 255f;
            //------------------------------------------

            for (int i = 0; i <= num_spans; ++i)
            {
                //render span by span 
                ScanlineSpan span = scanline.GetSpan(i);
                int x = span.x;
                int num_pix = span.len;
                int coverIndex = span.cover_index;
                //test subpixel rendering concept 
                //----------------------------------------------------

                int prev_cover = 0;
                while (num_pix > 0)
                {
                    byte coverageValue = covers[coverIndex++];
                    if (coverageValue >= 255)
                    {
                        //100% cover 
                        int a = (coverageValue * color.Alpha0To255) >> 8;
                        _square.Draw(rasToBmp,
                               ras, _sl, destImage,
                               Color.FromArgb(a, Color.FromArgb(color.red, color.green, color.blue)),
                               x, y);
                        prev_cover = 255;//full
                    }
                    else
                    {
                        //check direction : 
                        bool isUpHill = (coverageValue % 2) > 0;
                        //--------------------------------------------------------------------
                        //TODO: review here
                        //in somecase, demo3, isUpHill2 != isUpHill
                        //but we skip it, because we don't want context color around the point
                        //so when we use in fragment shader we can pick up a single color
                        //and determine what color it should be    
                        bool isUpHill2 = coverageValue > prev_cover;
                        if (isUpHill != isUpHill2)
                        {
                        }
                        //--------------------------------------------------------------------
                        prev_cover = coverageValue;
                        byte c_r, c_g, c_b;
                        float subpix_percent = ((float)(coverageValue) / 256f);
                        if (coverageValue < _cover_1_3)
                        {
                            //assume LCD color arrangement is RGB        
                            if (isUpHill)
                            {
                                c_r = bgColor.R;
                                c_g = bgColor.G;
                                c_b = (byte)(mix(cb_B, cf_B, subpix_percent) * 255);
                            }
                            else
                            {
                                c_r = (byte)(mix(cb_R, cf_R, subpix_percent) * 255);
                                c_g = bgColor.G;
                                c_b = bgColor.B;
                            }


                            int a = (coverageValue * color.Alpha0To255) >> 8;
                            _square.Draw(rasToBmp,
                                   ras, _sl, destImage,
                                    Color.FromArgb(a, Color.FromArgb(c_r, c_g, c_b)),
                                   x, y);
                        }
                        else if (coverageValue < _cover_2_3)
                        {
                            if (isUpHill)
                            {
                                c_r = bgColor.R;
                                c_g = (byte)(mix(cb_G, cf_G, subpix_percent) * 255);
                                c_b = (byte)(mix(cb_B, cf_B, 1) * 255);
                            }
                            else
                            {
                                c_r = (byte)(mix(cb_R, cf_R, 1) * 255);
                                c_g = (byte)(mix(cb_G, cf_G, subpix_percent) * 255);
                                c_b = bgColor.B;
                            }



                            int a = (coverageValue * color.Alpha0To255) >> 8;
                            _square.Draw(rasToBmp,
                                   ras, _sl, destImage,
                                   Color.FromArgb(a, Color.FromArgb(c_r, c_g, c_b)),
                                   x, y);
                        }
                        else
                        {
                            //cover > 2/3 but not full 
                            if (isUpHill)
                            {
                                c_r = (byte)(mix(cb_R, cf_R, subpix_percent) * 255);
                                c_g = (byte)(mix(cb_G, cf_G, 1) * 255);
                                c_b = (byte)(mix(cb_B, cf_B, 1) * 255);
                            }
                            else
                            {
                                c_r = (byte)(mix(cb_R, cf_R, 1) * 255);
                                c_g = (byte)(mix(cb_G, cf_G, 1) * 255);
                                c_b = (byte)(mix(cb_B, cf_B, subpix_percent) * 255);
                            }


                            int a = (coverageValue * color.Alpha0To255) >> 8;
                            _square.Draw(rasToBmp,
                                   ras, _sl, destImage,
                                   Color.FromArgb(a, Color.FromArgb(c_r, c_g, c_b)),
                                   x, y);
                        }
                    }


                    ++x;
                    --num_pix;
                }
            }
        }
    }

    class AggLcdDistributionLookupTable
    {
        //from agg-2.4,lcd_distribution_lut
        public readonly byte[] _primary = new byte[256];
        public readonly byte[] _secondary = new byte[256];
        public readonly byte[] _tertiary = new byte[256];
        public AggLcdDistributionLookupTable(double prim, double second, double tert)
        {
            double norm = 1.0 / (prim + second * 2 + tert * 2);
            prim *= norm;
            second *= norm;
            tert *= norm;
            for (int i = 0; i < 256; ++i)
            {
                _primary[i] = (byte)Math.Floor(prim * i);
                _secondary[i] = (byte)Math.Floor(second * i);
                _tertiary[i] = (byte)Math.Floor(tert * i);
            }
        }
    }

    [Info(OrderCode = "02")]
    [Info("Demonstration of the Anti-Aliasing principle with Subpixel Accuracy. The triangle "
                    + "is rendered two times, with its “natural” size (at the bottom-left) and enlarged. "
                    + "To draw the enlarged version there is a special scanline renderer written (see "
                    + "class renderer_enlarged in the source code). You can drag the whole triangle as well "
                    + "as each vertex of it. Also change “Gamma” to see how it affects the quality of Anti-Aliasing.")]
    public class aa_demo_test3 : DemoBase
    {
        double[] _x = new double[3];
        double[] _y = new double[3];
        double _dx;
        double _dy;
        int _idx;
        Stroke _stroke = new Stroke(2);
        public aa_demo_test3()
        {
            _idx = -1;
            _x[0] = 57; _y[0] = 100;
            _x[1] = 369; _y[1] = 170;
            _x[2] = 80; _y[2] = 310;
            //init value
            this.PixelSize = 32;
            this.GammaValue = 1;
        }

        [DemoConfig(MinValue = 8, MaxValue = 100)]
        public double PixelSize
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
            //specific for agg

            if (p is PixelFarm.CpuBlit.AggPainter)
            {


                var p2 = (PixelFarm.CpuBlit.AggPainter)p;
                AggRenderSurface aggsx = p2.RenderSurface;
                ScanlineRasterizer rasterizer = aggsx.ScanlineRasterizer;


                var widgetsSubImage = PixelProcessing.BitmapBlenderExtension.CreateSubBitmapBlender(aggsx.DestBitmapBlender, aggsx.GetClippingRect());
                aggsx.UseSubPixelLcdEffect = false;
                PixelProcessing.PixelBlenderBGRA normalBlender = new PixelProcessing.PixelBlenderBGRA();
                PixelProcessing.PixelBlenderBGRA gammaBlender = new PixelProcessing.PixelBlenderBGRA(); //TODO: revisit, and fix this again
                gammaBlender.GammaValue = this.GammaValue;
                gammaBlender.EnableGamma = true;
                var rasterGamma = new PixelProcessing.SubBitmapBlender(widgetsSubImage, gammaBlender);
                ClipProxyImage clippingProxyNormal = new ClipProxyImage(widgetsSubImage);
                ClipProxyImage clippingProxyGamma = new ClipProxyImage(rasterGamma);
                clippingProxyNormal.Clear(Color.White);

                ScanlineUnpacked8 sl = new ScanlineUnpacked8();
                int size_mul = (int)this.PixelSize;
                CustomScanlineRasToBmp_EnlargedSubPixelRendering ren_en = new CustomScanlineRasToBmp_EnlargedSubPixelRendering(size_mul, aggsx.DestBitmap);
                rasterizer.Reset();
                rasterizer.MoveTo(_x[0] / size_mul, _y[0] / size_mul);
                rasterizer.LineTo(_x[1] / size_mul, _y[1] / size_mul);
                rasterizer.LineTo(_x[2] / size_mul, _y[2] / size_mul);
                ren_en.RenderWithColor(clippingProxyGamma, rasterizer, sl, Color.Black);
                //----------------------------------------
                DestBitmapRasterizer sclineRasToBmp = aggsx.BitmapRasterizer;
                aggsx.UseSubPixelLcdEffect = false;
                sclineRasToBmp.RenderWithColor(clippingProxyGamma, rasterizer, sl, Color.Black);
                rasterizer.ResetGamma(new GammaNone());
                aggsx.UseSubPixelLcdEffect = false;
                //----------------------------------------

                using (VxsTemp.Borrow(out var v1, out var v2))
                using (VectorToolBox.Borrow(v1, out PathWriter ps))
                {
                    ps.Clear();
                    ps.MoveTo(_x[0], _y[0]);
                    ps.LineTo(_x[1], _y[1]);
                    ps.LineTo(_x[2], _y[2]);
                    ps.LineTo(_x[0], _y[0]);

                    rasterizer.AddPath(_stroke.MakeVxs(v1, v2));
                }


                //----------------------------------------
                //Stroke stroke = new Stroke(ps);
                //stroke.Width = 2;
                //rasterizer.AddPath(stroke.MakeVxs(ps.MakeVxs())); 
 
                //----------------------------------------

                sclineRasToBmp.RenderWithColor(clippingProxyNormal, rasterizer, sl, new Color(200, 0, 150, 160));
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
