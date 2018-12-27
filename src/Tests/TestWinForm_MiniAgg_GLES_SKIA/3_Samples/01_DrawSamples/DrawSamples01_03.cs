//BSD, 2014-present, WinterDev

//MatterHackers: BSD
// Much of the ui to the drawing functions still needs to be C#'ed and cleaned up.  A lot of
// it still follows the originall agg function names.  I have been cleaning these up over time
// and intend to do much more refactoring of these things over the long term.


using PixelFarm.CpuBlit.VertexProcessing;
using PixelFarm.Drawing;
using PixelFarm.VectorMath;

using Mini;

namespace PixelFarm.CpuBlit.Sample_Draw
{
    [Info(OrderCode = "01")]
    [Info("from MatterHackers' Agg DrawAndSave")]
    public class DrawSample01 : DemoBase
    {
        MemBitmap _lionImg;
        public override void Init()
        {
            UseBitmapExt = false;

            string imgFileName = "Samples\\lion1.png";
            if (System.IO.File.Exists(imgFileName))
            {
                _lionImg = DemoHelper.LoadImage(imgFileName);
            }

        }

        [DemoConfig]
        public bool UseBitmapExt
        {
            get;
            set;
        }
        public override void Draw(Painter p)
        {
            if (UseBitmapExt)
            {
                p.RenderQuality = RenderQuality.Fast;
            }
            else
            {
                p.RenderQuality = RenderQuality.HighQuality;
            }
            p.Clear(Drawing.Color.White);
            p.UseSubPixelLcdEffect = false;
            //string teststr = "ABCDE pqyt 1230";
            //p.FillColor = Color.Black;
            //p.CurrentFont = new RequestFont("tahoma", 10);
            //p.StrokeColor = Color.Red;

            p.RenderQuality = RenderQuality.Fast;
            //
            //---red reference line--
            p.DrawLine(0, 400, 800, 400);
            p.DrawLine(0, 400, 800, 500); //test oblique line

            //p.DrawString(teststr, 300, 400);
            //
            p.DrawRect(0.5, 400, 40, 40);
            //

            p.FillColor = Color.Yellow;
            p.StrokeColor = Color.Blue;

            p.FillEllipse(100.5, 400, 40, 60);
            p.DrawEllipse(50.5, 400, 40, 60);

            //---red reference line--
            p.StrokeColor = Color.Red;
            p.DrawLine(0, 500, 800, 500);

            p.StrokeColor = Color.Blue;
            p.FillColor = Color.Yellow;
            p.FillRect(0.5, 500, 40, 40);
            //---red reference line--


            p.RenderQuality = RenderQuality.Fast;
            //p.DrawImage(lionImg, 0, 0); //reference at 0,0 
            p.DrawImage(_lionImg, 300, 0);

            int _imgW = _lionImg.Width;
            int _imgH = _lionImg.Height;

            //p.RenderQuality = RenderQuality.Fast;
            p.DrawImage(_lionImg,
             //move to center of the image (hotspot x,y)
             AffinePlan.Translate(-_imgW / 2, -_imgH / 2),
             //AffinePlan.Scale(0.50, 0.50),//
             AffinePlan.Rotate(AggMath.deg2rad(30)),
             AffinePlan.Translate(_imgW / 2, _imgH / 2)
             );

            //AffinePlan.Scale(0.75, 0.75),
            //move to target 

            //p.DrawImage(lionImg,
            //   //move to center of the image (hotspot x,y)
            //   AffinePlan.Translate(-_imgW / 2, -_imgH / 2),
            //   AffinePlan.Rotate(AggMath.deg2rad(45)),
            //   AffinePlan.Scale(0.75, 0.75),
            //   //move to target
            //   AffinePlan.Translate(400, 200));
        }
    }

    [Info(OrderCode = "01")]
    [Info("test")]
    public class DrawSample02 : DemoBase
    {
        public override void Init()
        {
        }

        public override void Draw(Painter p)
        {
            p.Clear(Drawing.Color.White);
            p.StrokeColor = Color.Red;

            //lines
            p.DrawLine(0, 0, 100, 100);
            p.DrawLine(200, 100, 250, 200);
            p.DrawRect(10, 10, 20, 30);
        }
    }


    [Info(OrderCode = "01")]
    [Info("from MatterHackers' Agg DrawAndSave")]
    public class DrawSample03 : DemoBase
    {




        public override void Init()
        {

        }
        [DemoConfig]
        public bool UseBitmapExt
        {
            get;
            set;
        }
        public override void Draw(Painter p)
        {

            if (UseBitmapExt)
            {
                p.RenderQuality = RenderQuality.Fast;
            }
            else
            {
                p.RenderQuality = RenderQuality.HighQuality;
            }

            int width = 800;
            int height = 600;
            //clear the image to white         
            // draw a circle
            p.Clear(Drawing.Color.White);

            //Ellipse ellipseVxsGen = new Ellipse(0, 0, 100, 50);
            using (VectorToolBox.Borrow(out Ellipse ellipseVxsGen))
            using (VectorToolBox.Borrow(out Stroke stroke))
            {
                ellipseVxsGen.Set(0, 0, 100, 50);
                stroke.Width = 3;

                for (double angleDegrees = 0; angleDegrees < 180; angleDegrees += 22.5)
                {
                    var mat = Affine.NewMatix(
                        AffinePlan.Rotate(MathHelper.DegreesToRadians(angleDegrees)),
                        AffinePlan.Translate(width / 2, 150));

                    using (VxsTemp.Borrow(out var v1, out var v2, out var v3))
                    {

                        ellipseVxsGen.MakeVxs(mat, v2);
                        p.FillColor = Drawing.Color.Yellow;
                        p.Fill(v2);
                        //------------------------------------                
                        p.FillColor = Drawing.Color.Blue;
                        p.Fill(stroke.MakeVxs(v2, v3));
                    }

                }
            }


            // and a little polygon
            using (VxsTemp.Borrow(out var v1))
            using (VectorToolBox.Borrow(v1, out PathWriter littlePoly))
            {
                littlePoly.MoveTo(50, 50);
                littlePoly.LineTo(150, 50);
                littlePoly.LineTo(200, 200);
                littlePoly.LineTo(50, 150);
                littlePoly.LineTo(50, 50);
                p.FillColor = Drawing.Color.Blue;
                p.Fill(v1);
            }
            //---




            //----
            //test draw img



            //            
            //g.Render(littlePoly.MakeVertexSnap(), ColorRGBA.Cyan);
            // draw some text
            // draw some text  

            //var textPrinter = new TextPrinter();
            //textPrinter.CurrentActualFont = svgFontStore.LoadFont(SvgFontStore.DEFAULT_SVG_FONTNAME, 30);
            //new TypeFacePrinter("Printing from a printer", 30, justification: Justification.Center);

            //VertexStore vxs = textPrinter.CreateVxs("Printing from a printer".ToCharArray());
            //var affTx = Affine.NewTranslation(width / 2, height / 4 * 3);
            //VertexStore s1 = affTx.TransformToVxs(vxs);
            //p.FillColor = Drawing.Color.Black;
            //p.Fill(s1);
            ////g.Render(s1, ColorRGBA.Black);
            //p.FillColor = Drawing.Color.Red;
            //p.Fill(StrokeHelp.MakeVxs(s1, 1));
            ////g.Render(StrokeHelp.MakeVxs(s1, 1), ColorRGBA.Red);
            //var aff2 = Affine.NewMatix(
            //    AffinePlan.Rotate(MathHelper.DegreesToRadians(90)),
            //    AffinePlan.Translate(40, height / 2));
            //p.FillColor = Drawing.Color.Black;
            //p.Fill(aff2.TransformToVertexSnap(vxs));
            ////g.Render(aff2.TransformToVertexSnap(vxs), ColorRGBA.Black);
        }
    }


}
