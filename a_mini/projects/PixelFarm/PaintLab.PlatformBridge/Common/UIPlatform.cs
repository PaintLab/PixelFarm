//Apache2, 2014-2017, WinterDev
using System.Collections.Generic;
namespace LayoutFarm.UI
{
    public abstract class UIPlatform
    {
        static UIPlatform ui_plaform;

        public abstract void SetClipboardData(string textData);
        public abstract string GetClipboardData();
        public abstract void ClearClipboardData();

        protected void SetAsDefaultPlatform()
        {
            ui_plaform = this;
        }
        public static void RegisterTimerTask(UITimerTask uiTimer)
        {
            UIMsgQueueSystem.InternalMsgPumpRegister(uiTimer);
        }
        protected static void InvokeMsgPumpOneStep()
        {
            UIMsgQueueSystem.InternalMsgPumpOneStep();
        }
        protected static void SetUIMsgMinTimerCounterBackInMillisec(int millisec)
        {
            UIMsgQueueSystem.MinUICountDownInMillisec = millisec;
        }
    }
}