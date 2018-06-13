//Apache2, 2014-present, WinterDev

using System;
using System.Collections.Generic;
using LayoutFarm.RenderBoxes;
namespace LayoutFarm.UI
{
    class GraphicsTimerTaskManager
    {
        Dictionary<object, GraphicsTimerTask> registeredTasks = new Dictionary<object, GraphicsTimerTask>();
        List<GraphicsTimerTask> fastIntervalTaskList = new List<GraphicsTimerTask>();
        List<GraphicsTimerTask> caretIntervalTaskList = new List<GraphicsTimerTask>();
        RootGraphic rootgfx;

        int fastPlanInterval = 20;//ms 
        int caretBlinkInterval = 400;//ms (2 fps)
        int tickAccum = 0;
        bool enableCaretBlink = true;
        UITimerTask uiTimerTask;
        public GraphicsTimerTaskManager(RootGraphic rootgfx)
        {
            this.rootgfx = rootgfx;

            //register timer task
            uiTimerTask = new UITimerTask(graphicTimer1_Tick);
            uiTimerTask.IntervalInMillisec = fastPlanInterval; //fast task plan
            UIPlatform.RegisterTimerTask(uiTimerTask);
            uiTimerTask.Enabled = true;
        }
        public bool Enabled
        {
            get { return this.uiTimerTask.Enabled; }
            set { this.uiTimerTask.Enabled = value; }
        }
        public void CloseAllWorkers()
        {
            this.uiTimerTask.Enabled = false;
        }
        public void StartCaretBlinkTask()
        {
            enableCaretBlink = true;
        }
        public void StopCaretBlinkTask()
        {
            enableCaretBlink = false;
        }

        public GraphicsTimerTask SubscribeGraphicsTimerTask(
            object uniqueName,
            TaskIntervalPlan planName,
            int intervalMs,
            EventHandler<GraphicsTimerTaskEventArgs> tickhandler)
        {
            GraphicsTimerTask existingTask;
            if (!registeredTasks.TryGetValue(uniqueName, out existingTask))
            {
                existingTask = new GraphicsTimerTask(this.rootgfx, planName, uniqueName, intervalMs, tickhandler);
                registeredTasks.Add(uniqueName, existingTask);
                switch (planName)
                {
                    case TaskIntervalPlan.CaretBlink:
                        {
                            caretIntervalTaskList.Add(existingTask);
                        }
                        break;
                    default:
                        {
                            fastIntervalTaskList.Add(existingTask);
                        }
                        break;
                }
            }
            return existingTask;
        }
        public void UnsubscribeTimerTask(object uniqueName)
        {
            GraphicsTimerTask found;
            if (registeredTasks.TryGetValue(uniqueName, out found))
            {
                registeredTasks.Remove(uniqueName);
                switch (found.PlanName)
                {
                    case TaskIntervalPlan.CaretBlink:
                        {
                            caretIntervalTaskList.Remove(found);
                        }
                        break;
                    default:
                        {
                            fastIntervalTaskList.Remove(found);
                        }
                        break;
                }
            }
        }


#if DEBUG
        static int dbugCount = 0;
#endif


        void graphicTimer1_Tick(UITimerTask timerTask)
        {
            //-------------------------------------------------
            tickAccum += fastPlanInterval;
            //Console.WriteLine("tickaccum:" + tickAccum.ToString());
            //-------------------------------------------------
            bool doCaretPlan = false;
            if (tickAccum > caretBlinkInterval)
            {
                // Console.WriteLine("*********");
                tickAccum = 0;//reset
                doCaretPlan = true;
            }
            //-------------------------------------------------
            int needUpdate = 0;
            if (enableCaretBlink && doCaretPlan)
            {
                //-------------------------------------------------
                //1. fast and animation plan
                //------------------------------------------------- 
                MyIntervalTaskEventArgs args = GetTaskEventArgs();
                int j = this.fastIntervalTaskList.Count;
                if (j > 0)
                {
                    for (int i = 0; i < j; ++i)
                    {
                        fastIntervalTaskList[i].InvokeHandler(args);
                        needUpdate |= args.NeedUpdate;
                    }
                }
                //-------------------------------------------------
                //2. caret plan  
                //------------------------------------------------- 
                j = this.caretIntervalTaskList.Count;
                for (int i = 0; i < j; ++i)
                {
                    caretIntervalTaskList[i].InvokeHandler(args);
                    needUpdate |= args.NeedUpdate;
                }
                FreeTaskEventArgs(args);
            }
            else
            {
                int j = this.fastIntervalTaskList.Count;
                MyIntervalTaskEventArgs args = GetTaskEventArgs();
                if (j > 0)
                {
                    for (int i = 0; i < j; ++i)
                    {
                        fastIntervalTaskList[i].InvokeHandler(args);
                        needUpdate |= args.NeedUpdate;
                    }
                }
                FreeTaskEventArgs(args);
            }


            if (needUpdate > 0)
            {
                this.rootgfx.PrepareRender();
                this.rootgfx.FlushAccumGraphics();
            }
        }
        Stack<MyIntervalTaskEventArgs> taskEventPools = new Stack<MyIntervalTaskEventArgs>();
        MyIntervalTaskEventArgs GetTaskEventArgs()
        {
            if (taskEventPools.Count > 0)
            {
                return taskEventPools.Pop();
            }
            else
            {
                return new MyIntervalTaskEventArgs();
            }
        }
        void FreeTaskEventArgs(MyIntervalTaskEventArgs args)
        {
            //clear for reues
            args.ClearForReuse();
            taskEventPools.Push(args);
        }
    }
}