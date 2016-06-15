//2016 MIT, WinterDev

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using PixelFarm.Agg;
using PixelFarm.Agg.Fonts;
using PixelFarm.Agg.Image;
using PixelFarm.Agg.Transform;
namespace PixelFarm.Drawing.WinGdi
{
    public class CanvasGraphics2dGdi : Graphics2D
    {
        Graphics _g;
        //--------------------------------------------
        ScanlinePacked8 sclinePack8;
        ScanlineRasToDestBitmapRenderer sclineRasToBmp;
        ScanlineRasterizer sclineRas;
        ImageReaderWriterBase destImageReaderWriter;
        ActualImage destImage;
        public CanvasGraphics2dGdi(Graphics g)
        {
            //create from actual image
            destImage = new ActualImage(800, 600, PixelFormat.Rgba32);
            this.destActualImage = destImage;
            this.destImageReaderWriter = new MyImageReaderWriter(destImage);
            this._g = g;
            sclinePack8 = new ScanlinePacked8();
            this.sclineRas = new ScanlineRasterizer();
            this.sclineRasToBmp = new ScanlineRasToDestBitmapRenderer();
        }
        public Graphics InternalGraphics { get { return _g; } }
        public override ImageReaderWriterBase DestImage
        {
            get
            {
                return null;
            }
        }
        public override IPixelBlender PixelBlender
        {
            get
            {
                return null;
            }
            set
            {
            }
        }
        public override ScanlinePacked8 ScanlinePacked8
        {
            get
            {
                return sclinePack8;
            }
        }
        public override ScanlineRasToDestBitmapRenderer ScanlineRasToDestBitmap
        {
            get
            {
                return sclineRasToBmp;
            }
        }
        public override bool UseSubPixelRendering
        {
            get
            {
                return true;
            }
            set
            {
            }
        }

        public override void Clear(ColorRGBA color)
        {
            _g.Clear(ToGdiPlusARGBColor(color));
        }
        static System.Drawing.Color ToGdiPlusARGBColor(ColorRGBA color)
        {
            return System.Drawing.Color.FromArgb(
                color.alpha,
                color.red,
                color.green,
                color.blue
                );
        }
        public override RectInt GetClippingRect()
        {
            throw new NotImplementedException();
        }
        public override void Render(IImageReaderWriter source, AffinePlan[] affinePlans)
        {
        }
        public override void Render(VertexStoreSnap vertexSource, ColorRGBA colorBytes)
        {
            try
            {
                System.Drawing.Drawing2D.GraphicsPath p = VxsHelper.CreateGraphicsPath(vertexSource);
                using (var br = new System.Drawing.SolidBrush(ToGdiPlusARGBColor(colorBytes)))
                {
                    _g.FillPath(br, p);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public override void Render(IImageReaderWriter source, double x, double y)
        {
        }
        public override void SetClippingRect(RectInt rect)
        {
        }
    }
}