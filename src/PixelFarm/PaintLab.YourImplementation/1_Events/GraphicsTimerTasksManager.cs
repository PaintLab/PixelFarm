//Apache2, 2014-present, WinterDev

using System;
using System.Collections.Generic;
using LayoutFarm.RenderBoxes;
namespace LayoutFarm.UI
{
    class GraphicsTimerTaskManager
    {
        readonly Dictionary<object, GraphicsTimerTask> _registeredTasks = new Dictionary<object, GraphicsTimerTask>();
        readonly List<GraphicsTimerTask> _fastIntervalTaskList = new List<GraphicsTimerTask>();
        readonly List<GraphicsTimerTask> _caretIntervalTaskList = new List<GraphicsTimerTask>();
        readonly Stack<MyIntervalTaskEventArgs> _taskEventPools = new Stack<MyIntervalTaskEventArgs>();
        readonly RootGraphic _rootgfx;

        int _fastPlanInterval = 20;//ms 
        int _caretBlinkInterval = 400;//ms 
        int _tickAccum = 0;
        bool _enableCaretBlink = true;
        readonly UITimerTask _uiTimerTask;

        public GraphicsTimerTaskManager(RootGraphic rootgfx)
        {
            _rootgfx = rootgfx;

            //register timer task
            _uiTimerTask = new UITimerTask(graphicTimer1_Tick);
            _uiTimerTask.Interval = _fastPlanInterval; //fast task plan
            UIPlatform.RegisterTimerTask(_uiTimerTask);
            _uiTimerTask.Enabled = true;
        }
        public bool Enabled
        {
            get => _uiTimerTask.Enabled;
            set => _uiTimerTask.Enabled = value;
        }
        public void CloseAllWorkers()
        {
            _uiTimerTask.Enabled = false;
        }
        public void StartCaretBlinkTask()
        {
            _enableCaretBlink = true;
        }
        public void StopCaretBlinkTask()
        {
            _enableCaretBlink = false;
        }

        public GraphicsTimerTask SubscribeGraphicsTimerTask(
            object uniqueName,
            TaskIntervalPlan planName,
            int intervalMs,
            EventHandler<GraphicsTimerTaskEventArgs> tickhandler)
        {
            if (!_registeredTasks.TryGetValue(uniqueName, out GraphicsTimerTask existingTask))
            {
                existingTask = new GraphicsTimerTask(_rootgfx, planName, uniqueName, intervalMs, tickhandler);
                _registeredTasks.Add(uniqueName, existingTask);
                switch (planName)
                {
                    case TaskIntervalPlan.CaretBlink:
                        {
                            _caretIntervalTaskList.Add(existingTask);
                        }
                        break;
                    default:
                        {
                            _fastIntervalTaskList.Add(existingTask);
                        }
                        break;
                }
            }
            return existingTask;
        }
        public void UnsubscribeTimerTask(object uniqueName)
        {
            if (_registeredTasks.TryGetValue(uniqueName, out GraphicsTimerTask found))
            {
                _registeredTasks.Remove(uniqueName);
                switch (found.PlanName)
                {
                    case TaskIntervalPlan.CaretBlink:
                        {
                            _caretIntervalTaskList.Remove(found);
                        }
                        break;
                    default:
                        {
                            _fastIntervalTaskList.Remove(found);
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
            _tickAccum += _fastPlanInterval;
            //Console.WriteLine("tickaccum:" + tickAccum.ToString());
            //-------------------------------------------------
            bool doCaretPlan = false;
            if (_tickAccum > _caretBlinkInterval)
            {
                // Console.WriteLine("*********");
                _tickAccum = 0;//reset
                doCaretPlan = true;
            }
            //-------------------------------------------------
            int needUpdate = 0;
            if (_enableCaretBlink && doCaretPlan)
            {
                //-------------------------------------------------
                //1. fast and animation plan
                //------------------------------------------------- 
                MyIntervalTaskEventArgs args = GetTaskEventArgs();
                int j = _fastIntervalTaskList.Count;
                if (j > 0)
                {
                    for (int i = 0; i < j; ++i)
                    {
                        _fastIntervalTaskList[i].InvokeHandler(args);
                        needUpdate |= args.NeedUpdate;
                    }
                }
                //-------------------------------------------------
                //2. caret plan  
                //------------------------------------------------- 
                j = _caretIntervalTaskList.Count;
                for (int i = 0; i < j; ++i)
                {
                    _caretIntervalTaskList[i].InvokeHandler(args);
                    needUpdate |= args.NeedUpdate;
                }
                FreeTaskEventArgs(args);
            }
            else
            {
                int j = _fastIntervalTaskList.Count;

                if (j > 0)
                {
                    MyIntervalTaskEventArgs args = GetTaskEventArgs();
                    for (int i = 0; i < j; ++i)
                    {
                        _fastIntervalTaskList[i].InvokeHandler(args);
                        needUpdate |= args.NeedUpdate;
                    }
                    FreeTaskEventArgs(args);
                }
            }

            //remainnig tasks
            if (needUpdate > 0)
            {
                _rootgfx.PrepareRender();
                _rootgfx.FlushAccumGraphics();
            }
        }

        MyIntervalTaskEventArgs GetTaskEventArgs()
        {
            if (_taskEventPools.Count > 0)
            {
                return _taskEventPools.Pop();
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
            _taskEventPools.Push(args);
        }
    }
}