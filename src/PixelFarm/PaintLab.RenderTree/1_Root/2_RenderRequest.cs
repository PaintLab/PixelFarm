//Apache2, 2014-present, WinterDev

namespace LayoutFarm.RenderBoxes
{
    public readonly struct RenderElementRequest
    {
        public readonly RenderElement renderElem;
        public readonly RequestCommand req;
        public readonly object parameters;

        public RenderElementRequest(RenderElement ve, RequestCommand req)
        {
            this.renderElem = ve;
            this.req = req;
            this.parameters = null;
        }
        public RenderElementRequest(RenderElement ve, RequestCommand req,
            object parameters)
        {
            this.renderElem = ve;
            this.req = req;
            this.parameters = parameters;
        }
    }
    public enum RequestCommand
    {
        DoFocus,
        AddToWindowRoot,
        InvalidateArea,
        NotifySizeChanged,
        //
        ProcessFormattedString,
    }
}