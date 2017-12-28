//BSD, 2014-2017, WinterDev
//MatterHackers

using System;
using PixelFarm.Drawing;
using PixelFarm.Agg.Imaging;
using PixelFarm.Agg.VertexSource;
using Mini;
namespace PixelFarm.Agg.Sample_AADemoTest3
{
    class CustomScanlineRasToBmp_EnlargedSubPixelRendering : CustomScanlineRasToDestBitmapRenderer
    {
        //old idea not corrrect

        double m_size;
        Square m_square;
        ScanlineUnpacked8 m_sl = new ScanlineUnpacked8();
        AggRenderSurface gfx;
        AggLcdDistributionLookupTable lcdLut;
        double primary = 1;
        public CustomScanlineRasToBmp_EnlargedSubPixelRendering(double size, ActualImage destImage)
        {
            this.ScanlineRenderMode = Agg.ScanlineRenderMode.Custom;
            m_size = size;
            m_square = new Square(size);
            gfx = new AggRenderSurface(destImage);
            lcdLut = new Sample_AADemoTest3.AggLcdDistributionLookupTable(primary, 2.0 / 9, 1.0 / 9);
        }
        static float mix(float farColor, float nearColor, float weight)
        {
            //from ...
            //opengl es2 mix function              
            return farColor * (1f - weight) + (nearColor * weight);
        }

        const float cover_1_3 = 255f / 3f;
        const float cover_2_3 = cover_1_3 * 2f;
        protected override void CustomRenderSingleScanLine(IImageReaderWriter destImage, Scanline scanline, Color color)
        {
            SubPixRender(destImage, scanline, color);
        }

        void SubPixRender(IImageReaderWriter destImage, Scanline scanline, Color color)
        {
            int y = scanline.Y;
            int num_spans = scanline.SpanCount;
            byte[] covers = scanline.GetCovers();
            ScanlineRasterizer ras = gfx.ScanlineRasterizer;
            var rasToBmp = gfx.ScanlineRasToDestBitmap;
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
                        m_square.Draw(rasToBmp,
                               ras, m_sl, destImage,
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
                        if (coverageValue < cover_1_3)
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
                            m_square.Draw(rasToBmp,
                                   ras, m_sl, destImage,
                                    Color.FromArgb(a, Color.FromArgb(c_r, c_g, c_b)),
                                   x, y);
                        }
                        else if (coverageValue < cover_2_3)
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
                            m_square.Draw(rasToBmp,
                                   ras, m_sl, destImage,
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
                            m_square.Draw(rasToBmp,
                                   ras, m_sl, destImage,
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
        public readonly byte[] m_primary = new byte[256];
        public readonly byte[] m_secondary = new byte[256];
        public readonly byte[] m_tertiary = new byte[256];
        public AggLcdDistributionLookupTable(double prim, double second, double tert)
        {
            double norm = 1.0 / (prim + second * 2 + tert * 2);
            prim *= norm;
            second *= norm;
            tert *= norm;
            for (int i = 0; i < 256; ++i)
            {
                m_primary[i] = (byte)Math.Floor(prim * i);
                m_secondary[i] = (byte)Math.Floor(second * i);
                m_tertiary[i] = (byte)Math.Floor(tert * i);
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
        double[] m_x = new double[3];
        double[] m_y = new double[3];
        double m_dx;
        double m_dy;
        int m_idx;
        Stroke stroke = new Stroke(2);
        public aa_demo_test3()
        {
            m_idx = -1;
            m_x[0] = 57; m_y[0] = 100;
            m_x[1] = 369; m_y[1] = 170;
            m_x[2] = 80; m_y[2] = 310;
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

            if (p is PixelFarm.Agg.AggPainter)
            {


                var p2 = (PixelFarm.Agg.AggPainter)p;
                AggRenderSurface aggsx = p2.RenderSurface;
                ScanlineRasterizer rasterizer = aggsx.ScanlineRasterizer;


                var widgetsSubImage = ImageHelper.CreateSubImgRW(aggsx.DestImage, aggsx.GetClippingRect());
                aggsx.UseSubPixelRendering = false;
                PixelBlenderBGRA normalBlender = new PixelBlenderBGRA();
                PixelBlenderBGRA gammaBlender = new PixelBlenderBGRA(); //TODO: revisit, and fix this again
                gammaBlender.GammaValue = this.GammaValue;
                gammaBlender.EnableGamma = true;
                var rasterGamma = new SubImageRW(widgetsSubImage, gammaBlender);
                ClipProxyImage clippingProxyNormal = new ClipProxyImage(widgetsSubImage);
                ClipProxyImage clippingProxyGamma = new ClipProxyImage(rasterGamma);
                clippingProxyNormal.Clear(Color.White);

                ScanlineUnpacked8 sl = new ScanlineUnpacked8();
                int size_mul = (int)this.PixelSize;
                CustomScanlineRasToBmp_EnlargedSubPixelRendering ren_en = new CustomScanlineRasToBmp_EnlargedSubPixelRendering(size_mul, aggsx.DestActualImage);
                rasterizer.Reset();
                rasterizer.MoveTo(m_x[0] / size_mul, m_y[0] / size_mul);
                rasterizer.LineTo(m_x[1] / size_mul, m_y[1] / size_mul);
                rasterizer.LineTo(m_x[2] / size_mul, m_y[2] / size_mul);
                ren_en.RenderWithColor(clippingProxyGamma, rasterizer, sl, Color.Black);
                //----------------------------------------
                ScanlineRasToDestBitmapRenderer sclineRasToBmp = aggsx.ScanlineRasToDestBitmap;
                aggsx.UseSubPixelRendering = false;
                sclineRasToBmp.RenderWithColor(clippingProxyGamma, rasterizer, sl, Color.Black);
                rasterizer.ResetGamma(new GammaNone());
                aggsx.UseSubPixelRendering = false;
                //----------------------------------------
                PathWriter ps = new PathWriter();
                ps.Clear();
                ps.MoveTo(m_x[0], m_y[0]);
                ps.LineTo(m_x[1], m_y[1]);
                ps.LineTo(m_x[2], m_y[2]);
                ps.LineTo(m_x[0], m_y[0]);
                //----------------------------------------
                //Stroke stroke = new Stroke(ps);
                //stroke.Width = 2;
                //rasterizer.AddPath(stroke.MakeVxs(ps.MakeVxs()));
                var v1 = GetFreeVxs();
                rasterizer.AddPath(stroke.MakeVxs(ps.Vxs, v1));
                ReleaseVxs(ref v1);
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
                if (Math.Sqrt((x - m_x[i]) * (x - m_x[i]) + (y - m_y[i]) * (y - m_y[i])) < 5.0)
                {
                    m_dx = x - m_x[i];
                    m_dy = y - m_y[i];
                    m_idx = i;
                    break;
                }
            }
            if (i == 3)
            {
                if (AggMath.point_in_triangle(m_x[0], m_y[0],
                                      m_x[1], m_y[1],
                                      m_x[2], m_y[2],
                                      x, y))
                {
                    m_dx = x - m_x[0];
                    m_dy = y - m_y[0];
                    m_idx = 3;
                }
            }
        }
        public override void MouseDrag(int mx, int my)
        {
            double x = mx;
            double y = my;
            if (m_idx == 3)
            {
                double dx = x - m_dx;
                double dy = y - m_dy;
                m_x[1] -= m_x[0] - dx;
                m_y[1] -= m_y[0] - dy;
                m_x[2] -= m_x[0] - dx;
                m_y[2] -= m_y[0] - dy;
                m_x[0] = dx;
                m_y[0] = dy;
                return;
            }

            if (m_idx >= 0)
            {
                m_x[m_idx] = x - m_dx;
                m_y[m_idx] = y - m_dy;
            }
        }

        public override void MouseUp(int x, int y)
        {
            m_idx = -1;
            base.MouseUp(x, y);
        }
    }
}
