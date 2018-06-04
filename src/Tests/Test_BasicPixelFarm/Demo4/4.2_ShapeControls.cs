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
    class DemoShapeControl : DemoBase
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
            svgRenderVx.DisableBackingImage = true;


            var uiSprite = new UISprite(10, 10); //init size = (10,10), location=(0,0) 
            uiSprite.LoadSvg(svgRenderVx);
            viewport.AddContent(uiSprite); 

            var spriteEvListener = new GeneralEventListener();
            uiSprite.AttachExternalEventListener(spriteEvListener);



            //box1 = new LayoutFarm.CustomWidgets.SimpleBox(50, 50);
            //box1.BackColor = Color.Red;
            //box1.SetLocation(10, 10);
            ////box1.dbugTag = 1;
            //SetupActiveBoxProperties(box1);
            //viewport.AddContent(box1);
            //-------- 
            rectBoxController.Init();
            //polygonController.Visible = false;
            viewport.AddContent(polygonController);
            //-------------------------------------------
            viewport.AddContent(rectBoxController);

            //foreach (var ui in rectBoxController.GetControllerIter())
            //{
            //    viewport.AddContent(ui);
            //}

            spriteEvListener.MouseDown += e1 =>
            {
                //mousedown on ui sprite
                polygonController.SetPosition((int)uiSprite.Left, (int)uiSprite.Top);
                polygonController.SetTargetUISprite(uiSprite);
                polygonController.UpdateControlPoints(svgPart);

            };
            spriteEvListener.MouseMove += e1 =>
            {
                if (e1.IsDragging)
                {
                    //drag event on uisprite

                    int left = (int)uiSprite.Left;
                    int top = (int)uiSprite.Top;

                    int new_left = left + e1.DiffCapturedX;
                    int new_top = top + e1.DiffCapturedY;
                    uiSprite.SetLocation(new_left, new_top);
                    //-----
                    //also update controller position
                    polygonController.SetPosition(new_left, new_top);
                }
            };

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