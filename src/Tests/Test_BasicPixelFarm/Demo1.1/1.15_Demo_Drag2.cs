//Apache2, 2014-present, WinterDev

using PixelFarm.Drawing;
using LayoutFarm.UI;
namespace LayoutFarm
{
    [DemoNote("1.15 Demo_Drag2")]
    class Demo_Drag2 : DemoBase
    {
        protected override void OnStartDemo(SampleViewport viewport)
        {
            {
                var box1 = new LayoutFarm.CustomWidgets.Box(50, 50);
                box1.BackColor = Color.Red;
                box1.SetLocation(10, 10);
                //box1.dbugTag = 1;
                SetupActiveBoxProperties(box1);
                viewport.AddChild(box1);
            }
            //--------------------------------
            {
                var box2 = new LayoutFarm.CustomWidgets.Box(30, 30);
                box2.SetLocation(50, 50);
                //box2.dbugTag = 2;
                SetupActiveBoxProperties(box2);
                viewport.AddChild(box2);
            }
        }
        static void SetupActiveBoxProperties(LayoutFarm.CustomWidgets.Box box)
        {
            //1. mouse down         

            bool mouseUpFromDragging = false;
            System.DateTime mouseDownTime = System.DateTime.MinValue;
            System.DateTime mouseUpTime = System.DateTime.MinValue;
            int mdown_x = 0, mdown_y = 0;
            double velo_x = 0;
            double velo_y = 0;
            int x_dir = 0;
            int y_dir = 0;

            UI.UITimerTask animateTimer = new UITimerTask(tim =>
            {
                box.SetLocation(box.Left + (int)(velo_x * 10 * x_dir),
                    box.Top + (int)(velo_y * 10 * y_dir));

                velo_x -= 0.1;
                velo_y -= 0.1;

                if (velo_x <= 0)
                {
                    velo_x = 0;
                }
                if (velo_y <= 0)
                {
                    velo_y = 0;
                }
                if (velo_x == 0 && velo_y == 0)
                {
                    tim.Enabled = false;
                }
            });
            animateTimer.IntervalInMillisec = 10;
            UIPlatform.RegisterTimerTask(animateTimer);


            box.MouseDown += (s, e) =>
            {
                mdown_x = box.Left;
                mdown_y = box.Top;
                animateTimer.Enabled = false;

                mouseDownTime = System.DateTime.Now;
                mouseUpFromDragging = false;
                //
                box.BackColor = KnownColors.FromKnownColor(KnownColor.DeepSkyBlue);
                e.MouseCursorStyle = MouseCursorStyle.Pointer;
            };
            //2. mouse up
            box.MouseUp += (s, e) =>
            {
                mouseUpTime = System.DateTime.Now;
                e.MouseCursorStyle = MouseCursorStyle.Default;
                box.BackColor = Color.LightGray;

                if (mouseUpFromDragging)
                {
                    int mup_x = box.Left;
                    int mup_y = box.Top;

                    System.TimeSpan drag_timespan = mouseUpTime - mouseDownTime;
                    double ms = drag_timespan.Milliseconds;

                    double displacement_x = mup_x - mdown_x;
                    double displacement_y = mup_y - mdown_y;
                    velo_x = System.Math.Abs(displacement_x / ms);
                    velo_y = System.Math.Abs(displacement_y / ms);

                    x_dir = (mup_x >= mdown_x) ? 1 : -1;
                    y_dir = (mup_y >= mdown_y) ? 1 : -1;
                    animateTimer.Enabled = true;

                }

            };
            box.MouseDrag += (s, e) =>
            {
                mouseUpFromDragging = true;
                box.BackColor = KnownColors.FromKnownColor(KnownColor.GreenYellow);
                Point pos = box.Position;
                box.SetLocation(pos.X + e.XDiff, pos.Y + e.YDiff);
                e.MouseCursorStyle = MouseCursorStyle.Pointer;
                e.CancelBubbling = true;
            };
        }
    }
}