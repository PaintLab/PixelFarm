//BSD, 2014-present, WinterDev

namespace LayoutFarm
{
    class MyClientImageBinder : ImageBinder
    {
        UI.IUIEventListener _listener;
        public MyClientImageBinder(string src)
            : base(src)
        {
        }
        protected override void RaiseImageChanged()
        {
            if (_listener != null)
            {
                _listener.HandleContentUpdate();
            }
        }
        public void SetOwner(UI.IUIEventListener listener)
        {
            this._listener = listener;
        }
    }
}