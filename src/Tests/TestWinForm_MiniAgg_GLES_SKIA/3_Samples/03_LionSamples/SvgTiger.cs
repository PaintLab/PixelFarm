//BSD, 2014-2018, WinterDev 

using System;
using System.Collections.Generic;
using Mini;
using PaintLab.Svg;
using PixelFarm.Drawing;

namespace PixelFarm.Agg.Samples
{
    [Info(OrderCode = "03")]
    [Info("Test Svg")]
    public class SvgTiger : DemoBase
    {
        SvgRenderVx[] vxList;
        Agg.Transform.Affine affine1;
        public override void Init()
        {
            SvgParser svg = new SvgParser();
            //svg.ReadSvgDocument("d:\\WImageTest\\lion.svg");
            svg.ReadSvgDocument("d:\\WImageTest\\tiger.svg");
            vxList = svg.GetResult();
            affine1 = Agg.Transform.Affine.NewTranslation(100, 0);
        }
        VertexStore tempVxs = new VertexStore();
        Stack<TempRenderState> _renderStateContext = new Stack<TempRenderState>();

        public override void Draw(PixelFarm.Drawing.Painter p)
        {
            try
            {

                p.Clear(Drawing.Color.White);
                
                int j = vxList.Length;

               // p.SetOrigin(300, 200);
                p.StrokeColor = Color.Transparent;
                p.StrokeWidth = 1;//svg standard, init stroke-width =1

                PixelFarm.Agg.Transform.Affine currentTx = null;
                TempRenderState renderState = new TempRenderState();
                renderState.strokeColor = p.StrokeColor;
                renderState.strokeWidth = (float)p.StrokeWidth;
                renderState.fillColor = p.FillColor;
                renderState.affineTx = currentTx;

                //

                for (int i = 0; i < j; ++i)
                {
                    SvgRenderVx vx = vxList[i];
                    switch (vx.Kind)
                    {
                        case SvgRenderVxKind.BeginGroup:
                            {
                                //1. save current state before enter new state
                                _renderStateContext.Push(renderState);
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
                                renderState = _renderStateContext.Pop();
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
            }
            catch (Exception ex)
            {

            }
        }
        public override void MouseDrag(int x, int y)
        {

        }


        struct TempRenderState
        {
            public float strokeWidth;
            public Color strokeColor;
            public Color fillColor;
            public PixelFarm.Agg.Transform.Affine affineTx;
        }
    }
}