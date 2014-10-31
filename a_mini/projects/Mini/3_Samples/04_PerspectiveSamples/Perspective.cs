//2014 BSD,WinterDev
//MatterHackers

using System;

using MatterHackers.Agg.Transform;
using MatterHackers.Agg.Image;
using MatterHackers.Agg.VertexSource;

using MatterHackers.VectorMath;

using Mini;
namespace MatterHackers.Agg.Sample_Perspective
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
        MatterHackers.Agg.ScanlineRasterizer g_rasterizer = new ScanlineRasterizer();
        ScanlinePacked8 g_scanline = new ScanlinePacked8();

        UI.PolygonEditWidget quadPolygonControl;
        private LionShape lionShape;

        public perspective_application()
        {
            lionShape = new LionShape();
            lionShape.ParseLion();

            quadPolygonControl = new MatterHackers.Agg.UI.PolygonEditWidget(4, 5.0);
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
        bool didInit = false;
        public override void Draw(Graphics2D g)
        {
            OnDraw(g);
        }
        public void OnDraw(Graphics2D graphics2D)
        {
            var widgetsSubImage = ImageHelper.CreateChildImage(graphics2D.DestImage, graphics2D.GetClippingRectInt());

            IImage backBuffer = widgetsSubImage;

            if (!didInit)
            {
                didInit = true;
                OnInitialize();
            }
            ChildImage image;
            if (backBuffer.BitDepth == 32)
            {
                image = new ChildImage(backBuffer, new BlenderBGRA());
            }
            else
            {
                if (backBuffer.BitDepth != 24)
                {
                    throw new System.NotSupportedException();
                }
                image = new ChildImage(backBuffer, new BlenderBGR());
            }

            ClipProxyImage dest = new ClipProxyImage(image);
            dest.Clear(ColorRGBA.White);
            g_rasterizer.SetVectorClipBox(0, 0, Width, Height);

            ScanlineRasToDestBitmapRenderer sclineRasToBmp = new ScanlineRasToDestBitmapRenderer();

            if (this.PerspectiveTransformType == Sample_Perspective.PerspectiveTransformType.Bilinear)
            {

                var bound = lionShape.Bounds;
                var txBilinear = Bilinear.RectToQuad(bound.Left,
                    bound.Bottom,
                    bound.Right,
                    bound.Top,
                    quadPolygonControl.polygon());

                if (txBilinear.IsValid)
                {
                    //--------------------------
                    // Render transformed lion
                    // 

                    sclineRasToBmp.RenderSolidAllPaths(dest,
                        g_rasterizer,
                        g_scanline,
                        txBilinear.TransformToVxs(lionShape.Path),
                        lionShape.Colors,
                        lionShape.PathIndexList,
                        lionShape.NumPaths);

                    //--------------------------
                    // Render transformed ellipse
                    //
                    RectangleDouble lionBound = lionShape.Bounds;

                    Ellipse ell = new Ellipse((lionBound.Left + lionBound.Right) * 0.5,
                                     (lionBound.Bottom + lionBound.Top) * 0.5,
                                     (lionBound.Right - lionBound.Left) * 0.5,
                                     (lionBound.Top - lionBound.Bottom) * 0.5,
                                     200);

                    VertexStore vxs = ell.MakeVxs();
                    VertexStoreSnap trans_ell = txBilinear.TransformToVertexSnap(vxs);
                    VertexStoreSnap trans_ell_stroke = txBilinear.TransformToVertexSnap(new Stroke(3).MakeVxs(vxs));

                    g_rasterizer.AddPath(trans_ell);
                    sclineRasToBmp.RenderScanlineSolidAA(dest, g_rasterizer, g_scanline, ColorRGBA.Make(0.5f, 0.3f, 0.0f, 0.3f));

                    g_rasterizer.AddPath(trans_ell_stroke);
                    sclineRasToBmp.RenderScanlineSolidAA(dest, g_rasterizer, g_scanline, ColorRGBA.Make(0.0f, 0.3f, 0.2f, 1.0f));
                }
            }
            else
            {
                var txPerspective = new Perspective(
                    lionShape.Bounds,
                    quadPolygonControl.polygon());

                if (txPerspective.is_valid())
                {


                    sclineRasToBmp.RenderSolidAllPaths(dest,
                        g_rasterizer,
                        g_scanline,
                        txPerspective.TransformToVxs(lionShape.Path),
                        lionShape.Colors,
                        lionShape.PathIndexList,
                        lionShape.NumPaths);

                    //--------------------------------------------------------------------------------------
                    //filled Ellipse
                    //1. create original fill ellipse
                    RectangleDouble lionBound = lionShape.Bounds;
                    var filledEllipse = new Ellipse((lionBound.Left + lionBound.Right) * 0.5,
                                      (lionBound.Bottom + lionBound.Top) * 0.5,
                                      (lionBound.Right - lionBound.Left) * 0.5,
                                      (lionBound.Top - lionBound.Bottom) * 0.5,
                                      200);

                    var ellipseVertext = txPerspective.TransformToVxs(filledEllipse.MakeVxs());
                    //2. create transform version of fill ellipse
                    //var txFillEllipse = new VertexSourceApplyTransform(filledEllipse, txPerspective);
                    //3. add
                    //g_rasterizer.AddPath(txFillEllipse);
                    g_rasterizer.AddPath(new VertexStoreSnap(ellipseVertext));
                    //4. render it

                    sclineRasToBmp.RenderScanlineSolidAA(dest,
                        g_rasterizer,
                        g_scanline,
                        ColorRGBA.Make(0.5f, 0.3f, 0.0f, 0.3f));

                    //--------------------------------------------------------
                    //outline Ellipse
                    //1. create original version of stroke ellipse 
                    var vxs = filledEllipse.MakeVxs();
                    //2. create transform version of outlin  
                    var txOutline = txPerspective.TransformToVxs(new Stroke(3).MakeVxs(vxs));// new VertexSourceApplyTransform(strokeEllipse, txPerspective);
                    //3. add
                    g_rasterizer.AddPath(txOutline);
                    //4. render                      
                    sclineRasToBmp.RenderScanlineSolidAA(dest,
                        g_rasterizer,
                        g_scanline,
                        ColorRGBA.Make(0.0f, 0.3f, 0.2f, 1.0f));
                }
            }

            //--------------------------
            // Render the "quad" tool and controls
            var vxs2 = quadPolygonControl.MakeVxs();
            g_rasterizer.AddPath(vxs2);
            //g_rasterizer.AddPath(quadPolygonControl);
            sclineRasToBmp.RenderScanlineSolidAA(dest, g_rasterizer, g_scanline, ColorRGBA.Make(0f, 0.3f, 0.5f, 0.6f));


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
        //public override void OnMouseDown(MouseEventArgs mouseEvent)
        //{
        //    if (mouseEvent.Button == MouseButtons.Left)
        //    {
        //        quadPolygonControl.OnMouseDown(mouseEvent);
        //        if (MouseCaptured)
        //        {
        //            Invalidate();
        //        }
        //    }

        //    base.OnMouseDown(mouseEvent);
        //}

        //public override void OnMouseMove(MouseEventArgs mouseEvent)
        //{
        //    if (mouseEvent.Button == MouseButtons.Left)
        //    {
        //        quadPolygonControl.OnMouseMove(mouseEvent);
        //        if (MouseCaptured)
        //        {
        //            Invalidate();
        //        }
        //    }

        //    base.OnMouseMove(mouseEvent);
        //}

        //public override void OnMouseUp(MouseEventArgs mouseEvent)
        //{
        //    quadPolygonControl.OnMouseUp(mouseEvent);
        //    if (MouseCaptured)
        //    {
        //        Invalidate();
        //    }

        //    base.OnMouseUp(mouseEvent);
        //}


    }


    public enum PerspectiveTransformType
    {
        Bilinear,
        Perspective
    }
}

