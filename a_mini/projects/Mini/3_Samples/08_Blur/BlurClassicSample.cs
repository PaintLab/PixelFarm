//2014 BSD,WinterDev
//MatterHackers


using System;
using System.Diagnostics;

using PixelFarm.Agg.UI;
using PixelFarm.Agg.Transform;
using PixelFarm.Agg.Image;
using PixelFarm.Agg.VertexSource;

using PixelFarm.Agg.Font;
using PixelFarm.VectorMath;


using Mini;
namespace PixelFarm.Agg.Sample_Blur
{


    [Info(OrderCode = "08")]
    [Info(DemoCategory.Bitmap, @"Now you can blur rendered images rather fast! There two algorithms are used: 
    Stack Blur by Mario Klingemann and Fast Recursive Gaussian Filter, described 
    here and here (PDF). The speed of both methods does not depend on the filter radius. 
    Mario's method works 3-5 times faster; it doesn't produce exactly Gaussian response, 
    but pretty fair for most practical purposes. The recursive filter uses floating 
    point arithmetic and works slower. But it is true Gaussian filter, with theoretically 
    infinite impulse response. The radius (actually 2*sigma value) can be fractional 
    and the filter produces quite adequate result.")]
    public class Blur_classic : DemoBase
    {
        //RadioButtonGroup m_method;
        //Slider m_radius;
        PolygonEditWidget m_shadow_ctrl;
        //CheckBox m_channel_r;
        //CheckBox m_channel_g;
        //CheckBox m_channel_b;
        //CheckBox m_FlattenCurves;


        VertexStore m_pathVxs;
        VertexStoreSnap m_path_2;


         
        
        //ReferenceImage m_rbuf2; 
        //agg::stack_blur    <agg::rgba8, agg::stack_blur_calc_rgb<> >     m_stack_blur;
        RecursiveBlur m_recursive_blur = new RecursiveBlur(new RecursiveBlurCalcRGB());

        RectD m_shape_bounds; 
        Stopwatch stopwatch = new Stopwatch();

        public Blur_classic()
        {
            //m_rbuf2 = new ReferenceImage();
            m_shape_bounds = new RectD();
            m_shadow_ctrl = new PolygonEditWidget(4);


            this.FlattenCurveCheck = true;
            this.BlurMethod = BlurMethod.RecursiveBlur;
            this.BlurRadius = 15;

            
            StyledTypeFace typeFaceForLargeA = new StyledTypeFace(LiberationSansFont.Instance, 300, flatenCurves: false);
            m_pathVxs = typeFaceForLargeA.GetGlyphForCharacter('a');
            Affine shape_mtx = Affine.NewMatix(AffinePlan.Translate(150, 100));

            m_pathVxs = shape_mtx.TransformToVxs(m_pathVxs);
            var curveFlater = new FlattenCurves();
            m_path_2 = new VertexStoreSnap(curveFlater.MakeVxs(m_pathVxs));
            BoundingRect.GetBoundingRect(m_path_2, ref m_shape_bounds);

            m_shadow_ctrl.SetXN(0, m_shape_bounds.Left);
            m_shadow_ctrl.SetYN(0, m_shape_bounds.Bottom);
            m_shadow_ctrl.SetXN(1, m_shape_bounds.Right);
            m_shadow_ctrl.SetYN(1, m_shape_bounds.Bottom);
            m_shadow_ctrl.SetXN(2, m_shape_bounds.Right);
            m_shadow_ctrl.SetYN(2, m_shape_bounds.Top);
            m_shadow_ctrl.SetXN(3, m_shape_bounds.Left);
            m_shadow_ctrl.SetYN(3, m_shape_bounds.Top);

            m_shadow_ctrl.LineColor = ColorRGBAf.MakeColorRGBA(0f, 0.3f, 0.5f, 0.3f);
        }

        [DemoConfig]
        public bool FlattenCurveCheck
        {
            get;
            set;
        }
        [DemoConfig(MaxValue = 40)]
        public int BlurRadius
        {
            get;
            set;
        }
        [DemoConfig]
        public BlurMethod BlurMethod
        {
            get;
            set;
        }
        [DemoConfig]
        public bool ChannelRedChecked
        {
            get;
            set;
        }
        [DemoConfig]
        public bool ChannelGreenCheced
        {
            get;
            set;
        }
        [DemoConfig]
        public bool ChannelBlueChecked
        {
            get;
            set;
        }

        public override void MouseDown(int x, int y, bool isRightButton)
        {
            m_shadow_ctrl.OnMouseDown(
                new MouseEventArgs(
                    isRightButton ? MouseButtons.Right : MouseButtons.Left,
                    1,
                    x, y,
                    1));
        }
        public override void MouseUp(int x, int y)
        {
            m_shadow_ctrl.OnMouseUp(
                new MouseEventArgs(
                     MouseButtons.Left,
                    1,
                    x, y,
                    1));
        }
        public override void MouseDrag(int x, int y)
        {
            m_shadow_ctrl.OnMouseMove(
               new MouseEventArgs(
                   MouseButtons.Left,
                   1,
                   x, y,
                   1));
        }
        public override void Draw(Graphics2D graphics2D)
        {
            //DrawClassic(graphics2D);
            DrawWithPainter(graphics2D);

        }
        void DrawWithPainter(Graphics2D graphics2D)
        {
            //create painter
            CanvasPainter painter = new CanvasPainter(graphics2D);

            painter.SetClipBox(0, 0, Width, Height);
            painter.Clear(ColorRGBA.White);
            //-----------------------------------------------------------------------
            //green glyph
            Perspective shadow_persp = new Perspective(
                            m_shape_bounds,
                            m_shadow_ctrl.GetInnerCoords());

            VertexStoreSnap spath;
            if (FlattenCurveCheck)
            {
                var s2 = shadow_persp.TransformToVxs(m_path_2);
                spath = new VertexStoreSnap(s2);
            }
            else
            {
                var s2 = shadow_persp.TransformToVxs(m_pathVxs);
                spath = new VertexStoreSnap(s2);
            }
            painter.FillColor = new ColorRGBAf(0.2f, 0.3f, 0f).ToColorRGBA();
            painter.Fill(spath);

            //---------------------------------------------------------------------------------------------------------
            //shadow 
            //---------------------------------------------------------------------------------------------------------
            // Calculate the bounding box and extend it by the blur radius 

            RectInt boundRect = BoundingRectInt.GetBoundingRect(spath);
            var widgetImg = graphics2D.DestImage;

            int m_radius = this.BlurRadius;
            //expand bound rect
            boundRect.Left -= m_radius;
            boundRect.Bottom -= m_radius;
            boundRect.Right += m_radius;
            boundRect.Top += m_radius;

            if (BlurMethod == BlurMethod.RecursiveBlur)
            {
                // The recursive blur method represents the true Gaussian Blur,
                // with theoretically infinite kernel. The restricted window size
                // results in extra influence of edge pixels. It's impossible to
                // solve correctly, but extending the right and top areas to another
                // radius value produces fair result.
                //------------------
                boundRect.Right += m_radius;
                boundRect.Top += m_radius;
            }

            stopwatch.Stop();
            stopwatch.Reset();
            stopwatch.Start();

            if (BlurMethod != BlurMethod.ChannelBlur)
            {
                // Create a new pixel renderer and attach it to the main one as a child image. 
                // It returns true if the attachment succeeded. It fails if the rectangle 
                // (bbox) is fully clipped.
                //------------------ 

                if (boundRect.Clip(new RectInt(0, 0, widgetImg.Width - 1, widgetImg.Height - 1)))
                {
                    //check if intersect  
                    var prevClip = painter.ClipBox;
                    painter.ClipBox = boundRect;
                    // Blur it
                    switch (BlurMethod)
                    {
                        case BlurMethod.StackBlur:
                            {
                                // More general method, but 30-40% slower.
                                //------------------
                                //m_stack_blur.blur(pixf2, agg::uround(m_radius.Value));

                                // Faster, but bore specific. 
                                // Works only for 8 bits per channel and only with radii <= 254.
                                //------------------
                                painter.DoFilterBlurStack(boundRect, m_radius);

                            } break;
                        default:
                            {  // True Gaussian Blur, 3-5 times slower than Stack Blur,
                                // but still constant time of radius. Very sensitive
                                // to precision, doubles are must here.
                                //------------------
                                painter.DoFilterBlurRecursive(boundRect, m_radius);

                            } break;
                    }
                    //store back
                    painter.ClipBox = prevClip;
                }
            }

            double tm = stopwatch.ElapsedMilliseconds;
            painter.FillColor = ColorRGBAf.MakeColorRGBA(0.6f, 0.9f, 0.7f, 0.8f);

            // Render the shape itself
            ////------------------
            if (FlattenCurveCheck)
            {
                //m_ras.AddPath(m_path_2);
                painter.Fill(m_path_2);
            }
            else
            {
                //m_ras.AddPath(m_pathVxs);
                painter.Fill(m_pathVxs);
            }

            painter.DrawString(string.Format("{0:F2} ms", tm), 140, 30);

            //-------------------------------------------------------------
            //control
            m_shadow_ctrl.OnDraw(graphics2D);

        }


        void DrawClassic(Graphics2D graphics2D)
        {
            //original ***


            CanvasPainter painter = new CanvasPainter(graphics2D);
            var m_ras = graphics2D.ScanlineRasterizer;

            var widgetsSubImage = ImageHelper.CreateChildImage(graphics2D.DestImage, graphics2D.GetClippingRect());

            ClipProxyImage clippingProxy = new ClipProxyImage(widgetsSubImage);
            clippingProxy.Clear(ColorRGBA.White);
            m_ras.SetClipBox(0, 0, Width, Height);

            Perspective shadow_persp = new Perspective(
                            m_shape_bounds,
                            m_shadow_ctrl.GetInnerCoords());


            VertexStoreSnap spath;
            if (FlattenCurveCheck)
            {
                var s2 = shadow_persp.TransformToVxs(m_path_2);
                spath = new VertexStoreSnap(s2);
            }
            else
            {
                var s2 = shadow_persp.TransformToVxs(m_pathVxs);
                spath = new VertexStoreSnap(s2);

            }
            m_ras.AddPath(spath);

            ScanlineRasToDestBitmapRenderer sclineRasToBmp = graphics2D.ScanlineRasToDestBitmap;
            sclineRasToBmp.RenderWithSolidColor(clippingProxy, m_ras, graphics2D.ScanlinePacked8,  new ColorRGBAf(0.2f, 0.3f, 0f).ToColorRGBA());


            //---------------------------------------------------------------------------------------------------------
            // Calculate the bounding box and extend it by the blur radius
            RectInt boundRect = BoundingRectInt.GetBoundingRect(spath);

            int m_radius = this.BlurRadius;

            boundRect.Left -= m_radius;
            boundRect.Bottom -= m_radius;
            boundRect.Right += m_radius;
            boundRect.Top += m_radius;

            if (BlurMethod == BlurMethod.RecursiveBlur)
            {
                // The recursive blur method represents the true Gaussian Blur,
                // with theoretically infinite kernel. The restricted window size
                // results in extra influence of edge pixels. It's impossible to
                // solve correctly, but extending the right and top areas to another
                // radius value produces fair result.
                //------------------
                boundRect.Right += m_radius;
                boundRect.Top += m_radius;
            }

            stopwatch.Stop();
            stopwatch.Reset();
            stopwatch.Start();

            if (BlurMethod != BlurMethod.ChannelBlur)
            {
                // Create a new pixel renderer and attach it to the main one as a child image. 
                // It returns true if the attachment succeeded. It fails if the rectangle 
                // (bbox) is fully clipped.
                //------------------
#if SourceDepth24
                ImageBuffer image2 = new ImageBuffer(new BlenderBGR());
#else

#endif

                int x1 = (int)boundRect.Left;
                int y1 = (int)boundRect.Top;
                int x2 = (int)boundRect.Right;
                int y2 = (int)boundRect.Bottom;
                RectInt boundsRect = new RectInt(x1, y2, x2, y1);
                if (boundsRect.Clip(new RectInt(0, 0, widgetsSubImage.Width - 1, widgetsSubImage.Height - 1)))
                {
                    //check if intersect 
                    ChildImage image2 = new ChildImage(widgetsSubImage, new PixelBlenderBGRA(), x1, y2, x2, y1);
                    // Blur it
                    switch (BlurMethod)
                    {
                        case BlurMethod.StackBlur:
                            {
                                // More general method, but 30-40% slower.
                                //------------------
                                //m_stack_blur.blur(pixf2, agg::uround(m_radius.Value));

                                // Faster, but bore specific. 
                                // Works only for 8 bits per channel and only with radii <= 254.
                                //------------------
                                StackBlur test = new StackBlur();
                                test.Blur(image2, AggBasics.uround(m_radius), AggBasics.uround(m_radius));

                            } break;
                        default:
                            {  // True Gaussian Blur, 3-5 times slower than Stack Blur,
                                // but still constant time of radius. Very sensitive
                                // to precision, doubles are must here.
                                //------------------
                                m_recursive_blur.Blur(image2, m_radius);
                            } break;
                    }



                }


            }

            double tm = stopwatch.ElapsedMilliseconds;

            // Render the shape itself
            //------------------
            if (FlattenCurveCheck)
            {
                m_ras.AddPath(m_path_2);
            }
            else
            {
                m_ras.AddPath(m_pathVxs);
            }


            sclineRasToBmp.RenderWithSolidColor(clippingProxy, m_ras,graphics2D.ScanlinePacked8,
                ColorRGBAf.MakeColorRGBA(0.6f, 0.9f, 0.7f, 0.8f));

            graphics2D.DrawString(string.Format("{0:F2} ms", tm), 140, 30);

            //-------------------------------------------------------------
            m_shadow_ctrl.OnDraw(graphics2D);

        }
    }

}