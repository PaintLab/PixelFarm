//----------------------------------------------------------------------------
//MIT, 2014-2018, WinterDev

using System;
using System.Collections.Generic;
using PixelFarm;
using PixelFarm.Drawing;
using PixelFarm.Drawing.PainterExtensions;
using PixelFarm.Agg.Transform;

namespace PixelFarm.Agg
{
    //very simple svg parser 


    public enum SvgRenderVxKind
    {
        BeginGroup,
        EndGroup,
        Path
    }

    public class SvgRenderVx : RenderVx
    {

        struct TempRenderState
        {
            public float strokeWidth;
            public Color strokeColor;
            public Color fillColor;
            public PixelFarm.Agg.Transform.Affine affineTx;
        }




        SvgVx[] _vxList;//woring vxs
        SvgVx[] _originalVxs; //original definition

        public SvgRenderVx(SvgVx[] svgVxList)
        {
            //this is original version of the element
            this._originalVxs = svgVxList;
            this._vxList = svgVxList;
        }

        public void Render(Painter p)
        {
            //
            int j = _vxList.Length;
            PixelFarm.Agg.Transform.Affine currentTx = null;

            var renderState = new TempRenderState();
            renderState.strokeColor = p.StrokeColor;
            renderState.strokeWidth = (float)p.StrokeWidth;
            renderState.fillColor = p.FillColor;
            renderState.affineTx = currentTx;

            //------------------
            VertexStore tempVxs = p.GetTempVxsStore();

            for (int i = 0; i < j; ++i)
            {
                SvgVx vx = _vxList[i];
                switch (vx.Kind)
                {
                    case SvgRenderVxKind.BeginGroup:
                        {
                            //1. save current state before enter new state


                            p.StackPushUserObject(renderState);

                            //2. enter new px context
                            if (vx.HasFillColor)
                            {
                                p.FillColor = renderState.fillColor = vx.FillColor;
                            }
                            if (vx.HasStrokeColor)
                            {
                                p.StrokeColor = renderState.strokeColor = vx.StrokeColor;
                            }
                            if (vx.HasStrokeWidth)
                            {
                                p.StrokeWidth = renderState.strokeWidth = vx.StrokeWidth;
                            }
                            if (vx.AffineTx != null)
                            {
                                //apply this to current tx
                                if (currentTx != null)
                                {
                                    currentTx = currentTx * vx.AffineTx;
                                }
                                else
                                {
                                    currentTx = vx.AffineTx;
                                }
                                renderState.affineTx = currentTx;
                            }
                        }
                        break;
                    case SvgRenderVxKind.EndGroup:
                        {
                            //restore to prev state
                            renderState = (TempRenderState)p.StackPopUserObject();
                            p.FillColor = renderState.fillColor;
                            p.StrokeColor = renderState.strokeColor;
                            p.StrokeWidth = renderState.strokeWidth;
                            currentTx = renderState.affineTx;
                        }
                        break;

                    case SvgRenderVxKind.Path:
                        {

                            VertexStore vxs = vx.GetVxs();
                            if (vx.HasFillColor)
                            {
                                //has specific fill color
                                if (vx.FillColor.A > 0)
                                {
                                    if (currentTx == null)
                                    {
                                        p.Fill(vxs, vx.FillColor);
                                    }
                                    else
                                    {
                                        //have some tx
                                        tempVxs.Clear();
                                        currentTx.TransformToVxs(vxs, tempVxs);
                                        p.Fill(tempVxs, vx.FillColor);
                                    }
                                }
                            }
                            else
                            {
                                if (p.FillColor.A > 0)
                                {
                                    if (currentTx == null)
                                    {
                                        p.Fill(vxs);
                                    }
                                    else
                                    {
                                        //have some tx
                                        tempVxs.Clear();
                                        currentTx.TransformToVxs(vxs, tempVxs);
                                        p.Fill(tempVxs);
                                    }

                                }
                            }

                            if (p.StrokeWidth > 0)
                            {
                                //check if we have a stroke version of this render vx
                                //if not then request a new one 
                                VertexStore strokeVxs = vx.GetStrokeVxsOrCreateNew(p.StrokeWidth);
                                if (vx.HasStrokeColor)
                                {
                                    //has speciic stroke color 
                                    p.StrokeWidth = vx.StrokeWidth;
                                    if (currentTx == null)
                                    {
                                        p.Fill(strokeVxs, vx.StrokeColor);
                                    }
                                    else
                                    {
                                        //have some tx
                                        tempVxs.Clear();
                                        currentTx.TransformToVxs(strokeVxs, tempVxs);
                                        p.Fill(tempVxs, vx.StrokeColor);
                                    }

                                }
                                else if (p.StrokeColor.A > 0)
                                {
                                    if (currentTx == null)
                                    {
                                        p.Fill(strokeVxs, p.StrokeColor);
                                    }
                                    else
                                    {
                                        tempVxs.Clear();
                                        currentTx.TransformToVxs(strokeVxs, tempVxs);
                                        p.Fill(tempVxs, p.StrokeColor);
                                    }
                                }
                                else
                                {

                                }
                            }
                            else
                            {

                                if (vx.HasStrokeColor)
                                {
                                    VertexStore strokeVxs = vx.GetStrokeVxsOrCreateNew(p.StrokeWidth);
                                    p.Fill(strokeVxs);
                                }
                                else if (p.StrokeColor.A > 0)
                                {
                                    VertexStore strokeVxs = vx.GetStrokeVxsOrCreateNew(p.StrokeWidth);
                                    p.Fill(strokeVxs, p.StrokeColor);
                                }
                            }
                        }
                        break;
                }
            }


            //------------------
            p.ReleaseTempVxsStore(tempVxs);
        }


        public SvgVx GetInnerVx(int index)
        {
            return _vxList[index];
        }
        public int SvgVxCount
        {
            get { return _vxList.Length; }
        }
        public void ApplyTransform(Transform.Affine affine)
        {
            //apply transform to each elements
            int j = _originalVxs.Length;

            for (int i = 0; i < j; ++i)
            {
                SvgVx vx = _originalVxs[i];
                //_vxList[i] = 
            }
        }
    }


    public class SvgVx
    {
        VertexStore _vxs;
        VertexStore _vxs_org;
        Color _fillColor;
        Color _strokeColor;
        float _strokeWidth;
        VertexStore _strokeVxs;
        double _strokeVxsStrokeWidth;

        public SvgVx(SvgRenderVxKind kind)
        {
            this.Kind = kind;
        }
        public bool HasFillColor { get; private set; }
        public bool HasStrokeColor { get; private set; }
        public bool HasStrokeWidth { get; private set; }
        public Color FillColor
        {
            get { return _fillColor; }
            set
            {
                _fillColor = value;
                HasFillColor = true;
            }
        }
        public Color StrokeColor
        {
            get { return _strokeColor; }
            set
            {
                _strokeColor = value;
                HasStrokeColor = true;
            }
        }

        public void SetVxsAsOriginal(VertexStore vxs)
        {
            this._vxs = vxs;
            this._vxs_org = vxs;
        }
        public void RestoreOrg()
        {
            _vxs = _vxs_org;
        }
        public void SetVxs(VertexStore vxs)
        {
            this._vxs = vxs;
        }
        public VertexStore GetVxs()
        {
            return _vxs;
        }
        public float StrokeWidth
        {
            get { return _strokeWidth; }
            set
            {
                _strokeWidth = value;
                HasStrokeWidth = true;
            }
        }
        public SvgRenderVxKind Kind
        {
            get;
            private set;
        }
        public Affine AffineTx { get; set; }

        public VertexStore GetStrokeVxsOrCreateNew(double strokeWidth)
        {
            if (_strokeVxs != null && _strokeWidth == strokeWidth)
            {
                //use the cache
                return _strokeVxs;
            }

            //if not create a new one,
            //review here again
            Stroke aggStrokeGen = new Stroke(_strokeVxsStrokeWidth = strokeWidth);
            _strokeVxs = new VertexStore();
            aggStrokeGen.MakeVxs(_vxs, _strokeVxs);
            return _strokeVxs;
        }

        //
        public static SvgVx TransformToNew(SvgVx originalSvgVx, Affine affineTx)
        {
            SvgVx newSx = new SvgVx(originalSvgVx.Kind);
            if (newSx._vxs != null)
            {
                VertexStore vxs = new VertexStore();
                affineTx.TransformToVxs(originalSvgVx._vxs, vxs);
                newSx._vxs = vxs;
            }

            if (originalSvgVx.HasFillColor)
            {
                newSx._fillColor = originalSvgVx._fillColor;
            }
            if (originalSvgVx.HasStrokeColor)
            {
                newSx.StrokeColor = originalSvgVx.StrokeColor;
            }
            if (originalSvgVx.HasStrokeWidth)
            {
                newSx.StrokeWidth = originalSvgVx.StrokeWidth;
            }


            return newSx;
        }
    }

}