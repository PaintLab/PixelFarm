//BSD, 2014-2016, WinterDev

//MatterHackers: BSD
// Much of the ui to the drawing functions still needs to be C#'ed and cleaned up.  A lot of
// it still follows the originall agg function names.  I have been cleaning these up over time
// and intend to do much more refactoring of these things over the long term.

using System;
using PixelFarm.Agg.VertexSource;
using PixelFarm.VectorMath;
using PixelFarm.Agg.Transform;
using System.IO;

using Mini;
using PixelFarm.Drawing.Fonts;
using NRasterizer; //for read 

namespace PixelFarm.Agg.Sample_Draw
{
    [Info(OrderCode = "01")]
    [Info("OpenTypeReaderFromPureCs")]
    public class OpenTypeReaderFromPureCS : DemoBase
    {
        VertexStore vxs;
        CurveFlattener curveFlattener = new CurveFlattener();
        VertexStore left_vxs;
        VertexStore right_vxs;
        public override void Init()
        {
            var fontfile = "tahoma.ttf";
            var reader = new OpenTypeReader();

            this.FillBG = true;

            int size = 72;
            int resolution = 72;
            char testChar = 'B';

            using (var fs = new FileStream(fontfile, FileMode.Open))
            {
                //1. read typeface from font file
                Typeface typeFace = reader.Read(fs);

                //test left & right that has kern distance
                ushort left_g_index = (ushort)typeFace.LookupIndex('A');
                ushort right_g_index = (ushort)typeFace.LookupIndex('Y');
                short kern_distance = typeFace.GetKernDistance(left_g_index, right_g_index);

                //2. glyph-to-vxs builder
                var builder = new GlyphPathBuilderVxs(typeFace);
                left_vxs = BuildVxsForGlyph(builder, 'A', size, resolution);
                right_vxs = BuildVxsForGlyph(builder, 'Y', size, resolution);

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
        VertexStore BuildVxsForGlyph(GlyphPathBuilderVxs builder, char character, int size, int resolution)
        {
            builder.Build(character, size);
            VertexStore vxs1 = builder.GetVxs();
            var mat = PixelFarm.Agg.Transform.Affine.NewMatix(
                //translate
                 new PixelFarm.Agg.Transform.AffinePlan(
                     PixelFarm.Agg.Transform.AffineMatrixCommand.Translate, 10, 10),
                //scale
                 new PixelFarm.Agg.Transform.AffinePlan(
                     PixelFarm.Agg.Transform.AffineMatrixCommand.Scale, 1, 1)
                     );
            vxs1 = mat.TransformToVxs(vxs1);
            return curveFlattener.MakeVxs(vxs1);
        }
        [DemoConfig]
        public bool FillBG
        {
            get;
            set;
        }
        [DemoConfig]
        public bool FillBorder
        {
            get;
            set;
        }

        public override void Draw(CanvasPainter p)
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
                p.Fill(left_vxs);                
                p.Fill(right_vxs);
            }

            if (FillBorder)
            {
                //5.4 
                p.StrokeColor = PixelFarm.Drawing.Color.Green;
                //user can specific border width here...
                //p.StrokeWidth = 2;
                //5.5 
                //p.Draw(vxs);
                p.Draw(left_vxs);
                p.Draw(right_vxs);
            }
        }
    }

}