//MIT, 2014-present,WinterDev

using System;
using PixelFarm;
using PixelFarm.Drawing;
using PixelFarm.CpuBlit;
using PixelFarm.CpuBlit.VertexProcessing;
using PixelFarm.VectorMath;


using PaintLab.Svg;
using LayoutFarm.RenderBoxes;

namespace LayoutFarm.UI
{
    public abstract class BasicSprite : UIElement
    {
        public double _angle = 0;
        public double _spriteScale = 1.0;
        public double _skewX = 0;
        public double _skewY = 0;
        public override void Walk(UIVisitor visitor)
        {

        }
        public int Width { get; set; }
        public int Height { get; set; }

    }


    public class MyTestSprite : BasicSprite
    {
        SpriteShape _spriteShape;
        VgVisualElement _vgVisElem;

        float _posX, _posY;
        float _mouseDownX, _mouseDownY;
        Affine _currentTx = null;
        byte alpha;
        bool _hitTestOnSubPart;
        public MyTestSprite(VgVisualElement vgRenderVx)
        {
            this.Width = 500;
            this.Height = 500;
            AlphaValue = 255;
            _vgVisElem = vgRenderVx;
        }
        public float X { get { return _posX; } }
        public float Y { get { return _posY; } }
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
        public override RenderElement CurrentPrimaryRenderElement
        {
            get { return _spriteShape; }
        }
        public override void InvalidateGraphics()
        {
            if (this.HasReadyRenderElement)
            {
                //invalidate 'bubble' rect 
                //is (0,0,w,h) start invalidate from current primary render element
                this.CurrentPrimaryRenderElement.InvalidateGraphics();
            }
        }

        protected override bool HasReadyRenderElement
        {
            get { return _spriteShape != null; }
        }
        public override RenderElement GetPrimaryRenderElement(RootGraphic rootgfx)
        {
            if (_spriteShape == null)
            {
                //TODO: review bounds again
                RectD bounds = _vgVisElem.GetRectBounds();
                _spriteShape = new SpriteShape(_vgVisElem, rootgfx, (int)bounds.Width, (int)bounds.Height);
                _spriteShape.SetController(this);//listen event 
                _spriteShape.SetLocation((int)_posX, (int)_posY);
            }
            return _spriteShape;
        }
        public bool HitTestOnSubPart
        {
            get { return _hitTestOnSubPart; }
            set
            {
                _hitTestOnSubPart = value;
                if (_spriteShape != null)
                {
                    _hitTestOnSubPart = value;
                }
            }
        }
        public byte AlphaValue
        {
            get { return this.alpha; }
            set
            {
                this.alpha = value;
                //change alpha value
                //TODO: review here...   
                if (_spriteShape != null)
                {
                    _spriteShape.ApplyNewAlpha(value);
                }

                //int j = lionShape.NumPaths;
                //var colorBuffer = lionShape.Colors;
                //for (int i = lionShape.NumPaths - 1; i >= 0; --i)
                //{
                //    colorBuffer[i] = colorBuffer[i].NewFromChangeAlpha(alpha);
                //}
            }
        }

        public Affine CurrentAffineTx { get { return _currentTx; } }
        public void SetLocation(float left, float top)
        {
            _posX = left;
            _posY = top;
            if (_spriteShape != null)
            {
                _spriteShape.SetLocation((int)_posX, (int)_posY);
            }

        }


        public VgVisualElement HitTest(float x, float y, bool withSupPart)
        {
            VgVisualElement result = null;
            using (VgHitChainPool.Borrow(out VgHitChain svgHitChain))
            {
                svgHitChain.WithSubPartTest = withSupPart;
                if (HitTest(x, y, svgHitChain))
                {
                    int hitCount = svgHitChain.Count;
                    if (hitCount > 0)
                    {
                        result = svgHitChain.GetLastHitInfo().hitElem;
                    }
                }
            }
            return result;
        }
        public bool HitTest(float x, float y, VgHitChain svgHitChain)
        {
            RectD bounds = _spriteShape.Bounds;
            if (bounds.Contains(x, y))
            {
                _mouseDownX = x;
                _mouseDownY = y;

                //....
                if (svgHitChain.WithSubPartTest)
                {
                    //fine hit on sup part***

                    svgHitChain.SetHitTestPos(x, y);
                    _spriteShape.HitTestOnSubPart(svgHitChain);
                    //check if we hit on sup part
                    int hitCount = svgHitChain.Count;
                    if (hitCount > 0)
                    {
                        VgVisualElement svgElem = svgHitChain.GetLastHitInfo().hitElem;
                        //if yes then change its bg color
                        svgElem.VisualSpec.FillColor = Color.Red;
                        _spriteShape.InvalidateGraphics();
                    }
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


        //public override void Render(PixelFarm.Drawing.Painter p)
        //{
        //    if (_currentTx == null)
        //    {
        //        _currentTx = Affine.NewMatix(
        //              AffinePlan.Translate(-_spriteShape.Center.x, -_spriteShape.Center.y),
        //              AffinePlan.Scale(_spriteScale, _spriteScale),
        //              AffinePlan.Rotate(_angle + Math.PI),
        //              AffinePlan.Skew(_skewX / 1000.0, _skewY / 1000.0),
        //              AffinePlan.Translate(Width / 2, Height / 2)
        //      );
        //    }

        //    if (JustMove)
        //    {
        //        float ox = p.OriginX;
        //        float oy = p.OriginY;

        //        p.SetOrigin(ox + _posX, oy + _posY);
        //        _spriteShape.Paint(p);
        //        p.SetOrigin(ox, oy);

        //    }
        //    else
        //    {
        //        _spriteShape.Paint(p, _currentTx);
        //    }

        //}

        public SpriteShape GetSpriteShape()
        {
            return _spriteShape;
        }
    }



    public class SpriteShape : RenderElement
    {
        VgVisualElement _vgVisElem;
        byte _alpha;
        Vector2 _center;
        RectD _boundingRect;
        Affine _currentTx;
        public SpriteShape(VgVisualElement vgVisElem, RootGraphic root, int w, int h)
                   : base(root, w, h)
        {
            LoadFromSvg(vgVisElem);
        }
        public bool EnableHitOnSupParts { get; set; }
        protected override bool _MayHasOverlapChild()
        {
            return EnableHitOnSupParts;
        }

        public RectD Bounds
        {
            get
            {
                return _boundingRect;
            }
        }
        public void ResetTransform()
        {
            _currentTx = null;
        }
        public void ApplyTransform(Affine tx)
        {
            //apply transform to all part
            if (_currentTx == null)
            {
                _currentTx = tx;
            }
            else
            {
                //ORDER is IMPORTANT
                _currentTx = _currentTx * tx;
            }
        }
        public void ApplyTransform(Bilinear tx)
        {
            //int elemCount = _svgRenderVx.SvgVxCount;
            //for (int i = 0; i < elemCount; ++i)
            //{
            //    _svgRenderVx.SetInnerVx(i, SvgCmd.TransformToNew(_svgRenderVx.GetInnerVx(i), tx));
            //}
        }
        public Vector2 Center
        {
            get
            {
                return _center;
            }
        }
        public VgVisualElement GetRenderVx()
        {
            return _vgVisElem;
        }

        public void ApplyNewAlpha(byte alphaValue0_255)
        {
            _alpha = alphaValue0_255;
        }
        public void Paint(Painter p)
        {

            using (VgPainterArgsPool.Borrow(p, out VgPaintArgs paintArgs))
            {
                paintArgs._currentTx = _currentTx;
                _vgVisElem.Paint(paintArgs);
            }

        }
        public void Paint(VgPaintArgs paintArgs)
        {
            _vgVisElem.Paint(paintArgs);
        }

        public void Paint(Painter p, PixelFarm.CpuBlit.VertexProcessing.Perspective tx)
        {
            //TODO: implement this...
            //use prefix command for render vx
            //p.Render(_svgRenderVx);
            //_svgRenderVx.Render(p);
        }
        public void Paint(Painter p, PixelFarm.CpuBlit.VertexProcessing.Affine tx)
        {
            //TODO: implement this...
            //use prefix command for render vx 
            //------
            using (VgPainterArgsPool.Borrow(p, out VgPaintArgs paintArgs))
            {
                paintArgs._currentTx = tx;
                paintArgs.PaintVisitHandler = (vxs, painterA) =>
                {
                    //use external painter handler
                    //draw only outline with its fill-color.
                    Painter m_painter = painterA.P;
                    Color prevFillColor = m_painter.FillColor;
                    m_painter.FillColor = m_painter.FillColor;
                    m_painter.Fill(vxs);
                    m_painter.FillColor = prevFillColor;
                };
                _vgVisElem.Paint(paintArgs);
            }




        }
        public void DrawOutline(Painter p)
        {
            //walk all parts and draw only outline 
            //not fill
            //int renderVxCount = _svgRenderVx.VgCmdCount;
            //for (int i = 0; i < renderVxCount; ++i)
            //{ 
            //} 
            //int j = lionShape.NumPaths;
            //int[] pathList = lionShape.PathIndexList;
            //Drawing.Color[] colors = lionShape.Colors;

            //var vxs = GetFreeVxs();
            //var vxs2 = stroke1.MakeVxs(affTx.TransformToVxs(lionShape.Vxs, vxs), GetFreeVxs());
            //for (int i = 0; i < j; ++i)
            //{
            //    p.StrokeColor = colors[i];
            //    p.Draw(new PixelFarm.Drawing.VertexStoreSnap(vxs2, pathList[i]));

            //}
            ////not agg   
            //Release(ref vxs);
            //Release(ref vxs2);
            //return; //** 
        }

        public void LoadFromSvg(VgVisualElement svgRenderVx)
        {
            _vgVisElem = svgRenderVx;
            UpdateBounds();
            //find center 
            _center.x = (_boundingRect.Right - _boundingRect.Left) / 2.0;
            _center.y = (_boundingRect.Top - _boundingRect.Bottom) / 2.0;
        }
        public void UpdateBounds()
        {
            _vgVisElem.InvalidateBounds();
            _boundingRect = _vgVisElem.GetRectBounds();

            _boundingRect.Offset(this.X, this.Y);
            SetSize((int)_boundingRect.Width, (int)_boundingRect.Height);
        }
        public void HitTestOnSubPart(VgHitChain hitChain)
        {

            _vgVisElem.HitTest(hitChain);
        }

        public override void ResetRootGraphics(RootGraphic rootgfx)
        {
            DirectSetRootGraphics(this, rootgfx);
        }
        public override void ChildrenHitTestCore(HitChain hitChain)
        {
            base.ChildrenHitTestCore(hitChain);
        }
        public override void CustomDrawToThisCanvas(DrawBoard canvas, Rectangle updateArea)
        {
            Painter p = canvas.GetPainter();

            if (p != null)
            {
                float ox = p.OriginX;
                float oy = p.OriginY;
                //create agg's painter?
                p.SetOrigin(ox + X, oy + Y);
                Paint(p);
                p.SetOrigin(ox, oy);
            }
        }


    }
    public static class VgHitChainPool
    {

        public static TempContext<VgHitChain> Borrow(out VgHitChain hitTestArgs)
        {
            if (!Temp<VgHitChain>.IsInit())
            {
                Temp<VgHitChain>.SetNewHandler(
                    () => new VgHitChain(),
                    ch => ch.Clear()
                    );
            }
            return Temp<VgHitChain>.Borrow(out hitTestArgs);
        }
    }

   



}
