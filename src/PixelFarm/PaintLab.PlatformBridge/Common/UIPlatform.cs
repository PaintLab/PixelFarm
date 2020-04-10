//Apache2, 2014-present, WinterDev 
using System;
using LayoutFarm.UI;
namespace LayoutFarm
{


    public abstract class UIPlatform
    {
        static UIPlatform s_ui_platform;
        static bool s_Closing;

        public abstract void SetClipboardData(string textData);
        public abstract string GetClipboardData();
        public abstract void ClearClipboardData();
        public abstract System.Collections.Generic.List<string> GetClipboardFileDropList();
        public abstract PixelFarm.Drawing.Image GetClipboardImage();
        public abstract void SetClipboardImage(PixelFarm.Drawing.Image img);
        public abstract bool ContainsClipboardImage();

        protected abstract Cursor CreateCursorImpl(CursorRequest curReq);
        public static Cursor CreateCursor(CursorRequest curReq) => s_ui_platform.CreateCursorImpl(curReq);

        protected void SetAsDefaultPlatform()
        {
            s_ui_platform = this;
        }
        public static void Close()
        {
            s_Closing = true;
        }
        public static void RegisterTimerTask(UITimerTask uiTimerTask)
        {
            UIMsgQueueSystem.InternalMsgPumpRegister(uiTimerTask);
        }
        public static void RegisterRunOnceTask(Action<UITimerTask> action)
        {

            UIMsgQueueSystem.InternalMsgPumpRegister(new UI.UITimerTask(action) {
                Enabled = true,
                RunOnce = true
            });
        }
        public static void RegisterTimerTask(int intervalMillisec, Action<UITimerTask> timerTick)
        {
            UITimerTask timerTask = new UITimerTask(timerTick);
            timerTask.IntervalInMillisec = intervalMillisec;
            UIMsgQueueSystem.InternalMsgPumpRegister(timerTask);
            timerTask.Enabled = true;
        }
        protected static void InvokeMsgPumpOneStep()
        {
            if (s_Closing) return;
            //
            UIMsgQueueSystem.InternalMsgPumpOneStep();
        }


        protected static void SetUIMsgMinTimerCounterBackInMillisec(int millisec)
        {
            UIMsgQueueSystem.MinUICountDownInMillisec = millisec;
        }
    }
}