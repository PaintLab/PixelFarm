//2014 BSD,WinterDev   

//MatterHackers
//----------------------------------------------------------------------------
// Anti-Grain Geometry - Version 2.4
// Copyright (C) 2002-2005 Maxim Shemanarev (http://www.antigrain.com)
//
// Permission to copy, use, modify, sell and distribute this software 
// is granted provided this copyright notice appears in all copies. 
// This software is provided "as is" without express or implied
// warranty, and with no claim as to its suitability for any purpose.
//
//----------------------------------------------------------------------------
// Contact: mcseem@antigrain.com
//          mcseemagg@yahoo.com
//          http://www.antigrain.com
//----------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

using PixelFarm.Agg.Image;
using PixelFarm.Agg.VertexSource;
using OpenTK;
using OpenTK.Graphics.ES20;
using LayoutFarm.DrawingGL;

namespace PixelFarm.Agg
{

    /// <summary
    /// to bitmap
    /// </summary>  
    class GLScanlineRasToDestBitmapRenderer
    {

        BasicShader scanlineShader;
        // CoordList3f mySinglePixelBuffer = new CoordList3f();
        CoordList3f myLineBuffer = new CoordList3f();

        public GLScanlineRasToDestBitmapRenderer(BasicShader scanlineShader)
        {
            this.scanlineShader = scanlineShader;

        }
        internal void SetViewMatrix(MyMat4 mat)
        {
            scanlineShader.ViewMatrix = mat;
        }
        /// <summary>
        /// for fill shape
        /// </summary>
        /// <param name="sclineRas"></param>
        /// <param name="scline"></param>
        /// <param name="color"></param>
        /// <param name="shapeHint"></param>
        public void FillWithColor(GLScanlineRasterizer sclineRas,
                GLScanline scline,
                LayoutFarm.Drawing.Color color)
        {

            //early exit
            if (color.A == 0) { return; }
            if (!sclineRas.RewindScanlines()) { return; }
            //----------------------------------------------- 

            scline.ResetSpans(sclineRas.MinX, sclineRas.MaxX);
            //-----------------------------------------------  


            //this.mySinglePixelBuffer.Clear();
            this.myLineBuffer.Clear();

            while (sclineRas.SweepScanline(scline))
            {
                int y = scline.Y;
                int num_spans = scline.SpanCount;
                byte[] covers = scline.GetCovers();
                for (int i = 1; i <= num_spans; ++i)
                {
                    ScanlineSpan span = scline.GetSpan(i);
                    if (span.len > 0)
                    {
                        //outline
                        GLBlendSolidHSpan(span.x, y, span.len, color, covers, span.cover_index);
                    }
                    else
                    {
                        //fill
                        int x = span.x;
                        int x2 = (x - span.len - 1);
                        GLBlendHL(x, y, x2, color, covers[span.cover_index]);
                    }
                }
            }

            //----------------------------------------
            //draw ...
            //points
            //int nelements = mySinglePixelBuffer.Count;
            //if (nelements > 0)
            //{
            //    this.scanlineShader.DrawPointsWithVertexBuffer(mySinglePixelBuffer, nelements, color);
            //}
            //---------------------------------------------
            //lines
            int nelements = myLineBuffer.Count;
            if (nelements > 0)
            {
                this.scanlineShader.AggDrawLines(myLineBuffer, nelements, color);
            }

        }
        ///// <summary>
        ///// for lines
        ///// </summary>
        ///// <param name="sclineRas"></param>
        ///// <param name="scline"></param>
        ///// <param name="color"></param>
        ///// <param name="shapeHint"></param>
        //public void DrawWithColor(GLScanlineRasterizer sclineRas,
        //        GLScanline scline,
        //        LayoutFarm.Drawing.Color color)
        //{

        //    //early exit
        //    if (color.A == 0) { return; }
        //    if (!sclineRas.RewindScanlines()) { return; }
        //    //-----------------------------------------------  
        //    scline.ResetSpans(sclineRas.MinX, sclineRas.MaxX);
        //    //-----------------------------------------------   
        //    this.mySinglePixelBuffer.Clear();
        //    this.myLineBuffer.Clear();

        //    while (sclineRas.SweepScanline(scline))
        //    {
        //        int y = scline.Y;
        //        int num_spans = scline.SpanCount;
        //        byte[] covers = scline.GetCovers();
        //        for (int i = 1; i <= num_spans; ++i)
        //        {
        //            ScanlineSpan span = scline.GetSpan(i);
        //            if (span.len > 0)
        //            {
        //                //outline
        //                GLBlendSolidHSpan(span.x, y, span.len, color, covers, span.cover_index);
        //            }
        //            else
        //            {
        //                //fill
        //                int x = span.x;
        //                int x2 = (x - span.len - 1);
        //                GLBlendHL(x, y, x2, color, covers[span.cover_index]);
        //            }
        //        }
        //    }

        //    //---------------------------------------------
        //    //points
        //    //Angle under d3d9 (shader model=2) not works with PointSize
        //    //so we use point as a line with length 1

        //    int nelements = mySinglePixelBuffer.Count;
        //    if (nelements > 0)
        //    {

        //        this.scanlineShader.DrawPointsWithVertexBuffer(mySinglePixelBuffer, nelements, color);

        //    }
        //    ////---------------------------------------------
        //    ////lines
        //    nelements = myLineBuffer.Count;
        //    if (nelements > 0)
        //    {

        //        this.scanlineShader.DrawLinesWithVertexBuffer(myLineBuffer, nelements, color);

        //    }
        //}

        /// <summary>
        /// for lines
        /// </summary>
        /// <param name="sclineRas"></param>
        /// <param name="scline"></param>
        /// <param name="color"></param>
        /// <param name="shapeHint"></param>
        public void DrawWithColor(GLScanlineRasterizer sclineRas,
                GLScanline scline,
                LayoutFarm.Drawing.Color color)
        {

            //early exit
            if (color.A == 0) { return; }
            if (!sclineRas.RewindScanlines()) { return; }
            //-----------------------------------------------  
            scline.ResetSpans(sclineRas.MinX, sclineRas.MaxX);
            //-----------------------------------------------   
            //this.mySinglePixelBuffer.Clear();
            this.myLineBuffer.Clear();

            while (sclineRas.SweepScanline(scline))
            {
                int y = scline.Y;
                int num_spans = scline.SpanCount;
                byte[] covers = scline.GetCovers();
                for (int i = 1; i <= num_spans; ++i)
                {
                    ScanlineSpan span = scline.GetSpan(i);
                    if (span.len > 0)
                    {
                        //outline
                        GLBlendSolidHSpan(span.x, y, span.len, color, covers, span.cover_index);
                    }
                    else
                    {
                        //fill
                        int x = span.x;
                        int x2 = (x - span.len - 1);
                        GLBlendHL(x, y, x2, color, covers[span.cover_index]);
                    }
                }
            }

            //---------------------------------------------
            //points
            //Angle under d3d9 (shader model=2) not works with PointSize
            //so we use point as a line with length 1 
            //int nelements = mySinglePixelBuffer.Count;
            //if (nelements > 0)
            //{

            //    this.scanlineShader.DrawPointsWithVertexBuffer(mySinglePixelBuffer, nelements, color);

            //}
            ////---------------------------------------------
            ////lines
            int nelements = myLineBuffer.Count;
            if (nelements > 0)
            {   
                this.scanlineShader.AggDrawLines(myLineBuffer, nelements, color); 
            }
        }





        const int BASE_MASK = 255;
        void GLBlendHL(int x1, int y, int x2, LayoutFarm.Drawing.Color color, byte cover)
        {
            //if (color.A == 0) { return; }

            int len = x2 - x1 + 1;
            int alpha = (((int)(color.A) * (cover + 1)) >> 8);
            var singlePxBuff = this.myLineBuffer;// this.mySinglePixelBuffer;
            var lineBuffer = this.myLineBuffer;

            //same alpha...
            if (alpha == BASE_MASK)
            {

                switch (len)
                {
                    case 0:
                        {
                        } break;
                    case 1:
                        {
                            //singlePxBuff.AddVertex(new VertexV2S1Cvr(
                            //    LayoutFarm.Drawing.Color.FromArgb(alpha, color).ToARGB(),
                            //    x1, y));

                            singlePxBuff.AddCoord(x1, y, alpha);
                            //LayoutFarm.Drawing.Color.FromArgb(alpha, color).ToARGB(),
                            //x1, y));
                        } break;
                    default:
                        {
                            //var c = LayoutFarm.Drawing.Color.FromArgb(alpha, color).ToARGB();
                            //lineBuffer.AddVertex(new VertexV2S1Cvr(x1, y, alpha));
                            //lineBuffer.AddVertex(new VertexV2S1Cvr(x2 + 1, y, alpha));
                            lineBuffer.AddCoord(x1, y, alpha);
                            lineBuffer.AddCoord(x2 + 1, y, alpha);
                        } break;
                }
            }
            else
            {
                int xpos = x1;
                do
                {
                    //singlePxBuff.AddVertex(new VertexV2S1Cvr(xpos, y, alpha));
                    singlePxBuff.AddCoord(xpos, y, alpha);
                    xpos++;
                }
                while (--len != 0);
            }
        }
        void GLBlendSolidHSpan(int x, int y, int len,
            LayoutFarm.Drawing.Color sourceColor,
            byte[] covers, int coversIndex)
        {

            unchecked
            {

                int xpos = x;
                var pointAndColors = this.myLineBuffer;//this.mySinglePixelBuffer;

                do
                {
                    //alpha change ***
                    int alpha = ((sourceColor.A) * ((covers[coversIndex]) + 1)) >> 8;
                   
                    pointAndColors.AddCoord(xpos, y, alpha);
                    pointAndColors.AddCoord(xpos + 1, y, alpha);
              
                    xpos++;
                    coversIndex++;
                }
                while (--len != 0);
            }
        }
    }

}
