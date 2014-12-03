//2014 BSD,WinterDev
//MatterHackers

using System;

using PixelFarm.Agg.UI;
using PixelFarm.Agg.Image;
using PixelFarm.Agg.VertexSource;

using PixelFarm.VectorMath;

using Mini;
namespace PixelFarm.Agg.Sample_AADemoTest2
{


    class CustomScanlineRasToBmp_EnlargedV2 : CustomScanlineRasToDestBitmapRenderer
    {
        double m_size;
        Square m_square;
        ScanlineUnpacked8 m_sl = new ScanlineUnpacked8();

        Graphics2D gfx;
        public CustomScanlineRasToBmp_EnlargedV2(double size, ActualImage destImage)
        {
            this.ScanlineRenderMode = Agg.ScanlineRenderMode.Custom;
            m_size = size;
            m_square = new Square(size);
            gfx = Graphics2D.CreateFromImage(destImage);
        }
        protected override void CustomRenderSingleScanLine(IImageReaderWriter destImage, Scanline scanline, ColorRGBA color)
        {
            int y = scanline.Y;
            int num_spans = scanline.SpanCount;

            byte[] covers = scanline.GetCovers();

            int spanCount = scanline.SpanCount;
            var ras = gfx.ScanlineRasterizer;
            var rasToBmp = gfx.ScanlineRasToDestBitmap;

            for (int i = 1; i <= num_spans; ++i)
            {
                var span2 = scanline.GetSpan(i);
                int x = span2.x;
                int num_pix = span2.len;
                int coverIndex = span2.cover_index;
                do
                {
                    int a = (covers[coverIndex++] * color.Alpha0To255) >> 8;
                    m_square.Draw(rasToBmp,
                             ras, m_sl, destImage,
                             new ColorRGBA(color, a),
                            x, y);
                    ++x;
                }
                while (--num_pix > 0);

            }

             

        }
    }

    [Info(OrderCode = "02")]
    [Info("Demonstration of the Anti-Aliasing principle with Subpixel Accuracy. The triangle "
                    + "is rendered two times, with its “natural” size (at the bottom-left) and enlarged. "
                    + "To draw the enlarged version there is a special scanline renderer written (see "
                    + "class renderer_enlarged in the source code). You can drag the whole triangle as well "
                    + "as each vertex of it. Also change “Gamma” to see how it affects the quality of Anti-Aliasing.")]
    public class aa_demo_test2 : DemoBase
    {
        double[] m_x = new double[3];
        double[] m_y = new double[3];
        double m_dx;
        double m_dy;
        int m_idx;


        public aa_demo_test2()
        {
            m_idx = -1;
            m_x[0] = 57; m_y[0] = 100;
            m_x[1] = 369; m_y[1] = 170;
            m_x[2] = 143; m_y[2] = 310;

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
        public override void Draw(Graphics2D g)
        {
            OnDraw(g);
        }

        public void OnDraw(Graphics2D graphics2D)
        {

            var childImage = ImageHelper.CreateChildImage(graphics2D.DestImage, graphics2D.GetClippingRect());

            //IRecieveBlenderByte rasterBlender = new BlenderBGRA(); 
            var rasterGamma = new ChildImage(childImage, new PixelBlenderGammaBGRA(this.GammaValue));

            ClipProxyImage clippingProxyNormal = new ClipProxyImage(childImage);
            ClipProxyImage clippingProxyGamma = new ClipProxyImage(rasterGamma);

            clippingProxyNormal.Clear(ColorRGBA.White);

            var rasterizer = graphics2D.ScanlineRasterizer;
            var sl = new ScanlineUnpacked8();


            int size_mul = this.PixelSize;

            CustomScanlineRasToBmp_EnlargedV2 sclineToBmpEn2 = new CustomScanlineRasToBmp_EnlargedV2(size_mul, graphics2D.DestActualImage);

            rasterizer.Reset();
            rasterizer.MoveTo(m_x[0] / size_mul, m_y[0] / size_mul);
            rasterizer.LineTo(m_x[1] / size_mul, m_y[1] / size_mul);
            rasterizer.LineTo(m_x[2] / size_mul, m_y[2] / size_mul);

            sclineToBmpEn2.RenderWithColor(clippingProxyGamma, rasterizer, sl, ColorRGBA.Black);


            ScanlineRasToDestBitmapRenderer sclineRasToBmp = graphics2D.ScanlineRasToDestBitmap;
            
            sclineRasToBmp.RenderWithColor(clippingProxyGamma, rasterizer, sl, ColorRGBA.Black);

            //-----------------------------------------------------------------------------------------------------------
            rasterizer.ResetGamma(new GammaNone());

            PathWriter ps = new PathWriter();
            ps.Clear();
            ps.MoveTo(m_x[0], m_y[0]);
            ps.LineTo(m_x[1], m_y[1]);
            ps.LineTo(m_x[2], m_y[2]);
            ps.LineTo(m_x[0], m_y[0]);

            rasterizer.AddPath((new Stroke(2)).MakeVxs(ps.Vxs));
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
