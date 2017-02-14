//MIT, 2016, Viktor Chlumsky, Multi-channel signed distance field generator, from https://github.com/Chlumsky/msdfgen
//MIT, 2017, WinterDev (C# port)

using System;
using System.Collections.Generic;

namespace Msdfgen
{
    public struct FloatRGB
    {
        public float r, g, b;
        public FloatRGB(float r, float g, float b)
        {
            this.r = r;
            this.g = g;
            this.b = b;
        }
#if DEBUG
        public override string ToString()
        {
            return r + "," + g + "," + b;
        }
#endif
    }
    public struct Pair<T, U>
    {
        public T first;
        public U second;
        public Pair(T first, U second)
        {
            this.first = first;
            this.second = second;
        }
    }
    public class FloatBmp
    {
        float[] buffer;
        public FloatBmp(int w, int h)
        {
            this.Width = w;
            this.Height = h;
            buffer = new float[w * h];
        }
        public int Width { get; set; }
        public int Height { get; set; }
        public void SetPixel(int x, int y, float value)
        {
            this.buffer[x + (y * Width)] = value;
        }
        public float GetPixel(int x, int y)
        {
            return this.buffer[x + (y * Width)];
        }
    }
    public class FloatRGBBmp
    {
        FloatRGB[] buffer;
        public FloatRGBBmp(int w, int h)
        {
            this.Width = w;
            this.Height = h;
            buffer = new FloatRGB[w * h];
        }
        public int Width { get; set; }
        public int Height { get; set; }
        public void SetPixel(int x, int y, FloatRGB value)
        {
            this.buffer[x + (y * Width)] = value;
        }
        public FloatRGB GetPixel(int x, int y)
        {
            return this.buffer[x + (y * Width)];
        }
    }
    public static class SdfGenerator
    {
        //siged distance field generator
        public static void GenerateSdf(FloatBmp output,
            Shape shape,
            double range,
            Vector2 scale,
            Vector2 translate)
        {

            int w = output.Width;
            int h = output.Height;
            for (int y = 0; y < h; ++y)
            {
                int row = shape.InverseYAxis ? h - y - 1 : y;
                for (int x = 0; x < w; ++x)
                {
                    double dummy = 0;
                    Vector2 p = (new Vector2(x + 0.5f, y + 0.5) * scale) - translate;
                    SignedDistance minDistance = SignedDistance.MIN;
                    //TODO: review here
                    List<Contour> contours = shape.contours;
                    int m = contours.Count;
                    for (int n = 0; n < m; ++n)
                    {
                        Contour contour = contours[n];
                        List<EdgeHolder> edges = contour.edges;
                        int nn = edges.Count;
                        for (int i = 0; i < nn; ++i)
                        {
                            EdgeHolder edge = edges[i];
                            SignedDistance distance = edge.edgeSegment.signedDistance(p, out dummy);
                            if (distance < minDistance)
                            {
                                minDistance = distance;
                            }
                        }
                    }
                    output.SetPixel(x, row, (float)(minDistance.distance / (range + 0.5f)));
                }
            }
        }

    }
    //    void generateSDF(Bitmap<float> &output, const Shape &shape, double range, const Vector2 &scale, const Vector2 &translate)
    //    {
    //        int w = output.width(), h = output.height();
    //# ifdef MSDFGEN_USE_OPENMP
    //#pragma omp parallel for
    //#endif
    //        for (int y = 0; y < h; ++y)
    //        {
    //            int row = shape.inverseYAxis ? h - y - 1 : y;
    //            for (int x = 0; x < w; ++x)
    //            {
    //                double dummy;
    //                Point2 p = Vector2(x + .5, y + .5) / scale - translate;
    //                SignedDistance minDistance;
    //                for (std::vector<Contour>::const_iterator contour = shape.contours.begin(); contour != shape.contours.end(); ++contour)
    //                    for (std::vector<EdgeHolder>::const_iterator edge = contour->edges.begin(); edge != contour->edges.end(); ++edge)
    //                    {
    //                        SignedDistance distance = (*edge)->signedDistance(p, dummy);
    //                        if (distance < minDistance)
    //                            minDistance = distance;
    //                    }
    //                output(x, row) = float(minDistance.distance / range + .5);
    //            }
    //        }
    //    }

    public static class MsdfGenerator
    {
        static float median(float a, float b, float c)
        {
            return Math.Max(Math.Min(a, b), Math.Min(Math.Max(a, b), c));
        }

        static bool pixelClash(FloatRGB a, FloatRGB b, double threshold)
        {
            // Only consider pair where both are on the inside or both are on the outside
            bool aIn = ((a.r > 0.5f) ? 1 : 0) + ((a.g > .5f) ? 1 : 0) + ((a.b > .5f) ? 1 : 0) >= 2;
            bool bIn = ((b.r > 0.5f) ? 1 : 0) + ((b.g > .5f) ? 1 : 0) + ((b.b > .5f) ? 1 : 0) >= 2;

            if (aIn != bIn) return false;
            // If the change is 0 <-> 1 or 2 <-> 3 channels and not 1 <-> 1 or 2 <-> 2, it is not a clash
            if ((a.r > .5f && a.g > .5f && a.b > .5f) || (a.r < .5f && a.g < .5f && a.b < .5f)
                || (b.r > .5f && b.g > .5f && b.b > .5f) || (b.r < .5f && b.g < .5f && b.b < .5f))
                return false;
            // Find which color is which: _a, _b = the changing channels, _c = the remaining one
            float aa, ab, ba, bb, ac, bc;
            if ((a.r > .5f) != (b.r > .5f) && (a.r < .5f) != (b.r < .5f))
            {
                aa = a.r; ba = b.r;
                if ((a.g > .5f) != (b.g > .5f) && (a.g < .5f) != (b.g < .5f))
                {
                    ab = a.g; bb = b.g;
                    ac = a.b; bc = b.b;
                }
                else if ((a.b > .5f) != (b.b > .5f) && (a.b < .5f) != (b.b < .5f))
                {
                    ab = a.b; bb = b.b;
                    ac = a.g; bc = b.g;
                }
                else
                    return false; // this should never happen
            }
            else if ((a.g > .5f) != (b.g > .5f) && (a.g < .5f) != (b.g < .5f)
              && (a.b > .5f) != (b.b > .5f) && (a.b < .5f) != (b.b < .5f))
            {
                aa = a.g; ba = b.g;
                ab = a.b; bb = b.b;
                ac = a.r; bc = b.r;
            }
            else
                return false;
            // Find if the channels are in fact discontinuous
            return (Math.Abs(aa - ba) >= threshold)
                && (Math.Abs(ab - bb) >= threshold)
                && Math.Abs(ac - .5f) >= Math.Abs(bc - .5f); // Out of the pair, only flag the pixel farther from a shape edge
        }
        static void msdfErrorCorrection(FloatRGBBmp output, Vector2 threshold)
        {
            List<Pair<int, int>> clashes = new List<Msdfgen.Pair<int, int>>();
            int w = output.Width, h = output.Height;
            for (int y = 0; y < h; ++y)
            {
                for (int x = 0; x < w; ++x)
                {
                    if ((x > 0 && pixelClash(output.GetPixel(x, y), output.GetPixel(x - 1, y), threshold.x))
                        || (x < w - 1 && pixelClash(output.GetPixel(x, y), output.GetPixel(x + 1, y), threshold.x))
                        || (y > 0 && pixelClash(output.GetPixel(x, y), output.GetPixel(x, y - 1), threshold.y))
                        || (y < h - 1 && pixelClash(output.GetPixel(x, y), output.GetPixel(x, y + 1), threshold.y)))
                    {
                        clashes.Add(new Pair<int, int>(x, y));
                    }
                }
            }
            int clash_count = clashes.Count;
            for (int i = 0; i < clash_count; ++i)
            {
                Pair<int, int> clash = clashes[i];
                FloatRGB pixel = output.GetPixel(clash.first, clash.second);
                float med = median(pixel.r, pixel.g, pixel.b);
                pixel.r = med; pixel.g = med; pixel.b = med;
            }
            //for (std::vector<std::pair<int, int>>::const_iterator clash = clashes.begin(); clash != clashes.end(); ++clash)
            //{
            //    FloatRGB & pixel = output(clash->first, clash->second);
            //    float med = median(pixel.r, pixel.g, pixel.b);
            //    pixel.r = med, pixel.g = med, pixel.b = med;
            //}
        }
        //multi-channel signed distance field generator
        struct SignedDistanceAndEdge
        {
            public SignedDistance minDistance;
            public EdgeHolder nearEdge;
            public double nearParam;
        }

        public static void generateMSDF(FloatRGBBmp output, Shape shape, double range, Vector2 scale, Vector2 translate,
            double edgeThreshold)
        {
            int w = output.Width;
            int h = output.Height;
            //#ifdef MSDFGEN_USE_OPENMP
            //    #pragma omp parallel for
            //#endif
            for (int y = 0; y < h; ++y)
            {
                int row = shape.InverseYAxis ? h - y - 1 : y;
                for (int x = 0; x < w; ++x)
                {
                    Vector2 p = (new Vector2(x + .5, y + .5) / scale) - translate;
                    SignedDistanceAndEdge r = new SignedDistanceAndEdge { minDistance = SignedDistance.MIN },
                        g = new SignedDistanceAndEdge { minDistance = SignedDistance.MIN },
                        b = new SignedDistanceAndEdge { minDistance = SignedDistance.MIN };
                    //r.nearEdge = g.nearEdge = b.nearEdge = null;
                    //r.nearParam = g.nearParam = b.nearParam = 0;
                    List<Contour> contours = shape.contours;
                    int m = contours.Count;
                    for (int n = 0; n < m; ++n)
                    {
                        Contour contour = contours[n];
                        List<EdgeHolder> edges = contour.edges;
                        int j = edges.Count;
                        for (int i = 0; i < j; ++i)
                        {
                            EdgeHolder edge = edges[i];
                            double param;

                            SignedDistance distance = edge.edgeSegment.signedDistance(p, out param);
                             
                            if (edge.HasComponent(EdgeColor.RED) && distance < r.minDistance)
                            {
                                r.minDistance = distance;
                                r.nearEdge = edge;
                                r.nearParam = param;
                            }
                            if (edge.HasComponent(EdgeColor.GREEN) && distance < g.minDistance)
                            {
                                g.minDistance = distance;
                                g.nearEdge = edge;
                                g.nearParam = param;
                            }
                            if (edge.HasComponent(EdgeColor.BLUE) && distance < b.minDistance)
                            {
                                b.minDistance = distance;
                                b.nearEdge = edge;
                                b.nearParam = param;
                            }
                        }
                    }

                    if (r.nearEdge != null)
                    {
                        r.nearEdge.edgeSegment.distanceToPsedoDistance(ref r.minDistance, p, r.nearParam);
                    }
                    if (g.nearEdge != null)
                    {
                        g.nearEdge.edgeSegment.distanceToPsedoDistance(ref g.minDistance, p, g.nearParam);
                    }
                    if (b.nearEdge != null)
                    {
                        b.nearEdge.edgeSegment.distanceToPsedoDistance(ref b.minDistance, p, b.nearParam);
                    }

                    output.SetPixel(x, row,
                        new FloatRGB(
                            (float)(r.minDistance.distance / range + .5),
                            (float)(g.minDistance.distance / range + .5),
                            (float)(b.minDistance.distance / range + .5)
                        ));

                }
            }

            if (edgeThreshold > 0)
            {
                msdfErrorCorrection(output, edgeThreshold / (scale * range));
            }
        }
    }
}