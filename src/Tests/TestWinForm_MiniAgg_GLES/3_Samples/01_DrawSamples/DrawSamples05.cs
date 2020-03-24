//MIT, 2018-present, WinterDev

using PixelFarm.Drawing;
using PixelFarm.CpuBlit.VertexProcessing;

using Mini;

namespace PixelFarm.CpuBlit.Sample_Draw
{


    [Info(OrderCode = "01")]
    [Info("from MatterHackers' Agg DrawAndSave")]
    public class DrawSample05 : DemoBase
    {
        MemBitmap _lionImg;
        public override void Init()
        {
            UseBitmapExt = false;

            string imgFileName = "Samples\\lion1.png";
            if (System.IO.File.Exists(imgFileName))
            {
                _lionImg = MemBitmap.LoadBitmap(imgFileName);
            }

        }

        [DemoConfig]
        public bool UseBitmapExt { get; set; }

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



            p.Clear(Drawing.Color.Yellow);
            p.UseSubPixelLcdEffect = false;


            //
            //---red reference line--
            p.StrokeColor = Color.Black;
            p.DrawLine(0, 400, 800, 400); //draw reference line
            p.DrawImage(_lionImg, 300, 0);

            int _imgW = _lionImg.Width;
            int _imgH = _lionImg.Height;



            int x_pos = 0;
            for (int i = 0; i < 360; i += 30)
            {
                p.DrawImage(_lionImg,
                   //move to center of the image (hotspot x,y)
                   AffinePlan.Translate(-_imgW / 2f, -_imgH / 2f),
                   AffinePlan.Scale(0.50, 0.50),
                   AffinePlan.Rotate(AggMath.deg2rad(i)),
                   AffinePlan.Translate((_imgW / 2f) + x_pos, _imgH / 2f) //translate back
                   );

                x_pos += _imgW / 3;
            }


            using (VxsTemp.Borrow(out var vxs1, out var vxs2))
            {
                SimpleRect sRect = new SimpleRect();
                int x = 0, y = 0, w = 100, h = 100;
                sRect.SetRect(x, y, x + w, y + h);
                sRect.MakeVxs(vxs1);
                p.Fill(vxs1, Color.Blue);
                //-------------------
                Affine af = Affine.New(
                    AffinePlan.Translate(-w / 2f, -h / 2f),
                    AffinePlan.Rotate(AggMath.deg2rad(30)),
                    AffinePlan.Translate(w / 2f, h / 2f)
                    );

                af.TransformToVxs(vxs1, vxs2);
                p.Fill(vxs2, Color.Red);
                //-------------------
            }

        }
    }


    [Info(OrderCode = "01")]
    [Info("from MatterHackers' Agg DrawAndSave")]
    public class DrawSample06 : DemoBase
    {
        MemBitmap _lionImg;
        MemBitmap _halfLion;
        public override void Init()
        {
            UseBitmapExt = false;

            string imgFileName = "Samples\\lion1.png";
            if (System.IO.File.Exists(imgFileName))
            {
                _lionImg = MemBitmap.LoadBitmap(imgFileName);
                _halfLion = CreateHalfSize(_lionImg);
            }
        }

        bool _useBmpExt;
        [DemoConfig]
        public bool UseBitmapExt
        {
            get => _useBmpExt;
            set
            {
                _useBmpExt = value;
                this.InvalidateGraphics();
            }
        }

        MemBitmap CreateHalfSize(MemBitmap orgBmp)
        {
            //TODO: ...
            //
            //1. create a new one
            MemBitmap smallBmp = new MemBitmap(orgBmp.Width / 2, orgBmp.Height / 2);
            using (Tools.BorrowAggPainter(smallBmp, out var painter))
            {
                painter.DrawImage(orgBmp, AffinePlan.Scale(0.5, 0.5));
            }
            return smallBmp;
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

            //---red reference line--
            p.StrokeColor = Color.Black;
            p.DrawLine(0, 400, 800, 400); //draw reference line
            p.DrawImage(_lionImg, 300, 0);
            //p.DrawImage(lionImg, 0, 0, 10, 10, 100, 100);

            //
            //p.DrawImage(halfLion, 50, 0);

            int _imgW = _lionImg.Width;
            int _imgH = _lionImg.Height;
            int x_pos = 0;
            int y_pos = 0;

            var affPlans = new AffinePlan[4];
            //1. create new half-size lion image 

            //for (int i = 0; i < 360; i += 30)
            //{
            //    affPlans[0] = AffinePlan.Translate(-_imgW / 2f, -_imgH / 2f);
            //    affPlans[1] = AffinePlan.Scale(1, 1);
            //    affPlans[2] = AffinePlan.Rotate(AggMath.deg2rad(i));
            //    affPlans[3] = AffinePlan.Translate((_imgW / 2f) + x_pos, (_imgH / 2f) + y_pos);
            //    p.DrawImage(halfLion, affPlans);

            //    x_pos += _imgW / 3;
            //}


            x_pos = 0;
            y_pos = 100;
            for (int i = 0; i < 360; i += 30)
            {
                affPlans[0] = AffinePlan.Translate(-_imgW / 2f, -_imgH / 2f);
                affPlans[1] = AffinePlan.Scale(0.50, 0.50);
                affPlans[2] = AffinePlan.Rotate(AggMath.deg2rad(i));
                affPlans[3] = AffinePlan.Translate((_imgW / 2f) + x_pos, (_imgH / 2f) + y_pos);
                p.DrawImage(_lionImg, affPlans);
                x_pos += _imgW / 3;
            }




            //----
            //

            using (VxsTemp.Borrow(out var vxs1, out var vxs2))
            {
                SimpleRect sRect = new SimpleRect();
                int x = 0, y = 0, w = 100, h = 100;
                sRect.SetRect(x, y, x + w, y + h);
                sRect.MakeVxs(vxs1);
                p.Fill(vxs1, Color.Blue);
                //-------------------
                Affine af = Affine.New(
                    AffinePlan.Translate(-w / 2f, -h / 2f),
                    AffinePlan.Rotate(AggMath.deg2rad(30)),
                    AffinePlan.Translate(w / 2f, h / 2f)
                    );

                af.TransformToVxs(vxs1, vxs2);
                p.Fill(vxs2, Color.Red);
            }
        }
    }



}