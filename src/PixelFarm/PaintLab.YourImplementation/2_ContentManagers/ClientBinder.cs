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
    public class ClientImageBinderWithScale : ClientImageBinder
    {
        float _scale;
        public ClientImageBinderWithScale(float scale)
            : base(null)
        {
            _scale = scale;

        }
        public ClientImageBinderWithScale(string src, float scale)
            : base(src)
        {
            _scale = scale;
        }
        public override void SetImage(Image image)
        {
            if (image != null)
            {
                base.SetImage(image.CreateAnother(_scale, _scale));
            }
        }
        public float Scale
        {
            get { return _scale; }
        }
        protected override void OnImageChanged()
        {
            base.OnImageChanged();
        }
    }
}