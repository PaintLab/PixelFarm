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

        AggCoordList3f myLineBuffer = new AggCoordList3f();

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


            var lineBuff = this.myLineBuffer;
            lineBuff.Clear();

            while (sclineRas.SweepScanline(scline))
            {
                int y = scline.Y;
                lineBuff.BeginNewLine(y);
                int num_spans = scline.SpanCount;
                byte[] covers = scline.GetCovers();
                for (int i = 1; i <= num_spans; ++i)
                {
                    ScanlineSpan span = scline.GetSpan(i);
                    if (span.len > 0)
                    {
                        //outline
                        GLBlendSolidHSpan(span.x, y, span.len, lineBuff, color.A, covers, span.cover_index);
                    }
                    else
                    {
                        //fill
                        int x = span.x;
                        int x2 = (x - span.len - 1);
                        GLBlendHL(x, y, x2, lineBuff, color.A, covers[span.cover_index]);
                    }
                }

                lineBuff.CloseLine();
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

            var lineBuff = this.myLineBuffer;
            lineBuff.Clear();
            int srcColorA = color.A;
            while (sclineRas.SweepScanline(scline))
            {
                int y = scline.Y;
                lineBuff.BeginNewLine(y); 
                int num_spans = scline.SpanCount;
                byte[] covers = scline.GetCovers();
                
                for (int i = 1; i <= num_spans; ++i)
                {
                    ScanlineSpan span = scline.GetSpan(i);
                    if (span.len > 0)
                    {
                        //outline
                        GLBlendSolidHSpan(span.x, y, span.len, lineBuff, srcColorA, covers, span.cover_index);
                    }
                    else
                    {
                        //negative span
                        //fill
                        int x = span.x;
                        int x2 = (x - span.len - 1);
                        GLBlendHL(x, y, x2, lineBuff, srcColorA, covers[span.cover_index]);
                    }
                }
                lineBuff.CloseLine(); 
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
                myLineBuffer.Clear();
            }
        }
        const int BASE_MASK = 255;
        static void GLBlendHL(int x1, int y, int x2, AggCoordList3f lineBuffer, int srcColorAlpha, byte cover)
        {
            //if (color.A == 0) { return; }

            int len = x2 - x1 + 1;
            int alpha = (((int)(srcColorAlpha) * (cover + 1)) >> 8);

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
                            //single point
                            lineBuffer.AddCoord(x1, alpha);
                            lineBuffer.AddCoord(x1 + 1, alpha);
                        } break;
                    default:
                        {
                            //var c = LayoutFarm.Drawing.Color.FromArgb(alpha, color).ToARGB();
                            //lineBuffer.AddVertex(new VertexV2S1Cvr(x1, y, alpha));
                            //lineBuffer.AddVertex(new VertexV2S1Cvr(x2 + 1, y, alpha));
                            lineBuffer.AddCoord(x1, alpha);
                            lineBuffer.AddCoord(x2 + 1, alpha);
                        } break;
                }
            }
            else
            {

                int xpos = x1;
                do
                {
                    //singlePxBuff.AddVertex(new VertexV2S1Cvr(xpos, y, alpha));
                    lineBuffer.AddCoord(xpos, alpha);
                    lineBuffer.AddCoord(xpos + 1, alpha);
                    xpos++;
                }
                while (--len != 0);
            }
        }
        static void GLBlendSolidHSpan(
            int x, int y, int len, AggCoordList3f lineBuffer, int srcColorAlpha,
            byte[] covers, int coversIndex)
        {

            unchecked
            {

                int xpos = x;
                //var pointAndColors = this.myLineBuffer;//this.mySinglePixelBuffer;
                switch (len)
                {
                    case 1:
                        {
                            //just one pix , 
                            //alpha change ***
                            int alpha = ((srcColorAlpha) * ((covers[coversIndex]) + 1)) >> 8;
                            //single px
                            lineBuffer.AddCoord(xpos, alpha);
                            lineBuffer.AddCoord(xpos + 1, alpha);
                            xpos++;
                            coversIndex++;

                        } break;
                    default:
                        {
                            int alpha = 0;
                            do
                            {
                                //alpha change ***
                                alpha = ((srcColorAlpha) * ((covers[coversIndex]) + 1)) >> 8;
                                //single point
                                lineBuffer.AddCoord(xpos, alpha);
                                xpos++;
                                coversIndex++;
                            }
                            while (--len != 0);
                            //close with single px
                            lineBuffer.AddCoord(xpos, 0);

                        } break;
                }
            }
        }
    }

}
