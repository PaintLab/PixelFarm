using System;
using PixelFarm.Agg.Transform;
namespace PixelFarm.Agg
{
    public class LionFillSprite : BasicSprite
    {
        SpriteShape lionShape;
        VertexStore myvxs;
        byte alpha;
        public LionFillSprite()
        {
            lionShape = new SpriteShape();
            lionShape.ParseLion();
            this.Width = 500;
            this.Height = 500;
            AlphaValue = 255;
        }
        public bool AutoFlipY
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
                int j = lionShape.NumPaths;
                var colorBuffer = lionShape.Colors;
                for (int i = lionShape.NumPaths - 1; i >= 0; --i)
                {
                    colorBuffer[i] = colorBuffer[i].NewFromChangeAlpha(alpha);
                }
            }
        }

        public override bool Move(int mouseX, int mouseY)
        {
            bool result = base.Move(mouseX, mouseY);
            myvxs = null;
            return result;
        }

        public override void Draw(PixelFarm.Drawing.CanvasPainter p)
        {
            if (myvxs == null)
            {

                var transform = Affine.NewMatix(
                        AffinePlan.Translate(-lionShape.Center.x, -lionShape.Center.y),
                        AffinePlan.Scale(spriteScale, spriteScale),
                        AffinePlan.Rotate(angle + Math.PI),
                        AffinePlan.Skew(skewX / 1000.0, skewY / 1000.0),
                        AffinePlan.Translate(Width / 2, Height / 2)
                );
                //create vertextStore again from original path
                myvxs = new VertexStore();

                transform.TransformToVxs(lionShape.Path.Vxs, myvxs);

                if (AutoFlipY)
                {
                    //flip the lion
                    PixelFarm.Agg.Transform.Affine aff = PixelFarm.Agg.Transform.Affine.NewMatix(
                      PixelFarm.Agg.Transform.AffinePlan.Scale(-1, -1),
                      PixelFarm.Agg.Transform.AffinePlan.Translate(0, 600));
                    //
                    var v2 = new VertexStore();
                    myvxs = transform.TransformToVxs(myvxs, v2);
                }

            }
            //---------------------------------------------------------------------------------------------
            {

                int j = lionShape.NumPaths;
                int[] pathList = lionShape.PathIndexList;
                Drawing.Color[] colors = lionShape.Colors;
                //graphics2D.UseSubPixelRendering = true; 
                for (int i = 0; i < j; ++i)
                {
                    p.FillColor = colors[i];
                    p.Fill(new VertexStoreSnap(myvxs, pathList[i]));

                }

            }
        }

        public SpriteShape GetSpriteShape()
        {
            return lionShape;
        }
    }
}