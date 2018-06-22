//MIT, 2018-present, WinterDev

using PixelFarm.Drawing;
using PixelFarm.CpuBlit.VertexProcessing;
using PixelFarm.VectorMath;
using Mini;

namespace PixelFarm.CpuBlit.Sample_Draw
{


    [Info(OrderCode = "01")]
    [Info("from MatterHackers' Agg DrawAndSave")]
    public class DrawSample05 : DemoBase
    {
        ActualBitmap lionImg;
        public override void Init()
        {
            UseBitmapExt = false;

            string imgFileName = "Samples\\lion1.png";
            if (System.IO.File.Exists(imgFileName))
            {
                lionImg = DemoHelper.LoadImage(imgFileName);
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
                p.RenderQuality = RenderQualtity.Fast;
            }
            else
            {
                p.RenderQuality = RenderQualtity.HighQuality;
            }



            p.Clear(Drawing.Color.White);
            p.UseSubPixelLcdEffect = false;


            //
            //---red reference line--
            p.StrokeColor = Color.Black;
            p.DrawLine(0, 400, 800, 400); //draw reference line
            p.DrawImage(lionImg, 300, 0);

            int _imgW = lionImg.Width;
            int _imgH = lionImg.Height;



            int x_pos = 0;
            for (int i = 0; i < 360; i += 30)
            {
                p.DrawImage(lionImg,
                   //move to center of the image (hotspot x,y)
                   AffinePlan.Translate(-_imgW / 2f, -_imgH / 2f),
                   AffinePlan.Scale(0.50, 0.50),
                   AffinePlan.Rotate(AggMath.deg2rad(i)),
                   AffinePlan.Translate((_imgW / 2f) + x_pos, _imgH / 2f) //translate back
                   );

                x_pos += _imgW / 3;
            }

            //----
            //
            VectorToolBox.GetFreeVxs(out VertexStore vxs1);
            VectorToolBox.GetFreeVxs(out VertexStore vxs2);

            SimpleRect sRect = new SimpleRect();
            int x = 0, y = 0, w = 100, h = 100;
            sRect.SetRect(x, y, x + w, y + h);
            sRect.MakeVxs(vxs1);
            p.Fill(vxs1, Color.Blue);
            //-------------------
            Affine af = Affine.NewMatix(
                AffinePlan.Translate(-w / 2f, -h / 2f),
                AffinePlan.Rotate(AggMath.deg2rad(30)),
                AffinePlan.Translate(w / 2f, h / 2f)
                );

            af.TransformToVxs(vxs1, vxs2);
            p.Fill(vxs2, Color.Red);
            //-------------------
            VectorToolBox.ReleaseVxs(ref vxs1);
            VectorToolBox.ReleaseVxs(ref vxs2);
        }
    }


    [Info(OrderCode = "01")]
    [Info("from MatterHackers' Agg DrawAndSave")]
    public class DrawSample06 : DemoBase
    {
        ActualBitmap lionImg;
        public override void Init()
        {
            UseBitmapExt = false;

            string imgFileName = "Samples\\lion1.png";
            if (System.IO.File.Exists(imgFileName))
            {
                lionImg = DemoHelper.LoadImage(imgFileName);
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
                p.RenderQuality = RenderQualtity.Fast;
            }
            else
            {
                p.RenderQuality = RenderQualtity.HighQuality;
            }



            p.Clear(Drawing.Color.White);
            p.UseSubPixelLcdEffect = false;


            //
            //---red reference line--
            p.StrokeColor = Color.Black;
            p.DrawLine(0, 400, 800, 400); //draw reference line
            p.DrawImage(lionImg, 300, 0);

            int _imgW = lionImg.Width;
            int _imgH = lionImg.Height;


            int x_pos = 0;



            var affPlans = new AffinePlan[4];

            for (int i = 0; i < 360; i += 30)
            {
                
                affPlans[0] = AffinePlan.Translate(-_imgW / 2f, -_imgH / 2f);
                affPlans[1] = AffinePlan.Scale(0.50, 0.50);
                affPlans[2] = AffinePlan.Rotate(AggMath.deg2rad(i));
                affPlans[3] = AffinePlan.Translate((_imgW / 2f) + x_pos, _imgH / 2f);


                p.DrawImage(lionImg, affPlans);

                x_pos += _imgW / 3;
            }

            //----
            //
            VectorToolBox.GetFreeVxs(out VertexStore vxs1);
            VectorToolBox.GetFreeVxs(out VertexStore vxs2);

            SimpleRect sRect = new SimpleRect();
            int x = 0, y = 0, w = 100, h = 100;
            sRect.SetRect(x, y, x + w, y + h);
            sRect.MakeVxs(vxs1);
            p.Fill(vxs1, Color.Blue);
            //-------------------
            Affine af = Affine.NewMatix(
                AffinePlan.Translate(-w / 2f, -h / 2f),
                AffinePlan.Rotate(AggMath.deg2rad(30)),
                AffinePlan.Translate(w / 2f, h / 2f)
                );

            af.TransformToVxs(vxs1, vxs2);
            p.Fill(vxs2, Color.Red);
            //-------------------
            VectorToolBox.ReleaseVxs(ref vxs1);
            VectorToolBox.ReleaseVxs(ref vxs2);
        }
    }



}