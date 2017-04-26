//MIT, 2016-2017, WinterDev
//-----------------------------------  
using System;
using System.Collections.Generic;


namespace Typography.Rendering
{
    public static class MsdfGlyphGen
    {
        public static Msdfgen.Shape CreateMsdfShape(GlyphContourBuilder glyphToContour, float pxScale = 1)
        {
            List<GlyphContour> cnts = glyphToContour.GetContours();
            List<GlyphContour> newFitContours = new List<GlyphContour>();
            int j = cnts.Count;
            for (int i = 0; i < j; ++i)
            {
                newFitContours.Add(
                    CreateFitContour(
                        cnts[i], pxScale, false, true));
            }
            return CreateMsdfShape(newFitContours);
        }


        static Msdfgen.Shape CreateMsdfShape(List<GlyphContour> contours)
        {
            var shape = new Msdfgen.Shape();
            int j = contours.Count;
            for (int i = 0; i < j; ++i)
            {
                var cnt = new Msdfgen.Contour();
                shape.contours.Add(cnt);

                GlyphContour contour = contours[i];
                List<GlyphPart> parts = contour.parts;
                int m = parts.Count;
                for (int n = 0; n < m; ++n)
                {
                    GlyphPart p = parts[n];
                    switch (p.Kind)
                    {
                        default: throw new NotSupportedException();
                        case GlyphPartKind.Curve3:
                            {
                                GlyphCurve3 curve3 = (GlyphCurve3)p;
                                var p0 = curve3.FirstPoint;
                                cnt.AddQuadraticSegment(
                                    p0.X, p0.Y,
                                    curve3.x1, curve3.y1,
                                    curve3.x2, curve3.y2
                                   );
                            }
                            break;
                        case GlyphPartKind.Curve4:
                            {
                                GlyphCurve4 curve4 = (GlyphCurve4)p;
                                var p0 = curve4.FirstPoint;
                                cnt.AddCubicSegment(
                                    p0.X, p0.Y,
                                    curve4.x1, curve4.y1,
                                    curve4.x2, curve4.y2,
                                    curve4.x3, curve4.y3);
                            }
                            break;
                        case GlyphPartKind.Line:
                            {
                                GlyphLine line = (GlyphLine)p;
                                var p0 = line.FirstPoint;
                                cnt.AddLine(
                                    p0.X, p0.Y,
                                    line.x1, line.y1);
                            }
                            break;
                    }
                }
            }
            return shape;
        }
        static GlyphContour CreateFitContour(GlyphContour contour, float pixelScale, bool x_axis, bool y_axis)
        {
            GlyphContour newc = new GlyphContour();
            List<GlyphPart> parts = contour.parts;
            int m = parts.Count;
            GlyphPart latestPart = null;
            for (int n = 0; n < m; ++n)
            {
                GlyphPart p = parts[n];
                switch (p.Kind)
                {
                    default: throw new NotSupportedException();
                    case GlyphPartKind.Curve3:
                        {
                            GlyphCurve3 curve3 = (GlyphCurve3)p;
                            newc.AddPart(latestPart = new GlyphCurve3(
                                //curve3.x0 * pixelScale, curve3.y0 * pixelScale,
                                latestPart,
                                curve3.x1 * pixelScale, curve3.y1 * pixelScale,
                                curve3.x2 * pixelScale, curve3.y2 * pixelScale));
                        }
                        break;
                    case GlyphPartKind.Curve4:
                        {
                            GlyphCurve4 curve4 = (GlyphCurve4)p;
                            newc.AddPart(latestPart = new GlyphCurve4(
                                  //curve4.x0 * pixelScale, curve4.y0 * pixelScale,
                                  latestPart,
                                  curve4.x1 * pixelScale, curve4.y1 * pixelScale,
                                  curve4.x2 * pixelScale, curve4.y2 * pixelScale,
                                  curve4.x3 * pixelScale, curve4.y3 * pixelScale
                                ));
                        }
                        break;
                    case GlyphPartKind.Line:
                        {
                            GlyphLine line = (GlyphLine)p;
                            newc.AddPart(latestPart = new GlyphLine(
                                //line.x0 * pixelScale, line.y0 * pixelScale,
                                latestPart,
                                line.x1 * pixelScale, line.y1 * pixelScale
                                ));
                        }
                        break;
                }
            }
            return newc;
        }
        //---------------------------------------------------------------------

        public static GlyphImage CreateMsdfImage(GlyphContourBuilder glyphToContour)
        {
            // create msdf shape , then convert to actual image
            return CreateMsdfImage(CreateMsdfShape(glyphToContour, 1));
        }
        public static GlyphImage CreateMsdfImage(Msdfgen.Shape shape)
        {
            double left, bottom, right, top;
            shape.findBounds(out left, out bottom, out right, out top);
            int w = (int)Math.Ceiling((right - left));
            int h = (int)Math.Ceiling((top - bottom));
            if (w < 5)
            {
                w = 5;
            }
            if (h < 5)
            {
                h = 5;
            }


            int borderW = (int)((float)w / 5f);
            var translate = new Msdfgen.Vector2(left < 0 ? -left + borderW : borderW, bottom < 0 ? -bottom + borderW : borderW);
            w += borderW * 2; //borders,left- right
            h += borderW * 2; //borders, top- bottom



            Msdfgen.FloatRGBBmp frgbBmp = new Msdfgen.FloatRGBBmp(w, h);
            Msdfgen.EdgeColoring.edgeColoringSimple(shape, 3);


            Msdfgen.MsdfGenerator.generateMSDF(frgbBmp,
                shape,
                4,
                new Msdfgen.Vector2(1, 1), //scale                 
                translate,//translate to positive quadrant
                -1);
            //-----------------------------------
            int[] buffer = Msdfgen.MsdfGenerator.ConvertToIntBmp(frgbBmp);

            GlyphImage img = new GlyphImage(w, h);
            img.TextureOffsetX = translate.x;
            img.TextureOffsetY = translate.y;
            img.SetImageBuffer(buffer, false);
            return img;
        }

    }
}