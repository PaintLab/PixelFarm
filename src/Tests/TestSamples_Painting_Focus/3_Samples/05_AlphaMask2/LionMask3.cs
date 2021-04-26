//BSD, 2014-present, WinterDev
//MatterHackers

#define USE_CLIPPING_ALPHA_MASK

using System;

using PixelFarm.CpuBlit.VertexProcessing;
using PixelFarm.CpuBlit.PixelProcessing;
using PixelFarm.Drawing;


using Mini;

namespace PixelFarm.CpuBlit.Sample_LionAlphaMask
{


    [Info(OrderCode = "05")]
    [Info(DemoCategory.Bitmap, "Clipping to multiple rectangle regions")]
    public class LionAlphaMask3 : DemoBase
    {
        int _maskAlphaSliderValue = 100;


        double _angle = 0;
        double _lionScale = 1.0;
        double _skewX = 0;
        double err_skewY = 0;
        bool _isMaskSliderValueChanged = true;

        MemBitmap lionImg;
        MemBitmap _alphaBitmap;

        public LionAlphaMask3()
        {

            string imgFileName = "Data/lion1.png";

            if (System.IO.File.Exists(imgFileName))
            {
                lionImg = MemBitmapExt.LoadBitmap(imgFileName);
            }


            this.Width = 800;
            this.Height = 600;

#if USE_CLIPPING_ALPHA_MASK
            //alphaMask = new AlphaMaskByteClipped(alphaMaskImageBuffer, 1, 0);
#else
            //alphaMask = new AlphaMaskByteUnclipped(alphaMaskImageBuffer, 1, 0);
#endif

        }
        public override void Init()
        {
        }
        void SetupMaskPixelBlender(int width, int height)
        {
            //----------
            //same size
            _alphaBitmap = new MemBitmap(width, height);
            var alphaPainter = AggPainter.Create(_alphaBitmap, new PixelBlenderBGRA());
            alphaPainter.Clear(Color.Black);
            //------------ 

            System.Random randGenerator = new Random(1432);
            int i;
            int num = (int)_maskAlphaSliderValue;
            num = 50;

            int elliseFlattenStep = 64;
            using (Tools.BorrowVxs(out var v1))
            using (Tools.BorrowEllipse(out var ellipseForMask))
            {
                for (i = 0; i < num; i++)
                {

                    if (i == num - 1)
                    {
                        ////for the last one 
                        ellipseForMask.Set(Width / 2, (Height / 2) - 90, 110, 110, elliseFlattenStep);
                        ellipseForMask.MakeVxs(v1);
                        alphaPainter.FillColor = new Color(255, 255, 255, 0);
                        alphaPainter.Fill(v1);
                        v1.Clear();
                        //
                        ellipseForMask.Set(ellipseForMask.originX, ellipseForMask.originY, ellipseForMask.radiusX - 10, ellipseForMask.radiusY - 10, elliseFlattenStep);
                        ellipseForMask.MakeVxs(v1);
                        alphaPainter.FillColor = new Color(255, 255, 0, 0);
                        alphaPainter.Fill(v1);
                        v1.Clear();
                        //
                    }
                    else
                    {
                        ellipseForMask.Set(randGenerator.Next() % width,
                                 randGenerator.Next() % height,
                                 randGenerator.Next() % 100 + 20,
                                 randGenerator.Next() % 100 + 20,
                                 elliseFlattenStep);
                        ellipseForMask.MakeVxs(v1);
                        alphaPainter.FillColor = new Color(100, 255, 0, 0);
                        alphaPainter.Fill(v1);
                        v1.Clear();
                    }
                }
            }



            maskPixelBlender.SetMaskBitmap(_alphaBitmap);
            maskPixelBlenderPerCompo.SetMaskBitmap(_alphaBitmap);
        }


        [DemoConfig(MinValue = 0, MaxValue = 255)]
        public int MaskAlphaSliderValue
        {
            get
            {
                return _maskAlphaSliderValue;
            }
            set
            {
                _maskAlphaSliderValue = value;
                _isMaskSliderValueChanged = true;
            }
        }


        PixelBlenderWithMask maskPixelBlender = new PixelBlenderWithMask();
        PixelBlenderPerColorComponentWithMask maskPixelBlenderPerCompo = new PixelBlenderPerColorComponentWithMask();

        public override void Draw(Painter p)
        {
            AggPainter painter = p as AggPainter;
            if (painter == null) return;
            //
            painter.Clear(Color.White);

            int width = painter.Width;
            int height = painter.Height;
            //change value ***
            if (_isMaskSliderValueChanged)
            {
                SetupMaskPixelBlender(width, height);
                _isMaskSliderValueChanged = false;
                //
                //painter.DestBitmapBlender.OutputPixelBlender = maskPixelBlender; //change to new blender
                painter.DestBitmapBlender.OutputPixelBlender = maskPixelBlenderPerCompo; //change to new blender
            }
            //1. alpha mask...
            //p2.DrawImage(alphaBitmap, 0, 0);

            //2.
            painter.FillColor = Color.Black;
            painter.FillRect(0, 0, 200, 100);

            //3.
            painter.FillColor = Color.Blue;
            painter.FillCircle(300, 300, 100);
            painter.DrawImage(lionImg, 20, 20);
            painter.EnableMask = true;


            ////4.
            //painter.FillColor = Color.Black;
            ////this test lcd-effect => we need to draw it 3 times with different color component, on the same position
            ////(same as we do with OpenGLES rendering surface)
            //maskPixelBlenderPerCompo.SelectedMaskComponent = PixelBlenderColorComponent.B;
            //maskPixelBlenderPerCompo.EnableOutputColorComponent = EnableOutputColorComponent.B;
            //painter.FillRect(0, 0, 200, 100);
            //maskPixelBlenderPerCompo.SelectedMaskComponent = PixelBlenderColorComponent.G;
            //maskPixelBlenderPerCompo.EnableOutputColorComponent = EnableOutputColorComponent.G;
            //painter.FillRect(0, 0, 200, 100);
            //maskPixelBlenderPerCompo.SelectedMaskComponent = PixelBlenderColorComponent.R;
            //maskPixelBlenderPerCompo.EnableOutputColorComponent = EnableOutputColorComponent.R;
            //painter.FillRect(0, 0, 200, 100);

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
            _angle = Math.Atan2(y, x);
            _lionScale = Math.Sqrt(y * y + x * x) / 100.0;
        }
    }


}
