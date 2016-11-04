//BSD, 2014-2016, WinterDev
//MatterHackers 

using System;
using PixelFarm.Agg.Imaging;
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

    public class RoundRectApplication : DemoBase
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


        public RoundRectApplication()
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
            this.Gamma = 1.8f;
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
        public float Gamma
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
        public override void Draw(CanvasPainter p)
        {
            //-----------------------------------------------------------------
            //control
            CanvasPainter painter = p;
            painter.Clear(this.WhiteOnBlack ? Drawing.Color.Black : Drawing.Color.White);
            painter.FillColor = Drawing.Color.FromArgb(127, 127, 127);
            painter.FillCircle(m_x[0], m_y[0], 3); //left-bottom control box
            painter.FillCircle(m_x[1], m_y[1], 3); //right-top control box
            //-----------------------------------------------------------------

            double d = this.SubPixelOffset;
            AggCanvasPainter p2 = p as AggCanvasPainter;
            IPixelBlender prevBlender = null;
            Graphics2D gx = null;
            if (p2 != null)
            {
                //for agg only
                gx = p2.Graphics;
                prevBlender = gx.PixelBlender;
                //change gamma blender
                gx.PixelBlender = new PixelBlenderGammaBGRA(this.Gamma);
            }

            if (this.FillRoundRect)
            {
                painter.FillColor = this.WhiteOnBlack ? Drawing.Color.White : Drawing.Color.Black;
                painter.FillRoundRectangle(
                    m_x[0] + d,
                    m_y[0] + d,
                    m_x[1] + d,
                    m_y[1] + d,
                    this.Radius);
            }
            else
            {
                painter.StrokeColor = this.WhiteOnBlack ? Drawing.Color.White : Drawing.Color.Black;
                painter.DrawRoundRect(
                    m_x[0] + d,
                    m_y[0] + d,
                    m_x[1] + d,
                    m_y[1] + d,
                    this.Radius);
            }
            if (gx != null)
            {
                gx.PixelBlender = prevBlender;
            }
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
