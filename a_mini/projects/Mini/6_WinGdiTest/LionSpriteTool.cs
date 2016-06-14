using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using PixelFarm.Agg;
using PixelFarm.Drawing.WinGdi;
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
            var spriteShape = lionFill.GetSpriteShape();
            //---------------------------------------------------------------------------------------------
            {
                int j = spriteShape.NumPaths;
                var myvxs = spriteShape.Path.Vxs;
                int[] pathList = spriteShape.PathIndexList;
                ColorRGBA[] colors = spriteShape.Colors;
                for (int i = 0; i < j; ++i)
                {
                    VxsHelper.DrawVxsSnap(g, new VertexStoreSnap(myvxs, pathList[i]), colors[i]);
                }
            }
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
