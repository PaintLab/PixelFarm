//BSD, 2014-present, WinterDev
//MatterHackers

using System;
using System.Collections.Generic;

using PixelFarm.CpuBlit.VertexProcessing;
using PixelFarm.Drawing;

using Mini;
namespace PixelFarm.CpuBlit.Sample_PolygonClipping
{


    [Info(OrderCode = "20")]
    public class PolygonClippingDemo : DemoBase
    {
        double _x;
        double _y;
        Color _backgroundColor;

        public PolygonClippingDemo()
        {
            _backgroundColor = Color.White;
            this.Width = 800;
            this.Height = 600;
        }
        [DemoConfig]
        public PolygonExampleSet PolygonSet
        {
            get;
            set;
        }
        [DemoConfig]
        public OperationOption OpOption
        {
            get;
            set;
        }
        public override void Draw(Painter p)
        {
            p.Clear(Color.White);
            if (_backgroundColor.Alpha0To255 > 0)
            {
                p.FillColor = _backgroundColor;
                p.FillRect(0, 0, this.Width, Height);
            }
            RenderPolygon(p);
        }


        void RenderPolygon(Painter p)
        {
            switch (this.PolygonSet)
            {
                case PolygonExampleSet.TwoSimplePaths:
                    {
                        //------------------------------------
                        // Two simple paths
                        using (VxsTemp.Borrow(out var v1, out var v2))
                        {
                            double x = _x - Width / 2 + 100;
                            double y = _y - Height / 2 + 100;

                            PolygonClippingDemoHelper.WritePath1(v1, x, y);
                            PolygonClippingDemoHelper.WritePath2(v2, x, y);

                            p.Fill(v1, ColorEx.Make(0f, 0f, 0f, 0.1f));
                            p.Fill(v2, ColorEx.Make(0f, 0.6f, 0f, 0.1f));

                            CreateAndRenderCombined(p, v1, v2);
                        }
                    }
                    break;
                case PolygonExampleSet.CloseStroke:
                    {
                        //------------------------------------
                        // Closed stroke 
                        using (VxsTemp.Borrow(out var v1, out var v2))
                        {
                            double x = _x - Width / 2 + 100;
                            double y = _y - Height / 2 + 100;

                            PolygonClippingDemoHelper.WritePath1(v1, x, y);
                            PolygonClippingDemoHelper.WritePath2(v2, x, y);

                            p.FillStroke(v1, 2, ColorEx.Make(0f, 0f, 0f, 0.1f));
                            p.FillStroke(v2, 3, ColorEx.Make(0f, 0.6f, 0f, 0.1f));

                            CreateAndRenderCombined(p, v1, v2);
                        }
                    }
                    break;
                case PolygonExampleSet.GBAndArrow:
                    {
                        //------------------------------------
                        // Great Britain and Arrows
                        using (VxsTemp.Borrow(out var v1_gb_poly, out var v2_arrows))
                        using (VxsTemp.Borrow(out var v3))
                        {
                            Affine mtx1 = Affine.New(
                                    AffinePlan.Translate(-1150, -1150),
                                    AffinePlan.Scale(2)
                                 );
                            PolygonClippingDemoHelper.WriteGBObject(v1_gb_poly, 0, 0, mtx1);

                            p.Fill(v1_gb_poly, ColorEx.Make(0.5f, 0.5f, 0f, 0.1f));
                            p.FillStroke(v1_gb_poly, 0.1f, ColorEx.Make(0, 0, 0));

                            //
                            Affine mtx2 = mtx1 * Affine.NewTranslation(_x - Width / 2, _y - Height / 2);
                            PolygonClippingDemoHelper.WriteArrow(v2_arrows, 0, 0, mtx2);
                            p.Fill(v2_arrows, ColorEx.Make(0f, 0.5f, 0.5f, 0.1f));

                            CreateAndRenderCombined(p, v1_gb_poly, v2_arrows);
                        }
                    }
                    break;
                case PolygonExampleSet.GBAndSpiral:
                    {
                        //------------------------------------
                        // Great Britain and a Spiral
                        // 
                        using (VxsTemp.Borrow(out var v1_gb_poly))
                        using (VxsTemp.Borrow(out var v2_spiral, out var v2_spiral_outline))
                        using (VectorToolBox.Borrow(out Stroke stroke))
                        {
                            Affine mtx = Affine.New(
                                    AffinePlan.Translate(-1150, -1150),
                                    AffinePlan.Scale(2));

                            PolygonClippingDemoHelper.WriteGBObject(v1_gb_poly, 0, 0, mtx);
                            PolygonClippingDemoHelper.WriteSpiral(v2_spiral, _x, _y);

                            p.Fill(v1_gb_poly, ColorEx.Make(0.5f, 0.5f, 0f, 0.1f));
                            p.FillStroke(v1_gb_poly, 0.1f, Color.Black);

                            stroke.Width = 15;
                            p.Fill(stroke.MakeVxs(v2_spiral, v2_spiral_outline), ColorEx.Make(0.0f, 0.5f, 0.5f, 0.1f));

                            CreateAndRenderCombined(p, v1_gb_poly, v2_spiral_outline);
                        }
                    }
                    break;
                case PolygonExampleSet.SprialAndGlyph:
                    {
                        //------------------------------------
                        // Spiral and glyph                         
                        using (VxsTemp.Borrow(out var v1_spiral, out var v1_spiralOutline, out var v3))
                        using (VxsTemp.Borrow(out var glyph_vxs))
                        using (VectorToolBox.Borrow(out Stroke stroke))
                        {

                            Affine mtx = Affine.New(
                               AffinePlan.Scale(4),
                               AffinePlan.Translate(220, 200));

                            PolygonClippingDemoHelper.WriteSpiral(v1_spiral, _x, _y);
                            PolygonClippingDemoHelper.WriteGlyphObj(glyph_vxs, 0, 0, mtx);

                            //-----------------------------------------      
                            stroke.Width = 1;
                            stroke.MakeVxs(v1_spiral, v1_spiralOutline);

                            CreateAndRenderCombined(p, v1_spiralOutline, glyph_vxs);

                            p.Fill(v1_spiralOutline, ColorEx.Make(0f, 0f, 0f, 0.1f));
                            p.Fill(glyph_vxs, ColorEx.Make(0f, 0.6f, 0f, 0.1f));
                        }

                    }
                    break;
            }
        }


        void CreateAndRenderCombined(Painter p, VertexStore vxsSnap1, VertexStore vxsSnap2)
        {
            //TODO: review here again. 

            List<VertexStore> resultPolygons = new List<VertexStore>();
            PolygonClippingDemoHelper.CreateAndRenderCombined(vxsSnap1, vxsSnap2, OpOption, false, resultPolygons);
            if (resultPolygons.Count > 0)
            {
                //draw combine result
                p.Fill(resultPolygons[0], ColorEx.Make(0.5f, 0.0f, 0f, 0.5f));
            }
        }
        public override void MouseDrag(int x, int y)
        {
            _x = x;
            _y = y;
        }
        public override void MouseDown(int x, int y, bool isRightoy)
        {
            _x = x;
            _y = y;
        }
        public override void MouseUp(int x, int y)
        {
            _x = x;
            _y = y;
        }
    }


}
