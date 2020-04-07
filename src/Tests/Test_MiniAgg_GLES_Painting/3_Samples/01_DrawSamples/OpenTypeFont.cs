//BSD, 2014-present, WinterDev

//MatterHackers: BSD
// Much of the ui to the drawing functions still needs to be C#'ed and cleaned up.  A lot of
// it still follows the originall agg function names.  I have been cleaning these up over time
// and intend to do much more refactoring of these things over the long term.



using System.IO;
//

using PixelFarm.CpuBlit.VertexProcessing;
using PixelFarm.Drawing;
using PixelFarm.Drawing.Fonts;
//
using Typography.OpenFont;
using Typography.Contours;
using Typography.FontManagement;


using Mini;
namespace PixelFarm.CpuBlit.Sample_Draw
{
    [Info(OrderCode = "01")]
    [Info("OpenTypeReaderFromPureCs")]
    public class OpenTypeReaderFromPureCS : DemoBase
    {


        VertexStore _left_vxs;
        VertexStore _right_vxs;

        public override void Init()
        {
            string fontfile = YourImplementation.FrameworkInitWinGDI.GetFontLoader().GetInstalledTypeface("tahoma", TypefaceStyle.Regular).FontPath;

            this.FillBG = true;
            float sizeInPts = 72; 

            using (var fs = new FileStream(fontfile, FileMode.Open, FileAccess.Read))
            {
                var reader = new OpenFontReader();
                //1. read typeface from font file
                Typeface typeFace = reader.Read(fs); 
                //2. glyph-to-vxs builder
                var builder = new GlyphOutlineBuilder(typeFace);
                _left_vxs = BuildVxsForGlyph(builder, 'p', sizeInPts);
                _right_vxs = BuildVxsForGlyph(builder, 'f', sizeInPts);
            }
        }

        VertexStore BuildVxsForGlyph(GlyphOutlineBuilder builder, char character, float size)
        {
            //-----------
            //TODO: review here
            builder.Build(character, size);
            var txToVxs = new GlyphTranslatorToVxs();
            builder.ReadShapes(txToVxs);

            VertexStore v2 = new VertexStore();
            using (Tools.BorrowVxs(out var v0))
            using (Tools.BorrowCurveFlattener(out var flattener))
            {
                txToVxs.WriteOutput(v0);

                Q1RectD bounds = v0.GetBoundingRect();

                AffineMat mat = AffineMat.Iden();
                mat.Scale(1, -1);//flipY
                mat.Translate(0, bounds.Height);

                flattener.MakeVxs(v0, mat, v2);
            }
            return v2;
        }

        [DemoConfig]
        public bool FillBG { get; set; }
        [DemoConfig]
        public bool FillBorder { get; set; }

        public override void Draw(PixelFarm.Drawing.Painter p)
        {


            //---------------- 
            //5. use PixelFarm's Agg to render to bitmap...
            //5.1 clear background
            p.Clear(PixelFarm.Drawing.Color.White);

            if (FillBG)
            {
                //5.2 
                p.FillColor = PixelFarm.Drawing.Color.Black;
                //5.3
                //p.Fill(vxs);

                float x = p.OriginX;
                float y = p.OriginY;


                p.Fill(_left_vxs);
                p.SetOrigin(x + 50, y + 50);
                p.Fill(_right_vxs);
                p.SetOrigin(x, y);
            }

            if (FillBorder)
            {
                //5.4 
                p.StrokeColor = PixelFarm.Drawing.Color.Green;
                //user can specific border width here...
                p.StrokeWidth = 2;
                //5.5  

                float x = p.OriginX;
                float y = p.OriginY;
                p.Draw(_left_vxs);
                p.SetOrigin(x + 50, y + 20);
                p.Draw(_right_vxs);
                p.SetOrigin(x, y);
            }
        }
    }

}