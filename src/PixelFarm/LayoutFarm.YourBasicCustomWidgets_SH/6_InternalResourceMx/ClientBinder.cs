//BSD, 2014-present, WinterDev

namespace LayoutFarm
{
    class MyClientImageBinder : PixelFarm.Drawing.ImageBinder
    {
        UI.IUIEventListener _listener;
        public MyClientImageBinder(string src)
            : base(src)
        {
        }
        public override void RaiseImageChanged()
        {
            _listener?.HandleContentUpdate();
            base.RaiseImageChanged();//SET this too?
        }
        public void SetOwner(UI.IUIEventListener listener)
        {
            _listener = listener;
        }
    }
}