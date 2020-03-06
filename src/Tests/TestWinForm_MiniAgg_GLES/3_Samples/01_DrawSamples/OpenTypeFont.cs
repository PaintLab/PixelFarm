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
         
        CurveFlattener _curveFlattener = new CurveFlattener();
        VertexStore _left_vxs;
        VertexStore _right_vxs;
        public override void Init()
        {


            string fontfile = YourImplementation.FrameworkInitWinGDI.GetFontLoader().GetInstalledTypeface("tahoma", TypefaceStyle.Regular).FontPath;

            this.FillBG = true;
            int size = 72;
            int resolution = 72;
            char testChar = 'B';

            using (var fs = new FileStream(fontfile, FileMode.Open, FileAccess.Read))
            {
                var reader = new OpenFontReader();
                //1. read typeface from font file
                //Typeface typeFace = reader.Read(fs);

                ////test left & right that has kern distance
                //ushort left_g_index = (ushort)typeFace.LookupIndex('A');
                //ushort right_g_index = (ushort)typeFace.LookupIndex('Y');
                //short kern_distance = typeFace.GetKernDistance(left_g_index, right_g_index);

                ////2. glyph-to-vxs builder
                //var builder = new GlyphPathBuilderVxs(typeFace);
                //left_vxs = BuildVxsForGlyph(builder, 'A', size, resolution);
                //right_vxs = BuildVxsForGlyph(builder, 'Y', size, resolution);

                //builder.Build('A', size, resolution);
                //VertexStore vxs1 = builder.GetVxs();
                //builder.Build('Y', size, resolution);
                //VertexStore vxs2 = builder.GetVxs();
                //---------------- 

                ////3. do mini translate, scale
                //var mat = PixelFarm.Agg.Transform.Affine.NewMatix(
                //    //translate
                //     new PixelFarm.Agg.Transform.AffinePlan(
                //         PixelFarm.Agg.Transform.AffineMatrixCommand.Translate, 10, 10),
                //    //scale
                //     new PixelFarm.Agg.Transform.AffinePlan(
                //         PixelFarm.Agg.Transform.AffineMatrixCommand.Scale, 4, 4)
                //         );

                //vxs1 = mat.TransformToVxs(vxs1);
                ////----------------
                ////4. flatten all curves 
                //vxs = curveFlattener.MakeVxs(vxs1);
            }
        }

        VertexStore BuildVxsForGlyph(GlyphOutlineBuilder builder, char character, int size, int resolution)
        {
            //-----------
            //TODO: review here
            builder.Build(character, size);
            var txToVxs = new GlyphTranslatorToVxs();
            builder.ReadShapes(txToVxs);

            VertexStore v2 = new VertexStore();
            using (VxsTemp.Borrow(out var v0))
            {
                txToVxs.WriteOutput(v0);
                var mat = PixelFarm.CpuBlit.VertexProcessing.Affine.New(
                     //translate
                     new PixelFarm.CpuBlit.VertexProcessing.AffinePlan(
                         PixelFarm.CpuBlit.VertexProcessing.AffineMatrixCommand.Translate, 10, 10),
                     //scale
                     new PixelFarm.CpuBlit.VertexProcessing.AffinePlan(
                         PixelFarm.CpuBlit.VertexProcessing.AffineMatrixCommand.Scale, 1, 1)
                         );
                //mat.TransformToVxs(v0, v1);
                _curveFlattener.MakeVxs(v0, mat, v2);
            }
            return v2;
        }

        [DemoConfig]
        public bool FillBG { get; set; }
        [DemoConfig]
        public bool FillBorder { get; set; }

        public override void Draw(PixelFarm.Drawing.Painter p)
        {
            AggPainter aggPainter = (AggPainter)p;

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

                float x = aggPainter.OriginX;
                float y = aggPainter.OriginY;

                p.Fill(_left_vxs);
                aggPainter.SetOrigin(x + 50, y + 20);
                p.Fill(_right_vxs);
                aggPainter.SetOrigin(x, y);
            }

            if (FillBorder)
            {
                //5.4 
                p.StrokeColor = PixelFarm.Drawing.Color.Green;
                //user can specific border width here...
                //p.StrokeWidth = 2;
                //5.5 
                //p.Draw(vxs);
                float x = aggPainter.OriginX;
                float y = aggPainter.OriginY;
                p.Fill(_left_vxs);
                aggPainter.SetOrigin(x + 50, y + 20);
                p.Fill(_right_vxs);
                aggPainter.SetOrigin(x, y);
            }
        }
    }

}