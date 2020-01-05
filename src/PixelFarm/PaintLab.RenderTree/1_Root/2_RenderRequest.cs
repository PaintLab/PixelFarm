//Apache2, 2014-present, WinterDev

namespace LayoutFarm.RenderBoxes
{
    public struct RenderElementRequest
    {
        public RenderElement ve;
        public RequestCommand req;
        public object parameters;
        public object parameters2;
        public RenderElementRequest(RenderElement ve, RequestCommand req)
        {
            this.ve = ve;
            this.req = req;
            this.parameters = null;
            this.parameters2 = null;
        }
        public RenderElementRequest(RenderElement ve, RequestCommand req,
            object parameters,
            object parameters2)
        {
            this.ve = ve;
            this.req = req;
            this.parameters = parameters;
            this.parameters2 = parameters2;
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