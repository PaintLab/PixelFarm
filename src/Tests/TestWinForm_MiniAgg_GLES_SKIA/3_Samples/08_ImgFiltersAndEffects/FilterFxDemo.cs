//BSD, 2014-present, WinterDev
//MatterHackers

using System;
using System.Diagnostics;
using PixelFarm.CpuBlit.UI;
using PixelFarm.CpuBlit.VertexProcessing;
using PixelFarm.Drawing;

using Mini;
using PaintLab.Svg;
using PaintFx.Effects;

namespace PixelFarm.CpuBlit.Sample_Blur2
{



    public enum FilterMethod
    {
        None,

        StackBlur,
        ChannelBlur,
        Sharpen,
        Emboss,
        EdgeDetection,
        OilPaint,
        PencilSketch
    }


    ///*
    // * @"Now you can blur rendered images rather fast! There two algorithms are used: 
    //Stack Blur by Mario Klingemann and Fast Recursive Gaussian Filter, described 
    //here and here (PDF). The speed of both methods does not depend on the filter radius. 
    //Mario's method works 3-5 times faster; it doesn't produce exactly Gaussian response, 
    //but pretty fair for most practical purposes. The recursive filter uses floating 
    //point arithmetic and works slower. But it is true Gaussian filter, with theoretically 
    //infinite impulse response. The radius (actually 2*sigma value) can be fractional 
    //and the filter produces quite adequate result."*/

    [Info(OrderCode = "08")]
    [Info(DemoCategory.Bitmap)]
    public class FilterFxDemo : DemoBase
    {
        PolygonEditWidget _shadow_ctrl;
        RectD _shape_bounds;
        Stopwatch _sw = new Stopwatch();
        MyTestSprite _testSprite;



        ImgFilterStackBlur _fxBlurStack = new ImgFilterStackBlur();
        ImgFilterSharpen _fxSharpen = new ImgFilterSharpen();
        ImgFilterEmboss _fxEmboss = new ImgFilterEmboss();
        ImgFilterEdgeDetection _fxEdgeDetection = new ImgFilterEdgeDetection();

        ImgFilterOilPaint _oilPaintFilter = new ImgFilterOilPaint();
        ImgFilterPencilSketch _pencilSketch = new ImgFilterPencilSketch();


        public FilterFxDemo()
        {
            VgVisualElement renderVx = VgVisualDocHelper.CreateVgVisualDocFromFile(@"Samples\lion.svg").VgRootElem;
            var spriteShape = new SpriteShape(renderVx);
            _testSprite = new MyTestSprite(spriteShape);
            //--------------

            //m_rbuf2 = new ReferenceImage();
            _shape_bounds = new RectD();
            _shadow_ctrl = new PolygonEditWidget(4);
            this.FlattenCurveChecked = true;
            this.FilterMethod = FilterMethod.None;
            this.BlurRadius = 15;
            //

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
        public FilterMethod FilterMethod
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
            _shadow_ctrl.OnMouseDown(
                new MouseEventArgs(
                    isRightButton ? MouseButtons.Right : MouseButtons.Left,
                    1,
                    x, y,
                    1));
        }
        public override void MouseUp(int x, int y)
        {
            _shadow_ctrl.OnMouseUp(
                new MouseEventArgs(
                     MouseButtons.Left,
                    1,
                    x, y,
                    1));
        }
        public override void MouseDrag(int x, int y)
        {
            _shadow_ctrl.OnMouseMove(
               new MouseEventArgs(
                   MouseButtons.Left,
                   1,
                   x, y,
                   1));
        }


        public override void Draw(Painter p)
        {
            //create painter             
            p.SetClipBox(0, 0, Width, Height);
            p.Clear(Drawing.Color.White);
            _testSprite.Render(p);
            _testSprite.GetElementBounds(out float b_left, out float b_top, out float b_right, out float b_bottom);

            //-----------------------------------------------------------------------------


            if (FilterMethod == FilterMethod.None)
            {
                return;
            }

            RectInt boundRect = new RectInt((int)b_left, (int)b_bottom, (int)b_right, (int)b_top);
            int m_radius = this.BlurRadius;
            //expand bound rect
            boundRect.Left -= m_radius;
            boundRect.Bottom -= m_radius;
            boundRect.Right += m_radius;
            boundRect.Top += m_radius;
            // Create a new pixel renderer and attach it to the main one as a child image. 
            // It returns true if the attachment succeeded. It fails if the rectangle 
            // (bbox) is fully clipped.
            //------------------
            //create filter specfication
            //it will be resolve later by the platform similar to request font
            //------------------ 
            if (boundRect.Clip(new RectInt(0, 0, p.Width - 1, p.Height - 1)))
            {
                //check if intersect  
                var prevClip = p.ClipBox;
                p.ClipBox = boundRect;
                // Blur it

                IImageFilter selectedFilter = null;
                switch (FilterMethod)
                {
                    case FilterMethod.Sharpen:

                        selectedFilter = _fxSharpen;

                        break;
                    case FilterMethod.StackBlur:

                        //------------------  
                        // Faster, but bore specific. 
                        // Works only for 8 bits per channel and only with radii <= 254.
                        //------------------ 
                        selectedFilter = _fxBlurStack;
                        break;
                    case FilterMethod.Emboss:
                        selectedFilter = _fxEmboss;
                        break;
                    case FilterMethod.EdgeDetection:

                        selectedFilter = _fxEdgeDetection;

                        break;
                    case FilterMethod.OilPaint:
                        selectedFilter = _oilPaintFilter;
                        break;
                    case FilterMethod.PencilSketch:
                        selectedFilter = _pencilSketch;
                        break;
                    default:
                        {   // True Gaussian Blur, 3-5 times slower than Stack Blur,
                            // but still constant time of radius. Very sensitive
                            // to precision, doubles are must here.
                            //------------------                               

                        }
                        break;
                }

                if (selectedFilter != null)
                {
                    _sw.Reset();
                    _sw.Start();

                    p.ApplyFilter(selectedFilter);

                    _sw.Stop();


                    System.Diagnostics.Debug.WriteLine(_sw.ElapsedMilliseconds);

                }

                //store back
                p.ClipBox = prevClip;
            }


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
