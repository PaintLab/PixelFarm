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
    class CoordList
    {
        ArrayList<float> data = new ArrayList<float>();
        int coordCount = 0;
        public CoordList()
        {
        }
        public void AddCoord(int x, int y, int cover)
        {
            this.data.AddVertex(x);
            this.data.AddVertex(y);
            this.data.AddVertex(cover);
            this.coordCount++;
        }
        public void Clear()
        {
            this.coordCount = 0;
            this.data.Clear();
        }
        public int Count
        {
            get { return this.coordCount; }
        }

        public float[] GetInternalArray()
        {
            return this.data.Array;
        }

    }
    /// <summary
    /// to bitmap
    /// </summary>  
    public class GLScanlineRasToDestBitmapRenderer
    {

        ScanlineShader scanlineShader = new ScanlineShader();
        CoordList mySinglePixelBuffer = new CoordList();
        CoordList myLineBuffer = new CoordList();

        public GLScanlineRasToDestBitmapRenderer()
        {
            scanlineShader.InitShader();
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

            throw new NotSupportedException();
            //GL.EnableClientState(ArrayCap.ColorArray);
            //GL.EnableClientState(ArrayCap.VertexArray); 
            this.mySinglePixelBuffer.Clear();
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
            int nelements = mySinglePixelBuffer.Count;
            if (nelements > 0)
            {
                this.scanlineShader.DrawPointsWithVertexBuffer(mySinglePixelBuffer, nelements, color);
            }
            //---------------------------------------------
            //lines
            nelements = myLineBuffer.Count;
            if (nelements > 0)
            {
                this.scanlineShader.DrawLinesWithVertexBuffer(myLineBuffer, nelements, color);
            }
            //--------------------------------------------- 
            //GL.DisableClientState(ArrayCap.ColorArray);
            //GL.DisableClientState(ArrayCap.VertexArray);
            ////------------------------ 
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


            //GL.EnableClientState(ArrayCap.ColorArray);
            //GL.EnableClientState(ArrayCap.VertexArray);


            this.mySinglePixelBuffer.Clear();
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
            int nelements = mySinglePixelBuffer.Count;

            if (nelements > 0)
            {

                this.scanlineShader.DrawPointsWithVertexBuffer(mySinglePixelBuffer, nelements, color);

            }
            //---------------------------------------------
            //lines
            nelements = myLineBuffer.Count;
            if (nelements > 0)
            {

                this.scanlineShader.DrawLinesWithVertexBuffer(myLineBuffer, nelements, color);

            }
            //--------------------------------------------- 

            //GL.DisableClientState(ArrayCap.ColorArray);
            //GL.DisableClientState(ArrayCap.VertexArray);
            //------------------------ 
        }

        static void DrawPoint(float x1, float y1)
        {
            unsafe
            {
                float* arr = stackalloc float[2];
                arr[0] = x1; arr[1] = y1;


                throw new NotSupportedException();
                //GL.EnableClientState(ArrayCap.VertexArray); //***
                ////vertex
                //GL.VertexPointer(2, VertexPointerType.Float, 0, (IntPtr)arr); 
                //GL.DrawArrays(BeginMode.Points, 0, 1);
                //GL.DisableClientState(ArrayCap.VertexArray);
            }
        }

        static void DrawPoints(ArrayList<int> pointsList, ArrayList<uint> colorsList)
        {
            unsafe
            {
                int n = pointsList.Count / 2;
                int[] points = pointsList.Array;
                uint[] cbuff = colorsList.Array;

                fixed (uint* cbuff0 = &cbuff[0])
                fixed (int* arr = &points[0])
                {

                    throw new NotSupportedException();
                    //GL.EnableClientState(ArrayCap.ColorArray);
                    //GL.ColorPointer(4, ColorPointerType.UnsignedByte, 0, (IntPtr)cbuff0); 
                    //GL.EnableClientState(ArrayCap.VertexArray); //***
                    ////vertex
                    //GL.VertexPointer(2, VertexPointerType.Int, 0, (IntPtr)arr); 
                    //GL.DrawArrays(BeginMode.Points, 0, n);
                    //GL.DisableClientState(ArrayCap.VertexArray);
                    //GL.DisableClientState(ArrayCap.ColorArray);
                }
            }
        }



        static void DrawLine(float x1, float y1, float x2, float y2)
        {
            unsafe
            {
                float* arr = stackalloc float[4];
                arr[0] = x1; arr[1] = y1;
                arr[2] = x2; arr[3] = y2;

                throw new NotSupportedException();
                //GL.EnableClientState(ArrayCap.VertexArray); //***
                ////vertex
                //GL.VertexPointer(2, VertexPointerType.Float, 0, (IntPtr)arr);

                //GL.DrawArrays(BeginMode.Lines, 0, 2);
                //GL.DisableClientState(ArrayCap.VertexArray);
            }
        }
        const int BASE_MASK = 255;



        void GLBlendHL(int x1, int y, int x2, LayoutFarm.Drawing.Color color, byte cover)
        {
            //if (color.A == 0) { return; }

            int len = x2 - x1 + 1;
            int alpha = (((int)(color.A) * (cover + 1)) >> 8);
            var singlePxBuff = this.mySinglePixelBuffer;
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
                var pointAndColors = this.mySinglePixelBuffer;

                do
                {
                    //alpha change ***
                    int alpha = ((sourceColor.A) * ((covers[coversIndex]) + 1)) >> 8;
                    //pointAndColors.AddVertex(new VertexV2S1Cvr(xpos, y, alpha));
                    pointAndColors.AddCoord(xpos, y, alpha);
                    //if (alpha == BASE_MASK)
                    //{
                    //    pointAndColors.AddVertex(
                    //        new VertexV2S1Cvr(xpos, y, alpha));
                    //}
                    //else
                    //{
                    //    pointAndColors.AddVertex(
                    //        new VertexC4V2S(LayoutFarm.Drawing.Color.FromArgb(alpha, sourceColor).ToARGB(),
                    //        xpos, y));
                    //}
                    xpos++;
                    coversIndex++;
                }
                while (--len != 0);
            }
        }
    }

}
