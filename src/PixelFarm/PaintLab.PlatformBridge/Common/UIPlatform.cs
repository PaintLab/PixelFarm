//Apache2, 2014-present, WinterDev 
using LayoutFarm.UI;
namespace LayoutFarm
{


    public abstract class UIPlatform
    {


        static UIPlatform s_ui_plaform;

        public abstract void SetClipboardData(string textData);
        public abstract string GetClipboardData();
        public abstract void ClearClipboardData();

        protected void SetAsDefaultPlatform()
        {
            s_ui_plaform = this;
        }
        public static void RegisterTimerTask(UITimerTask uiTimerTask)
        {
            UIMsgQueueSystem.InternalMsgPumpRegister(uiTimerTask);
        }
        public static void RegisterRunOnceTask(UITimerTask.TimerTick action)
        {
            UI.UITimerTask uiTimerTask = new UI.UITimerTask(action);
            uiTimerTask.Enabled = true;
            uiTimerTask.RunOnce = true;
            UIMsgQueueSystem.InternalMsgPumpRegister(uiTimerTask);
        }
        public static void RegisterTimerTask(int intervalMillisec, UITimerTask.TimerTick timerTick)
        {
            UITimerTask timerTask = new UITimerTask(timerTick);
            timerTask.IntervalInMillisec = intervalMillisec;
            UIMsgQueueSystem.InternalMsgPumpRegister(timerTask);
            timerTask.Enabled = true;
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