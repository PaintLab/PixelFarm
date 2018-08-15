//BSD, 2014-present, WinterDev

namespace LayoutFarm
{
    public class ClientImageBinder : ImageBinder
    {
        public ClientImageBinder()
            : base(null)
        {
        }
        public ClientImageBinder(string src)
            : base(src)
        {
        }
        protected override void RaiseImageChanged()
        {
            base.RaiseImageChanged();
        }
    }

}