//MIT, 2014-2017, WinterDev

using System;
using PixelFarm.Agg;
using PixelFarm.DrawingGL;

namespace Mini
{

    static class DemoHelper
    {

        public static PixelFarm.DrawingGL.GLBitmap LoadTexture(string imgFileName)
        {
            using (System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(imgFileName))
            {
                return LoadTexture(bmp);
            }
        }
        public static PixelFarm.DrawingGL.GLBitmap LoadTexture(PixelFarm.Agg.ActualImage actualImg)
        {
            return new PixelFarm.DrawingGL.GLBitmap(actualImg.Width,
                actualImg.Height,
                PixelFarm.Agg.ActualImage.GetBuffer(actualImg), false);
        }
        public static PixelFarm.DrawingGL.GLBitmap LoadTexture(System.Drawing.Bitmap bmp)
        {
            var bmpdata = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height),
                System.Drawing.Imaging.ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            int stride = bmpdata.Stride;
            byte[] buffer = new byte[stride * bmp.Height];
            System.Runtime.InteropServices.Marshal.Copy(bmpdata.Scan0, buffer, 0, buffer.Length);
            bmp.UnlockBits(bmpdata);
            //---------------------------
            //if we are on Little-endian  machine,
            //
            //---------------------------
            return new PixelFarm.DrawingGL.GLBitmap(bmp.Width, bmp.Height, buffer, false);
        }
    }

    public class PrebuiltContext
    {
        public CanvasGL2d gl2dCanvas;
        public GLCanvasPainter glCanvasPainter;
    }

    public abstract class PrebuiltGLControlDemoBase : DemoBase
    {
        System.Windows.Forms.Timer aniTimer;
        IntPtr hh1;
        //-------------------------------
        System.Windows.Forms.Form formTestBed;
        OpenTK.MyGLControl _miniGLControl;
        //-------------------------------


        protected void SwapBuffers()
        {
            _miniGLControl.SwapBuffers();
        }
        protected int ControlWidth { get { return _miniGLControl.Width; } }
        protected int ControlHeight { get { return _miniGLControl.Height; } }
        public override void Draw(CanvasPainter p)
        {
            //this method is not used in this type of demo
        }
        public void SetGLControl(OpenTK.MyGLControl miniGLControl, PrebuiltContext prebuiltContext)
        {
            this._miniGLControl = miniGLControl;
            miniGLControl.SetGLPaintHandler(this.OnGLRender);


            OnInitGLProgram(prebuiltContext, EventArgs.Empty);
        }
        public override void Init()
        {
            //need for agg

            //this.aniTimer = new System.Windows.Forms.Timer();
            //formTestBed = new FormGLTest();
            ////this.formTestBed.Load += this.OnInitGLProgram;
            //this.formTestBed.FormClosing += formTestBed_FormClosing;
            //this.formTestBed.Text = this.GetType().Name;
            ////
            //FormGLTest frmGLTest = (FormGLTest)this.formTestBed;
            //this.miniGLControl = frmGLTest.InitMiniGLControl(this.Width, this.Height);//1276,720 
            //miniGLControl.SetGLPaintHandler(this.OnGLRender);
            //formTestBed.Show();
            //hh1 = miniGLControl.Handle;
            //miniGLControl.MakeCurrent();//*** make current before use
            //this.OnInitGLProgram(null, EventArgs.Empty);
            //formTestBed.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            ////this.aniTimer.Interval = 200;//ms
            ////this.aniTimer.Tick += TimerTick;
        }

        void formTestBed_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e)
        {
            //stop timer
            this.aniTimer.Enabled = false;
            this._miniGLControl.SetGLPaintHandler(null);
            DemoClosing();
        }
        protected virtual void DemoClosing()
        {
        }
        void TimerTick(object sender, EventArgs e)
        {
            OnTimerTick(sender, e);
            //this.miniGLControl.Refresh();
        }
        protected virtual void OnTimerTick(object sender, EventArgs e)
        {
        }
        protected bool EnableAnimationTimer
        {
            get { return this.aniTimer.Enabled; }
            set { this.aniTimer.Enabled = value; }
        }

        protected IntPtr getDisplay()
        {
            return this._miniGLControl.GetEglDisplay();
        }
        protected IntPtr getSurface()
        {
            return this._miniGLControl.GetEglSurface();
        }
    }



}