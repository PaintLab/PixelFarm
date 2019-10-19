//Apache2, 2014-present, WinterDev

using System;
using System.Collections.Generic;
using System.Windows.Forms; 

namespace LayoutFarm.UI
{
    partial class TopWindowBridgeWinForm
    {
        UIKeyEventArgs GetTranslatedUIKeyEventArg(System.Windows.Forms.KeyPressEventArgs e)
        {
            UIKeyEventArgs keyEventArg = _keyEventStack.Count > 0 ? _keyEventStack.Pop() : new UIKeyEventArgs();
            keyEventArg.SetKeyChar(e.KeyChar);
            return keyEventArg;
        }
        public abstract void BindWindowControl(IGpuOpenGLSurfaceView windowControl);
        public bool HandleProcessDialogKey(System.Windows.Forms.Keys keyData)
        {
            return HandleProcessDialogKey((LayoutFarm.UI.UIKeys)keyData);
        }
        public void HandleMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            HandleMouseDown(GetTranslatedUIMouseEventArgs(e));

        }
        UIMouseEventArgs GetTranslatedUIMouseEventArgs(System.Windows.Forms.MouseEventArgs e)
        {
            UIMouseButtons mouseButton = UIMouseButtons.Left;

            switch (e.Button)
            {
                case MouseButtons.Left:
                    mouseButton = UIMouseButtons.Left;
                    break;
                case MouseButtons.Right:
                    mouseButton = UIMouseButtons.Right;
                    break;
                case MouseButtons.Middle:
                    mouseButton = UIMouseButtons.Middle;
                    break;
                default:
                    mouseButton = UIMouseButtons.Left;
                    break;
            }

            UIMouseEventArgs mouseEventArgs = (_mouseEventStack.Count > 0) ? _mouseEventStack.Pop() : new UIMouseEventArgs();
            mouseEventArgs.SetEventInfo(
                e.X + _canvasViewport.ViewportX,
                e.Y + _canvasViewport.ViewportY,
                mouseButton,
                e.Clicks,
                e.Delta);
            return mouseEventArgs;
        }

        public void HandleMouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            HandleMouseMove(GetTranslatedUIMouseEventArgs(e));
        }

        public void HandleMouseWheel(System.Windows.Forms.MouseEventArgs e)
        {
            HandleMouseWheel(GetTranslatedUIMouseEventArgs(e));
        }
        public void HandleMouseUp(System.Windows.Forms.MouseEventArgs e)
        {
            HandleMouseUp(GetTranslatedUIMouseEventArgs(e));
        }

        public void HandleKeyUp(System.Windows.Forms.KeyEventArgs e)
        {
            HandleKeyUp(GetTranslatedUIKeyEventArg(e));

        }
        public void HandleKeyPress(System.Windows.Forms.KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar))
            {
                return;
            }
            HandleKeyPress(GetTranslatedUIKeyEventArg(e), e.KeyChar);
        }
        public void HandleKeyDown(System.Windows.Forms.KeyEventArgs e)
        {
            HandleKeyDown(GetTranslatedUIKeyEventArg(e));
        }
        //------------------------------------------------------
        UIKeyEventArgs GetTranslatedUIKeyEventArg(System.Windows.Forms.KeyEventArgs e)
        {
            UIKeyEventArgs keyEventArg = _keyEventStack.Count > 0 ? _keyEventStack.Pop() : new UIKeyEventArgs();
            keyEventArg.SetEventInfo((uint)e.KeyData, e.Shift, e.Alt, e.Control);
            return keyEventArg;
        }
    }

}