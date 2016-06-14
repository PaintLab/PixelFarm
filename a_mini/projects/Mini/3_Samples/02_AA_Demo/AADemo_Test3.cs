//2014 BSD,WinterDev
//MatterHackers

using System;
using PixelFarm.Agg.Image;
using PixelFarm.Agg.VertexSource;
using Mini;
namespace PixelFarm.Agg.Sample_AADemoTest3
{
    class CustomScanlineRasToBmp_EnlargedV3 : CustomScanlineRasToDestBitmapRenderer
    {
        double m_size;
        Square m_square;
        ScanlineUnpacked8 m_sl = new ScanlineUnpacked8();
        Graphics2D gfx;
        public CustomScanlineRasToBmp_EnlargedV3(double size, ActualImage destImage)
        {
            this.ScanlineRenderMode = Agg.ScanlineRenderMode.Custom;
            m_size = size;
            m_square = new Square(size);
            gfx = Graphics2D.CreateFromImage(destImage);
        }
        static float mix(float farColor, float nearColor, float weight)
        {
            //from ...
            //opengl es2 mix function              
            return farColor * (1f - weight) + (nearColor * weight);
        }

        const float cover_1_3 = 255f / 3f;
        const float cover_2_3 = cover_1_3 * 2f;
        protected override void CustomRenderSingleScanLine(IImageReaderWriter destImage, Scanline scanline, ColorRGBA color)
        {
            SubPixRender(destImage, scanline, color);
        }

        void SubPixRender(IImageReaderWriter destImage, Scanline scanline, ColorRGBA color)
        {
            int y = scanline.Y;
            int num_spans = scanline.SpanCount;
            byte[] covers = scanline.GetCovers();
            ScanlineRasterizer ras = gfx.ScanlineRasterizer;
            var rasToBmp = gfx.ScanlineRasToDestBitmap;
            ColorRGBA prevColor = ColorRGBA.White;
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
                    int coverageValue = covers[coverIndex++];
                    if (coverageValue >= 255)
                    {
                        //100% cover
                        ColorRGBA newc = new ColorRGBA(color.red, color.green, color.blue);
                        prevColor = newc;
                        int a = (coverageValue * color.Alpha0To255) >> 8;
                        m_square.Draw(rasToBmp,
                               ras, m_sl, destImage,
                                new ColorRGBA(newc, a),
                               x, y);
                        prev_cover = 255;//full
                    }
                    else
                    {
                        //check direction : 
                        bool isLeftToRight = coverageValue >= prev_cover;
                        prev_cover = coverageValue;
                        byte c_r, c_g, c_b;
                        float subpix_percent = ((float)(coverageValue) / 256f);
                        if (coverageValue < cover_1_3)
                        {
                            if (isLeftToRight)
                            {
                                c_r = 255;
                                c_g = 255;
                                c_b = (byte)(255 - (255f * (subpix_percent)));
                            }
                            else
                            {
                                c_r = (byte)(255 - (255f * (subpix_percent)));
                                c_g = 255;
                                c_b = 255;
                            }

                            ColorRGBA newc = prevColor = new ColorRGBA(c_r, c_g, c_b);
                            int a = (coverageValue * color.Alpha0To255) >> 8;
                            m_square.Draw(rasToBmp,
                                   ras, m_sl, destImage,
                                    new ColorRGBA(newc, a),
                                   x, y);
                        }
                        else if (coverageValue < cover_2_3)
                        {
                            if (isLeftToRight)
                            {
                                c_r = prevColor.blue;
                                c_g = (byte)(255 - (255f * (subpix_percent)));
                                c_b = color.blue;// (byte)(255 - (255f * (subpix_percent)));// color.blue;
                            }
                            else
                            {
                                c_r = color.blue;// (byte)(255 - (255f * (subpix_percent))); //color.blue;
                                c_g = (byte)(255 - (255f * (subpix_percent)));
                                c_b = 255;
                            }


                            ColorRGBA newc = prevColor = new ColorRGBA(c_r, c_g, c_b);
                            int a = (coverageValue * color.Alpha0To255) >> 8;
                            m_square.Draw(rasToBmp,
                                   ras, m_sl, destImage,
                                   new ColorRGBA(newc, a),
                                   x, y);
                        }
                        else
                        {
                            //cover > 2/3 but not full 
                            if (isLeftToRight)
                            {
                                c_r = (byte)(255 - (255f * (subpix_percent)));
                                c_g = color.green;
                                c_b = color.blue;
                            }
                            else
                            {
                                c_r = prevColor.green;
                                c_g = prevColor.blue;
                                c_b = (byte)(255 - (255f * (subpix_percent)));
                            }

                            ColorRGBA newc = prevColor = new ColorRGBA(c_r, c_g, c_b);
                            int a = (coverageValue * color.Alpha0To255) >> 8;
                            m_square.Draw(rasToBmp,
                                   ras, m_sl, destImage,
                                   new ColorRGBA(newc, a),
                                   x, y);
                        }
                    }




                    ++x;
                    --num_pix;
                }
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
        public override void Draw(Graphics2D g)
        {
            OnDraw(g);
        }

        public void OnDraw(Graphics2D graphics2D)
        {
            var widgetsSubImage = ImageHelper.CreateChildImage(graphics2D.DestImage, graphics2D.GetClippingRect());
            graphics2D.UseSubPixelRendering = false;
            IPixelBlender NormalBlender = new PixelBlenderBGRA();
            IPixelBlender GammaBlender = new PixelBlenderGammaBGRA(this.GammaValue);
            var rasterGamma = new ChildImage(widgetsSubImage, GammaBlender);
            ClipProxyImage clippingProxyNormal = new ClipProxyImage(widgetsSubImage);
            ClipProxyImage clippingProxyGamma = new ClipProxyImage(rasterGamma);
            clippingProxyNormal.Clear(ColorRGBA.White);
            var rasterizer = graphics2D.ScanlineRasterizer;
            ScanlineUnpacked8 sl = new ScanlineUnpacked8();
            int size_mul = (int)this.PixelSize;
            CustomScanlineRasToBmp_EnlargedV3 ren_en = new CustomScanlineRasToBmp_EnlargedV3(size_mul, graphics2D.DestActualImage);
            rasterizer.Reset();
            rasterizer.MoveTo(m_x[0] / size_mul, m_y[0] / size_mul);
            rasterizer.LineTo(m_x[1] / size_mul, m_y[1] / size_mul);
            rasterizer.LineTo(m_x[2] / size_mul, m_y[2] / size_mul);
            ren_en.RenderWithColor(clippingProxyGamma, rasterizer, sl, ColorRGBA.Black);
            //----------------------------------------
            ScanlineRasToDestBitmapRenderer sclineRasToBmp = graphics2D.ScanlineRasToDestBitmap;
            graphics2D.UseSubPixelRendering = true;
            sclineRasToBmp.RenderWithColor(clippingProxyGamma, rasterizer, sl, ColorRGBA.Black);
            rasterizer.ResetGamma(new GammaNone());
            graphics2D.UseSubPixelRendering = false;
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
            rasterizer.AddPath(StrokeHelp.MakeVxs(ps.Vxs, 2));
            //----------------------------------------

            sclineRasToBmp.RenderWithColor(clippingProxyNormal, rasterizer, sl, new ColorRGBA(0, 150, 160, 200));
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
