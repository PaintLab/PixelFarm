using System;
using System.Collections.Generic;
using System.ComponentModel;

using System.Drawing;

using System.Text;
using System.Windows.Forms;
using MatterHackers.Agg;
using MatterHackers.Agg.Image;

namespace Mini
{
    public partial class FormTestBed1 : Form
    {
        bool isMouseDown;
        WindowsFormsBitmapBackBuffer bitmapBackBuffer = new WindowsFormsBitmapBackBuffer();
        ExampleBase exampleBase;

        public FormTestBed1()
        {
            InitializeComponent();
            this.Load += new EventHandler(Form1_Load);
        }
        void Form1_Load(object sender, EventArgs e)
        {
            OnInitialize(800, 600);
        }
        public void LoadExample(ExampleBase exampleBase)
        {
            this.exampleBase = exampleBase;
            exampleBase.Init();
        }
        Graphics2D NewGraphics2D()
        {
            Graphics2D graphics2D;
            if (bitmapBackBuffer.backingImageBufferByte != null)
            {
                graphics2D = bitmapBackBuffer.backingImageBufferByte.NewGraphics2D();
            }
            else
            {
                graphics2D = bitmapBackBuffer.backingImageBufferFloat.NewGraphics2D();
            }
            graphics2D.PushTransform();
            return graphics2D;
        }
        void OnInitialize(int width, int height)
        {
            bitmapBackBuffer.Initialize(width, height, 32);
            NewGraphics2D().Clear(new RGBA_Floats(1, 1, 1, 1));
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            this.isMouseDown = true;
            base.OnMouseDown(e);
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            this.isMouseDown = false;
            base.OnMouseUp(e);
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (this.isMouseDown)
            {
                exampleBase.MouseDrag(e.X, e.Y);
                
                Invalidate();
            }
            base.OnMouseMove(e);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            int width = 800;
            int height = 600;
            var graphics = bitmapBackBuffer.backingImageBufferByte.NewGraphics2D();

            //--------------------------------
            exampleBase.Draw(graphics);
            //--------------------------------
            RectangleInt intRect = new RectangleInt(0, 0, width, height);
            Graphics g1 = e.Graphics;
            bitmapBackBuffer.UpdateHardwareSurface(intRect);

            //WidgetForWindowsFormsBitmap.copyTime.Restart();

            //if (OsInformation.OperatingSystem != OSType.Windows)
            //{
            //    //displayGraphics.DrawImage(aggBitmapAppWidget.bitmapBackBuffer.windowsBitmap, windowsRect, windowsRect, GraphicsUnit.Pixel);  // around 250 ms for full screen
            //    displayGraphics.DrawImageUnscaled(aggBitmapAppWidget.bitmapBackBuffer.windowsBitmap, 0, 0); // around 200 ms for full screnn
            //}
            //else
            //{
            // or the code below which calls BitBlt directly running at 17 ms for full screnn.
            const int SRCCOPY = 0xcc0020;

            using (Graphics bitmapGraphics = Graphics.FromImage(bitmapBackBuffer.windowsBitmap))
            {
                IntPtr displayHDC = g1.GetHdc();
                IntPtr bitmapHDC = bitmapGraphics.GetHdc();

                IntPtr hBitmap = bitmapBackBuffer.windowsBitmap.GetHbitmap();
                IntPtr hOldObject = SelectObject(bitmapHDC, hBitmap);

                int result = BitBlt(displayHDC, 0, 0,
                    bitmapBackBuffer.windowsBitmap.Width,
                    bitmapBackBuffer.windowsBitmap.Height,
                    bitmapHDC, 0, 0, SRCCOPY);

                SelectObject(bitmapHDC, hOldObject);
                DeleteObject(hBitmap);

                bitmapGraphics.ReleaseHdc(bitmapHDC);
                g1.ReleaseHdc(displayHDC);
            }
            //}
            //WidgetForWindowsFormsBitmap.copyTime.Stop();


            base.OnPaint(e);
        }

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        [System.Runtime.InteropServices.DllImportAttribute("gdi32.dll")]
        public static extern System.IntPtr SelectObject(System.IntPtr hdc, System.IntPtr h);

        [System.Runtime.InteropServices.DllImportAttribute("gdi32.dll")]
        private static extern int BitBlt(
            IntPtr hdcDest,     // handle to destination DC (device context)
            int nXDest,         // x-coord of destination upper-left corner
            int nYDest,         // y-coord of destination upper-left corner
            int nWidth,         // width of destination rectangle
            int nHeight,        // height of destination rectangle
            IntPtr hdcSrc,      // handle to source DC
            int nXSrc,          // x-coordinate of source upper-left corner
            int nYSrc,          // y-coordinate of source upper-left corner
            System.Int32 dwRop  // raster operation code
            );
    }
}
