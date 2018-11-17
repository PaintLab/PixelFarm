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
        bool _enable;
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
            this._uniqueName = uniqueName;
            this._enable = false;
            this._rootgfx = rootgfx;
            this._tickHandler = tickHandler;
        }

        public TaskIntervalPlan PlanName { get; private set; }
        public bool Enable
        {
            get
            {
                return this._enable;
            }
            set
            {
                this._enable = value;
            }
        }
        public void RemoveSelf()
        {
            if (this._rootgfx != null)
            {
                this._rootgfx.RemoveIntervalTask(this._uniqueName);
            }
        }
        public void InvokeHandler(GraphicsTimerTaskEventArgs args)
        {
            this._tickHandler(this, args);
        }
    }
}