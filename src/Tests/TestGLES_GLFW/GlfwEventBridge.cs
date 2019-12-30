//MIT, 2019-present, WinterDev
using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using LayoutFarm.UI.OpenGL;
namespace PixelFarm.Forms
{
    using Glfw;
    public class GlfwEventBridge : GlfwWindowEventListener
    {
        MyTopWindowBridgeOpenGL _myTopWindowBridge;
        public GlfwEventBridge() { }
        public void SetWindowBridge(MyTopWindowBridgeOpenGL bridge)
        {
            _myTopWindowBridge = bridge;
        }
        public override void MouseEvent(IntPtr winPtr, int button, int action, int modifier)
        {
            
            base.MouseEvent(winPtr, button, action, modifier);
        }
    }
}