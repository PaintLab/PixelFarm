//2014-2016 BSD, WinterDev

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Text;

using PixelFarm.Agg;
using System.IO;
using Microsoft.Win32;

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
            GdiPlus,
            OpenGLES2,
            SkiaMemoryBackend,
            SkiaGLBackend,
        }
        void LoadRenderBackendChoices()
        {
            cmbRenderBackend.Items.Clear();
            cmbRenderBackend.Items.Add(RenderBackendChoice.PureAgg); //pure software renderer with MiniAgg
            cmbRenderBackend.Items.Add(RenderBackendChoice.GdiPlus);
            cmbRenderBackend.Items.Add(RenderBackendChoice.OpenGLES2);
            cmbRenderBackend.Items.Add(RenderBackendChoice.SkiaMemoryBackend);
            cmbRenderBackend.Items.Add(RenderBackendChoice.SkiaGLBackend);
            cmbRenderBackend.SelectedIndex = 2;//set default 
        }

        void listBox1_DoubleClick(object sender, EventArgs e)
        {
            //load sample form
            ExampleAndDesc exAndDesc = this.listBox1.SelectedItem as ExampleAndDesc;
            if (exAndDesc != null)
            {
                switch ((RenderBackendChoice)cmbRenderBackend.SelectedItem)
                {
                    case RenderBackendChoice.PureAgg:
                        {
                            FormTestBed1 testBed = new FormTestBed1();
                            testBed.WindowState = FormWindowState.Maximized;
                            testBed.UseGdiPlusOutput = false;
                            testBed.UseGdiAntiAlias = chkGdiAntiAlias.Checked;
                            testBed.Show();
                            testBed.LoadExample(exAndDesc);
                        }
                        break;
                    case RenderBackendChoice.GdiPlus:
                        {
                            FormTestBed1 testBed = new FormTestBed1();
                            testBed.WindowState = FormWindowState.Maximized;
                            testBed.UseGdiPlusOutput = true;
                            testBed.UseGdiAntiAlias = chkGdiAntiAlias.Checked;
                            testBed.Show();
                            testBed.LoadExample(exAndDesc);
                        }
                        break;
                    case RenderBackendChoice.OpenGLES2:
                        {
                            FormGLTest formGLTest = new FormGLTest();
                            formGLTest.InitGLControl();
                            formGLTest.Show();
                            formGLTest.WindowState = FormWindowState.Maximized;
                            formGLTest.LoadExample(exAndDesc);
                        }
                        break;
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
                    default:
                        throw new NotSupportedException();
                }

            }
        }
        void DevForm_Load(object sender, EventArgs e)
        {
            //load examples
            Type[] allTypes = this.GetType().Assembly.GetTypes();
            Type exBase = typeof(Mini.DemoBase);
            int j = allTypes.Length;
            List<ExampleAndDesc> exlist = new List<ExampleAndDesc>();
            for (int i = 0; i < j; ++i)
            {
                Type t = allTypes[i];
                if (exBase.IsAssignableFrom(t) && t != exBase)
                {
                    ExampleAndDesc ex = new ExampleAndDesc(t, t.Name);
                    exlist.Add(ex);
                }
            }
            //-------
            exlist.Sort((ex1, ex2) =>
            {
                return ex1.OrderCode.CompareTo(ex2.OrderCode);
            });
            this.listBox1.Items.Clear();
            j = exlist.Count;
            for (int i = 0; i < j; ++i)
            {
                this.listBox1.Items.Add(exlist[i]);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //--------------
#if DEBUG

            //test01
            var lionShape = new PixelFarm.Agg.SpriteShape();
            lionShape.ParseLion();
            //test path serialize to binary stream
            System.Diagnostics.Debugger.Break();
            using (var fs = new System.IO.FileStream("..\\lion_stream.bin", System.IO.FileMode.Create))
            {
                var writer = new System.IO.BinaryWriter(fs);
                //1. all coords and commands
                PixelFarm.Agg.VertexSource.dbugVertexSourceIO.WriteToStream(
                    writer,
                    lionShape.Path);
                //2. colors
                PixelFarm.Agg.VertexSource.dbugVertexSourceIO.WriteColorsToStream(
                   writer, lionShape.Colors
                   );
                //---------------------------------------
                //3. num paths, & path index 
                int npath = lionShape.NumPaths;
                PixelFarm.Agg.VertexSource.dbugVertexSourceIO.WritePathIndexListToStream(
                  writer, lionShape.PathIndexList,
                  npath
                  );
                writer.Close();
                fs.Close();
            }
            //--------------
            //test load path from binary stream
            using (var fs = new System.IO.FileStream("..\\lion_stream.bin", System.IO.FileMode.Open))
            {
                var reader = new System.IO.BinaryReader(fs);
                var lionShape2 = new PixelFarm.Agg.SpriteShape();
                PixelFarm.Agg.VertexSource.PathWriter path;
                PixelFarm.Drawing.Color[] colors;
                int[] pathIndexList;
                //1. path and command
                PixelFarm.Agg.VertexSource.dbugVertexSourceIO.ReadPathDataFromStream(
                  reader, out path
                  );
                //2. colors
                PixelFarm.Agg.VertexSource.dbugVertexSourceIO.ReadColorDataFromStream(
                  reader, out colors
                  );
                //3. path indice
                int npaths;
                PixelFarm.Agg.VertexSource.dbugVertexSourceIO.ReadPathIndexListFromStream(
                  reader, out npaths, out pathIndexList
                 );
                PixelFarm.Agg.SpriteShape.UnsafeDirectSetData(
                     lionShape2,
                     npaths,
                     path, colors, pathIndexList);
                fs.Close();
            }
            //------------
#endif
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (Bitmap bmp = new Bitmap("d:\\WImageTest\\test002.png"))
            {
                //MatterHackers.StackBlur2.FastBlur32RGBA(bmp, 15);

                var rct = new Rectangle(0, 0, bmp.Width, bmp.Height);
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
                PixelFarm.Agg.Imaging.StackBlurARGB.FastBlur32ARGB(source, dest, width, height, 15);
                Marshal.Copy(dest, 0, bitmapData.Scan0, dest.Length);
                bmp.UnlockBits(bitmapData);
                bmp.Save("d:\\WImageTest\\test002_2.png");
            }
        }

        private void cmdTestRasterImage_Click(object sender, EventArgs e)
        {
        }

        //PixelFarm.Drawing.Fonts.GdiPathFontStore gdiPathFontStore = new PixelFarm.Drawing.Fonts.GdiPathFontStore();
        private void button3_Click(object sender, EventArgs e)
        {
            ////----------------------
            ////1. test gdi+ font path
            //char testChar = 'b';
            //float fontSize = 20;
            //string fontName = "tahoma";
            //using (System.Drawing.Font ff = new Font(fontName, fontSize))
            //using (Graphics g = this.pictureBox1.CreateGraphics())
            //{
            //    g.SmoothingMode = SmoothingMode.HighQuality;
            //    g.Clear(System.Drawing.Color.White);


            //    PixelFarm.Drawing.Fonts.ActualFont winFont = gdiPathFontStore.LoadFont(fontName, fontSize);

            //    var winFontGlyph = winFont.GetGlyph(testChar);
            //    //convert Agg vxs to bitmap
            //    int bmpW = 50;
            //    int bmpH = 50;
            //    using (Bitmap bufferBmp = new Bitmap(bmpW, bmpH))
            //    {
            //        ActualImage actualImage = new ActualImage(bmpW, bmpH, PixelFarm.Agg.Image.PixelFormat.ARGB32);
            //        Graphics2D gfx = Graphics2D.CreateFromImage(actualImage, Program._winGdiPlatForm);
            //        var vxs = winFontGlyph.originalVxs;
            //        gfx.Render(vxs, PixelFarm.Drawing.Color.Black);
            //        //test subpixel rendering 
            //        vxs = PixelFarm.Agg.Transform.Affine.TranslateToVxs(vxs, 15, 0);
            //        gfx.UseSubPixelRendering = true;
            //        gfx.Render(vxs, PixelFarm.Drawing.Color.Black);
            //        PixelFarm.Agg.Image.BitmapHelper.CopyToWindowsBitmap(
            //          actualImage, //src from actual img buffer
            //          bufferBmp, //dest to buffer bmp
            //         new RectInt(0, 0, bmpW, bmpH));
            //        //-----------------------------------------
            //        bufferBmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
            //        g.DrawImage(bufferBmp, new Point(0, 30));
            //    }
            //    //----------------------------------------------



            //    //----------------------------------------------
            //    //compare with GraphicsPath's Font                
            //    using (GraphicsPath gpath = new GraphicsPath())
            //    {
            //        gpath.AddString(testChar.ToString(), ff.FontFamily, 1, ff.Size,
            //            new Point(0, 0), null);
            //        g.FillPath(Brushes.Black, gpath);
            //        //g.DrawPath(Pens.Black, gpath);
            //    }
            //    //-------------------------------------------------
            //    //Compare with Gdi+ Font
            //    g.DrawString(testChar.ToString(), ff, Brushes.Black, new PointF(0, 50));
            //}
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //----------------------
            //1. test gdi+ font path
            char testChar = 'b';
            float fontSize = 20;

            using (Graphics g = this.pictureBox1.CreateGraphics())
            {
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.Clear(System.Drawing.Color.White);
                //convert Agg vxs to bitmap
                int bmpW = 500;
                int bmpH = 500;
                using (Bitmap bufferBmp = new Bitmap(bmpW, bmpH))
                {
                    ActualImage actualImage = new ActualImage(bmpW, bmpH, PixelFarm.Agg.PixelFormat.ARGB32);
                    Graphics2D gfx = Graphics2D.CreateFromImage(actualImage);
                    var vxs = new VertexStore();
                    //vxs.AddMoveTo(0, 0);
                    ////vxs.AddP3c(100, 0);
                    ////vxs.AddP3c(100,150);
                    ////vxs.AddLineTo(0,0);
                    //vxs.AddLineTo(0, 0);
                    //vxs.AddP3c(100, 0);
                    ////vxs.AddLineTo(100, 0);
                    ////vxs.AddLineTo(100, 150);
                    //vxs.AddP3c(100, 150);
                    //vxs.AddLineTo(0, 150);
                    //vxs.AddCloseFigure();

                    //PixelFarm.Agg.VertexSource.CurveFlattener cflat = new PixelFarm.Agg.VertexSource.CurveFlattener();
                    //vxs = cflat.MakeVxs(vxs);

                    gfx.Render(vxs, PixelFarm.Drawing.Color.Black);
                    //test subpixel rendering 

                    vxs = PixelFarm.Agg.Transform.Affine.TranslateToVxs(vxs, 15, 0, new VertexStore());
                    gfx.UseSubPixelRendering = true;
                    gfx.Render(vxs, PixelFarm.Drawing.Color.Black);
                    PixelFarm.Agg.Imaging.BitmapHelper.CopyToGdiPlusBitmapSameSize(
                      actualImage, //src from actual img buffer
                      bufferBmp //dest to buffer bmp
                     );
                    //-----------------------------------------
                    bufferBmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
                    g.DrawImage(bufferBmp, new Point(0, 30));
                }
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
            formGLTest.InitGLControl();
            formGLTest.Show();
            formGLTest.WindowState = FormWindowState.Maximized;
        }
        private void button7_Click(object sender, EventArgs e)
        {

            var installFontProvider = new PixelFarm.Drawing.WinGdi.InstallFontsProviderWin32();
            List<PixelFarm.Drawing.Fonts.InstalledFont> fonts = PixelFarm.Drawing.Fonts.InstalledFontCollection.ReadPreviewFontData(installFontProvider.GetInstalledFontIter());
            System.Drawing.Bitmap bmp1 = new Bitmap(800, 600, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            PixelFarm.Drawing.WinGdi.GdiPlusCanvasPainter p = new PixelFarm.Drawing.WinGdi.GdiPlusCanvasPainter(bmp1);

        }

        private void cmdTestNativeLib_Click(object sender, EventArgs e)
        {
            //#if DEBUG
            //            PixelFarm.Drawing.Text.dbugTestMyFtLib.Test1();
            //#endif
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

    }
}


