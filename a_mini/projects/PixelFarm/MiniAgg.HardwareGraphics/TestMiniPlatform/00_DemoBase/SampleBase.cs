using System;
using OpenTK.Graphics.ES20;
namespace OpenTkEssTest
{
    public abstract class SampleBase
    {
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
    }
}