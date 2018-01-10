using System;
using PixelFarm.Agg.Transform;
using PixelFarm.Drawing;
namespace PixelFarm.Agg
{
    public class LionFillSprite : BasicSprite
    {
        SpriteShape lionShape;

        byte alpha;
        public LionFillSprite()
        {
            lionShape = new SpriteShape();
            lionShape.ParseLion();
            this.Width = 500;
            this.Height = 500;
            AlphaValue = 255;
        }
        public SpriteShape LionShape
        {
            get { return lionShape; }
            set { lionShape = value; }
        }

        public bool AutoFlipY
        {
            get;
            set;
        }
        public int SharpenRadius
        {
            get;
            set;
        }

        public byte AlphaValue
        {
            get { return this.alpha; }
            set
            {
                this.alpha = value;
                //change alpha value
                lionShape.ApplyNewAlpha(value);
                //int j = lionShape.NumPaths;
                //var colorBuffer = lionShape.Colors;
                //for (int i = lionShape.NumPaths - 1; i >= 0; --i)
                //{
                //    colorBuffer[i] = colorBuffer[i].NewFromChangeAlpha(alpha);
                //}
            }
        }

        bool recreatePathAgain = true;
        public override bool Move(int mouseX, int mouseY)
        {
            bool result = base.Move(mouseX, mouseY);
            recreatePathAgain = true;
            return result;
        }



        public override void Draw(PixelFarm.Drawing.Painter p)
        {
            if (recreatePathAgain)
            {
                recreatePathAgain = false;

                var transform = Affine.NewMatix(
                        AffinePlan.Translate(-lionShape.Center.x, -lionShape.Center.y),
                        AffinePlan.Scale(spriteScale, spriteScale),
                        AffinePlan.Rotate(angle + Math.PI),
                        AffinePlan.Skew(skewX / 1000.0, skewY / 1000.0),
                        AffinePlan.Translate(Width / 2, Height / 2)
                );
                //create vertextStore again from original path


                //temp fix
                SvgRenderVx renderVx = lionShape.GetRenderVx();
                int count = renderVx.SvgVxCount;
                for (int i = 0; i < count; ++i)
                {
                    SvgVx vx = renderVx.GetInnerVx(i);
                    if (vx.Kind != SvgRenderVxKind.Path)
                    {
                        continue;
                    }

                    //Temp fix,

                    //TODO: review here,
                    //permanent transform each part?
                    //or create a copy. 
                    vx.RestoreOrg();
                    VertexStore vxvxs = vx.GetVxs();
                    VertexStore newVxs = new VertexStore();
                    transform.TransformToVxs(vxvxs, newVxs);
                    vx.SetVxs(newVxs);
                }


                //if (AutoFlipY)
                //{
                //    //flip the lion
                //    PixelFarm.Agg.Transform.Affine aff = PixelFarm.Agg.Transform.Affine.NewMatix(
                //      PixelFarm.Agg.Transform.AffinePlan.Scale(-1, -1),
                //      PixelFarm.Agg.Transform.AffinePlan.Translate(0, 600));
                //    //
                //    var v2 = new VertexStore();
                //    myvxs = transform.TransformToVxs(myvxs, v2);
                //}

            }
            //---------------------------------------------------------------------------------------------
            {
                lionShape.Paint(p);

                //int j = lionShape.NumPaths;
                //int[] pathList = lionShape.PathIndexList;
                //Drawing.Color[] colors = lionShape.Colors;
                ////graphics2D.UseSubPixelRendering = true; 
                //for (int i = 0; i < j; ++i)
                //{
                //    p.FillColor = colors[i];
                //    p.Fill(new VertexStoreSnap(myvxs, pathList[i]));
                //}
            }
            //test 
            if (SharpenRadius > 0)
            {
                //p.DoFilter(new RectInt(0, p.Height, p.Width, 0), 2);
                //PixelFarm.Agg.Imaging.SharpenFilterARGB.Sharpen()
            }
        }

        public SpriteShape GetSpriteShape()
        {
            return lionShape;
        }
    }
}