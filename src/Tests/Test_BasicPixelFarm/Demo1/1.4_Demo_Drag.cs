//Apache2, 2014-present, WinterDev

using PixelFarm.Drawing;
using LayoutFarm.UI;
using LayoutFarm.CustomWidgets;
namespace LayoutFarm
{
    [DemoNote("1.4 DemoDrag")]
    class Demo_Drag : App
    {
        protected override void OnStart(AppHost host)
        {

            var box_beh = new UIMouseBehaviour<Box, bool>();
            {
                box_beh.MouseDown += (s, e) =>
                {
                    e.MouseCursorStyle = MouseCursorStyle.Pointer;
                };
                box_beh.MouseUp += (s, e) =>
                {
                    e.MouseCursorStyle = MouseCursorStyle.Default;
                    //box.BackColor = Color.LightGray;
                    s.Source.BackColor = Color.FromArgb(50, KnownColors.FromKnownColor(KnownColor.DeepSkyBlue));
                };
                box_beh.MouseMove += (s, e) =>
                {
                    if (e.IsDragging)
                    {
                        Box box = s.Source;
                        box.BackColor = Color.FromArgb(180, KnownColors.FromKnownColor(KnownColor.GreenYellow));
                        Point pos = box.Position;
                        box.SetLocation(pos.X + e.XDiff, pos.Y + e.YDiff);
                        e.MouseCursorStyle = MouseCursorStyle.Pointer;
                        e.CancelBubbling = true;
                    }
                };
            }

            //-------------
            {
                var box1 = new LayoutFarm.CustomWidgets.Box(50, 50);
                box1.BackColor = Color.Red;
                box1.SetLocation(10, 10); 
                box_beh.AttachSharedBehaviorTo(box1); 
                host.AddChild(box1);
            }
            //--------------------------------
            {
                var box2 = new LayoutFarm.CustomWidgets.Box(30, 30);
                box2.SetLocation(50, 50);
                //box2.dbugTag = 2;
                box_beh.AttachSharedBehaviorTo(box2);
                host.AddChild(box2);
            }
        } 
    }
}