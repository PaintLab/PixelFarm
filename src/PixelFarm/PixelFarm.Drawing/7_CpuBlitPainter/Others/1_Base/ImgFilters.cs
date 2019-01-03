//BSD, 2014-present, WinterDev
using PixelFarm.Drawing;
using PixelFarm.CpuBlit.Imaging;

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
        ShapenFilterPdn pdnSharpen;
        /// <summary>
        /// pixels
        /// </summary>
        public int Radius { get; set; } = 1;
        public override void Apply()
        {
            if (pdnSharpen == null) pdnSharpen = new ShapenFilterPdn();
            pdnSharpen.Sharpen(_target, Radius);
        }
    }

}