//MIT, 2017, WinterDev
using System.Collections.Generic;
namespace LayoutFarm.EventQueueSystem
{ 
    public delegate void EventQueueDelegate();
    public static class CentralEventQueue
    {
        static List<EventQueueDelegate> s_queueDelegates = new List<EventQueueDelegate>();
        public static void RegisterEventQueue(EventQueueDelegate eventQueue)
        {
            s_queueDelegates.Add(eventQueue);
        }
        public static void InvokeEventQueue()
        {
            int j = s_queueDelegates.Count;
            for (int i = 0; i < j; ++i)
            {
                s_queueDelegates[i]();
            }
        }
    }

}