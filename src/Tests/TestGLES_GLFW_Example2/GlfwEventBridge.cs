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


    class MyGlfwTopWindowBridge : MyTopWindowBridgeOpenGL
    {
        public MyGlfwTopWindowBridge(RootGraphic root, ITopWindowEventRoot topWinEventRoot)
            : base(root, topWinEventRoot)
        {

        }

        public class GlfwEventBridge : GlfwWindowEventListener
        {
            readonly UIMouseEventArgs _mouseEventArgs = new UIMouseEventArgs();
            readonly UIKeyEventArgs _keyEventArgs = new UIKeyEventArgs();
            readonly UIPaintEventArgs _paintEventArgs = new UIPaintEventArgs();
            MyTopWindowBridgeOpenGL _myTopWindowBridge;

            int _mouseMoveX;
            int _mouseMoveY;
            UIMouseButtons _ui_button = UIMouseButtons.Left;//default

            public GlfwEventBridge() { }
            public void SetWindowBridge(MyTopWindowBridgeOpenGL bridge)
            {
                _myTopWindowBridge = bridge;
            }
            public override void MouseEvent(IntPtr winPtr, int button, int action, int modifier)
            {

                switch (button)
                {
                    case Glfw3.GLFW_MOUSE_BUTTON_LEFT:
                        _ui_button = UIMouseButtons.Left;
                        break;
                    case Glfw3.GLFW_MOUSE_BUTTON_RIGHT:
                        _ui_button = UIMouseButtons.Right;
                        break;
                    case Glfw3.GLFW_MOUSE_BUTTON_MIDDLE:
                        _ui_button = UIMouseButtons.Middle;
                        break;

                }
                if (action == Glfw3.GLFW_PRESS)
                {
                    _mouseEventArgs.SetEventInfo(_mouseMoveX, _mouseMoveY, _ui_button, 1, 0);
                    _mouseEventArgs.UIEventName = UIEventName.MouseDown;
                    _myTopWindowBridge.HandleMouseDown(_mouseEventArgs);

                }
                else
                {
                    _mouseEventArgs.SetEventInfo(_mouseMoveX, _mouseMoveY, _ui_button, 1, 0);
                    _mouseEventArgs.UIEventName = UIEventName.MouseUp;
                    _myTopWindowBridge.HandleMouseUp(_mouseEventArgs);


                    _ui_button = UIMouseButtons.Left;//reset after mouseup

                }
                base.MouseEvent(winPtr, button, action, modifier);
            }
            public override void CursorEvent(IntPtr windowPtr, double xpos, double ypos)
            {
                _mouseMoveX = (int)xpos;
                _mouseMoveY = (int)ypos;

                _mouseEventArgs.SetEventInfo(_mouseMoveX, _mouseMoveY, _ui_button, 1, 0);
                _mouseEventArgs.UIEventName = UIEventName.MouseMove;
                _myTopWindowBridge.HandleMouseMove(_mouseEventArgs);

                //mouse move
                base.CursorEvent(windowPtr, xpos, ypos);
            }
        }
    }
}