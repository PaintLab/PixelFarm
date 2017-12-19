//MIT, 2014-2017, WinterDev


namespace PixelFarm.Drawing
{

    public abstract class DrawBoard
    {

        //------------------------------
        //this class provides basic DrawBoard (canvas) interface for drawing
        //with 'screen' coordinate system
        //y axis points down
        //x axis points to the right
        //(0,0) is on left-upper corner
        //-------------------------------
        //who implement this class
        //1. PixelFarm.Drawing.WinGdi.MyGdiPlusCanvas (for win32,legacy)
        //2. Agg's 
        //3. PixelFarm.Drawing.GLES2.MyGLCanvas  (for GLES2)
        //4. PixelFarm.Drawing.Pdf.MyPdfCanvas (future)
        //5. PixelFarm.Drawing.Skia.MySkia Canvas (not complete)
        //------------------------------
        //who use this interface
        //the HtmlRenderer
        //------------------------------

#if DEBUG
        public static int dbug_canvasCount = 0;
        public int debug_resetCount = 0;
        public int debug_releaseCount = 0;
        public int debug_canvas_id = 0;
        public abstract void dbug_DrawRuler(int x);
        public abstract void dbug_DrawCrossRect(Color color, Rectangle rect);
#endif

        public abstract void CloseCanvas();

        ////////////////////////////////////////////////////////////////////////////
        //drawing properties
        public abstract SmoothingMode SmoothingMode { get; set; }
        public abstract float StrokeWidth { get; set; }
        public abstract Color StrokeColor { get; set; }
        public abstract Brush CurrentBrush { get; set; }

        ////////////////////////////////////////////////////////////////////////////
        //states
        public abstract void ResetInvalidateArea();
        public abstract void Invalidate(Rectangle rect);
        public abstract Rectangle InvalidateArea { get; }


        ////////////////////////////////////////////////////////////////////////////
        // canvas dimension, canvas origin
        public abstract int Top { get; }
        public abstract int Left { get; }
        public abstract int Width { get; }
        public abstract int Height { get; }
        public abstract int Bottom { get; }
        public abstract int Right { get; }

        public abstract Rectangle Rect { get; }

        public abstract int OriginX { get; }
        public abstract int OriginY { get; }
        public abstract void SetCanvasOrigin(int x, int y);

        //---------------------------------------------------------------------
        //clip area
        public abstract bool PushClipAreaRect(int width, int height, ref Rectangle updateArea);
        public abstract void PopClipAreaRect();
        public abstract void SetClipRect(Rectangle clip, CombineMode combineMode = CombineMode.Replace);
        public abstract Rectangle CurrentClipRect { get; }
        //------------------------------------------------------
        //buffer
        public abstract void Clear(Color c);
        public abstract void RenderTo(System.IntPtr destHdc, int sourceX, int sourceY, Rectangle destArea);
        //------------------------------------------------------- 


        public abstract void DrawLine(float x1, float y1, float x2, float y2);
        //-------------------------------------------------------
        //rects 
        public abstract void FillRectangle(float left, float top, float width, float height);
        public abstract void DrawRectangle(float left, float top, float width, float height);
        //------------------------------------------------------- 
        //path,  polygons,ellipse spline,contour,   
        public abstract void FillPath(GraphicsPath gfxPath);
        public abstract void DrawPath(GraphicsPath gfxPath);
        public abstract void FillPolygon(PointF[] points);
        //-------------------------------------------------------  
        //images
        public abstract void DrawImage(Image image, RectangleF dest, RectangleF src);
        public abstract void DrawImage(Image image, RectangleF dest);
        public abstract void DrawImages(Image image, RectangleF[] destAndSrcPairs);
        //---------------------------------------------------------------------------
        //text ,font, strings 
        //TODO: review these funcs
        public abstract RequestFont CurrentFont { get; set; }
        public abstract Color CurrentTextColor { get; set; }
        public abstract void DrawText(char[] buffer, int x, int y);
        public abstract void DrawText(char[] buffer, Rectangle logicalTextBox, int textAlignment);
        public abstract void DrawText(char[] buffer, int startAt, int len, Rectangle logicalTextBox, int textAlignment);
        //-------------------------------------------------------
    }

    public enum SmoothingMode
    {
        AntiAlias = 4,
        Default = 0,
        HighQuality = 2,
        HighSpeed = 1,
        Invalid = -1,
        None = 3
    }
    public enum CanvasOrientation
    {
        LeftTop,
        LeftBottom,
    }
    public enum CanvasBackEnd
    {
        Software,
        Hardware,
        HardwareWithSoftwareFallback
    }
    public delegate void CanvasInvalidateDelegate(Rectangle paintArea);

    public static class CanvasExtensionMethods
    {

        public static void OffsetCanvasOrigin(this DrawBoard canvas, int dx, int dy)
        {
            //TODO: review offset function
            canvas.SetCanvasOrigin(canvas.OriginX + dx, canvas.OriginY + dy);
        }
        public static void OffsetCanvasOriginX(this DrawBoard canvas, int dx)
        {
            //TODO: review offset function
            canvas.OffsetCanvasOrigin(dx, 0);
        }
        public static void OffsetCanvasOriginY(this DrawBoard canvas, int dy)
        {
            //TODO: review offset function
            canvas.OffsetCanvasOrigin(0, dy);
        }

        /// <summary>
        /// fill rectangle with specific brush
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="brush"></param>
        /// <param name="points"></param>
        public static void FillRectangle(this DrawBoard canvas, Brush brush, PointF[] points)
        {
            Brush temp = canvas.CurrentBrush; //save
            PointF p0 = points[0];
            PointF p1 = points[1];
            canvas.FillRectangle(p0.X, p0.Y, p1.X, p1.Y);
            canvas.CurrentBrush = temp; //restore
        }
        public static void FillRectangle(this DrawBoard canvas, Brush brush, float left, float top, float width, float height)
        {
            Brush temp = canvas.CurrentBrush; //save 
            canvas.FillRectangle(left, top, width, height);
            canvas.CurrentBrush = temp; //restore
        }
        public static void FillRectangle(this DrawBoard canvas, Color color, float left, float top, float width, float height)
        {
            Brush temp = canvas.CurrentBrush; //save 
            canvas.FillRectangle(left, top, width, height);
            canvas.CurrentBrush = temp; //restore
        }

        public static void DrawRectangle(this DrawBoard canvas, Color color, float left, float top, float width, float height)
        {
            Brush temp = canvas.CurrentBrush; //save 
            canvas.DrawRectangle(left, top, width, height);
            canvas.CurrentBrush = temp; //restore
        }

        public static void FillPolygon(this DrawBoard canvas, Brush brush, PointF[] points)
        {
            Brush temp = canvas.CurrentBrush; //save 
            canvas.FillPolygon(points);
            canvas.CurrentBrush = temp; //restore
        }
        public static void FillPath(this DrawBoard canvas, Color color, GraphicsPath gfxPath)
        {

        }
        public static void FillPath(this DrawBoard canvas, Brush brush, GraphicsPath gfxPath)
        {

        }


    }
}
