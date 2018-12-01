//BSD, 2018-present, WinterDev  
using System;

using PixelFarm.Drawing;
using PixelFarm.CpuBlit.PixelProcessing;

using Mini;

namespace PixelFarm.CpuBlit.Sample_LionAlphaMask
{


    [Info(OrderCode = "00")]
    public class FontTextureDemo : DemoBase
    {


        MemBitmap _alphaBitmap;
        MemBitmap _glyphAtlasBmp;
        PixelBlenderWithMask maskPixelBlender = new PixelBlenderWithMask();
        PixelBlenderPerColorComponentWithMask maskPixelBlenderPerCompo = new PixelBlenderPerColorComponentWithMask();
        bool _maskReady;
        public FontTextureDemo()
        {
            string glyphBmp = @"Data\tahoma -488129008.info.png";
            if (System.IO.File.Exists(glyphBmp))
            {
                _glyphAtlasBmp = DemoHelper.LoadImage(glyphBmp);
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
            _alphaBitmap = new MemBitmap(width, height);
            var maskBufferPainter = AggPainter.Create(_alphaBitmap, new PixelBlenderBGRA());
            maskBufferPainter.Clear(Color.Black);
            //------------  
            maskBufferPainter.DrawImage(_glyphAtlasBmp, 0, 0);

            maskPixelBlender.SetMaskBitmap(_alphaBitmap);
            maskPixelBlenderPerCompo.SetMaskBitmap(_alphaBitmap);
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


        public override void Draw(Painter p)
        {
            AggPainter painter = p as AggPainter;
            if (painter == null) return;
            //

            painter.Clear(Color.White);

            int width = painter.Width;
            int height = painter.Height;
            //change value ***

            if (!_maskReady)
            {
                SetupMaskPixelBlender(width, height);
                _maskReady = true;
            }

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
            //
            maskPixelBlenderPerCompo.SelectedMaskComponent = PixelBlenderColorComponent.G;
            maskPixelBlenderPerCompo.EnableOutputColorComponent = EnableOutputColorComponent.G;
            painter.FillRect(0, 0, 200, 100);
            //
            maskPixelBlenderPerCompo.SelectedMaskComponent = PixelBlenderColorComponent.R;
            maskPixelBlenderPerCompo.EnableOutputColorComponent = EnableOutputColorComponent.R;
            painter.FillRect(0, 0, 200, 100);
        }

    }



}
