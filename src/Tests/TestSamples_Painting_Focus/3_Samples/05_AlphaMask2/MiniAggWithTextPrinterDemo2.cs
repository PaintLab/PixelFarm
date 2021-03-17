//MIT, 2018-present, WinterDev 

using PixelFarm.Drawing;
using PixelFarm.CpuBlit.BitmapAtlas;

using Typography.Text;
using Mini;
namespace PixelFarm.CpuBlit.TextPrinterDev
{


    [Info(OrderCode = "00")]
    public class MiniAggWithTextPrinterDemo2 : DemoBase
    {


        TextPlatePrintHouse _printHouse = new TextPlatePrintHouse();
        FontAtlasTextPrinter _fontAtlasTextPrinter;
        bool _fontAtlasPrinterReady;
        public MiniAggWithTextPrinterDemo2()
        {
            this.Width = 800;
            this.Height = 600;
            _printHouse.Setup(800, 600);

        }
        public override void Init()
        {
        }
        //void DrawString(AggPainter p, string text, double x, double y)
        //{
        //    if (text != null)
        //    {
        //        DrawString(p, text.ToCharArray(), 0, text.Length, x, y);
        //    }
        //}

        void SetupFontAtlasPrinter(AggPainter p)
        {
            //use custom printer here

            //in this example we have 2 text printer
            //1. 
            if (_fontAtlasTextPrinter == null)
            {
                _fontAtlasTextPrinter = new FontAtlasTextPrinter(p);
                _fontAtlasTextPrinter.ChangeFont(p.CurrentFont);

                p.TextPrinter = _fontAtlasTextPrinter;
            }
            _fontAtlasPrinterReady = true;
            _printHouse.SetFont(p.CurrentFont);
        }

        System.Diagnostics.Stopwatch _sw1 = new System.Diagnostics.Stopwatch();

        System.Collections.Generic.List<RenderVxFormattedString> _stringList;
        public override void Draw(Painter painter)
        {
            if (!(painter is AggPainter p)) return;
            //Print1(p);
            //----------------

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

            _sw1.Reset();
            _sw1.Start();

            int lineSpaceInPx = 18;// p.CurrentLineSpaceHeight;
            if (_stringList == null)
            {
                //init once
                _stringList = new System.Collections.Generic.List<RenderVxFormattedString>();
                for (int ty = 0; ty < 400; ++ty)
                {
                    for (int tx = 0; tx < 20; ++tx)
                    {
                        //DrawString(p, "Hello World" + ty, xpos, ypos);
                        RenderVxFormattedString fmtstr = _printHouse.CreateRenderVxFormattedString("Hello Word" + ty);
                        _stringList.Add(fmtstr);
                    }
                }
            }

            int ypos = 0;
            _sw1.Stop();
            long ms1 = _sw1.ElapsedMilliseconds;

            _sw1.Reset();
            _sw1.Start();

            p.FillColor = Color.Blue;
            ypos = 0;
            int mm = 0;

            for (int ty = 0; ty < 400; ++ty)
            {
                int xpos = 10;
                for (int tx = 0; tx < 20; ++tx)
                {
                    AggRenderVxFormattedString fmtstr = (AggRenderVxFormattedString)_stringList[mm];
                    if (fmtstr.IsDelay)
                    {
                        if (fmtstr.State == RenderVxFormattedString.VxState.NoStrip)
                        {
                            _printHouse.PrepareFormattedString(fmtstr);
                        }
                    }

                    p.DrawString(fmtstr, xpos, ypos);

                    xpos += 50;
                    mm++;
                }
                ypos += lineSpaceInPx;
            }

            _sw1.Stop();

            long ms2 = _sw1.ElapsedMilliseconds;
        }


        class TextPlatePrintHouse
        {
            MemBitmap _plate;
            AggPainter _painter;
            FontAtlasTextPrinter _printer;
            public TextPlatePrintHouse()
            {

            }


            public RenderVxFormattedString CreateRenderVxFormattedString(string str, bool delay = false)
            {
                return _painter.CreateRenderVx(str, delay);
            }
            public void PrepareFormattedString(AggRenderVxFormattedString fmtstr)
            {
                _printer.PrepareStringForRenderVx(fmtstr);
            }
            public void SetFont(RequestFont font)
            {
                _painter.CurrentFont = font;
            }
            public void Setup(int w, int h)
            {
                if (_plate != null) { return; }
                _plate = new MemBitmap(w, h);
                AggRenderSurface renderSx = new AggRenderSurface();
                renderSx.AttachDstBitmap(_plate);
                _painter = new AggPainter(renderSx);
                _painter.CurrentFont = new RequestFont("Source Sans Pro", 10);
                _painter.Clear(Color.White);
                //

                _printer = new FontAtlasTextPrinter(_painter);
                _painter.TextPrinter = _printer;
            }
        }
    }
}
