//Apache2, 2014-present, WinterDev

namespace LayoutFarm.RenderBoxes
{
    public struct RenderElementRequest
    {
        public RenderElement renderElem;
        public RequestCommand req;
        public object parameters;

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