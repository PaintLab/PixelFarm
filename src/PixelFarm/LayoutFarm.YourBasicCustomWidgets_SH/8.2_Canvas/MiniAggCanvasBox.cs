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

        public override RenderElement GetPrimaryRenderElement()
        {
            if (_canvasRenderElement == null)
            {
                var canvas = new MiniAggCanvasRenderElement(this.Width, this.Height); 
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
    }
}