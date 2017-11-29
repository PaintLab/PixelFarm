//Apache2, 2014-2017, WinterDev


namespace LayoutFarm.UI
{

    public class UITimerTask
    {
        TimerTick tickAction;
        int _intervalInMillisec;
        public delegate void TimerTick();

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
        public bool IsInQueue
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
                _remaining = _intervalInMillisec = value;
            }
        }
        public bool RunOnce { get; set; }

        protected virtual void InvokeAction()
        {
            //direct invoke action
            tickAction();
        }
        public virtual void Reset()
        {
        }
        public bool IsRegistered
        {
            //TODO: review this member accessibility here
            get;
            set;
        }


        //TODO: review here
        int _remaining;
        public static void CountDown(UITimerTask timer_task, int decrement)
        {
            timer_task._remaining -= decrement;
            if (timer_task._remaining <= 0)
            {
                timer_task._remaining = timer_task._intervalInMillisec;//reset
                //invoke action
                timer_task.tickAction();

            }
        }
    }
}