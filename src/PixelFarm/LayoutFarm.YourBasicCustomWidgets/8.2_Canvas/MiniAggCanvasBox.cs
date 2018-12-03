//Apache2, 2014-present, WinterDev

using LayoutFarm.UI;
namespace LayoutFarm.CustomWidgets
{
    public class MiniAggCanvasBox : AbstractRectUI
    {
        MiniAggCanvasRenderElement _canvasRenderElement;
        public MiniAggCanvasBox(int w, int h)
            : base(w, h)
        {
        }

        public PixelFarm.Drawing.Painter Painter => _canvasRenderElement.Painter;
        public override RenderElement CurrentPrimaryRenderElement => _canvasRenderElement;
        protected override bool HasReadyRenderElement => _canvasRenderElement != null;

        public override RenderElement GetPrimaryRenderElement(RootGraphic rootgfx)
        {
            if (_canvasRenderElement == null)
            {
                var canvas = new MiniAggCanvasRenderElement(rootgfx, this.Width, this.Height);
                canvas.HasSpecificHeight = this.HasSpecificHeight;
                canvas.HasSpecificWidth = this.HasSpecificWidth;
                canvas.SetLocation(this.Left, this.Top);
                canvas.Painter.StrokeWidth = 1;
                canvas.Painter.StrokeColor = PixelFarm.Drawing.Color.Black;
                _canvasRenderElement = canvas;
                canvas.SetController(this);
            }
            return _canvasRenderElement;
        }
        protected void InvalidateCanvasContent()
        {
            _canvasRenderElement.InvalidateCanvasContent();
        }
        public override void Walk(UIVisitor visitor)
        {
            visitor.BeginElement(this, "canvas");
            this.Describe(visitor);
            visitor.EndElement();
        }
    }
}