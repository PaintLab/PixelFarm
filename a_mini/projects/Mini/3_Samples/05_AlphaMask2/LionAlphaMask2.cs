//2014 BSD,WinterDev
//MatterHackers

#define USE_CLIPPING_ALPHA_MASK

using System;

using MatterHackers.Agg.Transform;
using MatterHackers.Agg.Image;
using MatterHackers.Agg.VertexSource;

using MatterHackers.VectorMath;

using Mini;

namespace MatterHackers.Agg.Sample_LionAlphaMask2
{
    [Info(OrderCode = "05")]
    [Info(DemoCategory.Bitmap, "Clipping to multiple rectangle regions")]
    public class alpha_mask2_application : DemoBase
    {

        int maskAlphaSliderValue = 100;
        ActualImage alphaBitmap;
        LionShape lionShape;
        MatterHackers.Agg.ScanlineRasterizer rasterizer = new ScanlineRasterizer();
        ScanlinePacked8 scanlineCache = new ScanlinePacked8();
        double angle = 0;
        double lionScale = 1.0;
        double skewX = 0;
        double skewY = 0;
        bool isMaskSliderValueChanged = true;

        ChildImage alphaMaskImageBuffer;
        IAlphaMask alphaMask;

        public alpha_mask2_application()
        {
            lionShape = new LionShape();
            lionShape.ParseLion();

            this.Width = 800;
            this.Height = 600;

            //AnchorAll();
            //alphaMaskImageBuffer = new ReferenceImage();

#if USE_CLIPPING_ALPHA_MASK
            //alphaMask = new AlphaMaskByteClipped(alphaMaskImageBuffer, 1, 0);
#else
            //alphaMask = new AlphaMaskByteUnclipped(alphaMaskImageBuffer, 1, 0);
#endif

            //numMasksSlider = new UI.Slider(5, 5, 150, 12);
            //sliderValue = 0.0;
            //AddChild(numMasksSlider);
            //numMasksSlider.SetRange(5, 100);
            //numMasksSlider.Value = 10;
            //numMasksSlider.Text = "N={0:F3}";
            //numMasksSlider.OriginRelativeParent = Vector2.Zero;

        }

        public override void Init()
        {


        }
        void generate_alpha_mask(int width, int height)
        {
            alphaBitmap = new ActualImage(width, height, 8, new blender_gray(1));

            alphaMaskImageBuffer = new ChildImage(alphaBitmap, new blender_gray(1));

            alphaMask = new AlphaMaskByteClipped(alphaMaskImageBuffer, 1, 0);

#if USE_CLIPPING_ALPHA_MASK
            //alphaMaskImageBuffer.AttachBuffer(alphaBitmap.GetBuffer(), 20 * width + 20, width - 40, height - 40, width, 8, 1);
#else
            alphaMaskImageBuffer.attach(alphaByteArray, (int)cx, (int)cy, cx, 1);
#endif

            var image = new ChildImage(alphaMaskImageBuffer, new blender_gray(1), 1, 0, 8);

            ClipProxyImage clippingProxy = new ClipProxyImage(image);
            ScanlinePacked8 sclnPack = new ScanlinePacked8();

            clippingProxy.Clear(ColorRGBA.Black);

            VertexSource.Ellipse ellipseForMask = new MatterHackers.Agg.VertexSource.Ellipse();

            System.Random randGenerator = new Random(1432);

            ScanlineRenderer scanlineRenderer = new ScanlineRenderer();
            int i;
            int num = (int)maskAlphaSliderValue;
            for (i = 0; i < num; i++)
            {
                if (i == num - 1)
                {
                    ellipseForMask.init(Width / 2, Height / 2, 110, 110, 100);
                    rasterizer.AddPath(ellipseForMask);
                    scanlineRenderer.RenderScanlineSolidAA(clippingProxy, rasterizer, sclnPack, new ColorRGBA(0, 0, 0, 255));

                    ellipseForMask.init(ellipseForMask.originX, ellipseForMask.originY, ellipseForMask.radiusX - 10, ellipseForMask.radiusY - 10, 100);
                    rasterizer.AddPath(ellipseForMask);
                    scanlineRenderer.RenderScanlineSolidAA(clippingProxy, rasterizer, sclnPack, new ColorRGBA(255, 0, 0, 255));
                }
                else
                {
                    ellipseForMask.init(randGenerator.Next() % width,
                             randGenerator.Next() % height,
                             randGenerator.Next() % 100 + 20,
                             randGenerator.Next() % 100 + 20,
                             100);
                    // set the color to draw into the alpha channel.
                    // there is not very much reason to set the alpha as you will get the amount of 
                    // transparency based on the color you draw.  (you might want some type of different edeg effect but it will be minor).
                    rasterizer.AddPath(ellipseForMask);
                    scanlineRenderer.RenderScanlineSolidAA(clippingProxy, rasterizer, sclnPack,
                       ColorRGBA.Make((int)((float)i / (float)num * 255), 0, 0, 255));
                }
            }
           
        }


        [DemoConfig(MinValue = 0, MaxValue = 255)]
        public int MaskAlphaSliderValue
        {
            get
            {
                return maskAlphaSliderValue;
            }
            set
            {
                this.maskAlphaSliderValue = value;
                isMaskSliderValueChanged = true;
            }
        }
        public override void Draw(Graphics2D g)
        {
            var widgetsSubImage = ImageHelper.CreateChildImage(g.DestImage, g.GetClippingRect());

            int width = (int)widgetsSubImage.Width;
            int height = (int)widgetsSubImage.Height;

            //change value ***
            if (isMaskSliderValueChanged)
            {
                generate_alpha_mask(width, height);

                this.isMaskSliderValueChanged = false;

            }

            rasterizer.SetVectorClipBox(0, 0, width, height);


            //alphaMaskImageBuffer.AttachBuffer(alphaByteArray, 0, width, height, width, 8, 1);

            MatterHackers.Agg.Image.AlphaMaskAdaptor imageAlphaMaskAdaptor = new MatterHackers.Agg.Image.AlphaMaskAdaptor(widgetsSubImage, alphaMask);
            ClipProxyImage alphaMaskClippingProxy = new ClipProxyImage(imageAlphaMaskAdaptor);
            ClipProxyImage clippingProxy = new ClipProxyImage(widgetsSubImage);

            //Affine transform = Affine.NewIdentity();
            //transform *= Affine.NewTranslation(-lionShape.Center.x, -lionShape.Center.y);
            //transform *= Affine.NewScaling(lionScale, lionScale);
            //transform *= Affine.NewRotation(angle + Math.PI);
            //transform *= Affine.NewSkewing(skewX / 1000.0, skewY / 1000.0);
            //transform *= Affine.NewTranslation(Width / 2, Height / 2);
            Affine transform = Affine.NewMatix( 
                    AffinePlan.Translate(-lionShape.Center.x, -lionShape.Center.y),
                    AffinePlan.Scale(lionScale, lionScale),
                    AffinePlan.Rotate(angle + Math.PI),
                    AffinePlan.Skew(skewX / 1000.0, skewY / 1000.0),
                    AffinePlan.Translate(width / 2, height / 2));
            clippingProxy.Clear(ColorRGBA.White);

            ScanlineRenderer scanlineRenderer = new ScanlineRenderer();
            // draw a background to show how the mask is working better
            int RectWidth = 30;
            for (int i = 0; i < 40; i++)
            {
                for (int j = 0; j < 40; j++)
                {
                    if ((i + j) % 2 != 0)
                    {
                        VertexSource.RoundedRect rect = new VertexSource.RoundedRect(i * RectWidth, j * RectWidth, (i + 1) * RectWidth, (j + 1) * RectWidth, 0);
                        rect.normalize_radius();

                        // Drawing as an outline
                        rasterizer.AddPath(rect);
                        scanlineRenderer.RenderScanlineSolidAA(clippingProxy, rasterizer, scanlineCache, ColorRGBA.Make(.9, .9, .9));
                    }
                }
            }

            //int x, y;

            // Render the lion
            VertexSourceApplyTransform trans = new VertexSourceApplyTransform(lionShape.Path, transform);
            scanlineRenderer.RenderSolidAllPaths(alphaMaskClippingProxy, 
                   rasterizer, scanlineCache, trans, 
                   lionShape.Colors, 
                   lionShape.PathIndexList,
                   lionShape.NumPaths);

            /*
            // Render random Bresenham lines and markers
            agg::renderer_markers<amask_ren_type> m(r);
            for(i = 0; i < 50; i++)
            {
                m.line_color(agg::rgba8(randGenerator.Next() & 0x7F, 
                                        randGenerator.Next() & 0x7F, 
                                        randGenerator.Next() & 0x7F, 
                                        (randGenerator.Next() & 0x7F) + 0x7F)); 
                m.fill_color(agg::rgba8(randGenerator.Next() & 0x7F, 
                                        randGenerator.Next() & 0x7F, 
                                        randGenerator.Next() & 0x7F, 
                                        (randGenerator.Next() & 0x7F) + 0x7F));

                m.line(m.coord(randGenerator.Next() % width), m.coord(randGenerator.Next() % height), 
                       m.coord(randGenerator.Next() % width), m.coord(randGenerator.Next() % height));

                m.marker(randGenerator.Next() % width, randGenerator.Next() % height, randGenerator.Next() % 10 + 5,
                         agg::marker_e(randGenerator.Next() % agg::end_of_markers));
            }


            // Render random anti-aliased lines
            double w = 5.0;
            agg::line_profile_aa profile;
            profile.width(w);

            typedef agg::renderer_outline_aa<amask_ren_type> renderer_type;
            renderer_type ren(r, profile);

            typedef agg::rasterizer_outline_aa<renderer_type> rasterizer_type;
            rasterizer_type ras(ren);
            ras.round_cap(true);

            for(i = 0; i < 50; i++)
            {
                ren.Color = agg::rgba8(randGenerator.Next() & 0x7F, 
                                     randGenerator.Next() & 0x7F, 
                                     randGenerator.Next() & 0x7F, 
                                     //255));
                                     (randGenerator.Next() & 0x7F) + 0x7F); 
                ras.move_to_d(randGenerator.Next() % width, randGenerator.Next() % height);
                ras.line_to_d(randGenerator.Next() % width, randGenerator.Next() % height);
                ras.render(false);
            }


            // Render random circles with gradient
            typedef agg::gradient_linear_color<color_type> grad_color;
            typedef agg::gradient_circle grad_func;
            typedef agg::span_interpolator_linear<> interpolator_type;
            typedef agg::span_gradient<color_type, 
                                      interpolator_type, 
                                      grad_func, 
                                      grad_color> span_grad_type;

            agg::trans_affine grm;
            grad_func grf;
            grad_color grc(agg::rgba8(0,0,0), agg::rgba8(0,0,0));
            agg::ellipse ell;
            agg::span_allocator<color_type> sa;
            interpolator_type inter(grm);
            span_grad_type sg(inter, grf, grc, 0, 10);
            agg::renderer_scanline_aa<amask_ren_type, 
                                      agg::span_allocator<color_type>,
                                      span_grad_type> rg(r, sa, sg);
            for(i = 0; i < 50; i++)
            {
                x = randGenerator.Next() % width;
                y = randGenerator.Next() % height;
                double r = randGenerator.Next() % 10 + 5;
                grm.reset();
                grm *= agg::trans_affine_scaling(r / 10.0);
                grm *= agg::trans_affine_translation(x, y);
                grm.invert();
                grc.colors(agg::rgba8(255, 255, 255, 0),
                           agg::rgba8(randGenerator.Next() & 0x7F, 
                                      randGenerator.Next() & 0x7F, 
                                      randGenerator.Next() & 0x7F, 
                                      255));
                sg.color_function(grc);
                ell.init(x, y, r, r, 32);
                g_rasterizer.add_path(ell);
                agg::render_scanlines(g_rasterizer, g_scanline, rg);
            }
             */
            //m_num_cb.Render(g_rasterizer, g_scanline, clippingProxy);

             
        }
        public override void MouseDown(int x, int y, bool isRightButton)
        {
            doTransform(this.Width, this.Height, x, y);
        }
        public override void MouseDrag(int x, int y)
        {
            doTransform(this.Width, this.Height, x, y);
            base.MouseDrag(x, y);
        }

        void doTransform(double width, double height, double x, double y)
        {
            x -= width / 2;
            y -= height / 2;
            angle = Math.Atan2(y, x);
            lionScale = Math.Sqrt(y * y + x * x) / 100.0;
        }

        //void UpdateLion(MouseEventArgs mouseEvent)
        //{
        //    double x = mouseEvent.X;
        //    double y = mouseEvent.Y;
        //    if (mouseEvent.Button == MouseButtons.Left)
        //    {
        //        int width = (int)Width;
        //        int height = (int)Height;
        //        doTransform(width, height, x, y);
        //        Invalidate();
        //    }

        //    if (mouseEvent.Button == MouseButtons.Right)
        //    {
        //        skewX = x;
        //        skewY = y;
        //        Invalidate();
        //    }
        //}

        //public override void OnMouseDown(MouseEventArgs mouseEvent)
        //{
        //    base.OnMouseDown(mouseEvent);
        //    if (MouseCaptured)
        //    {
        //        UpdateLion(mouseEvent);
        //    }
        //}

        //public override void OnMouseMove(MouseEventArgs mouseEvent)
        //{
        //    base.OnMouseMove(mouseEvent);
        //    if (MouseCaptured)
        //    {
        //        UpdateLion(mouseEvent);
        //    }
        //}
        //public override void OnBoundsChanged(EventArgs e)
        //{
        //    if (Width > 0 && Height > 0)
        //    {
        //        generate_alpha_mask((int)Math.Ceiling(Width), (int)Math.Ceiling(Height));
        //    }
        //    base.OnBoundsChanged(e);
        //}

    }


}
