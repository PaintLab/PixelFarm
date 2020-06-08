//Apache2, 2014-present, WinterDev

using System;
using PixelFarm.Drawing;

namespace LayoutFarm
{

    public static class GlobalTextService
    {

        static ITextService s_textServices;
        public static ITextService TextService
        {
            get => s_textServices;
            set
            {
#if DEBUG
                if (s_textServices != null)
                {

                }
#endif
                s_textServices = value;

            }
        }
    }
}
