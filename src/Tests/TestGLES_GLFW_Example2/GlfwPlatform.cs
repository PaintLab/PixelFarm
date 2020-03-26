//MIT, 2019-present, WinterDev
using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using LayoutFarm;
using LayoutFarm.UI;
using LayoutFarm.UI.InputBridge;
using LayoutFarm.UI.OpenGL;
namespace PixelFarm.Forms
{
    using Glfw;
    public class GlfwPlatform : UIPlatform
    {
        public GlfwPlatform()
        {
            SetAsDefaultPlatform();
        }
        public override void ClearClipboardData()
        {

        }

        public override string GetClipboardData()
        {
            return null;
        }

        public override void SetClipboardData(string textData)
        {
            //TODO: implement 
        }

        protected override Cursor CreateCursorImpl(CursorRequest curReq) => new GlfwCursor();

    }
    class GlfwCursor : Cursor
    {

    }
}