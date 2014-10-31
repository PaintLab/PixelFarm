//2014 BSD,WinterDev
//MatterHackers

using System; 
using PixelFarm.Agg.UI;
using PixelFarm.Agg.Image;
using PixelFarm.Agg.VertexSource;


using Mini;
namespace PixelFarm.Agg.Sample_TransCurve
{
    [Info(OrderCode = "11")]
    [Info("AGG has a gray-scale renderer that can use any 8-bit color channel of an RGB or RGBA frame buffer. Most likely it will be used to draw gray-scale images directly in the alpha-channel.")]
    public class trans_curve1_application : DemoBase
    {
        PolygonEditWidget m_poly; 

        double[] m_dx = new double[6];
        double[] m_dy = new double[6];

        public trans_curve1_application()
        {


            m_poly = new PolygonEditWidget(6, 5);
            on_init();
            m_poly.Changed += NeedsRedraw;
            this.NumPoints = 200;


            //m_num_points = new MatterHackers.Agg.UI.Slider(5, 5, 340, 12);

            //m_num_points.ValueChanged += new EventHandler(NeedsRedraw);

            //AddChild(m_num_points);

            //m_num_points.Text = "Number of intermediate Points = {0:F3}";
            //m_num_points.SetRange(10, 400);
            //m_num_points.Value = 200;

            //m_close = new CheckBox(350, 5.0, "Close");
            //m_close.CheckedStateChanged += NeedsRedraw;
            //AddChild(m_close);

            //m_preserve_x_scale = new CheckBox(460, 5, "Preserve X scale");
            //m_preserve_x_scale.CheckedStateChanged += NeedsRedraw;
            //AddChild(m_preserve_x_scale);

            //m_fixed_len = new CheckBox(350, 25, "Fixed Length");
            //m_fixed_len.CheckedStateChanged += NeedsRedraw;
            //AddChild(m_fixed_len);

            //m_animate = new CheckBox(460, 25, "Animate");
            //m_animate.CheckedStateChanged += new CheckBox.CheckedStateChangedEventHandler(m_animate_CheckedStateChanged);
            //AddChild(m_animate);
        }

        [DemoConfig(MinValue = 10, MaxValue = 400)]
        public int NumPoints
        {
            get;
            set;
        }
        [DemoConfig]
        public bool Animate
        {
            get;
            set;
        }
        [DemoConfig]
        public bool FixedLength
        {
            get;
            set;
        }
        [DemoConfig]
        public bool Close
        {
            get;
            set;
        }
        [DemoConfig]
        public bool PreserveXScale
        {
            get;
            set;
        }
        void on_init()
        {
            m_poly.SetXN(0, 50);
            m_poly.SetYN(0, 50);
            m_poly.SetXN(1, 150 + 20);
            m_poly.SetYN(1, 150 - 20);
            m_poly.SetXN(2, 250 - 20);
            m_poly.SetYN(2, 250 + 20);
            m_poly.SetXN(3, 350 + 20);
            m_poly.SetYN(3, 350 - 20);
            m_poly.SetXN(4, 450 - 20);
            m_poly.SetYN(4, 450 + 20);
            m_poly.SetXN(5, 550);
            m_poly.SetYN(5, 550);
        }

        Random rand = new Random();

        void m_animate_CheckedStateChanged(object sender, EventArgs e)
        {
            if (this.Animate)
            {
                on_init();
                int i;
                for (i = 0; i < 6; i++)
                {
                    m_dx[i] = ((rand.Next() % 1000) - 500) * 0.01;
                    m_dy[i] = ((rand.Next() % 1000) - 500) * 0.01;
                }
                UiThread.RunOnIdle(guiSurface_Idle);
            }
        }

        void NeedsRedraw(object sender, EventArgs e)
        {

        }
        public override void Draw(Graphics2D g)
        {
            g.Clear(ColorRGBA.White);
            this.m_poly.OnDraw(g);

        }
        public override void MouseDown(int x, int y, bool isRightButton)
        {
            m_poly.OnMouseDown(
                new MouseEventArgs(MouseButtons.Left,
                    1,
                    x, y,
                    1));

            base.MouseDown(x, y, isRightButton);
        }
        public override void MouseUp(int x, int y)
        {
            m_poly.OnMouseUp(
                new MouseEventArgs(MouseButtons.Left,
                    1,
                    x, y,
                    1));
            base.MouseUp(x, y);
        }
        public override void MouseDrag(int x, int y)
        {
            m_poly.OnMouseMove(
                 new MouseEventArgs(MouseButtons.Left,
                     1,
                     x, y,
                     1));

            base.MouseDrag(x, y);
        }

        void move_point(ref double x, ref double y, ref double dx, ref double dy)
        {
            if (x < 0.0) { x = 0.0; dx = -dx; }
            if (x > this.Width) { x = this.Width; dx = -dx; }
            if (y < 0.0) { y = 0.0; dy = -dy; }
            if (y > this.Height) { y = this.Height; dy = -dy; }
            x += dx;
            y += dy;
        }

        void guiSurface_Idle(object state)
        {
            int i;
            for (i = 0; i < 6; i++)
            {
                double x = m_poly.GetXN(i);
                double y = m_poly.GetYN(i);
                move_point(ref x, ref y, ref m_dx[i], ref m_dy[i]);
                m_poly.SetXN(i, x);
                m_poly.SetYN(i, y);
            }


            if (this.Animate)
            {
                UiThread.RunOnIdle(guiSurface_Idle);
            }
        }


    }


}
