//2014 BSD,WinterDev
//MatterHackers


using System;
using PixelFarm.Agg.UI;
using PixelFarm.Agg.Image;
using PixelFarm.Agg.VertexSource;

using PixelFarm.VectorMath;

using Mini;
namespace PixelFarm.Agg.Sample_RoundRect
{
    [Info(OrderCode = "10")]
    [Info("Yet another example dedicated to Gamma Correction. If you have a CRT monitor: The rectangle looks bad - "
                + " the rounded corners are thicker than its side lines. First try to drag the “subpixel offset” control "
                + "— it simply adds some fractional value to the coordinates. When dragging you will see that the rectangle"
                + "is 'blinking'. Then increase 'Gamma' to about 1.5. The result will look almost perfect — the visual "
                + "thickness of the rectangle remains the same. That's good, but turn the checkbox 'White on black' on — what "
                + "do we see? Our rounded rectangle looks terrible. Drag the 'subpixel offset' slider — it's blinking as hell."
                + "Now decrease 'Gamma' to about 0.6. What do we see now? Perfect result! If you use an LCD monitor, the good "
                + "value of gamma will be closer to 1.0 in both cases — black on white or white on black. There's no "
                + "perfection in this world, but at least you can control Gamma in Anti-Grain Geometry :-).")]

    public class rounded_rect_application : DemoBase
    {
        double[] m_x = new double[2];
        double[] m_y = new double[2];
        double m_dx;
        double m_dy;
        int m_idx;

        //MatterHackers.Agg.UI.Slider m_radius;
        //MatterHackers.Agg.UI.Slider m_gamma;
        //MatterHackers.Agg.UI.Slider m_offset;
        //MatterHackers.Agg.UI.CheckBox m_white_on_black;
        //MatterHackers.Agg.UI.CheckBox m_DrawAsOutlineCheckBox;


        public rounded_rect_application()
        {
            //AnchorAll();
            //m_idx = (-1);
            //m_radius = new MatterHackers.Agg.UI.Slider(new Vector2(10, 10), new Vector2(580, 9));
            //m_gamma = new MatterHackers.Agg.UI.Slider(new Vector2(10, 10 + 40), new Vector2(580, 9));
            //m_offset = new MatterHackers.Agg.UI.Slider(new Vector2(10, 10 + 80), new Vector2(580, 9));
            //m_white_on_black = new CheckBox(10, 10 + 60, "White on black");
            //m_DrawAsOutlineCheckBox = new CheckBox(10 + 180, 10 + 60, "Fill Rounded Rect");

            //m_radius.ValueChanged += new EventHandler(NeedsRedraw);
            //m_gamma.ValueChanged += new EventHandler(NeedsRedraw);
            //m_offset.ValueChanged += new EventHandler(NeedsRedraw);
            //m_white_on_black.CheckedStateChanged += new CheckBox.CheckedStateChangedEventHandler(NeedsRedraw);
            //m_DrawAsOutlineCheckBox.CheckedStateChanged += new CheckBox.CheckedStateChangedEventHandler(NeedsRedraw);

            m_x[0] = 100; m_y[0] = 100;
            m_x[1] = 500; m_y[1] = 350;
            //AddChild(m_radius);
            //AddChild(m_gamma);
            //AddChild(m_offset);
            //AddChild(m_white_on_black);
            //AddChild(m_DrawAsOutlineCheckBox);
            //m_gamma.Text = "gamma={0:F3}";
            //m_gamma.SetRange(0.0, 3.0);
            //m_gamma.Value = 1.8;



            //m_offset.Text = "subpixel offset={0:F3}";
            //m_offset.SetRange(-2.0, 3.0);

            //m_white_on_black.TextColor = new RGBA_Bytes(127, 127, 127);
            ////m_white_on_black.inactive_color(new RGBA_Bytes(127, 127, 127));

            //m_DrawAsOutlineCheckBox.TextColor = new RGBA_Floats(.5, .5, .5).GetAsRGBA_Bytes();
            ////m_DrawAsOutlineCheckBox.inactive_color(new RGBA_Bytes(127, 127, 127));

            this.Radius = 25;
            this.Gamma = 1.8;

        }
        [DemoConfig]
        public bool FillRoundRect
        {
            get;
            set;
        }
        [DemoConfig]
        public bool WhiteOnBlack
        {
            get;
            set;
        }
        [DemoConfig(MaxValue = 50)]
        public double Radius
        {
            get;
            set;
        }
        [DemoConfig(MaxValue = 3)]
        public double Gamma
        {
            get;
            set;
        }
        [DemoConfig(MinValue = -2, MaxValue = 3)]
        public double SubPixelOffset
        {
            get;
            set;
        }
        public override void Draw(Graphics2D graphics2D)
        {
            var widgetsSubImage = ImageHelper.CreateChildImage(graphics2D.DestImage, graphics2D.GetClippingRectInt());

            IImage backBuffer = widgetsSubImage;

            
            var normalBlender = new PixelBlenderBGRA();
            var gammaBlender = new PixelBlenderGammaBGRA(this.Gamma);

            var rasterNormal = new ChildImage(backBuffer, normalBlender);
            var rasterGamma = new ChildImage(backBuffer, gammaBlender);

            var clippingProxyNormal = new ClipProxyImage(rasterNormal);
            var clippingProxyGamma = new ClipProxyImage(rasterGamma);
            

            clippingProxyNormal.Clear(this.WhiteOnBlack ? ColorRGBA.Black : ColorRGBA.White);

            var ras = new ScanlineRasterizer();
            var sl = new ScanlinePacked8();

            Ellipse ellipse = new Ellipse();

            // TODO: If you drag the control circles below the bottom of the window we get an exception.  This does not happen in AGG.
            // It needs to be debugged.  Turning on clipping fixes it.  But standard agg works without clipping.  Could be a bigger problem than this.
            //ras.clip_box(0, 0, width(), height());

            // Render two "control" circles
            ellipse.Reset(m_x[0], m_y[0], 3, 3, 16);
            ras.AddPath(ellipse.MakeVxs());
            ScanlineRasToDestBitmapRenderer sclineRasToBmp = new ScanlineRasToDestBitmapRenderer();
            sclineRasToBmp.RenderScanlineSolidAA(clippingProxyNormal, ras, sl, new ColorRGBA(127, 127, 127));

            ellipse.Reset(m_x[1], m_y[1], 3, 3, 16);
            ras.AddPath(ellipse.MakeVertexSnap());
            sclineRasToBmp.RenderScanlineSolidAA(clippingProxyNormal, ras, sl, new ColorRGBA(127, 127, 127));


            double d = this.SubPixelOffset;

            // Creating a rounded rectangle
            VertexSource.RoundedRect r = new VertexSource.RoundedRect(m_x[0] + d, m_y[0] + d, m_x[1] + d, m_y[1] + d,
                this.Radius);
            r.NormalizeRadius();
            if (this.FillRoundRect)
            {

                ras.AddPath(new Stroke(1).MakeVxs(r.MakeVxs()));
            }
            else
            {
                ras.AddPath(r.MakeVxs());
            }
            sclineRasToBmp.RenderScanlineSolidAA(clippingProxyGamma, ras, sl, this.WhiteOnBlack ? new ColorRGBA(255, 255, 255) : new ColorRGBA(0, 0, 0));


        }
        public override void MouseDown(int mx, int my, bool isRightButton)
        {
            for (int i = 0; i < 2; i++)
            {
                double x = mx;
                double y = my;
                if (Math.Sqrt((x - m_x[i]) * (x - m_x[i]) + (y - m_y[i]) * (y - m_y[i])) < 5.0)
                {
                    m_dx = x - m_x[i];
                    m_dy = y - m_y[i];
                    m_idx = i;
                    break;
                }
            }
        }
        public override void MouseDrag(int mx, int my)
        {
            m_x[m_idx] = mx - m_dx;
            m_y[m_idx] = my - m_dy;
        }

        public override void MouseUp(int x, int y)
        {
            m_idx = -1;

        }

    }

}
