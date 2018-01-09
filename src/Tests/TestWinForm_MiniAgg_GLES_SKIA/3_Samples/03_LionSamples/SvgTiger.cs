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
            svg.ReadSvgDocument("d:\\WImageTest\\tiger.svg");
            vxList = svg.GetResult();
            affine1 = Agg.Transform.Affine.NewTranslation(100, 0);
        }
        VertexStore tempVxs = new VertexStore();

        public override void Draw(PixelFarm.Drawing.Painter p)
        {
            try
            {
                p.Clear(Drawing.Color.White);
                //
                int j = vxList.Length;
                for (int i = 0; i < j; ++i)
                {
                    SvgRenderVx vx = vxList[i];
                    switch (vx.Kind)
                    {
                        case SvgRenderVxKind.BeginGroup:
                            if (vx.HasFillColor)
                            {
                                p.FillColor = vx.FillColor;
                            }
                            break;
                        case SvgRenderVxKind.EndGroup:

                            break;
                        case SvgRenderVxKind.Path:
                            {
                                //<path id = "path8" d = "m-122.3,84.285s0.1,1.894-0.73,1.875c-0.82-0.019-17.27-48.094-37.8-45.851,0,0,17.78-7.353,38.53,43.976z" />

                                VertexStore vxs = vx.GetVxs();
                                if (vx.HasFillColor)
                                {
                                    p.FillColor = vx.FillColor;
                                }
                                p.Fill(vxs);
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
        }
    }
}