//Apache2, 2014-present, WinterDev

using System;
using LayoutFarm.RenderBoxes;
using LayoutFarm.UI;
namespace LayoutFarm.TextEditing
{
    static class GlobalCaretController
    {
        static bool _enableCaretBlink = true;//default
        static TextEditRenderBox _currentTextBox;
        static EventHandler<GraphicsTimerTaskEventArgs> _tickHandler;
        static object _caretBlinkTask = new object();
        static GraphicsTimerTask _task;
        //
        static GlobalCaretController()
        {
            _tickHandler = new EventHandler<GraphicsTimerTaskEventArgs>(caret_TickHandler);
        }
        internal static void RegisterCaretBlink(RootGraphic root)
        {
            if (!root.CaretHandleRegistered)
            {
                root.CaretHandleRegistered = true;
                _task = root.SubscribeGraphicsIntervalTask(
                    _caretBlinkTask,
                    TaskIntervalPlan.CaretBlink,
                    20,
                    _tickHandler);
            }
        }
        static void caret_TickHandler(object sender, GraphicsTimerTaskEventArgs e)
        {

//#if DEBUG
//            return;
//#endif
            if (_currentTextBox != null)
            {
                _currentTextBox.SwapCaretState();
                e.NeedUpdate = 1;
            }
            else
            {
                //Console.WriteLine("no current textbox");
            }
        }
        public static bool EnableCaretBlink
        {
            get => _enableCaretBlink;
            set => _enableCaretBlink = value;
        }
        internal static TextEditRenderBox CurrentTextEditBox
        {
            get => _currentTextBox;
            set
            {
                if (_currentTextBox != value)//&& textEditBox != null)
                {
                    //make lost focus on current textbox
                    if (_currentTextBox != null)
                    {
                        //stop caret on prev element
                        _currentTextBox.SetCaretState(false);
                        var evlistener = _currentTextBox.GetController() as IUIEventListener;
                        _currentTextBox = null;
                        if (evlistener != null)
                        {
                            evlistener.ListenLostKeyboardFocus(null);
                        }
                    }
                }
                _currentTextBox = value;
            }
        }
    }
}