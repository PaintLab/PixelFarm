//BSD, 2018-present, WinterDev 

using Mini;
using PixelFarm.Drawing;
using PixelFarm.Drawing.Fonts;
using Typography.Contours;

namespace PixelFarm.CpuBlit.Sample_LionAlphaMask
{


    [Info(OrderCode = "00")]
    public class MiniAggWithTextPrinterDemo : DemoBase
    {

        bool _fontAtlasPrinterReady;
        FontAtlasTextPrinter _fontAtlasTextPrinter;
        VxsTextPrinter _vxsTextPrinter;

        TextPrinterBase _printer;
        LayoutFarm.OpenFontTextService _openFontTextServices;

        public MiniAggWithTextPrinterDemo()
        {
            this.Width = 800;
            this.Height = 600;
        }
        public override void Init()
        {
        }
        void DrawString(AggPainter p, string text, double x, double y)
        {
            if (text != null)
            {
                DrawString(p, text.ToCharArray(), 0, text.Length, x, y);
            }
        }

        bool _useFontAtlas;

        [DemoConfig]
        public bool UseFontAtlas
        {
            get { return _useFontAtlas; }
            set
            {
                _useFontAtlas = value;
                _printer = (_useFontAtlas) ?
                    (TextPrinterBase)_fontAtlasTextPrinter :
                    _vxsTextPrinter;
                this.NeedRedraw = true;
            }
        }

        [DemoConfig]
        public AntialiasTechnique AntialiasTechnique
        {
            get
            {
                if (_printer == null)
                {
                    return AntialiasTechnique.LcdStencil;
                }
                else
                {
                    return AntialiasTechnique.LcdStencil;
                    //return _printer.AntialiasTech;
                }
            }
            set
            {
                //_printer.AntialiasTech = value;
                this.NeedRedraw = true;
            }
        }

        void SetupFontAtlasPrinter(AggPainter p)
        {
            //use custom printer here
            //_printer = new FontAtlasTextPrinter(p);
            if (_fontAtlasTextPrinter == null)
            {
                _fontAtlasTextPrinter = new FontAtlasTextPrinter(p);
            }

            if (_vxsTextPrinter == null)
            {
                _openFontTextServices = new LayoutFarm.OpenFontTextService();
                _vxsTextPrinter = new VxsTextPrinter(p, _openFontTextServices);
            }

            _printer = (_useFontAtlas) ?
                    (TextPrinterBase)_fontAtlasTextPrinter :
                    _vxsTextPrinter;

            _fontAtlasPrinterReady = true;
        }
        public void DrawString(AggPainter painter, char[] buffer, int startAt, int len, double x, double y)
        {
            if (!_fontAtlasPrinterReady)
            {
                SetupFontAtlasPrinter(painter);
            }

            _printer.DrawString(buffer, startAt, len, (float)x, (float)y);
        }
        public override void Draw(Painter painter)
        {
            AggPainter p = painter as AggPainter;
            if (p == null) return;
            if (!_fontAtlasPrinterReady)
            {
                SetupFontAtlasPrinter(p);
            }

            p.RenderQuality = RenderQuality.HighQuality;
            p.Orientation = DrawBoardOrientation.LeftBottom;

            //clear the image to white         
            // draw a circle
            p.Clear(Drawing.Color.Yellow);
            p.FillColor = Color.Black;


            int lineSpaceInPx = (int)p.CurrentFont.LineSpacingInPixels;
            int ypos = 0;


            DrawString(p, "iiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiii", 10, ypos);
            ypos += lineSpaceInPx;
            //--------  

            p.FillColor = Color.Black;
            if (_useFontAtlas)
            {
                DrawString(p, "Hello World from FontAtlasTextPrinter", 10, ypos);
            }
            else
            {
                DrawString(p, "Hello World from VxsTextPrinter", 10, ypos);
            }

            ypos += lineSpaceInPx;

            p.FillColor = Color.Blue;
            DrawString(p, "Hello World", 10, ypos);
            ypos += lineSpaceInPx;

            p.FillColor = Color.Red;
            DrawString(p, "Hello World", 10, ypos);
            ypos += lineSpaceInPx;

            p.FillColor = Color.Yellow;
            DrawString(p, "Hello World", 10, ypos);
            ypos += lineSpaceInPx;

            p.FillColor = Color.Gray;
            DrawString(p, "Hello World", 10, ypos);
            ypos += lineSpaceInPx;

            p.FillColor = Color.Black;
            DrawString(p, "Hello World", 10, ypos);
            ypos += lineSpaceInPx;
        }
    }
}
