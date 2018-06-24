//BSD, 2014-present, WinterDev
//MatterHackers

#define USE_CLIPPING_ALPHA_MASK

using System;
using Mini;
using PixelFarm.Drawing.WinGdi;
using PixelFarm.Drawing;
using PixelFarm.CpuBlit.PixelProcessing;


namespace PixelFarm.CpuBlit.Sample_LionAlphaMask
{


    [Info(OrderCode = "00")]
    public class FontTextureDemo : DemoBase
    {

        ActualBitmap alphaBitmap;
        ActualBitmap glyphAtlasBmp;

        public FontTextureDemo()
        {
            string glyphBmp = @"Data\tahoma -488129008.info.png";
            if (System.IO.File.Exists(glyphBmp))
            {
                glyphAtlasBmp = DemoHelper.LoadImage(glyphBmp);
            }
            this.Width = 800;
            this.Height = 600;

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

            alphaPainter.DrawImage(glyphAtlasBmp, 0, 0);

            maskPixelBlender.SetMaskImage(alphaBitmap);
            maskPixelBlenderPerCompo.SetMaskImage(alphaBitmap);
        }



        [DemoConfig]
        public PixelProcessing.PixelBlenderColorComponent SelectedComponent
        {
            get
            {
                if (maskPixelBlender != null)
                {
                    return maskPixelBlender.SelectedMaskComponent;
                }
                else
                {
                    return PixelProcessing.PixelBlenderColorComponent.R;//default
                }
            }
            set
            {

                maskPixelBlender.SelectedMaskComponent = value;
                maskPixelBlenderPerCompo.SelectedMaskComponent = value;
                NeedRedraw = true;
            }
        }

        PixelBlenderWithMask maskPixelBlender = new PixelBlenderWithMask();
        PixelBlenderPerColorComponentWithMask maskPixelBlenderPerCompo = new PixelBlenderPerColorComponentWithMask();

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

            SetupMaskPixelBlender(width, height);

            //
            //painter.DestBitmapBlender.OutputPixelBlender = maskPixelBlender; //change to new blender
            painter.DestBitmapBlender.OutputPixelBlender = maskPixelBlenderPerCompo; //change to new blender
             

            //4.
            painter.FillColor = Color.Black;
            //this test lcd-effect => we need to draw it 3 times with different color component, on the same position
            //(same as we do with OpenGLES rendering surface)
            maskPixelBlenderPerCompo.SelectedMaskComponent = PixelBlenderColorComponent.B;
            maskPixelBlenderPerCompo.EnableOutputColorComponent = EnableOutputColorComponent.B;
            painter.FillRect(0, 0, 200, 100);
            maskPixelBlenderPerCompo.SelectedMaskComponent = PixelBlenderColorComponent.G;
            maskPixelBlenderPerCompo.EnableOutputColorComponent = EnableOutputColorComponent.G;
            painter.FillRect(0, 0, 200, 100);
            maskPixelBlenderPerCompo.SelectedMaskComponent = PixelBlenderColorComponent.R;
            maskPixelBlenderPerCompo.EnableOutputColorComponent = EnableOutputColorComponent.R;
            painter.FillRect(0, 0, 200, 100);
        }
    }


}
