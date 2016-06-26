//MIT 2014-2016, WinterDev 

namespace PixelFarm.DrawingGL
{
    partial class CanvasGL2d
    {
        void SetupFonts()
        {
        }
       
        public void DrawString(string str, float x, float y)
        {
            this.textPriner.Print(str.ToCharArray(), x, y);
        }
        public void DrawString(char[] buff, float x, float y)
        {
            this.textPriner.Print(buff, x, y);
        }
    }
}