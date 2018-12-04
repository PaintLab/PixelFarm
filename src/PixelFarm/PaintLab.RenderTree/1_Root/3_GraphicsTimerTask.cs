//Apache2, 2014-present, WinterDev

using System;
using PixelFarm.Drawing;
namespace LayoutFarm.RenderBoxes
{
    public abstract class GraphicsTimerTaskEventArgs : EventArgs
    {
        public int NeedUpdate
        {
            get;
            set;
        }
        public Rectangle GraphicUpdateArea
        {
            get;
            set;
        }
    }
    public enum TaskIntervalPlan
    {
        FastUpDate,
        Animation,
        CaretBlink
    }
    public class GraphicsTimerTask
    {
        RootGraphic _rootgfx;
        object _uniqueName;
        EventHandler<GraphicsTimerTaskEventArgs> _tickHandler;
        //
        public GraphicsTimerTask(RootGraphic rootgfx,
            TaskIntervalPlan planName,
            object uniqueName,
            int internvalMs,
            EventHandler<GraphicsTimerTaskEventArgs> tickHandler)
        {
            this.PlanName = planName;
            _uniqueName = uniqueName; 
            _rootgfx = rootgfx;
            _tickHandler = tickHandler;
        }

        public TaskIntervalPlan PlanName { get; private set; }
        public bool Enable { get; set; }
        public void RemoveSelf()
        {
            _rootgfx?.RemoveIntervalTask(_uniqueName);
        }
        public void InvokeHandler(GraphicsTimerTaskEventArgs args)
        {
            _tickHandler(this, args);
        }
    }
}