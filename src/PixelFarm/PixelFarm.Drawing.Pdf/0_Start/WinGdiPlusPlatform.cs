//BSD, 2014-2018, WinterDev 
 
using Typography.TextServices;
namespace PixelFarm.Drawing.Pdf
{
    public static class PdfPlaform
    {

        static PdfPlaform()
        {

            //PixelFarm.Agg.AggBuffMx.SetNaiveBufferImpl(new Win32AggBuffMx());
            //3. set default encoding
            // WinGdiTextService.SetDefaultEncoding(System.Text.Encoding.ASCII);
        }

        public static void SetFontEncoding(System.Text.Encoding encoding)
        {
            //WinGdiTextService.SetDefaultEncoding(encoding);
        }
        public static void SetFontLoader(IFontLoader fontLoader)
        {
            // WinGdiFontFace.SetFontLoader(fontLoader);
        }
    }



}