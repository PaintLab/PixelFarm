//MIT 2014, WinterDev
//ArthurHub

using System;
using System.Collections.Generic;
using System.Text;
 
namespace LayoutFarm.Drawing
{

    class MyCanvasGL : MyCanvas
    {
        public MyCanvasGL(GraphicsPlatform gfxPlatform,
            int horizontalPageNum,
            int verticalPageNum,
            int x, int y, int w, int h)
            : base(gfxPlatform, horizontalPageNum, verticalPageNum,
            x, y, w, h)
        {

        }
        public override void DrawLine(float x1, float y1, float x2, float y2)
        {
            base.DrawLine(x1, y1, x2, y2);
        }
        public override void DrawLine(PointF p1, PointF p2)
        {
            base.DrawLine(p1, p2);
        }
        public override void DrawLines(Point[] points)
        {
            base.DrawLines(points);
        }

    }


}