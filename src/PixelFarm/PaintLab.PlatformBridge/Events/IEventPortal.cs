//Apache2, 2014-present, WinterDev

namespace LayoutFarm.UI
{
    public interface IEventPortal
    {
        //-------------------------------------------- 
        void PortalKeyPress(UIKeyEventArgs e);
        void PortalKeyDown(UIKeyEventArgs e);
        void PortalKeyUp(UIKeyEventArgs e);
        bool PortalProcessDialogKey(UIKeyEventArgs e);
        //----------------------------------------------

        void PortalMouseDown(UIMouseDownEventArgs e);
        void PortalMouseMove(UIMouseMoveEventArgs e);
        void PortalMouseUp(UIMouseUpEventArgs e);
        void PortalMouseWheel(UIMouseWheelEventArgs e);
        //---------------------------------------------- 
        void PortalGotFocus(UIFocusEventArgs e);
        void PortalLostFocus(UIFocusEventArgs e);
        //---------------------------------------------- 
    }
}