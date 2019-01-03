//MIT, 2014-present, WinterDev
using PixelFarm.Drawing;
using PixelFarm.CpuBlit.Imaging;
using System;

namespace PaintFx.Effects
{
    public interface ICpuBlitImgFilter
    {
        void SetTarget(PixelFarm.CpuBlit.PixelProcessing.BitmapBlenderBase targt);
    }
    public abstract class CpuBlitImgFilter : PixelFarm.Drawing.IImageFilter, ICpuBlitImgFilter
    {
        protected PixelFarm.CpuBlit.PixelProcessing.BitmapBlenderBase _target;
        public abstract void Apply();
        public void SetTarget(PixelFarm.CpuBlit.PixelProcessing.BitmapBlenderBase target)
        {
            _target = target;
        }
    }
    public class ImgFilterStackBlur : CpuBlitImgFilter
    {
        StackBlur _stackBlur = new StackBlur();
        public int RadiusX { get; set; } = 1;
        public int RadiusY { get; set; } = 1;
        public override void Apply()
        {
            _stackBlur.Blur(_target, RadiusX, RadiusY);
        }

    }
    //public class ImgFilterRecursiveBlur : CpuBlitImgFilter
    //{
    //    RecursiveBlur m_recursive_blur;
    //    /// <summary>
    //    /// pixels
    //    /// </summary>
    //    public double Radius { get; set; } = 1;
    //    public override void Apply()
    //    {
    //        if (m_recursive_blur == null) m_recursive_blur = new RecursiveBlur(new RecursiveBlurCalcRGB());
    //        //----------
    //        m_recursive_blur.Blur(_target, Radius);
    //    }

    //}



    public class ImgFilterSharpen : CpuBlitImgFilter
    {
        SharpenRenderer _shRenderer1 = new SharpenRenderer();
        /// <summary>
        /// pixels
        /// </summary>
        public int Radius { get; set; } = 1;
        public override void Apply()
        {
            unsafe
            {

                PixelFarm.CpuBlit.PixelProcessing.BitmapBlenderBase img = this._target;

                using (TempMemPtr bufferPtr = img.GetBufferPtr())
                {
                    int[] output = new int[bufferPtr.LengthInBytes / 4]; //TODO: review here again

                    fixed (int* outputPtr = &output[0])
                    {
                        byte* srcBuffer = (byte*)bufferPtr.Ptr;
                        int* srcBuffer1 = (int*)srcBuffer;
                        int* outputBuffer1 = (int*)outputPtr;
                        int stride = img.Stride;
                        int w = img.Width;
                        int h = img.Height;

                        MemHolder srcMemHolder = new MemHolder((IntPtr)srcBuffer1, bufferPtr.LengthInBytes / 4);//
                        Surface srcSurface = new Surface(stride, w, h, srcMemHolder);
                        //
                        MemHolder destMemHolder = new MemHolder((IntPtr)outputPtr, bufferPtr.LengthInBytes / 4);
                        Surface destSurface = new Surface(stride, w, h, destMemHolder);
                        //

                        _shRenderer1.Amount = Radius;
                        _shRenderer1.Render(srcSurface, destSurface, new PixelFarm.Drawing.Rectangle[]{
                            new PixelFarm.Drawing.Rectangle(0,0,w,h)
                        }, 0, 1);
                    }

                    //ActualImage.SaveImgBufferToPngFile(output, img.Stride, img.Width + 1, img.Height + 1, "d:\\WImageTest\\test_1.png");
                    img.WriteBuffer(output);
                }
            }
        }
    }

}