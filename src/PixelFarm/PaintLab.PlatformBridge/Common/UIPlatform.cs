//Apache2, 2014-present, WinterDev 
using System;
using System.Collections.Generic;
using LayoutFarm.UI;


namespace LayoutFarm
{


    public abstract class UIPlatform
    {
        static UIPlatform s_ui_platform;
        static bool s_Closing;

        //----------------------------------------------------------------
        //monitor:
        public abstract PixelFarm.Drawing.Size GetPrimaryMonitorSize();
        //----------------------------------------------------------------
        //clipboard
        public abstract void ClearClipboardData();
        public abstract bool ContainsClipboardData(string dataformat);
        public abstract object GetClipboardData(string dataformat);

        public abstract string GetClipboardText();
        public abstract PixelFarm.Drawing.Image GetClipboardImage();
        public abstract IEnumerable<string> GetClipboardFileDropList();
        public abstract void SetClipboardText(string textData);
        public abstract void SetClipboardImage(PixelFarm.Drawing.Image img);
        public abstract void SetClipboardFileDropList(string[] filedrops);

        //----------------------------------------------------------------
        //cursor
        protected abstract Cursor CreateCursorImpl(CursorRequest curReq);
        public static Cursor CreateCursor(CursorRequest curReq) => s_ui_platform.CreateCursorImpl(curReq);
        //----------------------------------------------------------------

     

        //----------------------------------------------------------------
        protected void SetAsDefaultPlatform()
        {
            s_ui_platform = this;
        }
        public static void Close()
        {
            s_Closing = true;
        }

        public static UIPlatform CurrentPlatform => s_ui_platform;
        //----------------------------------------------------------------
        //timer and msg loop
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
            timerTask.Interval = intervalMillisec;
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