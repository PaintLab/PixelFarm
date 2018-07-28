//BSD, 2014-present, WinterDev

namespace LayoutFarm
{
    class MyClientImageBinder : ImageBinder
    {
        UI.IUIEventListener listener;
        public MyClientImageBinder(string src)
            : base(src)
        {
        }
        protected override void RaiseImageChanged()
        {
            if (listener != null)
            {
                listener.HandleContentUpdate();
            }
        }
        public void SetOwner(UI.IUIEventListener listener)
        {
            this.listener = listener;
        }
    }
}