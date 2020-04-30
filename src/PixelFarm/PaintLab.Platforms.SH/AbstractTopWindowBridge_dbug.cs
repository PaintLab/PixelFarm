//Apache2, 2014-present, WinterDev

using System;
using System.Collections.Generic;
using PixelFarm.Drawing;
namespace LayoutFarm.UI
{
#if DEBUG
    partial class AbstractTopWindowBridge : IdbugOutputWindow
    {
        //internal Control dbugWinControl;
        public event EventHandler dbug_VisualRootDrawMsg;
        public event EventHandler dbug_VisualRootHitChainMsg;
        List<dbugLayoutMsg> dbugrootDocDebugMsgs = new List<dbugLayoutMsg>();
        List<dbugLayoutMsg> dbugrootDocHitChainMsgs = new List<dbugLayoutMsg>();
        //
        RenderBoxBase dbugTopwin => (RenderBoxBase)_rootgfx.TopWindowRenderBox;
        //
        public List<dbugLayoutMsg> dbug_rootDocDebugMsgs => dbugrootDocDebugMsgs;
        public List<dbugLayoutMsg> dbug_rootDocHitChainMsgs => dbugrootDocHitChainMsgs;

        //System.Drawing.Graphics dbugCreateGraphics() => dbugWinControl.CreateGraphics();
        public void dbug_HighlightMeNow(Rectangle rect)
        {
            //using (System.Drawing.Pen mpen = new System.Drawing.Pen(System.Drawing.Brushes.White, 2))
            //using (System.Drawing.Graphics g = this.dbugCreateGraphics())
            //{
            //    System.Drawing.Rectangle r = rect.ToRect();
            //    g.DrawRectangle(mpen, r);
            //    g.DrawLine(mpen, new System.Drawing.Point(r.X, r.Y), new System.Drawing.Point(r.Right, r.Bottom));
            //    g.DrawLine(mpen, new System.Drawing.Point(r.X, r.Bottom), new System.Drawing.Point(r.Right, r.Y));
            //}
        }
        public void dbug_InvokeVisualRootDrawMsg()
        {
            dbug_VisualRootDrawMsg?.Invoke(this, EventArgs.Empty);
        }
        public void dbug_InvokeHitChainMsg()
        {
            dbug_VisualRootHitChainMsg?.Invoke(this, EventArgs.Empty);
        }
        public void dbug_BeginLayoutTraceSession(string beginMsg)
        {
            this.dbugTopwin.dbugVisualRoot?.dbug_BeginLayoutTraceSession(beginMsg);
        }
        public void dbug_DisableAllDebugInfo()
        {
            this.dbugTopwin.dbugVisualRoot?.dbug_DisableAllDebugInfo();
        }
        public void dbug_EnableAllDebugInfo()
        {
            this.dbugTopwin.dbugVisualRoot?.dbug_EnableAllDebugInfo();
        }
        public void dbug_ReArrangeWithBreakOnSelectedNode()
        {
            vinv_dbugBreakOnSelectedVisuallElement = true;
            this.dbugTopwin.dbugTopDownReArrangeContentIfNeed();
        }
        public bool vinv_dbugBreakOnSelectedVisuallElement { get; set; }
    }
#endif
}