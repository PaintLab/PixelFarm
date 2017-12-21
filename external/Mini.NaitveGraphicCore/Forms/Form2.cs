using System;
using System.Collections.Generic;
using System.ComponentModel;

using System.Drawing;
using System.Text;

using LayoutFarm.NativePixelLib;
namespace Mini.GraphicCore
{
    public partial class Form2 : System.Windows.Forms.Form
    {
        bool isMouseDown;
        bool isReady = false;

        NativeBmp nativeBmp;
        NativeCanvas nativeCanvas;
        public Form2()
        {
            InitializeComponent();

        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            NativePixelLibInterOp.LoadLib();

            nativeCanvas = NativeCanvas.CreateNativeCanvas(this.Handle, 800, 600);
            Bitmap bmp = new Bitmap("res_fish.bmp");
            nativeBmp = NativeBmp.CreateNativeWrapper(bmp);
            isReady = true;
        }
        protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            isMouseDown = true;
            base.OnMouseDown(e);

        }
        protected override void OnMouseUp(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseUp(e);
            isMouseDown = false;

            //if (isReady)
            //{
            //    NativePixelLibInterOp.CallServerService(nativeWinVideo, ServerServiceName.Draw4);
            //    NativePixelLibInterOp.CallServerService(nativeWinVideo, ServerServiceName.RefreshScreen);
            //}

            //Form2 newForm2 = new Form2();
            //newForm2.Show();
        }
        protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs e)
        {

            base.OnMouseMove(e);

            //if (isMouseDown)
            //{
            //    if (isReady)
            //    {
            //        NativePixelLibInterOp.CallServerService(nativeWinVideo, ServerServiceName.Draw4);
            //        NativePixelLibInterOp.CallServerService(nativeWinVideo, ServerServiceName.RefreshScreen);
            //    }
            //}
        }

        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            base.OnPaint(e);
            if (isReady)
            {
                //1. clear background 
                nativeCanvas.ClearBackground();
                //2. load sample bitmap
                if (nativeBmp != null)
                {
                    int x1 = 10;
                    int y1 = 400;

                    nativeCanvas.DrawImage(this.nativeBmp,
                      x1, y1);
                    
                    nativeCanvas.SetLineWidth(1);
                    nativeCanvas.SetBrushColor(Color.Red);
                    nativeCanvas.DrawLine(10, 10, 500, 500);

                    nativeCanvas.DrawImage(this.nativeBmp,
                        new Point[]{
                            new Point(10+200,10),
                            new Point(200+200,10),
                            new Point(200+50+200,300),
                            new Point(10+50+200,300)});

                } 

                

                nativeCanvas.Render();
            }
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            //close native windows
            if (nativeCanvas != null)
            {
                nativeCanvas.Dispose();
                nativeCanvas = null;
            }
            base.OnClosing(e);
        }
    }
}
