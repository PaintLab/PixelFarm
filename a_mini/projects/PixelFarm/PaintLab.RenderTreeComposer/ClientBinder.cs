//BSD, 2014-2017, WinterDev

using System;
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
        protected override void OnImageChanged()
        {
            base.OnImageChanged();
        }
    }
}