
//BSD 2014, WinterDev

namespace MatterHackers.Agg.UI
{
    public static class DoubleRectHelper
    {
        public static void Inflate(ref RectangleDouble r, BorderDouble borderDouble)
        {
            r.Left -= borderDouble.Left;
            r.Right += borderDouble.Right;
            r.Bottom -= borderDouble.Bottom;
            r.Top += borderDouble.Top;
        }

        public static void Deflate(ref RectangleDouble r, BorderDouble borderDouble)
        {
            r.Left += borderDouble.Left;
            r.Right -= borderDouble.Right;
            r.Bottom += borderDouble.Bottom;
            r.Top -= borderDouble.Top;
        }

    }
}