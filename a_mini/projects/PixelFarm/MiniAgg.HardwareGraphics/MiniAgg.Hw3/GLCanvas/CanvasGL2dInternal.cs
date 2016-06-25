//MIT 2014-2016, WinterDev

namespace PixelFarm.DrawingGL
{
    public partial class CanvasGL2d
    {
        static float[] CreatePolyLineRectCoords(
                float x, float y, float w, float h)
        {
            return new float[]
            {
                x,y,
                x+w,y,
                x+w,y+h,
                x,x+h
            };
        }

        unsafe void DrawPolygonUnsafe(float* polygon2dVertices, int npoints)
        {
            this.basicFillShader.DrawLineLoopWithVertexBuffer(polygon2dVertices, npoints, this.strokeColor);
        }
    }
}