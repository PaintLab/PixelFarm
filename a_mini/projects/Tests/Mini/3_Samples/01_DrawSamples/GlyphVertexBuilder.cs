//MIT, 2016,  WinterDev
using System;
using System.Collections.Generic;
using NRasterizer;

using System.Drawing;
using System.Drawing.Imaging;

namespace PixelFarm.Agg
{
    public class GlyphVxsBuilder
    {
        readonly Typeface _typeface;
        const int pointsPerInch = 72;

        public GlyphVxsBuilder(Typeface typeface)
        {
            _typeface = typeface;
        }
        const double FT_RESIZE = 64; //essential to be floating point
        PixelFarm.Agg.VertexSource.PathWriter ps = new PixelFarm.Agg.VertexSource.PathWriter();


        void RenderGlyph(ushort[] contours, FtPoint[] ftpoints, Flag[] flags)
        {

            //outline version
            //-----------------------------
            int npoints = ftpoints.Length;
            List<PixelFarm.VectorMath.Vector2> points = new List<PixelFarm.VectorMath.Vector2>(npoints);
            int startContour = 0;
            int cpoint_index = 0;
            int todoContourCount = contours.Length;
            ps.Clear();

            int controlPointCount = 0;
            while (todoContourCount > 0)
            {
                int nextContour = contours[startContour] + 1;
                bool isFirstPoint = true;
                FtPointD secondControlPoint = new FtPointD();
                FtPointD thirdControlPoint = new FtPointD();
                bool justFromCurveMode = false;

                for (; cpoint_index < nextContour; ++cpoint_index)
                {
                    FtPoint vpoint = ftpoints[cpoint_index];
                    int vtag = (int)flags[cpoint_index] & 0x1;
                    //bool has_dropout = (((vtag >> 2) & 0x1) != 0);
                    //int dropoutMode = vtag >> 3;
                    if ((vtag & 0x1) != 0)
                    {
                        //on curve
                        if (justFromCurveMode)
                        {
                            switch (controlPointCount)
                            {
                                case 1:
                                    {
                                        ps.Curve3(secondControlPoint.x / FT_RESIZE, secondControlPoint.y / FT_RESIZE,
                                            vpoint.X / FT_RESIZE, vpoint.Y / FT_RESIZE);
                                    }
                                    break;
                                case 2:
                                    {
                                        ps.Curve4(secondControlPoint.x / FT_RESIZE, secondControlPoint.y / FT_RESIZE,
                                           thirdControlPoint.x / FT_RESIZE, thirdControlPoint.y / FT_RESIZE,
                                           vpoint.X / FT_RESIZE, vpoint.Y / FT_RESIZE);
                                    }
                                    break;
                                default:
                                    {
                                        throw new NotSupportedException();
                                    }
                            }
                            controlPointCount = 0;
                            justFromCurveMode = false;
                        }
                        else
                        {
                            if (isFirstPoint)
                            {
                                isFirstPoint = false;
                                ps.MoveTo(vpoint.X / FT_RESIZE, vpoint.Y / FT_RESIZE);
                            }
                            else
                            {
                                ps.LineTo(vpoint.X / FT_RESIZE, vpoint.Y / FT_RESIZE);
                            }

                            //if (has_dropout)
                            //{
                            //    //printf("[%d] on,dropoutMode=%d: %d,y:%d \n", mm, dropoutMode, vpoint.x, vpoint.y);
                            //}
                            //else
                            //{
                            //    //printf("[%d] on,x: %d,y:%d \n", mm, vpoint.x, vpoint.y);
                            //}
                        }
                    }
                    else
                    {
                        switch (controlPointCount)
                        {
                            case 0:
                                {   //bit 1 set=> off curve, this is a control point
                                    //if this is a 2nd order or 3rd order control point
                                    if (((vtag >> 1) & 0x1) != 0)
                                    {
                                        //printf("[%d] bzc3rd,  x: %d,y:%d \n", mm, vpoint.x, vpoint.y);
                                        thirdControlPoint = new FtPointD(vpoint);
                                    }
                                    else
                                    {
                                        //printf("[%d] bzc2nd,  x: %d,y:%d \n", mm, vpoint.x, vpoint.y);
                                        secondControlPoint = new FtPointD(vpoint);
                                    }
                                }
                                break;
                            case 1:
                                {
                                    if (((vtag >> 1) & 0x1) != 0)
                                    {
                                        //printf("[%d] bzc3rd,  x: %d,y:%d \n", mm, vpoint.x, vpoint.y);
                                        thirdControlPoint = new FtPointD(vpoint.X, vpoint.Y);
                                    }
                                    else
                                    {
                                        //we already have prev second control point
                                        //so auto calculate line to 
                                        //between 2 point
                                        FtPointD mid = GetMidPoint(secondControlPoint, vpoint);
                                        //----------
                                        //generate curve3
                                        ps.Curve3(secondControlPoint.x / FT_RESIZE, secondControlPoint.y / FT_RESIZE,
                                            mid.x / FT_RESIZE, mid.y / FT_RESIZE);
                                        //------------------------
                                        controlPointCount--;
                                        //------------------------
                                        //printf("[%d] bzc2nd,  x: %d,y:%d \n", mm, vpoint.x, vpoint.y);
                                        secondControlPoint = new FtPointD(vpoint);
                                    }
                                }
                                break;
                            default:
                                {
                                    throw new NotSupportedException();
                                }
                                break;
                        }

                        controlPointCount++;
                        justFromCurveMode = true;
                    }
                }
                //--------
                //close figure
                //if in curve mode
                if (justFromCurveMode)
                {
                    switch (controlPointCount)
                    {
                        case 0: break;
                        case 1:
                            {
                                ps.Curve3(secondControlPoint.x / FT_RESIZE, secondControlPoint.y / FT_RESIZE,
                                    ps.LastMoveX, ps.LastMoveY);
                            }
                            break;
                        case 2:
                            {
                                ps.Curve4(secondControlPoint.x / FT_RESIZE, secondControlPoint.y / FT_RESIZE,
                                   thirdControlPoint.x / FT_RESIZE, thirdControlPoint.y / FT_RESIZE,
                                   ps.LastMoveX, ps.LastMoveY);
                            }
                            break;
                        default:
                            { throw new NotSupportedException(); }
                    }
                    justFromCurveMode = false;
                    controlPointCount = 0;
                }
                ps.CloseFigure();
                //--------                   
                startContour++;
                todoContourCount--;
            }
        }
        static FtPointD GetMidPoint(FtPoint v1, FtPoint v2)
        {
            return new FtPointD(
                ((double)v1.X + (double)v2.X) / 2d,
                ((double)v1.Y + (double)v2.Y) / 2d);
        }
        static FtPointD GetMidPoint(FtPointD v1, FtPointD v2)
        {
            return new FtPointD(
                ((double)v1.x + (double)v2.x) / 2d,
                ((double)v1.y + (double)v2.y) / 2d);
        }
        static FtPointD GetMidPoint(FtPointD v1, FtPoint v2)
        {
            return new FtPointD(
                (v1.x + (double)v2.X) / 2d,
                (v1.y + (double)v2.Y) / 2d);
        }

        void RenderGlyph(Glyph glyph)
        {
            ushort[] endPoints;
            Flag[] flags;
            FtPoint[] ftpoints = glyph.GetPoints(out endPoints, out flags);
            RenderGlyph(endPoints, ftpoints, flags);
        }
        /// <summary>
        /// create vertex store 
        /// </summary>
        /// <param name="c"></param>
        /// <param name="size"></param>
        /// <param name="resolution"></param>
        /// <param name="toFlags"></param>
        /// <returns></returns>
        public VertexStore CreateVxs(char c, int size, int resolution)
        {
            float scale = (float)(size * resolution) / (pointsPerInch * _typeface.UnitsPerEm);
            RenderGlyph(_typeface.Lookup(c));
            return ps.Vxs;
        }
    }
}