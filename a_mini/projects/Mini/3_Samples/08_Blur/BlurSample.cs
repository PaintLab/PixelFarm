using System;
using System.Diagnostics;

using MatterHackers.Agg.UI;
using MatterHackers.Agg.Transform;
using MatterHackers.Agg.Image;
using MatterHackers.Agg.VertexSource;

using MatterHackers.Agg.Font;
using MatterHackers.VectorMath;


using Mini;
namespace MatterHackers.Agg.Sample_Blur
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
    public class blur : DemoBase
    {
        //RadioButtonGroup m_method;
        //Slider m_radius;
        PolygonEditWidget m_shadow_ctrl;
        //CheckBox m_channel_r;
        //CheckBox m_channel_g;
        //CheckBox m_channel_b;
        //CheckBox m_FlattenCurves;

        IVertexSource m_path;
        FlattenCurves m_shape;

        ScanlineRasterizer m_ras = new ScanlineRasterizer();
        ScanlinePacked8 m_sl;
        //ReferenceImage m_rbuf2;

        //agg::stack_blur    <agg::rgba8, agg::stack_blur_calc_rgb<> >     m_stack_blur;
        RecursiveBlur m_recursive_blur = new RecursiveBlur(new recursive_blur_calc_rgb());

        RectangleDouble m_shape_bounds;

        Stopwatch stopwatch = new Stopwatch();

        public blur()
        {
            //m_rbuf2 = new ReferenceImage();
            m_shape_bounds = new RectangleDouble();
            m_shadow_ctrl = new PolygonEditWidget(4);


            this.FlattenCurveCheck = true;
            this.BlurMethod = Sample_Blur.BlurMethod.RecursiveBlur;
            this.BlurRadius = 15;

            m_sl = new ScanlinePacked8();
            StyledTypeFace typeFaceForLargeA = new StyledTypeFace(LiberationSansFont.Instance, 300, flatenCurves: false);
            m_path = typeFaceForLargeA.GetGlyphForCharacter('a');

            Affine shape_mtx = Affine.NewMatix(AffinePlan.Translate(150, 100));
            //shape_mtx *= Affine.NewTranslation(150, 100);

            m_path = new VertexSourceApplyTransform(m_path, shape_mtx);
            m_shape = new FlattenCurves(m_path);

            bounding_rect.bounding_rect_single(m_shape, 0, ref m_shape_bounds);

            m_shadow_ctrl.SetXN(0, m_shape_bounds.Left);
            m_shadow_ctrl.SetYN(0, m_shape_bounds.Bottom);
            m_shadow_ctrl.SetXN(1, m_shape_bounds.Right);
            m_shadow_ctrl.SetYN(1, m_shape_bounds.Bottom);
            m_shadow_ctrl.SetXN(2, m_shape_bounds.Right);
            m_shadow_ctrl.SetYN(2, m_shape_bounds.Top);
            m_shadow_ctrl.SetXN(3, m_shape_bounds.Left);
            m_shadow_ctrl.SetYN(3, m_shape_bounds.Top);
            m_shadow_ctrl.line_color(new ColorRGBAf(0, 0.3, 0.5, 0.3));
        }

        [DemoConfig]
        public bool FlattenCurveCheck
        {
            get;
            set;
        }
        [DemoConfig(MaxValue = 40)]
        public double BlurRadius
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
            var widgetsSubImage = ImageHelper.CreateChildImage(graphics2D.DestImage, graphics2D.GetClippingRect());
            ClipProxyImage clippingProxy = new ClipProxyImage(widgetsSubImage);
            clippingProxy.clear(ColorRGBA.White);
            m_ras.SetVectorClipBox(0, 0, Width, Height);

            Affine move = Affine.NewTranslation(10, 10);

            Perspective shadow_persp = new Perspective(m_shape_bounds.Left, m_shape_bounds.Bottom,
                                                m_shape_bounds.Right, m_shape_bounds.Top,
                                                m_shadow_ctrl.polygon());

            IVertexSource shadow_trans;
            if (FlattenCurveCheck)
            {
                shadow_trans = new VertexSourceApplyTransform(m_shape, shadow_persp);
            }
            else
            {
                shadow_trans = new VertexSourceApplyTransform(m_path, shadow_persp);
                // this will make it very smooth after the transform
                //shadow_trans = new conv_curve(shadow_trans);
            }


            // Render shadow
            m_ras.add_path(shadow_trans);
            ScanlineRenderer scanlineRenderer = new ScanlineRenderer();
            scanlineRenderer.render_scanlines_aa_solid(clippingProxy, m_ras, m_sl, new ColorRGBAf(0.2, 0.3, 0).GetAsRGBA_Bytes());

            // Calculate the bounding box and extend it by the blur radius
            RectangleDouble bbox = new RectangleDouble();
            bounding_rect.bounding_rect_single(shadow_trans, 0, ref bbox);

            double m_radius = this.BlurRadius;

            bbox.Left -= m_radius;
            bbox.Bottom -= m_radius;
            bbox.Right += m_radius;
            bbox.Top += m_radius;

            if (BlurMethod == Sample_Blur.BlurMethod.RecursiveBlur)
            {
                // The recursive blur method represents the true Gaussian Blur,
                // with theoretically infinite kernel. The restricted window size
                // results in extra influence of edge pixels. It's impossible to
                // solve correctly, but extending the right and top areas to another
                // radius value produces fair result.
                //------------------
                bbox.Right += m_radius;
                bbox.Top += m_radius;
            }

            stopwatch.Stop();
            stopwatch.Reset();
            stopwatch.Start();

            if (BlurMethod != Sample_Blur.BlurMethod.ChannelBlur)
            {
                // Create a new pixel renderer and attach it to the main one as a child image. 
                // It returns true if the attachment succeeded. It fails if the rectangle 
                // (bbox) is fully clipped.
                //------------------
#if SourceDepth24
                ImageBuffer image2 = new ImageBuffer(new BlenderBGR());
#else

#endif

                int x1 = (int)bbox.Left;
                int y1 = (int)bbox.Top;
                int x2 = (int)bbox.Right;
                int y2 = (int)bbox.Bottom;
                RectangleInt boundsRect = new RectangleInt(x1, y2, x2, y1);
                if (boundsRect.clip(new RectangleInt(0, 0, widgetsSubImage.Width - 1, widgetsSubImage.Height - 1)))
                {
                    //check if intersect 
                    ChildImage image2 = new ChildImage(widgetsSubImage, new BlenderBGRA(), x1, y2, x2, y1); 
                    // Blur it
                    switch (BlurMethod)
                    {
                        case Sample_Blur.BlurMethod.StackBlur:
                            {
                                // More general method, but 30-40% slower.
                                //------------------
                                //m_stack_blur.blur(pixf2, agg::uround(m_radius.Value));

                                // Faster, but bore specific. 
                                // Works only for 8 bits per channel and only with radii <= 254.
                                //------------------
                                stack_blur test = new stack_blur();
                                test.Blur(image2, AggBasics.uround(m_radius), AggBasics.uround(m_radius));

                            } break;
                        default:
                            {  // True Gaussian Blur, 3-5 times slower than Stack Blur,
                                // but still constant time of radius. Very sensitive
                                // to precision, doubles are must here.
                                //------------------
                                m_recursive_blur.blur(image2, m_radius);
                            } break;
                    }



                }


            }
            else
            {
                /*
                // Blur separate channels
                //------------------
                if(m_channel_r.Checked)
                {
                    typedef agg::pixfmt_alpha_blend_gray<
                        agg::blender_gray8, 
                        agg::rendering_buffer,
                        3, 2> pixfmt_gray8r;

                    pixfmt_gray8r pixf2r(m_rbuf2);
                    if(pixf2r.attach(pixf, int(bbox.x1), int(bbox.y1), int(bbox.x2), int(bbox.y2)))
                    {
                        agg::stack_blur_gray8(pixf2r, agg::uround(m_radius.Value), 
                                                      agg::uround(m_radius.Value));
                    }
                }

                if(m_channel_g.Checked)
                {
                    typedef agg::pixfmt_alpha_blend_gray<
                        agg::blender_gray8, 
                        agg::rendering_buffer,
                        3, 1> pixfmt_gray8g;

                    pixfmt_gray8g pixf2g(m_rbuf2);
                    if(pixf2g.attach(pixf, int(bbox.x1), int(bbox.y1), int(bbox.x2), int(bbox.y2)))
                    {
                        agg::stack_blur_gray8(pixf2g, agg::uround(m_radius.Value), 
                                                      agg::uround(m_radius.Value));
                    }
                }

                if(m_channel_b.Checked)
                {
                    typedef agg::pixfmt_alpha_blend_gray<
                        agg::blender_gray8, 
                        agg::rendering_buffer,
                        3, 0> pixfmt_gray8b;

                    pixfmt_gray8b pixf2b(m_rbuf2);
                    if(pixf2b.attach(pixf, int(bbox.x1), int(bbox.y1), int(bbox.x2), int(bbox.y2)))
                    {
                        agg::stack_blur_gray8(pixf2b, agg::uround(m_radius.Value), 
                                                      agg::uround(m_radius.Value));
                    }
                }
                 */
            }

            double tm = stopwatch.ElapsedMilliseconds;

            // Render the shape itself
            //------------------
            if (FlattenCurveCheck)
            {
                m_ras.add_path(m_shape);
            }
            else
            {
                m_ras.add_path(m_path);
            }

            scanlineRenderer.render_scanlines_aa_solid(clippingProxy, m_ras, m_sl, new ColorRGBAf(0.6, 0.9, 0.7, 0.8).GetAsRGBA_Bytes());

            graphics2D.DrawString(string.Format("{0:F2} ms", tm), 140, 30);

            m_shadow_ctrl.OnDraw(graphics2D);

        }
    }
    public enum BlurMethod
    {
        StackBlur,
        RecursiveBlur,
        ChannelBlur
    }
}