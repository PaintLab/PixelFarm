//MIT, 2014-2017, WinterDev

using System.Collections.Generic;
namespace LayoutFarm.UI
{

    public class UITimerTask
    {
        TimerTick tickAction;
        int _intervalInMillisec;
        public delegate void TimerTick(UITimerTask timerTask);

        public UITimerTask(TimerTick tickAction)
        {
            this.tickAction = tickAction;
            RunOnce = false;
        }
        public bool Enabled
        {
            get;
            set;
        }

        /// <summary>
        /// interval of this timer in ms
        /// </summary>
        public int IntervalInMillisec
        {
            get { return _intervalInMillisec; }
            set
            {
                _intervalInMillisec = value;
                _remaining = 0;
            }
        }
        public bool RunOnce { get; set; }

        protected virtual void InvokeAction()
        {
            //direct invoke action
            tickAction(this);
        }
        public virtual void Reset()
        {
        }
        public bool IsRegistered
        {
            //TODO: review this member accessibility here
            get;
            internal set;
        }

        internal bool RemoveFromQueue { get; set; }
        public void Remove()
        {
            RemoveFromQueue = true;
        }
        //TODO: review here
        int _remaining;
        internal static bool CountDown(UITimerTask timer_task, int decrement)
        {
            timer_task._remaining -= decrement;
            if (timer_task._remaining <= 0)
            {
                timer_task._remaining = timer_task._intervalInMillisec;//reset
                //invoke action
                timer_task.InvokeAction();
                if (timer_task.RunOnce)
                {
                    timer_task.RemoveFromQueue = true;
                }
                return true;
            }
            return false;
        }
    }

    static class UIMsgQueueSystem
    {

        static Queue<UITimerTask> s_uiTimerTasks = new Queue<UITimerTask>();
        internal static int MinUICountDownInMillisec = 10; //default
        internal static void InternalMsgPumpRegister(UITimerTask timerTask)
        {
            if (timerTask.IsRegistered || timerTask.IntervalInMillisec <= 0)
                return;
            //
            s_uiTimerTasks.Enqueue(timerTask);
            timerTask.IsRegistered = true;
        }
        internal static void InternalMsgPumpOneStep()
        {
            //platform must invoke this in UI/msg queue thread ***
            for (int i = s_uiTimerTasks.Count - 1; i >= 0; --i)
            {
                //just snap 
                UITimerTask timer_task = s_uiTimerTasks.Dequeue();
                if (timer_task.Enabled)
                {
                    UITimerTask.CountDown(timer_task, MinUICountDownInMillisec);
                }
                if (timer_task.RemoveFromQueue)
                {
                    timer_task.IsRegistered = false;
                    //don't enqueue back 
                }
                else
                {   //add back to the queue
                    s_uiTimerTasks.Enqueue(timer_task);
                }
            }

        }

    }
}