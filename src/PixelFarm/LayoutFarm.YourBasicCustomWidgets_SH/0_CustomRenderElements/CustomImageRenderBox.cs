//Apache2, 2014-present, WinterDev

using PixelFarm.Drawing;
using PixelFarm.CpuBlit;
using PixelFarm.CpuBlit.VertexProcessing;
namespace LayoutFarm.CustomWidgets
{
    public class CustomMaskBox
    {
        VertexStore _maskVxs;
        public CustomMaskBox(int width, int height) { }
        public int Width { get; set; }
        public int Height { get; set; }
        public void SetEllipseMask(int width, int height, int cx, int cy, int rx, int ry)
        {
            Width = width;
            Height = height;
            _maskVxs = new VertexStore();
            //
            using (Tools.BorrowEllipse(out Ellipse ellipse))
            {
                ellipse.Set(cx, cy, rx, ry);
                ellipse.MakeVxs(_maskVxs);
            }
        }
        public void SetRoundRectMask(int width, int height, int cornerRad)
        {
            Width = width;
            Height = height;
            _maskVxs = new VertexStore();
            using (Tools.BorrowRoundedRect(out RoundedRect rr))
            {
                rr.SetRect(0, height, width, 0);
                rr.SetRadius(cornerRad);
                ellipse.MakeVxs(_maskVxs);
            }
        }
        internal VertexStore Vxs => _maskVxs;
    }

    public class CustomImageRenderBox : CustomRenderBox
    {
        //
        public CustomImageRenderBox(int width, int height)
            : base(width, height)
        {
            this.BackColor = KnownColors.LightGray;
        }
        public override void ClearAllChildren()
        {

        }
        public ImageBinder ImageBinder { get; set; } //img source
        public CustomMaskBox MaskBox { get; set; }

        protected override void RenderClientContent(DrawBoard d, UpdateArea updateArea)
        {
            //this render element dose not have child node, so
            //if WaitForStartRenderElement == true,
            //then we skip rendering its content
            //else if this renderElement has more child, we need to walk down)

            if (WaitForStartRenderElement) { return; }

            if (ImageBinder == null) { return; }

            //----------------------------------
            switch (ImageBinder.State)
            {
                case BinderState.Loaded:
                    {
                        //may not need background

                        if (MaskBox != null)
                        {
                            //draw maskbox first
                            Painter p = d.GetPainter();
                            p.SetClipRgn(MaskBox.Vxs);
                            d.FillRectangle(this.BackColor, 0, 0, this.Width, this.Height);
                            d.DrawImage(ImageBinder,
                                new RectangleF(
                                ContentLeft, ContentTop,
                                ContentWidth,
                                ContentHeight));

                            p.SetClipRgn(null);
                        }
                        else
                        {
                            d.FillRectangle(this.BackColor, 0, 0, this.Width, this.Height);
                            d.DrawImage(ImageBinder,
                                new RectangleF(
                                ContentLeft, ContentTop,
                                ContentWidth,
                                ContentHeight));
                        }


                    }
                    break;
                case BinderState.Unload:
                    {
                        //wait next round ...
                        if (ImageBinder.HasLazyFunc)
                        {
                            ImageBinder.LazyLoadImage();
                        }
                        else if (ImageBinder is AtlasImageBinder)
                        {
                            //resolve this and draw
                            goto case BinderState.Loaded;

                            //d.DrawImage(ImageBinder,
                            //   new RectangleF(
                            //   ContentLeft, ContentTop,
                            //   ContentWidth,
                            //   ContentHeight));
                        }
                    }
                    break;
            }

#if DEBUG
            //canvasPage.dbug_DrawCrossRect(PixelFarm.Drawing.Color.Black,
            //    new Rectangle(0, 0, this.Width, this.Height));
#endif
        }
    }
}