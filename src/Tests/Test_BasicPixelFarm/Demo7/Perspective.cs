////BSD, 2014-present, WinterDev
////MatterHackers

//using System;
//using PixelFarm.CpuBlit.VertexProcessing;
//using PixelFarm.Drawing;
//using Mini;


//namespace PixelFarm.CpuBlit.Sample_Perspective
//{
//    [Info(OrderCode = "04")]
//    [Info("Perspective and bilinear transformations. In general, these classes can transform an arbitrary quadrangle "
//            + " to another arbitrary quadrangle (with some restrictions). The example demonstrates how to transform "
//            + "a rectangle to a quadrangle defined by 4 vertices. You can drag the 4 corners of the quadrangle, "
//            + "as well as its boundaries. Note, that the perspective transformations don't work correctly if "
//            + "the destination quadrangle is concave. Bilinear thansformations give a different result, but "
//            + "remain valid with any shape of the destination quadrangle.")]
//    public class perspective_application : DemoBase
//    {
//        UI.PolygonEditWidget _quadPolygonControl;
//        private SpriteShape _lionShape;
//        MyTestSprite _lionFill;

//        public perspective_application()
//        {

//            _lionShape = new SpriteShape(PaintLab.Svg.VgVisualDocHelper.CreateVgVisualDocFromFile(@"Samples\lion.svg").VgRootElem);
//            _lionFill = new MyTestSprite(_lionShape);


//            _quadPolygonControl = new PixelFarm.CpuBlit.UI.PolygonEditWidget(4, 5.0);
//            _quadPolygonControl.SetXN(0, _lionShape.Bounds.Left);
//            _quadPolygonControl.SetYN(0, _lionShape.Bounds.Top);
//            _quadPolygonControl.SetXN(1, _lionShape.Bounds.Right);
//            _quadPolygonControl.SetYN(1, _lionShape.Bounds.Top);
//            _quadPolygonControl.SetXN(2, _lionShape.Bounds.Right);
//            _quadPolygonControl.SetYN(2, _lionShape.Bounds.Bottom);
//            _quadPolygonControl.SetXN(3, _lionShape.Bounds.Left);
//            _quadPolygonControl.SetYN(3, _lionShape.Bounds.Bottom);
//        }


//        public override void Init()
//        {
//            OnInitialize();
//            base.Init();
//        }

//        [DemoConfig]
//        public PerspectiveTransformType PerspectiveTransformType { get; set; }
//        public void OnInitialize()
//        {
//            double dx = Width / 2.0 - (_quadPolygonControl.GetXN(1) - _quadPolygonControl.GetXN(0)) / 2.0;
//            double dy = Height / 2.0 - (_quadPolygonControl.GetYN(0) - _quadPolygonControl.GetYN(2)) / 2.0;
//            _quadPolygonControl.AddXN(0, dx);
//            _quadPolygonControl.AddYN(0, dy);
//            _quadPolygonControl.AddXN(1, dx);
//            _quadPolygonControl.AddYN(1, dy);
//            _quadPolygonControl.AddXN(2, dx);
//            _quadPolygonControl.AddYN(2, dy);
//            _quadPolygonControl.AddXN(3, dx);
//            _quadPolygonControl.AddYN(3, dy);
//        }

//        bool _setQuadLion;
//        bool _didInit = false;


//        Color _ellipseColor = ColorEx.Make(0.5f, 0.3f, 0.0f, 0.3f);

//        public override void Draw(Painter p)
//        {
//            Painter painter = p;
//            if (!_didInit)
//            {
//                _didInit = true;
//                OnInitialize();
//            }
//            //-----------------------------------
//            painter.Clear(Drawing.Color.White);
//            //lionFill.Render(painter);

//            //IBitmapBlender backBuffer = ImageHelper.CreateChildImage(gx.DestImage, gx.GetClippingRect());
//            //ChildImage image;
//            //if (backBuffer.BitDepth == 32)
//            //{
//            //    image = new ChildImage(backBuffer, new PixelBlenderBGRA());
//            //}
//            //else
//            //{
//            //    if (backBuffer.BitDepth != 24)
//            //    {
//            //        throw new System.NotSupportedException();
//            //    }
//            //    image = new ChildImage(backBuffer, new PixelBlenderBGR());
//            //}
//            //ClipProxyImage dest = new ClipProxyImage(image);
//            //gx.Clear(ColorRGBA.White);
//            //gx.SetClippingRect(new RectInt(0, 0, Width, Height)); 
//            //ScanlineRasToDestBitmapRenderer sclineRasToBmp = gx.ScanlineRasToDestBitmap;
//            //-----------------------------------


//            RectD lionBound = _lionShape.Bounds;
//            if (!_setQuadLion)
//            {
//                _quadPolygonControl.SetXN(0, lionBound.Left);
//                _quadPolygonControl.SetYN(0, lionBound.Top);
//                _quadPolygonControl.SetXN(1, lionBound.Right);
//                _quadPolygonControl.SetYN(1, lionBound.Top);
//                _quadPolygonControl.SetXN(2, lionBound.Right);
//                _quadPolygonControl.SetYN(2, lionBound.Bottom);
//                _quadPolygonControl.SetXN(3, lionBound.Left);
//                _quadPolygonControl.SetYN(3, lionBound.Bottom);
//                _setQuadLion = true;
//            }
//            //
//            //
//            //Bilinear txBilinear = Bilinear.RectToQuad(lionBound.Left,
//            //        lionBound.Bottom,
//            //        lionBound.Right,
//            //        lionBound.Top,
//            //        quadPolygonControl.GetInnerCoords());

//            //Ellipse ell = new Ellipse((lionBound.Left + lionBound.Right) * 0.5,
//            //                        (lionBound.Bottom + lionBound.Top) * 0.5,
//            //                        (lionBound.Right - lionBound.Left) * 0.5,
//            //                        (lionBound.Top - lionBound.Bottom) * 0.5,
//            //                        200);

//            //var v1 = new VertexStore();
//            //var trans_ell = new VertexStore();
//            //txBilinear.TransformToVxs(ell.MakeVxs(v1), trans_ell);
//            ////ell.MakeVxs(v1);
//            //painter.FillColor = ColorEx.Make(0.5f, 0.3f, 0.0f, 0.3f);
//            //painter.Fill(trans_ell);

//            ////outline
//            //double prevStrokeWidth = painter.StrokeWidth;
//            //painter.StrokeWidth = 3;
//            //painter.StrokeColor = ColorEx.Make(0.0f, 0.3f, 0.2f, 1.0f);
//            //painter.Draw(trans_ell);
//            //painter.StrokeWidth = prevStrokeWidth;


//            if (this.PerspectiveTransformType == Sample_Perspective.PerspectiveTransformType.Bilinear)
//            {
//                RectD bound = lionBound;
//                //transform from original lionBounds to quadPolygon 
//                Bilinear txBilinear = Bilinear.RectToQuad(
//                    bound.Left,
//                    bound.Top,
//                    bound.Right,
//                    bound.Bottom,
//                    _quadPolygonControl.GetInnerCoords());

//                if (txBilinear.IsValid)
//                {
//                    using (Tools.BorrowVxs(out var trans_ell_vxs))
//                    using (Tools.BorrowEllipse(out var ellipse))
//                    {
//                        _lionShape.Paint(painter, txBilinear); //transform before draw
//                        //
//                        ellipse.Set((lionBound.Left + lionBound.Right) * 0.5,
//                                    (lionBound.Bottom + lionBound.Top) * 0.5,
//                                    (lionBound.Right - lionBound.Left) * 0.5,
//                                    (lionBound.Top - lionBound.Bottom) * 0.5);

//                        //ellipse=> transform coord with tx => fill to output
//                        ellipse.MakeVxs(txBilinear, trans_ell_vxs);

//                        painter.FillColor = _ellipseColor;
//                        painter.Fill(trans_ell_vxs);
//                        //-------------------------------------------------------------
//                        //outline
//                        double prevStrokeWidth = painter.StrokeWidth;
//                        painter.StrokeWidth = 3;
//                        painter.StrokeColor = ColorEx.Make(0.0f, 0.3f, 0.2f, 1.0f);
//                        painter.Draw(trans_ell_vxs);
//                        painter.StrokeWidth = prevStrokeWidth;
//                    }

//                }
//            }
//            else
//            {
//                RectD r = lionBound;

//                //var txPerspective = new Perspective(
//                //   r.Left, r.Bottom, r.Right, r.Top,
//                //    quadPolygonControl.GetInnerCoords());

//                var txPerspective = new Perspective(
//                   r.Left, r.Top, r.Right, r.Bottom,
//                    _quadPolygonControl.GetInnerCoords());

//                if (txPerspective.IsValid)
//                {

//                    //lionFill.Draw(p);
//                    //lionShape.Paint(p, txPerspective); //transform -> paint

//                    //painter.PaintSeries(txPerspective.TransformToVxs(lionShape.Vxs, v1),
//                    //  lionShape.Colors,
//                    //  lionShape.PathIndexList,
//                    //  lionShape.NumPaths);
//                    //--------------------------------------------------------------------------------------
//                    //filled Ellipse
//                    //1. create original fill ellipse
//                    //RectD lionBound = lionShape.Bounds;

//                    using (Tools.BorrowEllipse(out var filledEllipse))
//                    using (Tools.BorrowVxs(out var trans_ell_vxs))
//                    {
//                        filledEllipse.Set((lionBound.Left + lionBound.Right) * 0.5,
//                                      (lionBound.Bottom + lionBound.Top) * 0.5,
//                                      (lionBound.Right - lionBound.Left) * 0.5,
//                                      (lionBound.Top - lionBound.Bottom) * 0.5,
//                                      200);
//                        _lionShape.Paint(painter, txPerspective);

//                        //ellipse=> transform coord with tx => fill to output
//                        filledEllipse.MakeVxs(txPerspective, trans_ell_vxs);

//                        painter.FillColor = ColorEx.Make(0.5f, 0.3f, 0.0f, 0.3f);
//                        painter.Fill(trans_ell_vxs);
//                        //-------------------------------------------------------- 
//                        double prevStrokeW = painter.StrokeWidth;
//                        painter.StrokeWidth = 3;
//                        painter.StrokeColor = ColorEx.Make(0.0f, 0.3f, 0.2f, 1.0f);
//                        painter.Draw(trans_ell_vxs);
//                        painter.StrokeWidth = prevStrokeW;
//                    }

//                }
//                //}

//                ////--------------------------
//                //// Render the "quad" tool and controls
//                //painter.FillColor = ColorEx.Make(0f, 0.3f, 0.5f, 0.6f);

//                //VectorToolBox.GetFreeVxs(out var v4);
//                //painter.Fill(quadPolygonControl.MakeVxs(v4));
//                //VectorToolBox.ReleaseVxs(ref v4);
//            }
//        }
//        public override void MouseDown(int x, int y, bool isRightButton)
//        {
//            var mouseEvent = new UI.MouseEventArgs(UI.MouseButtons.Left, 1, x, y, 0);
//            _quadPolygonControl.OnMouseDown(mouseEvent);
//        }
//        public override void MouseDrag(int x, int y)
//        {
//            var mouseEvent = new UI.MouseEventArgs(UI.MouseButtons.Left, 1, x, y, 0);
//            _quadPolygonControl.OnMouseMove(mouseEvent);
//            base.MouseDrag(x, y);
//        }
//        public override void MouseUp(int x, int y)
//        {
//            var mouseEvent = new UI.MouseEventArgs(UI.MouseButtons.Left, 1, x, y, 0);
//            _quadPolygonControl.OnMouseUp(mouseEvent);
//            base.MouseUp(x, y);
//        }
//    }


//    public enum PerspectiveTransformType
//    {
//        Bilinear,
//        Perspective
//    }
//}

