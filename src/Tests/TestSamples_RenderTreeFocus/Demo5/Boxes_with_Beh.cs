//Apache2, 2020-present, WinterDev

using PixelFarm.Drawing;
using LayoutFarm.UI;
namespace LayoutFarm
{
    //The following demos demonstrate General Event Listener
    //and mouse buttons behaviors


    [DemoNote("5.1 BoxEvents")]
    public class Demo_BoxEvents : App
    {
        protected override void OnStart(AppHost host)
        {
            int x_pos = 0;

            GeneralEventListener evListener = new GeneralEventListener();
            evListener.MouseEnter += (s, e) =>
            {
                IUIEventListener ctx = e.CurrentContextElement;
                LayoutFarm.CustomWidgets.Box box = (LayoutFarm.CustomWidgets.Box)ctx;
                box.BackColor = Color.Red;
                System.Diagnostics.Debug.WriteLine("mouse_enter:" + box.dbugId);
            };
            evListener.MouseLeave += (s, e) =>
            {
                IUIEventListener ctx = e.CurrentContextElement;
                LayoutFarm.CustomWidgets.Box box = (LayoutFarm.CustomWidgets.Box)ctx;
                box.BackColor = Color.Blue;
                System.Diagnostics.Debug.WriteLine("mouse_leave:" + box.dbugId);
            };

            for (int i = 0; i < 10; ++i)
            {
                var sampleButton = new LayoutFarm.CustomWidgets.Box(30, 30);
                sampleButton.BackColor = Color.Blue;
                sampleButton.SetLocation(x_pos, 10);
                sampleButton.AttachExternalEventListener(evListener);

                host.AddChild(sampleButton);

                x_pos += 30 + 5;
            }

        }
    }

    [DemoNote("5.2 BoxEvents")]
    public class Demo_BoxEvents2 : App
    {


        readonly Color _normalState = Color.FromArgb(100, Color.Blue);
        readonly Color _mouseEnterState = Color.FromArgb(100, Color.Red);
        readonly Color _hoverState = Color.FromArgb(100, Color.Green);

        protected override void OnStart(AppHost host)
        {
            int x_pos = 0;

            //create behaviour for specific host, => LayoutFarm.CustomWidgets.Box  
            var beh = new UIMouseBehaviour<LayoutFarm.CustomWidgets.Box, object>();
            beh.MouseEnter += (s, e) =>
            {
                LayoutFarm.CustomWidgets.Box box = s.Source;
                box.BackColor = _mouseEnterState;
                System.Diagnostics.Debug.WriteLine("mouse_enter:" + box.dbugId);
            };
            beh.MouseLeave += (s, e) =>
            {
                LayoutFarm.CustomWidgets.Box box = s.Source;
                box.BackColor = _normalState;
                System.Diagnostics.Debug.WriteLine("mouse_leave:" + box.dbugId);
            };
            beh.MouseHover += (s, e) =>
            {
                LayoutFarm.CustomWidgets.Box box = s.Source;
                box.BackColor = _hoverState;
                System.Diagnostics.Debug.WriteLine("mouse_hover:" + box.dbugId);
            };

            beh.MousePress += (s, e) =>
            {
                LayoutFarm.CustomWidgets.Box box = s.Source;
                Color back_color = box.BackColor;
                box.BackColor = new Color((byte)System.Math.Min(back_color.A + 10, 255), back_color.R, back_color.G, back_color.B);
                System.Diagnostics.Debug.WriteLine("mouse_press:" + box.dbugId);
            };

            for (int i = 0; i < 10; ++i)
            {
                var sampleButton = new LayoutFarm.CustomWidgets.Box(30, 30);
                sampleButton.BackColor = _normalState;
                sampleButton.SetLocation(x_pos, 10);

                beh.AttachSharedBehaviorTo(sampleButton);

                host.AddChild(sampleButton);
                x_pos += 30 + 5;
            }
        }
    }


    [DemoNote("5.3 BoxEvents")]
    public class Demo_BoxEvents3 : App
    {
        readonly Color _normalState = Color.FromArgb(100, Color.Blue);
        readonly Color _mouseEnterState = Color.FromArgb(100, Color.Red);
        readonly Color _hoverState = Color.FromArgb(100, Color.Green);

        class MyButtonState
        {
            //store extra variable for behavior
            public int ClickCount;
        }

        protected override void OnStart(AppHost host)
        {
            int x_pos = 0;


            //mouse behavior for LayoutFarm.CustomWidgets.Box,            
            //with special attachment state => MyButtonState


            var mouseBeh = new UIMouseBehaviour<LayoutFarm.CustomWidgets.Box, MyButtonState>();

            mouseBeh.MouseEnter += (s, e) =>
            {
                //s is a behaviour object that raise the event
                //not the the current context element
                LayoutFarm.CustomWidgets.Box box = s.Source;
                box.BackColor = _mouseEnterState;
                System.Diagnostics.Debug.WriteLine("mouse_enter:" + box.dbugId);
            };
            mouseBeh.MouseLeave += (s, e) =>
            {
                //s is a behaviour object that raise the event
                //not the the current context element

                LayoutFarm.CustomWidgets.Box box = s.Source;
                box.BackColor = _normalState;
                System.Diagnostics.Debug.WriteLine("mouse_leave:" + box.dbugId);
            };
            mouseBeh.MouseHover += (s, e) =>
            {
                //b is a behaviour object that raise the event
                //not the the current context element

                LayoutFarm.CustomWidgets.Box box = s.Source;
                box.BackColor = _hoverState;
                System.Diagnostics.Debug.WriteLine("mouse_hover:" + box.dbugId);
            };
            mouseBeh.MousePress += (s, e) =>
            {
                //s is a behaviour object that raise the event
                //not the the current context element  
                LayoutFarm.CustomWidgets.Box box = s.Source;
                Color back_color = box.BackColor;
                box.BackColor = new Color((byte)System.Math.Min(back_color.A + 10, 255), back_color.R, back_color.G, back_color.B);
                System.Diagnostics.Debug.WriteLine("mouse_press:" + box.dbugId);
            };
            mouseBeh.MouseDown += (s, e) =>
            {
                MyButtonState buttonState = s.State;
                if (buttonState != null)
                {
                    if (buttonState.ClickCount > 3)
                    {
                        s.Source.BackColor = Color.Magenta;
                    }
                    buttonState.ClickCount++;
                }
            };


            for (int i = 0; i < 10; ++i)
            {
                var sampleButton = new LayoutFarm.CustomWidgets.Box(30, 30);
                sampleButton.BackColor = _normalState;
                sampleButton.SetLocation(x_pos, 10);

                MyButtonState state = new MyButtonState();
                mouseBeh.AttachUniqueBehaviorTo(sampleButton, state);

                host.AddChild(sampleButton);

                x_pos += 30 + 5;
            }

        }
    }
}