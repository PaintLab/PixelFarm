using System;
using System.Collections.Generic;
using System.Drawing;
using PixelFarm.Agg;
using PixelFarm.Drawing.WinGdi;
namespace Mini.WinForms
{
    class MyLionSpriteTool : PixelToolController
    {
        PixelFarm.Agg.TestFillSprite lionFill;
        List<PixelToolController> prevPixTools;
        int _latest_mouseX, _latest_mouseY;
        bool validBoundingRect;
        RectD boundingRect = new RectD();
        int offsetX, offSetY;
        public MyLionSpriteTool()
        {
            lionFill = new TestFillSprite(new SpriteShape(SvgRenderVxLoader.CreateSvgRenderVxFromFile(@"Samples\arrow2.svg")));
        }
        public override bool IsDrawingTool { get { return true; } }
        public override void Draw(Graphics g)
        {
            //draw a lion here
            var spriteShape = lionFill.GetSpriteShape();
            //---------------------------------------------------------------------------------------------
            {
                g.TranslateTransform(offsetX, offSetY);
                throw new NotImplementedException();

                //SvgRenderVx renderVx = null;
                //int j = spriteShape.NumPaths;
                //var myvxs = spriteShape.Vxs;

                //int[] pathList = spriteShape.PathIndexList;

                //PixelFarm.Drawing.Color[] colors = spriteShape.Colors;
                //for (int i = 0; i < j; ++i)
                //{
                //    VxsHelper.FillVxsSnap(g,
                //        new PixelFarm.Drawing.VertexStoreSnap(myvxs, pathList[i]), colors[i]);
                //}


                g.TranslateTransform(-offsetX, -offSetY);
            }
        }
        public override void Offset(int dx, int dy)
        {
            offsetX += dx;
            offSetY += dy;
            //lionFill.Move(dx, dy);
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
        internal override PixelFarm.Drawing.VertexStore GetVxs()
        {
            return null;
        }
        internal override void SetVxs(PixelFarm.Drawing.VertexStore vxs)
        {
        }
        public override bool HitTest(int x, int y)
        {
            //if (!validBoundingRect)
            //{
            //    var spriteShape = lionFill.GetSpriteShape();
            //    PixelFarm.Agg.BoundingRect.GetBoundingRect(new PixelFarm.Drawing.VertexStoreSnap(spriteShape.Vxs), ref boundingRect);
            //    validBoundingRect = true;
            //}
            //if (this.boundingRect.Contains(x, y))
            //{
            //    //fine tune
            //    //hit test ***
            //    var spriteShape = lionFill.GetSpriteShape();
            //    return VertexHitTester.IsPointInVxs(spriteShape.Vxs, x, y);
            //}
            return false;
        }
    }
}
