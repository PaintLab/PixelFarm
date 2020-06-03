//Apache2, 2014-present, WinterDev

using System;
using PixelFarm.Drawing;

namespace LayoutFarm
{

    public static class GlobalTextService
    {

        static ITextService _textServices;
        public static ITextService TextService
        {
            get => _textServices;
            set
            {
#if DEBUG
                if (_textServices != null)
                {

                }
#endif
                _textServices = value;

            }
        }
    }
}
