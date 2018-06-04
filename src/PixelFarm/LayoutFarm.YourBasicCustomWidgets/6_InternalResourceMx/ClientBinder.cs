//BSD, 2014-2018, WinterDev

namespace LayoutFarm
{
    class MyClientImageBinder : ImageBinder
    {
        UI.IUIEventListener listener;
        public MyClientImageBinder(string src)
            : base(src)
        {
        }
        protected override void OnImageChanged()
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