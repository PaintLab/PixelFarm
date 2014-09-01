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
        //void NeedsRedraw(object sender, EventArgs e)
        //{
        //    Invalidate();
        //}
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
            var widgetsSubImage = ImageHelper.CreateChildImage(graphics2D.DestImage, graphics2D.GetClippingRect());

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
            ClipProxyImage clippingProxy = new ClipProxyImage(image);
            clippingProxy.Clear(ColorRGBA.White);

            g_rasterizer.SetVectorClipBox(0, 0, Width, Height);

            ScanlineRenderer scanlineRenderer = new ScanlineRenderer();
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
                    VertexSourceApplyTransform trans = new VertexSourceApplyTransform(lionShape.Path, txBilinear);
                    scanlineRenderer.RenderSolidAllPaths(clippingProxy, g_rasterizer, g_scanline, trans, lionShape.Colors, lionShape.PathIndexList, lionShape.NumPaths);

                    //--------------------------
                    // Render transformed ellipse
                    //
                    VertexSource.Ellipse ell = new MatterHackers.Agg.VertexSource.Ellipse((lionShape.Bounds.Left + lionShape.Bounds.Right) * 0.5, (lionShape.Bounds.Bottom + lionShape.Bounds.Top) * 0.5,
                                     (lionShape.Bounds.Right - lionShape.Bounds.Left) * 0.5, (lionShape.Bounds.Top - lionShape.Bounds.Bottom) * 0.5,
                                     200);
                    Stroke ell_stroke = new Stroke(ell);
                    ell_stroke.Width = 3;
                    VertexSourceApplyTransform trans_ell = new VertexSourceApplyTransform(ell, txBilinear);

                    VertexSourceApplyTransform trans_ell_stroke = new VertexSourceApplyTransform(ell_stroke, txBilinear);

                    g_rasterizer.add_path(trans_ell);
                    scanlineRenderer.render_scanlines_aa_solid(clippingProxy, g_rasterizer, g_scanline, ColorRGBA.Make(0.5, 0.3, 0.0, 0.3));

                    g_rasterizer.add_path(trans_ell_stroke);
                    scanlineRenderer.render_scanlines_aa_solid(clippingProxy, g_rasterizer, g_scanline, ColorRGBA.Make(0.0, 0.3, 0.2, 1.0));
                }
            }
            else
            {
                var txPerspective = new Perspective(lionShape.Bounds.Left, lionShape.Bounds.Bottom, lionShape.Bounds.Right, lionShape.Bounds.Top, quadPolygonControl.polygon());
                if (txPerspective.is_valid())
                {
                    //lion:
                    //1. create transform version of lion
                    var txLion = new VertexSourceApplyTransform(lionShape.Path, txPerspective);
                    //2.render it
                    scanlineRenderer.RenderSolidAllPaths(clippingProxy, g_rasterizer, g_scanline, txLion, lionShape.Colors, lionShape.PathIndexList, lionShape.NumPaths);

                    //--------------------------------------------------------------------------------------
                    //filled Ellipse
                    //1. create original fill ellipse
                    var filledEllipse = new MatterHackers.Agg.VertexSource.Ellipse((lionShape.Bounds.Left + lionShape.Bounds.Right) * 0.5, (lionShape.Bounds.Bottom + lionShape.Bounds.Top) * 0.5,
                                      (lionShape.Bounds.Right - lionShape.Bounds.Left) * 0.5, (lionShape.Bounds.Top - lionShape.Bounds.Bottom) * 0.5,
                                      200);
                    //2. create transform version of fill ellipse
                    var txFillEllipse = new VertexSourceApplyTransform(filledEllipse, txPerspective);
                    //3. add
                    g_rasterizer.add_path(txFillEllipse);
                    //4. render it
                    scanlineRenderer.render_scanlines_aa_solid(clippingProxy,
                        g_rasterizer,
                        g_scanline,
                        ColorRGBA.Make(0.5, 0.3, 0.0, 0.3));

                    //--------------------------------------------------------
                    //outline Ellipse
                    //1. create original version of stroke ellipse 
                    var strokeEllipse = new Stroke(filledEllipse);
                     
                    strokeEllipse.Width = 3;
                    //2. create transform version of outlin  
                    var txOutline = new VertexSourceApplyTransform(strokeEllipse, txPerspective);
                    //3. add
                    g_rasterizer.add_path(txOutline);                    
                    //4. render                      
                    scanlineRenderer.render_scanlines_aa_solid(clippingProxy,
                        g_rasterizer,
                        g_scanline,
                        ColorRGBA.Make(0.0, 0.3, 0.2, 1.0));
                }
            }

            //--------------------------
            // Render the "quad" tool and controls
            g_rasterizer.add_path(quadPolygonControl);
            scanlineRenderer.render_scanlines_aa_solid(clippingProxy, g_rasterizer, g_scanline, ColorRGBA.Make(0, 0.3, 0.5, 0.6));

            //base.OnDraw(graphics2D);
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

