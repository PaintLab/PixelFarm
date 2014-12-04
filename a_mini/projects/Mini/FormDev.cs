//2014 BSD, WinterDev

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;

using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using PixelFarm.Agg;

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

            //test native font


        }
        void listBox1_DoubleClick(object sender, EventArgs e)
        {
            //load sample form
            ExampleAndDesc exAndDesc = this.listBox1.SelectedItem as ExampleAndDesc;
            if (exAndDesc != null)
            {
                FormTestBed1 testBed = new FormTestBed1();
                testBed.WindowState = FormWindowState.Maximized;
                testBed.Show();
                testBed.LoadExample(exAndDesc);
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


            //test01
            var lionShape = new PixelFarm.Agg.SpriteShape();
            lionShape.ParseLion();

            //test path serialize to binary stream
            System.Diagnostics.Debugger.Break();

            using (var fs = new System.IO.FileStream("..\\lion_stream.bin", System.IO.FileMode.Create))
            {
                var writer = new System.IO.BinaryWriter(fs);


                //1. all coords and commands
                PixelFarm.Agg.VertexSource.VertexSourceIO.WriteToStream(
                    writer,
                    lionShape.Path);

                //2. colors
                PixelFarm.Agg.VertexSource.VertexSourceIO.WriteColorsToStream(
                   writer, lionShape.Colors
                   );
                //---------------------------------------
                //3. num paths, & path index 
                int npath = lionShape.NumPaths;
                PixelFarm.Agg.VertexSource.VertexSourceIO.WritePathIndexListToStream(
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
                PixelFarm.Agg.ColorRGBA[] colors;
                int[] pathIndexList;

                //1. path and command
                PixelFarm.Agg.VertexSource.VertexSourceIO.ReadPathDataFromStream(
                  reader, out path
                  );
                //2. colors
                PixelFarm.Agg.VertexSource.VertexSourceIO.ReadColorDataFromStream(
                  reader, out colors
                  );
                //3. path indice
                int npaths;
                PixelFarm.Agg.VertexSource.VertexSourceIO.ReadPathIndexListFromStream(
                  reader, out npaths, out pathIndexList
                 );

                PixelFarm.Agg.SpriteShape.UnsafeDirectSetData(
                     lionShape2,
                     npaths,
                     path, colors, pathIndexList);

                fs.Close();
            }
            //------------
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
                PixelFarm.Agg.Image.StackBlurARGB.FastBlur32ARGB(source, dest, width, height, 15);
                Marshal.Copy(dest, 0, bitmapData.Scan0, dest.Length);

                bmp.UnlockBits(bitmapData);

                bmp.Save("d:\\WImageTest\\test002_2.png");
            }
        }

        private void cmdTestRasterImage_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            //----------------------
            //1. test gdi+ font path
            string teststr = "Q";
            float fontSize = 24;

            using (System.Drawing.Font ff = new Font("tahoma", fontSize))
            using (GraphicsPath gpath = new GraphicsPath())
            using (Graphics g = this.pictureBox1.CreateGraphics())
            {
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.Clear(Color.White);
                gpath.AddString(teststr, ff.FontFamily, 0, fontSize, new Point(0, 0), null);
                //-----------------------------
                //get font shape from gpath
                int pointCount = gpath.PointCount;
                byte[] pointTypes = gpath.PathTypes;
                PointF[] points = gpath.PathPoints;
                //from MSDN document
                //0 = start of figure (MoveTo)
                //1 = one of the two endpoints of a line (LineTo)
                //3 = an endpoint or control point of a cubic Bezier spline (4 points spline)
                //masks..
                //0x7 = 111b (binary) => for masking lower 3 bits
                //0x20 = (1<<6) specific that point is a marker
                //0x80 = (1<<7) specific that point is the last point of a closed subpath( figure)

                //----------------------------------
                //convert to Agg's VertexStorage  
                VertexStore vxs = new VertexStore();
                int curvePointCount = 0;

                for (int i = 0; i < pointCount; ++i)
                {
                    byte pointType = pointTypes[i];
                    PointF p = points[i];

                    switch (0x7 & pointType)
                    {
                        case 0:
                            //move to
                            vxs.AddMoveTo(p.X, p.Y);
                            curvePointCount = 0;
                            break;
                        case 1:
                            //line to
                            vxs.AddLineTo(p.X, p.Y);
                            curvePointCount = 0;
                            break;
                        case 3:
                            //end point of control point of cubic Bezier spline
                            {
                                switch (curvePointCount)
                                {
                                    case 0:
                                        {
                                            vxs.AddP2c(p.X, p.Y);
                                            curvePointCount++;
                                        } break;
                                    case 1:
                                        {
                                            vxs.AddP3c(p.X, p.Y);
                                            curvePointCount++;
                                        } break;
                                    case 2:
                                        {
                                            vxs.AddLineTo(p.X, p.Y);
                                            curvePointCount = 0;//reset
                                        } break;
                                    default:
                                        {
                                            throw new NotSupportedException();
                                        }
                                }

                            } break;
                        default:
                            {

                            } break;
                    }

                    if ((pointType >> 7) == 1)
                    {
                        //close figure to
                        vxs.AddCloseFigure();
                    }
                    if ((pointType >> 6) == 1)
                    {

                    }
                }
                //-----------------------------
                //convert Agg vxs to bitmap
                int bmpW = 50;
                int bmpH = 50;
                using (Bitmap bufferBmp = new Bitmap(bmpW, bmpH))
                {
                    ActualImage actualImage = new ActualImage(bmpW, bmpH, PixelFarm.Agg.Image.PixelFormat.Rgba32);
                    Graphics2D gfx = Graphics2D.CreateFromImage(actualImage);
                    gfx.Render(vxs, ColorRGBA.Black);
                    //convert to bmp 
                    BitmapHelper.CopyToWindowsBitmap(
                      actualImage, //src from actual img buffer
                      bufferBmp, //dest to buffer bmp
                     new RectInt(0, 0, bmpW, bmpH));
                    //-----------------------------------------
                    bufferBmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
                    g.DrawImage(bufferBmp, new Point(0, 30));
                }
                //-----------------------------

                //g.DrawPath(Pens.Black, gpath);
                g.FillPath(Brushes.Black, gpath);
                g.DrawString(teststr, ff, Brushes.Black, new PointF(0, 50));
            } 
        }
    }
}
