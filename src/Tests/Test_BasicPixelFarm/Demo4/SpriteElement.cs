//Apache2, 2014-2018, WinterDev

using System;
using PixelFarm.Drawing;
using PixelFarm.Agg;

namespace LayoutFarm.UI
{
   


    class BackBoardRenderElement : LayoutFarm.CustomWidgets.CustomRenderBox
    {

        DrawBoard _canvas;
        public BackBoardRenderElement(RootGraphic rootgfx, int width, int height)
           : base(rootgfx, width, height)
        {

        }
        protected override void DrawBoxContent(DrawBoard canvas, Rectangle updateArea)
        {
            _canvas = canvas;
            base.DrawBoxContent(canvas, updateArea);

        }
    }
    public class BackDrawBoardUI : LayoutFarm.CustomWidgets.Box
    {
        BackBoardRenderElement _backboardRenderE;
        public BackDrawBoardUI(int w, int h)
            : base(w, h)
        {

        }
        public override void Walk(UIVisitor visitor)
        {

        }
        public override RenderElement GetPrimaryRenderElement(RootGraphic rootgfx)
        {
            if (_backboardRenderE != null)
            {
                return _backboardRenderE;
            }
            _backboardRenderE = new BackBoardRenderElement(rootgfx, this.Width, this.Height);
            _backboardRenderE.NeedClipArea = true;

            SetPrimaryRenderElement(_backboardRenderE);
            BuildChildrenRenderElement(_backboardRenderE);

            return _backboardRenderE;
        }
        public void CopyImageBuffer(DrawBoard canvas, int x, int y, int w, int h)
        {
            //copy content image to specific img buffer

        }
    }

}