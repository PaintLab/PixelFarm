//BSD, 2014-2016, WinterDev
//MatterHackers


using System.Diagnostics;
using PixelFarm.Agg.UI;
using PixelFarm.Agg.Transform;
using PixelFarm.Agg.VertexSource;

using Mini;
using System;
using PixelFarm.Drawing.Fonts;

namespace PixelFarm.Agg.Sample_Blur2
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
    public class BlurWithPainter : DemoBase
    {
        PolygonEditWidget m_shadow_ctrl;
        VertexStore m_pathVxs;
        VertexStore m_pathVxs2;
        VertexStoreSnap m_path_2;
        RectD m_shape_bounds;
        Stopwatch stopwatch = new Stopwatch();
        SvgFontStore svgFontStore = new SvgFontStore();
        GdiPathFontStore gdiPathFontStore = new GdiPathFontStore();

        public BlurWithPainter()
        {
            //m_rbuf2 = new ReferenceImage();
            m_shape_bounds = new RectD();
            m_shadow_ctrl = new PolygonEditWidget(4);
            this.FlattenCurveChecked = true;
            this.BlurMethod = BlurMethod.RecursiveBlur;
            this.BlurRadius = 15;

            ActualFont svgFont = svgFontStore.LoadFont("svg-LiberationSansFont", 300);
            //PathWriter p01 = new PathWriter();
            //p01.MoveTo(0, 0);
            //p01.LineTo(50, 100);
            //p01.LineTo(100, 0);
            ////-
            //p01.MoveTo(220, 10);
            //p01.LineTo(50, 75);
            //p01.LineTo(25, 15);
            //p01.CloseFigure();
            //p01.Stop();
            //m_pathVxs = p01.Vxs;
            var winFontGlyph = svgFont.GetGlyph('a');
            m_pathVxs = winFontGlyph.originalVxs;// typeFaceForLargeA.GetGlyphForCharacter('a');
            Affine shape_mtx = Affine.NewMatix(AffinePlan.Translate(150, 100));
            m_pathVxs = shape_mtx.TransformToVxs(m_pathVxs);
            var curveFlattener = new CurveFlattener();
            m_pathVxs2 = curveFlattener.MakeVxs(m_pathVxs);
            m_path_2 = new VertexStoreSnap(m_pathVxs2);
            BoundingRect.GetBoundingRect(m_path_2, ref m_shape_bounds);
            m_shadow_ctrl.SetXN(0, m_shape_bounds.Left);
            m_shadow_ctrl.SetYN(0, m_shape_bounds.Bottom);
            m_shadow_ctrl.SetXN(1, m_shape_bounds.Right);
            m_shadow_ctrl.SetYN(1, m_shape_bounds.Bottom);
            m_shadow_ctrl.SetXN(2, m_shape_bounds.Right);
            m_shadow_ctrl.SetYN(2, m_shape_bounds.Top);
            m_shadow_ctrl.SetXN(3, m_shape_bounds.Left);
            m_shadow_ctrl.SetYN(3, m_shape_bounds.Top);
            m_shadow_ctrl.LineColor = PixelFarm.Drawing.Color.FromArgb(0.3f, 0f, 0.3f, 0.5f);
        }

        [DemoConfig]
        public bool FlattenCurveChecked
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
        public override void Draw(CanvasPainter p)
        {
            //create painter             
            p.SetClipBox(0, 0, Width, Height);
            p.Clear(Drawing.Color.White);
            //-----------------------------------------------------------------------
            //green glyph
            Perspective shadow_persp = new Perspective(
                            m_shape_bounds,
                            m_shadow_ctrl.GetInnerCoords());
            VertexStore s2 = this.m_pathVxs2;
            //if (FlattenCurveChecked)
            //{
            //    //s2 = shadow_persp.TransformToVxs(m_path_2);
            //    s2 = shadow_persp.TransformToVxs(m_pathVxs2);
            //}
            //else
            //{
            //    s2 = shadow_persp.TransformToVxs(m_pathVxs);
            //}
            p.FillColor = PixelFarm.Drawing.Color.Make(0.2f, 0.3f, 0f);
            p.Fill(s2);
            //---------------------------------------------------------------------------------------------------------
            //shadow 
            //---------------------------------------------------------------------------------------------------------
            // Calculate the bounding box and extend it by the blur radius 

            RectInt boundRect = BoundingRectInt.GetBoundingRect(s2);
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

                if (boundRect.Clip(new RectInt(0, 0, p.Width - 1, p.Height - 1)))
                {
                    //check if intersect  
                    var prevClip = p.ClipBox;
                    p.ClipBox = boundRect;
                    // Blur it
                    switch (BlurMethod)
                    {
                        case BlurMethod.StackBlur:
                            {
                                //------------------  
                                // Faster, but bore specific. 
                                // Works only for 8 bits per channel and only with radii <= 254.
                                //------------------
                                //p.DoFilterBlurStack(boundRect, m_radius);
                                p.DoFilterBlurStack(new RectInt(0, 0, Width, Height), m_radius);
                            }
                            break;
                        default:
                            {   // True Gaussian Blur, 3-5 times slower than Stack Blur,
                                // but still constant time of radius. Very sensitive
                                // to precision, doubles are must here.
                                //------------------
                                p.DoFilterBlurRecursive(boundRect, m_radius);
                            }
                            break;
                    }
                    //store back
                    p.ClipBox = prevClip;
                }
            }

            double tm = stopwatch.ElapsedMilliseconds;
            p.FillColor = Drawing.Color.FromArgb(0.8f, 0.6f, 0.9f, 0.7f);
            // Render the shape itself
            ////------------------
            //if (FlattenCurveChecked)
            //{
            //    //m_ras.AddPath(m_path_2);
            //    p.Fill(m_path_2);
            //}
            //else
            //{
            //    //m_ras.AddPath(m_pathVxs);
            //    p.Fill(m_pathVxs);
            //}

            p.FillColor = Drawing.Color.Black;
            //p.DrawString(string.Format("{0:F2} ms", tm), 140, 30);
            //-------------------------------------------------------------
            //control
            //m_shadow_ctrl.OnDraw(p);
        }
    }
}