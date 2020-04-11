//MIT, 2019-present, WinterDev
using System;
using System.Collections.Generic;
using LayoutFarm;
using LayoutFarm.UI;
using PixelFarm.Drawing;
namespace PixelFarm.Forms
{
    public class GlfwPlatform : UIPlatform
    {
        public GlfwPlatform()
        {
            SetAsDefaultPlatform();
        }
        public override void ClearClipboardData()
        {

        }
        public override object GetClipboardData(string dataformat)
        {
            throw new NotImplementedException();
        }
        public override string GetClipboardText()
        {
            throw new NotImplementedException();
        }
        public override bool ContainsClipboardData(string datatype)
        {
            throw new NotImplementedException();
        }


        protected override Cursor CreateCursorImpl(CursorRequest curReq) => new GlfwCursor();

        public override void SetClipboardImage(Image img)
        {

        }

        public override Image GetClipboardImage()
        {
            return null;
        }

        public override IEnumerable<string> GetClipboardFileDropList()
        {
            throw new NotImplementedException();
        }

        public override void SetClipboardText(string textData)
        {
            throw new NotImplementedException();
        }

        public override void SetClipboardFileDropList(string[] filedrops)
        {
            throw new NotImplementedException();
        }
    }
    class GlfwCursor : Cursor
    {

    }
    public class GlfwWindowWrapper : IGpuOpenGLSurfaceView
    {
        readonly GlFwForm _form;
        public GlfwWindowWrapper(GlFwForm form)
        {
            _form = form;
        }

        public IntPtr NativeWindowHwnd => _form.Handle;

        public int Width => _form.Width;

        public int Height => _form.Height;

        public Cursor CurrentCursor
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public void Dispose()
        {
        }

        public IntPtr GetEglDisplay()
        {

            return IntPtr.Zero;
        }

        public IntPtr GetEglSurface()
        {
            return IntPtr.Zero;
        }

        public Size GetSize() => new Size(_form.Width, _form.Height);

        public void Invalidate()
        {
            _form.Invalidate();
        }

        public void MakeCurrent()
        {
            _form.MakeCurrent();
        }

        public void SwapBuffers()
        {
            _form.SwapBuffers();
        }
        public void Refresh()
        {
            //???
        }
        public void SetBounds(int left, int top, int width, int height)
        {
            _form.SetBounds(left, top, width, height);
        }

        public void SetSize(int width, int height)
        {
            _form.SetSize(width, height);
        }
    }
}