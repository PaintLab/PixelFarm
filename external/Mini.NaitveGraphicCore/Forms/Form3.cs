using System;
using System.Collections.Generic;
using System.ComponentModel;
 
using System.Drawing;
using System.Text;

using LayoutFarm.NativeAgg;
using LayoutFarm.NativeWindows;

namespace Mini.GraphicCore
{
    public partial class Form3 : System.Windows.Forms.Form
    {
        bool isMouseDown;
        bool isReady = false;
        AggCanvas appCanvas;

        List<AggSprite> sprites = new List<AggSprite>();
        bool animationReady = false;

        public Form3()
        {
            InitializeComponent();

        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            NativeAggInterOp.LoadLib();
            appCanvas = AggCanvas.CreateCanvas();
            for (int i = 0; i < 5; ++i)
            {
                var sprite = AggSprite.CreateNewSprite();
                sprite.SetLocation(200 + (i * 100), 300);
                sprites.Add(sprite);
            }

            animationReady = true;
            this.timer1.Interval = 50;
            //this.timer1.Enabled = true;
            this.timer1.Tick += new EventHandler(timer1_Tick);
        }

        void timer1_Tick(object sender, EventArgs e)
        {
            int j = this.sprites.Count;

            for (int i = 0; i < j; ++i)
            {
                var s = sprites[i];
                s.SetLocation(s.X - 10, s.Y - 10);
            }
            UpdateViewport();
        }
        void UpdateViewport()
        {
            Graphics g = this.CreateGraphics();
            IntPtr hdc = g.GetHdc();
            //----------------------------- 
            //1. clear canvas bg
            appCanvas.RenderTo(hdc);
            appCanvas.ClearBackground();

            //2. draw sprite
            int j = sprites.Count;
            for (int i = 0; i < j; ++i)
            {
                sprites[i].Draw(appCanvas);
            }
            //3. render to screen
            appCanvas.RenderTo(hdc);
            //-----------------------------
            g.ReleaseHdc(hdc);

        }
        protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            isMouseDown = true;
            base.OnMouseDown(e);
            appCanvas.Resize(e.X, -e.Y);

            IntPtr hwnd = this.Handle;
            IntPtr hdc = WinNative.GetDC(hwnd);

            //canvas paint, clear bg
            appCanvas.RenderTo(hdc);

            WinNative.ReleaseDC(hwnd, hdc);

        }
        protected override void OnMouseUp(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseUp(e);
            isMouseDown = false;

            if (isReady)
            {
                NativeAggInterOp.CallServerService(ServerServiceName.Draw4);
                NativeAggInterOp.CallServerService(ServerServiceName.RefreshScreen);
            }
            

        }
        protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs e)
        {

            base.OnMouseMove(e);

            if (isMouseDown)
            {
                if (isReady)
                {
                    NativeAggInterOp.CallServerService(ServerServiceName.Draw4);
                    NativeAggInterOp.CallServerService(ServerServiceName.RefreshScreen);
                }
            }
        }

        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {

            // base.OnPaint(e);  
            IntPtr hdc = e.Graphics.GetHdc();
            //----------------------------- 
            //1. clear canvas bg
            appCanvas.RenderTo(hdc);
            appCanvas.ClearBackground();

            //2. draw sprite
            int j = sprites.Count;
            for (int i = 0; i < j; ++i)
            {
                sprites[i].Draw(appCanvas);
            }
            //3. render to screen
            appCanvas.RenderTo(hdc);
            //-----------------------------
            e.Graphics.ReleaseHdc(hdc);
            
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            //close native windows
            NativeAggInterOp.CallServerService(ServerServiceName.Shutdown);
            base.OnClosing(e);
        }


    }
}
