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

                //2. glyph-to-vxs builder
                var builder = new GlyphPathBuilderVxs(typeFace); 
                builder.Build(testChar, size, resolution);
                VertexStore vxs1 = builder.GetVxs(); 
                //----------------
                //3. do mini translate, scale
                var mat = PixelFarm.Agg.Transform.Affine.NewMatix(
                    //translate
                     new PixelFarm.Agg.Transform.AffinePlan(
                         PixelFarm.Agg.Transform.AffineMatrixCommand.Translate, 10, 10),
                    //scale
                     new PixelFarm.Agg.Transform.AffinePlan(
                         PixelFarm.Agg.Transform.AffineMatrixCommand.Scale, 4, 4)
                         );

                vxs1 = mat.TransformToVxs(vxs1);
                //----------------
                //4. flatten all curves 
                vxs = curveFlattener.MakeVxs(vxs1);
            }
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
                p.Fill(vxs);
            }

            if (FillBorder)
            {
                //5.4 
                p.StrokeColor = PixelFarm.Drawing.Color.Green;
                //user can specific border width here...
                //p.StrokeWidth = 2;
                //5.5 
                p.Draw(vxs);
            }
        }
    }

}