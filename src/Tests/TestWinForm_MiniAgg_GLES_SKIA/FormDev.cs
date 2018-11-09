//BSD, 2014-present, WinterDev

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using PixelFarm.CpuBlit;


namespace Mini
{
    partial class FormDev : Form
    {

        public FormDev()
        {
            InitializeComponent();
            this.Load += new EventHandler(DevForm_Load);
            this.listBox1.DoubleClick += new EventHandler(listBox1_DoubleClick);
            this.Text = "DevForm: Double Click The Example!";
            //render backend choices
            LoadRenderBackendChoices();
        }

        enum RenderBackendChoice
        {
            PureAgg,
            AggOnGLES,
            GdiPlus,
            OpenGLES,

            OpenGLES_OnFormTestBed,
            SkiaMemoryBackend,
            SkiaGLBackend,
        }
        void LoadRenderBackendChoices()
        {

            lstBackEndRenderer.Items.Clear();
            lstBackEndRenderer.Items.Add(RenderBackendChoice.PureAgg); //pure software renderer with MiniAgg
            lstBackEndRenderer.Items.Add(RenderBackendChoice.OpenGLES);
            lstBackEndRenderer.Items.Add(RenderBackendChoice.AggOnGLES);
            //
            lstBackEndRenderer.Items.Add(RenderBackendChoice.GdiPlus);// legacy ***, for printing
            lstBackEndRenderer.Items.Add(RenderBackendChoice.OpenGLES_OnFormTestBed); //legacy , for test only

            //lstBackEndRenderer.Items.Add(RenderBackendChoice.SkiaMemoryBackend);
            //lstBackEndRenderer.Items.Add(RenderBackendChoice.SkiaGLBackend);

            lstBackEndRenderer.SelectedIndex = 0;//set default 
            lstBackEndRenderer.DoubleClick += (s, e) => listBox1_DoubleClick(null, EventArgs.Empty);
        }



        static DemoBase InitDemo(ExampleAndDesc exampleAndDesc)
        {
            DemoBase exBase = Activator.CreateInstance(exampleAndDesc.Type) as DemoBase;
            if (exBase == null)
            {
                return null;
            }
            exBase.Init();
            return exBase;
        }


        CpuBlitContextWinForm _cpuBlitContextWinForm;

        void listBox1_DoubleClick(object sender, EventArgs e)
        {
            //load sample form
            ExampleAndDesc exAndDesc = this.listBox1.SelectedItem as ExampleAndDesc;
            if (exAndDesc == null)
            {
                return; //early exit
            }
            //
            //
            switch ((RenderBackendChoice)lstBackEndRenderer.SelectedItem)
            {
                case RenderBackendChoice.PureAgg:
                    {
                        DemoBase demo = InitDemo(exAndDesc);
                        if (demo == null) { return; }

                        FormTestBed testBed = new FormTestBed();
                        testBed.WindowState = FormWindowState.Maximized;

                        LayoutFarm.UI.InnerViewportKind innerViewportKind = LayoutFarm.UI.InnerViewportKind.PureAgg;
                        LayoutFarm.UI.FormCanvasHelper.CreateConvasControlOnExistingControl(
                            testBed.GetLandingControl(),
                            0, 0, 800, 600,
                            innerViewportKind,
                            out LayoutFarm.UI.UISurfaceViewportControl surfaceViewport
                            );
                        testBed.Show();

                        _cpuBlitContextWinForm = new CpuBlitContextWinForm();
                        _cpuBlitContextWinForm.BindSurface(surfaceViewport, innerViewportKind);
                        _cpuBlitContextWinForm.LoadExample(demo);

                        testBed.LoadExample(exAndDesc, demo);

                    }
                    break;

                case RenderBackendChoice.GdiPlus:
                    {
                        DemoBase demo = InitDemo(exAndDesc);
                        if (demo == null) { return; }

                        FormTestBed testBed = new FormTestBed();
                        testBed.WindowState = FormWindowState.Maximized;

                        LayoutFarm.UI.InnerViewportKind innerViewportKind = LayoutFarm.UI.InnerViewportKind.GdiPlus;
                        LayoutFarm.UI.FormCanvasHelper.CreateConvasControlOnExistingControl(
                            testBed.GetLandingControl(),
                            0, 0, 800, 600,
                            innerViewportKind,
                            out LayoutFarm.UI.UISurfaceViewportControl surfaceViewport
                            );

                        testBed.Show();

                        _cpuBlitContextWinForm = new CpuBlitContextWinForm();
                        _cpuBlitContextWinForm.BindSurface(surfaceViewport, innerViewportKind);
                        _cpuBlitContextWinForm.LoadExample(demo);

                        testBed.LoadExample(exAndDesc, demo);

                    }
                    break;
                case RenderBackendChoice.AggOnGLES:
                    {
                        DemoBase demo = InitDemo(exAndDesc);
                        if (demo == null) { return; }

                        FormTestBed testBed = new FormTestBed();
                        testBed.WindowState = FormWindowState.Maximized;


                        LayoutFarm.UI.FormCanvasHelper.CreateConvasControlOnExistingControl(
                            testBed.GetLandingControl(),
                            0, 0, 800, 600,
                            LayoutFarm.UI.InnerViewportKind.AggOnGLES,
                            out LayoutFarm.UI.UISurfaceViewportControl surfaceViewport
                            );


                        testBed.Show();
                        testBed.LoadExample(exAndDesc, demo);

                        GLDemoContextWinForm glbaseDemo = new GLDemoContextWinForm();
                        glbaseDemo.AggOnGLES = true;
                        glbaseDemo.LoadGLControl(surfaceViewport.GetOpenTKControl());
                        glbaseDemo.LoadSample(demo);
                        testBed.FormClosing += (s2, e2) =>
                        {
                            glbaseDemo.CloseDemo();
                        };

                    }
                    break;
                case RenderBackendChoice.OpenGLES: //gles 2 and 3
                    {
                        DemoBase demo = InitDemo(exAndDesc);
                        if (demo == null) { return; }

                        FormTestBed testBed = new FormTestBed();
                        testBed.WindowState = FormWindowState.Maximized;

                        //--------------------------------------------
                        LayoutFarm.UI.FormCanvasHelper.CreateConvasControlOnExistingControl(
                          testBed.GetLandingControl(),
                          0, 0, 800, 600,
                          LayoutFarm.UI.InnerViewportKind.GLES,
                          out LayoutFarm.UI.UISurfaceViewportControl surfaceViewport
                          );


                        GLDemoContextWinForm glbaseDemo = new GLDemoContextWinForm();
                        glbaseDemo.AggOnGLES = false;
                        glbaseDemo.LoadGLControl(surfaceViewport.GetOpenTKControl());
                        glbaseDemo.LoadSample(demo);
                        testBed.FormClosing += (s2, e2) =>
                        {
                            glbaseDemo.CloseDemo();
                        };
                    }
                    break;
                case RenderBackendChoice.OpenGLES_OnFormTestBed:
                    {
                        //create demo
                        DemoBase demo = InitDemo(exAndDesc);
                        if (demo == null) { return; }


                        //create form
                        FormGLTest formGLTest = new FormGLTest();
                        formGLTest.Text = exAndDesc.ToString();
                        formGLTest.Show();
                        //---------------------- 
                        //get target control that used to present the example
                        OpenTK.MyGLControl control = formGLTest.InitMiniGLControl(800, 600);
                        GLDemoContextWinForm glbaseDemo = new GLDemoContextWinForm();
                        glbaseDemo.LoadGLControl(control);
                        glbaseDemo.LoadSample(demo);
                        //----------------------
                        formGLTest.FormClosing += (s2, e2) =>
                        {
                            glbaseDemo.CloseDemo();
                        };


                        formGLTest.WindowState = FormWindowState.Maximized;




                        //////create form
                        //FormGLTest formGLTest = new FormGLTest();
                        //formGLTest.Text = exAndDesc.ToString();
                        //formGLTest.Show();
                        ////---------------------- 
                        ////get target control that used to present the example
                        //OpenTK.MyGLControl control = formGLTest.InitMiniGLControl(800, 600);
                        //{
                        //    GLDemoContextWinForm glbaseDemo = new GLDemoContextWinForm();
                        //    glbaseDemo.AggOnGLES = true;
                        //    glbaseDemo.LoadGLControl(control);
                        //    glbaseDemo.LoadSample(exBase);
                        //    //----------------------
                        //    formGLTest.FormClosing += (s2, e2) =>
                        //    {
                        //        glbaseDemo.CloseDemo();
                        //    };
                        //}
                        //formGLTest.WindowState = FormWindowState.Maximized;
                    }
                    break;

#if SKIA_ENABLE
                case RenderBackendChoice.SkiaMemoryBackend:
                    {
                        TestSkia1.FormSkia1 formSkia = new TestSkia1.FormSkia1();
                        formSkia.SelectBackend(TestSkia1.FormSkia1.SkiaBackend.Memory);
                        formSkia.Show();
                        formSkia.LoadExample(exAndDesc);
                    }
                    break;
                case RenderBackendChoice.SkiaGLBackend:
                    {
                        TestSkia1.FormSkia1 formSkia = new TestSkia1.FormSkia1();
                        formSkia.SelectBackend(TestSkia1.FormSkia1.SkiaBackend.GLES);
                        formSkia.Show();
                        formSkia.LoadExample(exAndDesc);
                    }
                    break;
#endif
                default:
                    throw new NotSupportedException();
            }
        }
        static void LoadSamplesFromAssembly(Type srcType, List<ExampleAndDesc> outputList)
        {
            //load examples
            Type[] allTypes = srcType.Assembly.GetTypes();
            Type exBase = typeof(Mini.DemoBase);
            int j = allTypes.Length;
            for (int i = 0; i < j; ++i)
            {
                Type t = allTypes[i];
                if (exBase.IsAssignableFrom(t) && t != exBase)
                {
                    ExampleAndDesc ex = new ExampleAndDesc(t, t.Name);
                    outputList.Add(ex);
                }
            }
        }

        void DevForm_Load(object sender, EventArgs e)
        {

            List<ExampleAndDesc> exlist = new List<ExampleAndDesc>();
            LoadSamplesFromAssembly(this.GetType(), exlist);
            LoadSamplesFromAssembly(typeof(GLDemoContext), exlist);

            //-------
            exlist.Sort((ex1, ex2) =>
            {
                return ex1.OrderCode.CompareTo(ex2.OrderCode);
            });
            this.listBox1.Items.Clear();
            int j = exlist.Count;
            for (int i = 0; i < j; ++i)
            {
                this.listBox1.Items.Add(exlist[i]);
            }
        }


        private void button2_Click(object sender, EventArgs e)
        {
            using (Bitmap bmp = new Bitmap("d:\\WImageTest\\test002.png"))
            {
                //MatterHackers.StackBlur2.FastBlur32RGBA(bmp, 15);

                var rct = new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height);
                //assign dimension info and copy buffer 
                var bitmapData = bmp.LockBits(rct, System.Drawing.Imaging.ImageLockMode.ReadWrite, bmp.PixelFormat);
                int bmpStride = bitmapData.Stride;
                int width = bmp.Width;
                int height = bmp.Height;
                int wh = width * height;
                //var dest = new int[wh];
                //var source = new int[wh];

                var source = new int[width * height];
                var dest = new int[width * height];
                Marshal.Copy(bitmapData.Scan0, source, 0, source.Length);
                PixelFarm.CpuBlit.Imaging.StackBlurARGB.FastBlur32ARGB(source, dest, width, height, 15);
                Marshal.Copy(dest, 0, bitmapData.Scan0, dest.Length);
                bmp.UnlockBits(bitmapData);
                bmp.Save("d:\\WImageTest\\test002_2.png");
            }
        }


        private void button5_Click(object sender, EventArgs e)
        {
            FormGdiTest formGdiTest = new FormGdiTest();
            formGdiTest.Show();
        }
        private void button6_Click(object sender, EventArgs e)
        {
            FormGLTest formGLTest = new FormGLTest();
            formGLTest.InitMiniGLControl(800, 600);
            formGLTest.Show();
            formGLTest.WindowState = FormWindowState.Maximized;
        }




        private void cmdSignedDistance_Click(object sender, EventArgs e)
        {
            double d1 = FindDistance(7, 10, 0, 0, 5, 5);
            double d2 = FindDistance(1, 1, 0, 0, 5, 5);
            double d3 = FindDistance(3, 1, 0, 0, 5, 5);
        }


        /// <summary>
        /// find (perpendicular) distance from point(x0,y0) to 
        /// a line that pass through point (x1,y1) and (x2,y2)
        /// </summary>
        /// <param name="x0"></param>
        /// <param name="y0"></param>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <returns></returns>
        static double FindDistance(double x0, double y0,
            double x1, double y1,
            double x2, double y2)
        {
            //https://en.wikipedia.org/wiki/Distance_from_a_point_to_a_line

            double upperEq = Math.Abs((((y2 - y1) * x0) - ((x2 - x1) * y0) + (x2 * y1) - (y2 * x1)));
            double lowerEq = Math.Sqrt(((y2 - y1) * (y2 - y1)) + ((x2 - x1) * (x2 - x1)));
            return upperEq / lowerEq;
        }

        private void cmdFreeTransform_Click(object sender, EventArgs e)
        {

            PixelFarm.CpuBlit.Imaging.FreeTransform freeTx = new PixelFarm.CpuBlit.Imaging.FreeTransform();
            ActualBitmap img = LoadImage("Samples\\lion1.png");


            freeTx.Interpolation = PixelFarm.CpuBlit.Imaging.FreeTransform.InterpolationMode.None;// PixelFarm.Agg.Imaging.FreeTransform.InterpolationMode.Bilinear;
            freeTx.SetFourCorners(
                new PixelFarm.VectorMath.PointF(0, 0),
                new PixelFarm.VectorMath.PointF(img.Width / 2, 0),
                new PixelFarm.VectorMath.PointF(img.Width, img.Height),
                new PixelFarm.VectorMath.PointF(0, img.Height)
            );

            ActualBitmap transformImg = freeTx.GetTransformedBitmap(img);

            SaveImage(transformImg, "d:\\WImageTest\\test01_tx.png");
        }
        static void SaveImage(ActualBitmap img, string filename)
        {
            Bitmap newBmp = new Bitmap(img.Width, img.Height);
            PixelFarm.CpuBlit.Imaging.BitmapHelper.CopyToGdiPlusBitmapSameSize(img, newBmp);
            newBmp.Save("d:\\WImageTest\\test01_tx.png");
        }
        static ActualBitmap LoadImage(string filename)
        {
            //read sample image
            using (System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(filename))
            {
                //read to image buffer 
                int bmpW = bmp.Width;
                int bmpH = bmp.Height;
                ActualBitmap img = new ActualBitmap(bmpW, bmpH);
                PixelFarm.CpuBlit.Imaging.BitmapHelper.CopyFromGdiPlusBitmapSameSizeTo32BitsBuffer(bmp, img);
                return img;
            }
        }
    }
}


