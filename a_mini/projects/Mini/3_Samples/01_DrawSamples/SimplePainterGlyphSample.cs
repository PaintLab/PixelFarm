//2014 BSD,WinterDev

//MatterHackers: BSD
// Much of the ui to the drawing functions still needs to be C#'ed and cleaned up.  A lot of
// it still follows the originall agg function names.  I have been cleaning these up over time
// and intend to do much more refactoring of these things over the long term.

using System;

using PixelFarm.Agg.Image;
using PixelFarm.Agg.VertexSource;
using PixelFarm.Agg.Font;
using PixelFarm.VectorMath;
using PixelFarm.Agg.Transform;


using Mini;
namespace PixelFarm.Agg.SimplePainter
{

    [Info(OrderCode = "01")]
    [Info("SimplePainterGlyph")]
    public class SimplePainterGlyphSample : DemoBase
    {
        string fontfile = "c:\\Windows\\Fonts\\tahoma.ttf";
        PixelFarm.Font2.FontFace font;
        public override void Init()
        {
            font = PixelFarm.Font2.MyFonts.LoadFont(fontfile, 150);
        }
        public override void Draw(Graphics2D g)
        {
            //1.
            // clear the image to white
            CanvasPainter p = new CanvasPainter(g);
            p.Clear(ColorRGBA.White);
            p.FillColor = new ColorRGBA(ColorRGBA.Blue, 80);

            //M414 -20q-163 0 -245 86t-82 236z
            //<path d="M414 20q-163 40 -245 126t-82 276z"/> 
            //PathWriter path = new PathWriter();
            //path.MoveTo(414, 20);
            //path.Curve3Rel(-163, 40, -245, 126);
            //path.SmoothCurve3Rel(-82, 246);
            //path.CloseFigure();

            //M414 -20q-163 0 -245 86t-82 236z
            //PathWriter path = new PathWriter();
            //path.MoveTo(414, -20);
            //path.Curve3Rel(-163, 0, -245, 86);
            //path.SmoothCurve3Rel(-82, 236);
            //path.CloseFigure(); 

            //p.Fill(p.FlattenCurves(path.Vxs));

            //p.DrawBezierCurve(120, 500 - 160, 220, 500 - 40, 35, 500 - 200, 220, 500 - 260);
            //--------------------------------------------------- 
            var fontGlyph = font.GetGlyph('a'); 
            //p.Fill(fontGlyph.vxs);
#if DEBUG             
            //p.Fill(fontGlyph.vxs); 

            var flat_v = p.FlattenCurves(fontGlyph.vxs);
            p.Fill(flat_v);
           // dbugVxsDrawPoints.DrawVxsPoints(flat_v, g);
            //dbugVxsDrawPoints.DrawVxsPoints(fontGlyph.vxs, g); 
#endif

            //p.Fill(p.FlattenCurves(fontGlyph.vxs));


            //StyledTypeFace typeFaceForLargeA = new StyledTypeFace(LiberationSansFont.Instance, 300, flatenCurves: false);
            //var m_pathVxs = typeFaceForLargeA.GetGlyphForCharacter('a');
            //Affine shape_mtx = Affine.NewMatix(AffinePlan.Translate(150, 50));
            //m_pathVxs = shape_mtx.TransformToVxs(m_pathVxs);

            ////p.FillColor = new ColorRGBA(ColorRGBA.Red, 100);
            ////p.Fill(m_pathVxs);
            //p.FillColor = new ColorRGBA(ColorRGBA.Green, 120);
            //p.Fill(p.FlattenCurves(m_pathVxs));
        }
    }


}