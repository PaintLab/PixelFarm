//BSD, 2018-present, WinterDev 

using PixelFarm.Drawing;
using PixelFarm.CpuBlit.BitmapAtlas;

using Typography.Text;
using Mini;
namespace PixelFarm.CpuBlit.TextPrinterDev
{


    [Info(OrderCode = "00")]
    public class MiniAggWithTextPrinterDemo : DemoBase
    {

        bool _fontAtlasPrinterReady;
        FontAtlasTextPrinter _fontAtlasTextPrinter;
        VxsTextSpanPrinter _vxsTextPrinter;

        AbstractTextSpanPrinter _printer;
        OpenFontTextService _openFontTextServices;

        public MiniAggWithTextPrinterDemo()
        {
            this.Width = 800;
            this.Height = 600;
            UserText = "";
            UseFontAtlas = true;
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
        public string UserText { get; set; }
        [DemoConfig]
        public bool UseFontAtlas
        {
            get => _useFontAtlas;
            set
            {
                _useFontAtlas = value;
                _printer = (_useFontAtlas) ?
                    (AbstractTextSpanPrinter)_fontAtlasTextPrinter :
                    _vxsTextPrinter;
                this.InvalidateGraphics();
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
                this.InvalidateGraphics();
            }
        }

        void SetupFontAtlasPrinter(AggPainter p)
        {
            //use custom printer here

            //in this example we have 2 text printer
            //1. 
            if (_fontAtlasTextPrinter == null)
            {
                _fontAtlasTextPrinter = new FontAtlasTextPrinter(p);
            }

            if (_vxsTextPrinter == null)
            {

                _openFontTextServices = new OpenFontTextService();

                _vxsTextPrinter = new VxsTextSpanPrinter(p, _openFontTextServices.CreateNewServiceClient());
                _vxsTextPrinter.ChangeFont(p.CurrentFont);
                _vxsTextPrinter.TextBaseline = Typography.Text.TextBaseline.Top;
            }

            _printer = (_useFontAtlas) ?
                    (AbstractTextSpanPrinter)_fontAtlasTextPrinter :
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

        System.Diagnostics.Stopwatch _sw1 = new System.Diagnostics.Stopwatch();
        public override void Draw(Painter painter)
        {
            if (!(painter is AggPainter p)) return;
            if (!_fontAtlasPrinterReady)
            {
                SetupFontAtlasPrinter(p);
            }

            p.RenderQuality = RenderQuality.HighQuality;
            p.Orientation = PixelFarm.Drawing.RenderSurfaceOriginKind.LeftBottom;

            //clear the image to white         
            // draw a circle
            p.Clear(Drawing.Color.White);
            p.FillColor = Color.Black;


            int lineSpaceInPx = p.CurrentLineSpaceHeight;
            if (lineSpaceInPx == 0)
            {
                lineSpaceInPx = 16; //tmp fix
            }
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
            _sw1.Reset();
            _sw1.Start();
            for (int ty = 0; ty < 400; ++ty)
            {
                int xpos = 10;
                for (int tx = 0; tx < 20; ++tx)
                {
                    DrawString(p, "Hello World" + ty, xpos, ypos);
                    xpos += 50;
                }

                ypos += lineSpaceInPx;
            }
            _sw1.Stop();
            long ms = _sw1.ElapsedMilliseconds;



            //DrawString(p, "Hello World", 10, ypos);
            //ypos += lineSpaceInPx;

            //p.FillColor = Color.Red;
            //DrawString(p, "Hello World", 10, ypos);
            //ypos += lineSpaceInPx;

            //p.FillColor = Color.Yellow;
            //DrawString(p, "Hello World", 10, ypos);
            //ypos += lineSpaceInPx;

            //p.FillColor = KnownColors.Gray;
            //DrawString(p, "Hello World", 10, ypos);
            //ypos += lineSpaceInPx;

            //p.FillColor = Color.Black;
            //DrawString(p, "Hello World", 10, ypos);
            //ypos += lineSpaceInPx;

            if (!string.IsNullOrEmpty(UserText))
            {
                DrawString(p, UserText, 10, ypos);
                ypos += lineSpaceInPx;
            }
        }
    }
}
