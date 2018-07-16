//BSD, 2014-present, WinterDev

using System;
using PixelFarm.Drawing;

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