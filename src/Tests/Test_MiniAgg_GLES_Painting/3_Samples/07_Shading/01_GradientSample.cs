//BSD, 2014-present, WinterDev
//MatterHackers

using System;
using System.Diagnostics;
using PixelFarm.Drawing;
using PixelFarm.CpuBlit.VertexProcessing;

using Mini;
namespace PixelFarm.CpuBlit.Sample_Gradient
{

    [Info(OrderCode = "01_1", AvailableOn = AvailableOn.GLES | AvailableOn.Agg)]
    public class GradientDemo : DemoBase
    {
        public enum BrushKind
        {
            SolidBrush,
            LinearGradient,
            CircularGradient,
            PolygonGradient,
        }


        VertexStore _triangleVxs;
        VertexStore _triangleVxs2;

        SolidBrush _solidBrush;
        LinearGradientBrush _linearGrBrush;

        RadialGradientBrush _circularGrBrush;
        PolygonGradientBrush _polygonGradientBrush;


        public GradientDemo()
        {
            //solid brush
            _solidBrush = new SolidBrush(Color.Blue);

            //1. linear gradient
            _linearGrBrush = new LinearGradientBrush(
                     new PointF(0, 0), new PointF(200, 200),
                     new ColorStop[]
                     {
                       new ColorStop(0.0f,  Drawing.Color.Black),
                       new ColorStop(0.20f, Drawing.Color.Red),
                       new ColorStop(0.50f, Drawing.KnownColors.OrangeRed),
                       new ColorStop(0.75f, Drawing.Color.Yellow)
                     });


            //2. circular gradient
            _circularGrBrush = new RadialGradientBrush(
                    new PointF(50, 20), new PointF(300, 20),
                    new ColorStop[]
                    {
                        //for test different colors
                        new ColorStop(0.0f, Drawing.Color.Yellow),
                        new ColorStop(0.25f, Drawing.Color.Blue),
                        new ColorStop(0.50f, Drawing.Color.Green),
                        new ColorStop(0.75f, Drawing.Color.Yellow),
                    });



            //3. polygon gradient: this version, just a simple rect 
            PolygonGradientBrush.ColorVertex2d[] vertices = new PolygonGradientBrush.ColorVertex2d[]
            {
                new PolygonGradientBrush.ColorVertex2d(0,0,KnownColors.OrangeRed),
                new PolygonGradientBrush.ColorVertex2d(300,0,Color.Black),
                new PolygonGradientBrush.ColorVertex2d(300,400,Color.Yellow),
                new PolygonGradientBrush.ColorVertex2d(0,400,Color.Blue),
            };
            _polygonGradientBrush = new PolygonGradientBrush(vertices);

            using (Tools.BorrowVxs(out var v1, out var v2))
            using (Tools.BorrowPathWriter(v1, out PathWriter p))
            {
                p.MoveTo(0, 0);
                p.LineTo(50, 20);
                p.LineTo(10, 100);
                p.CloseFigure();

                AffineMat aff1 = AffineMat.GetScaleMat(2);
                _triangleVxs = v1.CreateTrim(aff1);

                AffineMat tx = AffineMat.GetTranslateMat(100, 120);
                _triangleVxs2 = tx.TransformToVxs(_triangleVxs, v2).CreateTrim();
            }
        }

        [DemoConfig]
        public bool UseOffset { get; set; }
        [DemoConfig]
        public bool EnableGLPainterTwoColorsMask { get; set; }

        [DemoConfig]
        public BrushKind SelectedBrushKind { get; set; }
        public override void Draw(PixelFarm.Drawing.Painter p)
        {

            p.RenderQuality = RenderQuality.Fast;
            Brush prevBrush = p.CurrentBrush;
            Brush selectedBrush;

            p.Clear(Color.White);

            switch (SelectedBrushKind)
            {

                default: throw new NotSupportedException();
                case BrushKind.SolidBrush:
                    selectedBrush = _solidBrush;
                    break;
                case BrushKind.LinearGradient:
                    selectedBrush = _linearGrBrush;
                    break;
                case BrushKind.CircularGradient:
                    selectedBrush = _circularGrBrush;
                    break;
                case BrushKind.PolygonGradient:
                    selectedBrush = _polygonGradientBrush;
                    break;
            }

            //
            p.CurrentBrush = selectedBrush;

            //special set for our GLPainter
            bool glPainter2MaskColor = false;
            DrawingGL.GLPainter glPainter = p as DrawingGL.GLPainter;
            if (glPainter != null)
            {
                glPainter2MaskColor = p.EnableMask;
                glPainter.UseTwoColorsMask = EnableGLPainterTwoColorsMask;
               
            }

            //p.FillRect(0, 100, 500, 500); 
            //p.FillRect(0, 200, 200, 50);


            p.Fill(_triangleVxs);
            if (UseOffset)
            {
                float prev_ox = p.OriginX;
                float prev_oy = p.OriginY;
                p.SetOrigin(100, 120);
                p.Fill(_triangleVxs);
                p.SetOrigin(prev_ox, prev_oy);//restore
            }
            else
            {
                //p.Fill(_triangleVxs);
                p.Fill(_triangleVxs2);
            }
           
            ////-------------               

            p.CurrentBrush = prevBrush;

            if (glPainter != null)
            {
                //restore
                glPainter.UseTwoColorsMask = glPainter2MaskColor;
            }
        }

    }
}




