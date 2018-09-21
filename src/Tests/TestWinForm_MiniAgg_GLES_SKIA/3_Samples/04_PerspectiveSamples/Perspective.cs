//BSD, 2014-present, WinterDev
//MatterHackers

using System;
using PixelFarm.CpuBlit.VertexProcessing;
using PixelFarm.CpuBlit.Imaging;
using Mini;
using PixelFarm.Drawing;
using PaintLab.Svg;

namespace PixelFarm.CpuBlit.Sample_Perspective
{
    [Info(OrderCode = "04")]
    [Info("Perspective and bilinear transformations. In general, these classes can transform an arbitrary quadrangle "
            + " to another arbitrary quadrangle (with some restrictions). The example demonstrates how to transform "
            + "a rectangle to a quadrangle defined by 4 vertices. You can drag the 4 corners of the quadrangle, "
            + "as well as its boundaries. Note, that the perspective transformations don't work correctly if "
            + "the destination quadrangle is concave. Bilinear thansformations give a different result, but "
            + "remain valid with any shape of the destination quadrangle.")]
    public class perspective_application : DemoBase
    {
        UI.PolygonEditWidget quadPolygonControl;
        private SpriteShape lionShape;
        MyTestSprite lionFill;

        public perspective_application()
        {

            lionShape = new SpriteShape(SvgRenderVxLoader.CreateSvgRenderVxFromFile(@"Samples\lion.svg"));
            lionFill = new MyTestSprite(lionShape);


            quadPolygonControl = new PixelFarm.CpuBlit.UI.PolygonEditWidget(4, 5.0);
            quadPolygonControl.SetXN(0, lionShape.Bounds.Left);
            quadPolygonControl.SetYN(0, lionShape.Bounds.Top);
            quadPolygonControl.SetXN(1, lionShape.Bounds.Right);
            quadPolygonControl.SetYN(1, lionShape.Bounds.Top);
            quadPolygonControl.SetXN(2, lionShape.Bounds.Right);
            quadPolygonControl.SetYN(2, lionShape.Bounds.Bottom);
            quadPolygonControl.SetXN(3, lionShape.Bounds.Left);
            quadPolygonControl.SetYN(3, lionShape.Bounds.Bottom);
        }


        public override void Init()
        {
            OnInitialize();
            base.Init();
        }

        [DemoConfig]
        public PerspectiveTransformType PerspectiveTransformType
        {
            get;
            set;
        }
        public void OnInitialize()
        {
            double dx = Width / 2.0 - (quadPolygonControl.GetXN(1) - quadPolygonControl.GetXN(0)) / 2.0;
            double dy = Height / 2.0 - (quadPolygonControl.GetYN(0) - quadPolygonControl.GetYN(2)) / 2.0;
            quadPolygonControl.AddXN(0, dx);
            quadPolygonControl.AddYN(0, dy);
            quadPolygonControl.AddXN(1, dx);
            quadPolygonControl.AddYN(1, dy);
            quadPolygonControl.AddXN(2, dx);
            quadPolygonControl.AddYN(2, dy);
            quadPolygonControl.AddXN(3, dx);
            quadPolygonControl.AddYN(3, dy);
        }
        bool setQuadLion;

        bool didInit = false;
        public override void Draw(Painter p)
        {
            Painter painter = p;
            if (!didInit)
            {
                didInit = true;
                OnInitialize();
            }
            //-----------------------------------
            painter.Clear(Drawing.Color.White);
            //lionFill.Render(painter);

            //IBitmapBlender backBuffer = ImageHelper.CreateChildImage(gx.DestImage, gx.GetClippingRect());
            //ChildImage image;
            //if (backBuffer.BitDepth == 32)
            //{
            //    image = new ChildImage(backBuffer, new PixelBlenderBGRA());
            //}
            //else
            //{
            //    if (backBuffer.BitDepth != 24)
            //    {
            //        throw new System.NotSupportedException();
            //    }
            //    image = new ChildImage(backBuffer, new PixelBlenderBGR());
            //}
            //ClipProxyImage dest = new ClipProxyImage(image);
            //gx.Clear(ColorRGBA.White);
            //gx.SetClippingRect(new RectInt(0, 0, Width, Height)); 
            //ScanlineRasToDestBitmapRenderer sclineRasToBmp = gx.ScanlineRasToDestBitmap;
            //-----------------------------------

            RectD lionBound = new RectD(0, 2, 238, 379);
            if (!setQuadLion)
            {

                quadPolygonControl.SetXN(0, lionShape.Bounds.Left);
                quadPolygonControl.SetYN(0, lionShape.Bounds.Top);
                quadPolygonControl.SetXN(1, lionShape.Bounds.Right);
                quadPolygonControl.SetYN(1, lionShape.Bounds.Top);
                quadPolygonControl.SetXN(2, lionShape.Bounds.Right);
                quadPolygonControl.SetYN(2, lionShape.Bounds.Bottom);
                quadPolygonControl.SetXN(3, lionShape.Bounds.Left);
                quadPolygonControl.SetYN(3, lionShape.Bounds.Bottom);
                setQuadLion = true;
            }
            //Bilinear txBilinear = Bilinear.RectToQuad(lionBound.Left,
            //        lionBound.Bottom,
            //        lionBound.Right,
            //        lionBound.Top,
            //        quadPolygonControl.GetInnerCoords());

            //Ellipse ell = new Ellipse((lionBound.Left + lionBound.Right) * 0.5,
            //                        (lionBound.Bottom + lionBound.Top) * 0.5,
            //                        (lionBound.Right - lionBound.Left) * 0.5,
            //                        (lionBound.Top - lionBound.Bottom) * 0.5,
            //                        200);

            //var v1 = new VertexStore();
            //var trans_ell = new VertexStore();
            //txBilinear.TransformToVxs(ell.MakeVxs(v1), trans_ell);
            ////ell.MakeVxs(v1);
            //painter.FillColor = ColorEx.Make(0.5f, 0.3f, 0.0f, 0.3f);
            //painter.Fill(trans_ell);

            ////outline
            //double prevStrokeWidth = painter.StrokeWidth;
            //painter.StrokeWidth = 3;
            //painter.StrokeColor = ColorEx.Make(0.0f, 0.3f, 0.2f, 1.0f);
            //painter.Draw(trans_ell);
            //painter.StrokeWidth = prevStrokeWidth;


            if (this.PerspectiveTransformType == Sample_Perspective.PerspectiveTransformType.Bilinear)
            {
                RectD bound = lionBound;
                Bilinear txBilinear = Bilinear.RectToQuad(
                    bound.Left,
                    bound.Top,
                    bound.Right,
                    bound.Bottom,
                    quadPolygonControl.GetInnerCoords());
                if (txBilinear.IsValid)
                {


                    using (VxsTemp.Borrow(out var v3, out var v1, out var trans_ell))
                    {
                        //lionShape.ApplyTransform(txBilinear);
                        //lionShape.Paint(painter);
                        //lionFill.Draw(p);
                        //RectD lionBound = lionShape.Bounds;

                        lionShape.Paint(painter, txBilinear);

                        Ellipse ell = new Ellipse((lionBound.Left + lionBound.Right) * 0.5,
                                         (lionBound.Bottom + lionBound.Top) * 0.5,
                                         (lionBound.Right - lionBound.Left) * 0.5,
                                         (lionBound.Top - lionBound.Bottom) * 0.5,
                                         200);


                        txBilinear.TransformToVxs(ell.MakeVxs(v1), trans_ell);
                        painter.FillColor = ColorEx.Make(0.5f, 0.3f, 0.0f, 0.3f);
                        painter.Fill(trans_ell);
                        //-------------------------------------------------------------
                        //outline
                        double prevStrokeWidth = painter.StrokeWidth;
                        painter.StrokeWidth = 3;
                        painter.StrokeColor = ColorEx.Make(0.0f, 0.3f, 0.2f, 1.0f);
                        painter.Draw(trans_ell);
                        painter.StrokeWidth = prevStrokeWidth;
                    }

                }
            }
            else
            {
                RectD r = lionBound;

                //var txPerspective = new Perspective(
                //   r.Left, r.Bottom, r.Right, r.Top,
                //    quadPolygonControl.GetInnerCoords());

                var txPerspective = new Perspective(
                   r.Left, r.Top, r.Right, r.Bottom,
                    quadPolygonControl.GetInnerCoords());
                if (txPerspective.IsValid)
                {

                    //lionFill.Draw(p);
                    //lionShape.Paint(p, txPerspective); //transform -> paint

                    //painter.PaintSeries(txPerspective.TransformToVxs(lionShape.Vxs, v1),
                    //  lionShape.Colors,
                    //  lionShape.PathIndexList,
                    //  lionShape.NumPaths);
                    //--------------------------------------------------------------------------------------
                    //filled Ellipse
                    //1. create original fill ellipse
                    //RectD lionBound = lionShape.Bounds;
                    var filledEllipse = new Ellipse((lionBound.Left + lionBound.Right) * 0.5,
                                      (lionBound.Bottom + lionBound.Top) * 0.5,
                                      (lionBound.Right - lionBound.Left) * 0.5,
                                      (lionBound.Top - lionBound.Bottom) * 0.5,
                                      200);

                    using (VxsTemp.Borrow(out var v2, out var transformedEll))
                    {
                        //lionShape.ApplyTransform(txPerspective);

                        lionShape.Paint(painter, txPerspective);

                        txPerspective.TransformToVxs(filledEllipse.MakeVxs(v2), transformedEll);
                        painter.FillColor = ColorEx.Make(0.5f, 0.3f, 0.0f, 0.3f);
                        painter.Fill(transformedEll);
                        //-------------------------------------------------------- 
                        double prevStrokeW = painter.StrokeWidth;
                        painter.StrokeWidth = 3;
                        painter.StrokeColor = ColorEx.Make(0.0f, 0.3f, 0.2f, 1.0f);
                        painter.Draw(transformedEll);
                        painter.StrokeWidth = prevStrokeW;
                    }

                }
                //}

                ////--------------------------
                //// Render the "quad" tool and controls
                //painter.FillColor = ColorEx.Make(0f, 0.3f, 0.5f, 0.6f);

                //VectorToolBox.GetFreeVxs(out var v4);
                //painter.Fill(quadPolygonControl.MakeVxs(v4));
                //VectorToolBox.ReleaseVxs(ref v4);
            }
        }
        public override void MouseDown(int x, int y, bool isRightButton)
        {
            var mouseEvent = new UI.MouseEventArgs(UI.MouseButtons.Left, 1, x, y, 0);
            quadPolygonControl.OnMouseDown(mouseEvent);
        }
        public override void MouseDrag(int x, int y)
        {
            var mouseEvent = new UI.MouseEventArgs(UI.MouseButtons.Left, 1, x, y, 0);
            quadPolygonControl.OnMouseMove(mouseEvent);
            base.MouseDrag(x, y);
        }
        public override void MouseUp(int x, int y)
        {
            var mouseEvent = new UI.MouseEventArgs(UI.MouseButtons.Left, 1, x, y, 0);
            quadPolygonControl.OnMouseUp(mouseEvent);
            base.MouseUp(x, y);
        }
    }


    public enum PerspectiveTransformType
    {
        Bilinear,
        Perspective
    }
}

