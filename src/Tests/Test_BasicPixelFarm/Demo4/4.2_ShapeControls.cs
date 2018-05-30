//MIT, 2014-2018, WinterDev

using System.Collections.Generic;

using PixelFarm.Drawing;
using PaintLab.Svg;
using LayoutFarm.UI;
using PaintLab;
using PixelFarm.Agg;

namespace LayoutFarm
{
    [DemoNote("4.2 ShapeControls")]
    class DemoShaprControl : DemoBase
    {
        LayoutFarm.CustomWidgets.PolygonController polygonController = new CustomWidgets.PolygonController();
        LayoutFarm.CustomWidgets.RectBoxController rectBoxController = new CustomWidgets.RectBoxController();
        LayoutFarm.CustomWidgets.SimpleBox box1;


        protected override void OnStartDemo(SampleViewport viewport)
        {
           

            SvgPart svgPart = new SvgPart(SvgRenderVxKind.Path);
            VertexStore vxs = new VertexStore();
            vxs.AddMoveTo(100, 20);
            vxs.AddLineTo(150, 50);
            vxs.AddLineTo(110, 80);
            vxs.AddCloseFigure();
            //-------------------------------------------
            svgPart.SetVxsAsOriginal(vxs);
            svgPart.FillColor = Color.Red;
            SvgRenderVx svgRenderVx = new SvgRenderVx(new SvgPart[] { svgPart });

            var uiSprite = new UISprite(200, 200);

            uiSprite.LoadSvg(svgRenderVx);
            viewport.AddContent(uiSprite);


            box1 = new LayoutFarm.CustomWidgets.SimpleBox(50, 50);
            box1.BackColor = Color.Red;
            box1.SetLocation(10, 10);
            //box1.dbugTag = 1;
            SetupActiveBoxProperties(box1);
            viewport.AddContent(box1);
            //-------- 
            rectBoxController.Init();
            //------------

            //


            ////-------------------------------------------
            foreach (var ui in rectBoxController.GetControllerIter())
            {
                viewport.AddContent(ui);
            }


            //List<PointF> ctrlPoints = new List<PointF>();
            //ctrlPoints.Add(new PointF(10, 20));
            //ctrlPoints.Add(new PointF(50, 50));
            //ctrlPoints.Add(new PointF(10, 80));
            //polygonController.UpdateControlPoints(ctrlPoints);

            //foreach (var ui in polygonController.GetControllerIter())
            //{
            //    viewport.AddContent(ui);
            //}

        }
        void SetupActiveBoxProperties(LayoutFarm.CustomWidgets.EaseBox box)
        {
            //1. mouse down         
            box.MouseDown += (s, e) =>
            {
                box.BackColor = KnownColors.FromKnownColor(KnownColor.DeepSkyBlue);
                e.MouseCursorStyle = MouseCursorStyle.Pointer;
                //--------------------------------------------
                e.SetMouseCapture(rectBoxController.ControllerBoxMain);
                rectBoxController.UpdateControllerBoxes(box);

            };
            //2. mouse up
            box.MouseUp += (s, e) =>
            {
                e.MouseCursorStyle = MouseCursorStyle.Default;
                box.BackColor = Color.LightGray;
                //controllerBox1.Visible = false;
                //controllerBox1.TargetBox = null;
            };
        }
    }
}