//BSD, 2018-present, WinterDev 



using System;
using Mini;
using PixelFarm.Drawing;
using PixelFarm.CpuBlit.PixelProcessing;


namespace PixelFarm.CpuBlit.Sample_LionAlphaMask
{
    [Info(OrderCode = "00")]
    public class FontTextureDemo3 : DemoBase
    {


        MemBitmap _alphaBitmap;
        MemBitmap _glyphAtlasBmp;
        PixelBlenderWithMask _maskPixelBlender = new PixelBlenderWithMask();
        PixelBlenderPerColorComponentWithMask _maskPixelBlenderPerCompo = new PixelBlenderPerColorComponentWithMask();
        bool _maskReady;
        public FontTextureDemo3()
        {
            string glyphBmp = @"Data\tahoma -488129008.info.png";
            if (System.IO.File.Exists(glyphBmp))
            {
                _glyphAtlasBmp = MemBitmap.LoadBitmap(glyphBmp);
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

            _maskPixelBlender.SetMaskBitmap(_alphaBitmap);
            _maskPixelBlenderPerCompo.SetMaskBitmap(_alphaBitmap);
        }
        [DemoConfig]
        public PixelProcessing.PixelBlenderColorComponent SelectedComponent
        {
            get
            {
                if (_maskPixelBlender != null)
                {
                    return _maskPixelBlender.SelectedMaskComponent;
                }
                else
                {
                    return PixelProcessing.PixelBlenderColorComponent.R;//default
                }
            }
            set
            {
                _maskPixelBlender.SelectedMaskComponent = value;
                _maskPixelBlenderPerCompo.SelectedMaskComponent = value;
                this.InvalidateGraphics();
            }
        }
        public override void Draw(Painter p)
        {
            AggPainter painter = p as AggPainter;
            if (painter == null) return;

            painter.Clear(Color.White);

            int width = painter.Width;
            int height = painter.Height;


            //switch to alpha mask
            painter.TargetBufferName = TargetBufferName.AlphaMask;
            //draw white rect on the mask
            painter.FillColor = Color.White;
            painter.FillRect(20, 20, 50, 50);
            painter.FillRect(20, 5, 20, 10);

            //------------------------------------
            //switch back to default color
            painter.TargetBufferName = TargetBufferName.Default;
            painter.FillColor = Color.Red;
            //enable mask composite
            painter.EnableBuiltInMaskComposite = true;
            painter.FillRect(0, 0, 100, 100);
            //disable mask buffer
            painter.EnableBuiltInMaskComposite = false;
            painter.FillColor = Color.Yellow;
            painter.FillRect(0, 0, 20, 20);

            //if (!_maskReady)
            //{
            //    SetupMaskPixelBlender(width, height);
            //    _maskReady = true;
            //}

            ////
            ////painter.DestBitmapBlender.OutputPixelBlender = maskPixelBlender; //change to new blender
            //painter.DestBitmapBlender.OutputPixelBlender = maskPixelBlenderPerCompo; //change to new blender 

            ////4.
            //painter.FillColor = Color.Black;
            ////this test lcd-effect => we need to draw it 3 times with different color component, on the same position
            ////(same as we do with OpenGLES rendering surface)
            //maskPixelBlenderPerCompo.SelectedMaskComponent = PixelBlenderColorComponent.B;
            //maskPixelBlenderPerCompo.EnableOutputColorComponent = EnableOutputColorComponent.B;
            //painter.FillRect(0, 0, 200, 100);
            ////
            //maskPixelBlenderPerCompo.SelectedMaskComponent = PixelBlenderColorComponent.G;
            //maskPixelBlenderPerCompo.EnableOutputColorComponent = EnableOutputColorComponent.G;
            //painter.FillRect(0, 0, 200, 100);
            ////
            //maskPixelBlenderPerCompo.SelectedMaskComponent = PixelBlenderColorComponent.R;
            //maskPixelBlenderPerCompo.EnableOutputColorComponent = EnableOutputColorComponent.R;
            //painter.FillRect(0, 0, 200, 100);
        }

    }
}