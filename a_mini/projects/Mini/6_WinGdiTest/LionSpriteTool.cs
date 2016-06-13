using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using PixelFarm.Agg;
namespace Mini.WinForms
{
    class MyLionSpriteTool : PixelToolController
    {
        PixelFarm.Agg.LionFillSprite lionFill;
        List<PixelToolController> prevPixTools;
        int _latest_mouseX, _latest_mouseY;
        public MyLionSpriteTool()
        {
            lionFill = new LionFillSprite();
        }
        public override bool IsDrawingTool { get { return true; } }
        public override void Draw(Graphics g)
        {
            //draw a lion here
            g.FillRectangle(Brushes.Blue, 0, 0, 20, 20);
        }
        internal override void SetPreviousPixelControllerObjects(List<PixelToolController> prevPixTools)
        {
            this.prevPixTools = prevPixTools;
        }
        protected override void OnMouseUp(int x, int y)
        {
            base.OnMouseUp(x, y);
        }
        protected override void OnMouseMove(int x, int y)
        {
            if (this.IsMouseDown)
            {
                _latest_mouseX = x;
                _latest_mouseY = y;
            }
            base.OnMouseMove(x, y);
        }
        protected override void OnMouseDown(int x, int y)
        {
            _latest_mouseX = x;
            _latest_mouseY = y;
            base.OnMouseDown(x, y);
        }
        internal override VertexStore GetVxs()
        {
            return null;
        }
        internal override void SetVxs(VertexStore vxs)
        {
        }
    }
}
