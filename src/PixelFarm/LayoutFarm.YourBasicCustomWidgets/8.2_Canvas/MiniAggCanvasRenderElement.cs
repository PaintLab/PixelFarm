//Apache2, 2014-present, WinterDev

using System;
using PixelFarm.CpuBlit;
using PixelFarm.Drawing;
namespace LayoutFarm.CustomWidgets
{
    public class MiniAggCanvasRenderElement : RenderBoxBase, IDisposable
    {

        Painter _painter;
        bool _needUpdate;
        MemBitmap _memBmp;
        Image _bmp;

        public MiniAggCanvasRenderElement(RootGraphic rootgfx, int width, int height)
            : base(rootgfx, width, height)
        {

            _memBmp = new MemBitmap(width, height);
#if DEBUG
            _memBmp._dbugNote = "MiniAggCanvasRenderElement";
#endif
            _painter = AggPainter.Create(_memBmp);
            _needUpdate = true;
            this.BackColor = Color.White;
        }
        public override void ClearAllChildren()
        {
        }
        public Color BackColor { get; set; }
        protected override void DrawBoxContent(DrawBoard canvas, Rectangle updateArea)
        {

            if (_needUpdate)
            {
                //default bg => transparent !, 
                //gfx2d.Clear(ColorRGBA.White);//if want opaque bg
                //ReleaseUnmanagedResources();
                //if (bmp != null)
                //{
                //    bmp.Dispose();
                //}

                _bmp = _memBmp;// new Bitmap(this.Width, this.Height, this.actualImage.GetBuffer(), false);
                // canvas.Platform.CreatePlatformBitmap(this.Width, this.Height, this.actualImage.GetBuffer(), false);
                Image.ClearCache(_bmp);

                _needUpdate = false;
            }
            //canvas.FillRectangle(this.BackColor, 0, 0, this.Width, this.Height);

            if (_bmp != null)
            {
                canvas.DrawImage(this._bmp, new RectangleF(0, 0, this.Width, this.Height));
            }
            //---------------------



#if DEBUG
            //canvasPage.dbug_DrawCrossRect(PixelFarm.Drawing.Color.Black,
            //    new Rectangle(0, 0, this.Width, this.Height));
#endif
        }
        void ReleaseUnmanagedResources()
        {
            //-------------------------
            //TODO: review this again 
            //about resource mx
            //------------------------- 
            //if (bmp != null)
            //{
            //    bmp.Dispose();
            //    bmp = null;
            //}
            //if (currentGdiPlusBmp != null)
            //{
            //    currentGdiPlusBmp.Dispose();
            //    currentGdiPlusBmp = null;
            //}
        }
        void IDisposable.Dispose()
        {
            ReleaseUnmanagedResources();
        }


        public Painter Painter => _painter;
        public void InvalidateCanvasContent()
        {
            _needUpdate = true;
            this.InvalidateGraphics();
        }


    }
}