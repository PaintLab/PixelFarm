//BSD, 2014-present, WinterDev
//MattersHackers
//AGG 2.4

using System;
using PixelFarm.CpuBlit.VertexProcessing;
using PaintLab.Svg;

namespace PixelFarm.CpuBlit
{


    public class MyTestSprite : BasicSprite
    {
        SpriteShape _spriteShape;

        float _posX, _posY;
        float _mouseDownX, _mouseDownY;
        Affine _currentTx = null;
        byte alpha;
        public MyTestSprite(SpriteShape spriteShape)
        {
            _spriteShape = spriteShape;

            this.Width = 500;
            this.Height = 500;
            AlphaValue = 255;
            JustMove = true;
        }
        public SpriteShape SpriteShape
        {
            get { return _spriteShape; }
            set { _spriteShape = value; }
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
                //TODO: review here...            
                _spriteShape.ApplyNewAlpha(value);
                //int j = lionShape.NumPaths;
                //var colorBuffer = lionShape.Colors;
                //for (int i = lionShape.NumPaths - 1; i >= 0; --i)
                //{
                //    colorBuffer[i] = colorBuffer[i].NewFromChangeAlpha(alpha);
                //}
            }
        }
        public bool JustMove { get; set; }
        public Affine CurrentAffineTx { get { return _currentTx; } }
        public override bool Move(int mouseX, int mouseY)
        {

            if (JustMove)
            {
                _posX += mouseX - _mouseDownX;
                _posY += mouseY - _mouseDownY;

                _mouseDownX = mouseX;
                _mouseDownY = mouseY;
                return true;
            }
            else
            {
                bool result = base.Move(mouseX, mouseY);
                _currentTx = null;// reset
                return result;
            }
        }

        public bool HitTest(float x, float y, bool withSubPathTest)
        {
            RectD bounds = _spriteShape.Bounds;

            if (this._currentTx != null)
            {
                double left = bounds.Left;
                double top = bounds.Top;
                double right = bounds.Right;
                double bottom = bounds.Bottom;
            }


            bounds.Offset(_posX, _posY);
            if (bounds.Contains(x, y))
            {
                _mouseDownX = x;
                _mouseDownY = y;
                x -= _posX; //offset x to the coordinate of the sprite
                y -= _posY;
                //....
                if (withSubPathTest)
                {
                    //fine hit on sup part***
                    VgHitChainPool.GetFreeHitTestChain(out VgHitChain svgHitChain);
                    svgHitChain.SetHitTestPos(x, y);
                    svgHitChain.WithSubPartTest = withSubPathTest;
                    _spriteShape.HitTestOnSubPart(svgHitChain);

                    //check if we hit on sup part
                    int hitCount = svgHitChain.Count;
                    if (hitCount > 0)
                    {
                        VgVisualElement svgElem = svgHitChain.GetLastHitInfo().hitElem;
                        //if yes then change its bg color
                        svgElem.VisualSpec.FillColor = Drawing.Color.Red;
                    }

                    VgHitChainPool.ReleaseHitTestChain(ref svgHitChain);

                    return hitCount > 0;
                }
                return true;
            }
            else
            {
                _mouseDownX = _mouseDownY = 0;
            }
            return false;
        }


        public override void Render(PixelFarm.Drawing.Painter p)
        {
            if (_currentTx == null)
            {
                _currentTx = Affine.NewMatix(
                      AffinePlan.Translate(-_spriteShape.Center.x, -_spriteShape.Center.y),
                      AffinePlan.Scale(_spriteScale, _spriteScale),
                      AffinePlan.Rotate(_angle + Math.PI),
                      AffinePlan.Skew(_skewX / 1000.0, _skewY / 1000.0),
                      AffinePlan.Translate(Width / 2, Height / 2)
              );
            }

            if (JustMove)
            {
                float ox = p.OriginX;
                float oy = p.OriginY;

                p.SetOrigin(ox + _posX, oy + _posY);
                _spriteShape.Paint(p);
                p.SetOrigin(ox, oy);

            }
            else
            {
                _spriteShape.Paint(p, _currentTx);
            }

        }

        public SpriteShape GetSpriteShape()
        {
            return _spriteShape;
        }
    }
}