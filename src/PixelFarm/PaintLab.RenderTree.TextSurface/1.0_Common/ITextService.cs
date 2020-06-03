//Apache2, 2014-present, WinterDev

using System;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;
using Typography.TextBreak;

namespace LayoutFarm
{
  

    public static class GlobalRootGraphic2
    {

        static ITextService2 _textServices;
        public static ITextService2 TextService
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
