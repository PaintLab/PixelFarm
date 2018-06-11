//BSD, 2014-2018, WinterDev
//MatterHackers

#define USE_CLIPPING_ALPHA_MASK

using System;
using PixelFarm.Agg.Transform;
using PixelFarm.Agg.Imaging;
using Mini;
using PixelFarm.Drawing.WinGdi;
using PixelFarm.Drawing;

namespace PixelFarm.Agg.Sample_LionAlphaMask
{


    [Info(OrderCode = "05")]
    [Info(DemoCategory.Bitmap, "Clipping to multiple rectangle regions")]
    public class LionAlphaMask2 : DemoBase
    {
        int maskAlphaSliderValue = 100;

        SpriteShape lionShape;
        double angle = 0;
        double lionScale = 1.0;
        double skewX = 0;
        double skewY = 0;
        bool isMaskSliderValueChanged = true;

        ActualBitmap lionImg;
        ActualBitmap alphaBitmap;
        ActualBitmap glyphAtlasBmp;

        public LionAlphaMask2()
        {

            string imgFileName = "Data/lion1.png";

            if (System.IO.File.Exists(imgFileName))
            {
                lionImg = DemoHelper.LoadImage(imgFileName);
            }

            string glyphBmp = @"D:\projects\PixelFarm\src\Tests\Debug\tahoma -488129008.info.png";
            if (System.IO.File.Exists(glyphBmp))
            {
                glyphAtlasBmp = DemoHelper.LoadImage(glyphBmp);
            }



            lionShape = new SpriteShape(SvgRenderVxLoader.CreateSvgRenderVxFromFile("Samples/arrow2.svg"));
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

        void SetupMaskPixelBlender(int width, int height)
        {
            //----------
            //same size
            alphaBitmap = new ActualBitmap(width, height);
            var alphaPainter = AggPainter.Create(alphaBitmap, new PixelBlenderBGRA());
            alphaPainter.Clear(Color.Black);
            //------------ 

            System.Random randGenerator = new Random(1432);
            int i;
            int num = (int)maskAlphaSliderValue;
            num = 50;

            int elliseFlattenStep = 64;
            //VectorToolBox.GetFreeVxs(out var v1);
            //VertexSource.Ellipse ellipseForMask = new PixelFarm.Agg.VertexSource.Ellipse(); 
            //for (i = 0; i < num; i++)
            //{

            //    if (i == num - 1)
            //    {
            //        ////for the last one 
            //        ellipseForMask.Reset(Width / 2, (Height / 2) - 90, 110, 110, elliseFlattenStep);
            //        ellipseForMask.MakeVertexSnap(v1);
            //        alphaPainter.FillColor = new Color(255, 255, 255, 0);
            //        alphaPainter.Fill(v1);
            //        v1.Clear();
            //        //
            //        ellipseForMask.Reset(ellipseForMask.originX, ellipseForMask.originY, ellipseForMask.radiusX - 10, ellipseForMask.radiusY - 10, elliseFlattenStep);
            //        ellipseForMask.MakeVertexSnap(v1);
            //        alphaPainter.FillColor = new Color(255, 255, 0, 0);
            //        alphaPainter.Fill(v1);
            //        v1.Clear();
            //        //
            //    }
            //    else
            //    {
            //        ellipseForMask.Reset(randGenerator.Next() % width,
            //                 randGenerator.Next() % height,
            //                 randGenerator.Next() % 100 + 20,
            //                 randGenerator.Next() % 100 + 20,
            //                 elliseFlattenStep);
            //        ellipseForMask.MakeVertexSnap(v1);
            //        alphaPainter.FillColor = new Color(255, 255, 0, 0);
            //        alphaPainter.Fill(v1);
            //        v1.Clear();
            //    }
            //}
            //VectorToolBox.ReleaseVxs(ref v1);

            alphaPainter.DrawImage(glyphAtlasBmp, 0, 0);

            maskPixelBlender.SetMaskImage(alphaBitmap);
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

        [DemoConfig]
        public PixelBlenderWithMask.ColorComponent SelectedComponent
        {
            get
            {
                if (maskPixelBlender != null)
                {
                    return maskPixelBlender.SelectedMaskComponent;
                }
                else
                {
                    return PixelBlenderWithMask.ColorComponent.R;//default
                }
            }
            set
            {
                isMaskSliderValueChanged = true;
                maskPixelBlender.SelectedMaskComponent = value;

            }
        }

        PixelBlenderWithMask maskPixelBlender = new PixelBlenderWithMask();
        public override void Draw(Painter p)
        {
            if (p is GdiPlusPainter)
            {
                return;
            }

            //
            AggPainter painter = (AggPainter)p;
            painter.Clear(Color.White);

            int width = painter.Width;
            int height = painter.Height;
            //change value ***
            if (isMaskSliderValueChanged)
            {
                SetupMaskPixelBlender(width, height);
                this.isMaskSliderValueChanged = false;
                //
                painter.DestBitmapBlender.OutputPixelBlender = maskPixelBlender; //change to new blender
            }
            //1. alpha mask...
            //p2.DrawImage(alphaBitmap, 0, 0);               

            painter.FillColor = Color.Black;
            painter.FillRect(0, 0, 200, 100);


            //painter.FillColor = Color.Blue;
            //painter.FillCircle(300, 300, 100);
            //painter.DrawImage(lionImg, 20, 20);

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
    }


}
