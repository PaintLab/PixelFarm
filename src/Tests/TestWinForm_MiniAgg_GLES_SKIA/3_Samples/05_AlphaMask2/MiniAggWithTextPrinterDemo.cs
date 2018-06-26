//BSD, 2018-present, WinterDev 

using Mini;
using PixelFarm.Drawing;
using PixelFarm.Drawing.Fonts;
using Typography.Rendering;
namespace PixelFarm.CpuBlit.Sample_LionAlphaMask
{


    [Info(OrderCode = "00")]
    public class MiniAggWithTextPrinterDemo : DemoBase
    {

        bool _fontAtlasPrinterReady;
        //FontAtlasTextPrinter _printer;
        DevTextPrinterBase _printer;

        public MiniAggWithTextPrinterDemo()
        {
            this.Width = 800;
            this.Height = 600;
        }
        public override void Init()
        {
        }
        public void DrawString(Painter p, string text, double x, double y)
        {
            if (text != null)
            {
                AggPainter painter = p as AggPainter;
                if (painter == null) return;
                //
                DrawString(painter, text.ToCharArray(), 0, text.Length, x, y);
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
            _printer = new VxsTextPrinter2(p);
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
        public override void Draw(Painter p)
        {
            AggPainter painter = p as AggPainter;
            if (painter == null) return;
            if (!_fontAtlasPrinterReady)
            {
                SetupFontAtlasPrinter(painter);
            }

            p.RenderQuality = RenderQualtity.HighQuality;
            p.Orientation = DrawBoardOrientation.LeftBottom;

            //clear the image to white         
            // draw a circle
            p.Clear(Drawing.Color.White);
            p.FillColor = Color.Black;


            int lineSpaceInPx = (int)painter.CurrentFont.LineSpacingInPx;
            int ypos = 0;


            DrawString(p, "iiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiii", 10, ypos);
            ypos += lineSpaceInPx;
            //--------  

            p.FillColor = Color.Green;
            DrawString(p, "Hello World", 10, ypos);
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
