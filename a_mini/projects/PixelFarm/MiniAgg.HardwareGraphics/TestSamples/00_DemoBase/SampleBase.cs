using System;
namespace OpenTkEssTest
{
    public abstract class SampleBase
    {
        bool _enableAnimationTimer;

        public void InitGLProgram()
        {
            OnInitGLProgram(this, EventArgs.Empty);
        }
        public void Close()
        {
            DemoClosing();
        }
        public void Render()
        {
            OnGLRender(this, EventArgs.Empty);
        }
        protected abstract void OnInitGLProgram(object sender, EventArgs e);
        protected abstract void DemoClosing();
        protected virtual void OnGLRender(object sender, EventArgs args) { }
        public int Width { get; set; }
        public int Height { get; set; }

        protected void SwapBuffer() { }

        protected static PixelFarm.DrawingGL.GLBitmap LoadTexture(string imgFileName)
        {
            int imgW, imgH, imgComponent;
            int outputComponent = 4;//note that we want output color component=4
            IntPtr imgData = PixelFarm.Drawing.MyFtImageLib.stbi_load(imgFileName, out imgW, out imgH, out imgComponent, outputComponent);
            //copy data
            byte[] buffer = null;
            buffer = new byte[imgW * imgH * outputComponent];
            System.Runtime.InteropServices.Marshal.Copy(imgData, buffer, 0, buffer.Length);
            PixelFarm.Drawing.MyFtImageLib.DeleteUnmanagedObj(imgData);
            var glbmp = new PixelFarm.DrawingGL.GLBitmap(imgW, imgH, buffer, false);
            //we load image from myft's image module
            //its already big-endian
            glbmp.IsBigEndianPixel = true;
            return glbmp;
        }
        protected bool EnableAnimationTimer
        {
            get { return _enableAnimationTimer; }
            set
            {
                _enableAnimationTimer = value;
            }
        }
        protected virtual void OnTimerTick(object sender, EventArgs e)
        {

        }
    }
}