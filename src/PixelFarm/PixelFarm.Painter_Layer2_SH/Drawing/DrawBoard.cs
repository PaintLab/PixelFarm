﻿//MIT, 2014-present, WinterDev


namespace PixelFarm.Drawing
{
    public sealed class UpdateArea
    {
        int _left, _top, _width, _height;
        public UpdateArea()
        {

        }
        public Rectangle CurrentRect
        {
            get => new Rectangle(_left, _top, _width, _height);
            set
            {
                _left = value.Left;
                _top = value.Top;
                _width = value.Width;
                _height = value.Height;
            }
        }
        public void Reset()
        {
            _left = _top = _width = _height = 0;
            //not need to reset _prev* BUT use it with care
        }

        /// <summary>
        /// create a copy of intersect rectangle
        /// </summary>
        /// <returns></returns>
        public Rectangle Intersects(int left, int top, int width, int height)
        {
            return Rectangle.FromLTRB(
                System.Math.Max(_left, left),
                System.Math.Max(_top, top),
                System.Math.Min(_left + _width, left + width),
                System.Math.Min(_top + _height, top + height));
        }
        /// <summary>
        /// create a copy of intersect rectangle
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public Rectangle LocalIntersects(int width, int height)
        {
            //when left=0 and top =0
            return Rectangle.FromLTRB(
                System.Math.Max(_left, 0),
                System.Math.Max(_top, 0),
                System.Math.Min(_left + _width, width),
                System.Math.Min(_top + _height, height));
        }
        int _prev_left, _prev_top, _prev_width, _prev_height;

        public Rectangle PreviousRect => new Rectangle(_prev_left, _prev_top, _prev_width, _prev_height);

        public void MakeBackup()
        {
            _prev_left = _left;
            _prev_top = _top;
            _prev_width = _width;
            _prev_height = _height;
        }

        public int Left => _left;
        public int Top => _top;
        public int Width => _width;

        public int Height { get => _height; set => _height = value; }

        public int Right => _left + _width;
        public int Bottom => _top + _height;

        public void Offset(int dx, int dy)
        {
            _left += dx;
            _top += dy;
        }
        public void OffsetX(int dx)
        {
            _left += dx;
        }
        public void OffsetY(int dy)
        {
            _top += dy;
        }


#if DEBUG
        public override string ToString()
        {
            return $"({_left},{_top},{_width},{_height})";
        }
#endif
    }

    public abstract class DrawBoard : System.IDisposable
    {

        //------------------------------
        //this class provides canvas interface for drawing
        //with 'screen' coordinate system
        //y axis points down
        //x axis points to the right
        //(0,0) is on left-upper corner
        //-------------------------------
        //who implement this class
        //1. PixelFarm.Drawing.WinGdi.GdiPlusDrawBoard (for win32,legacy) 
        //2. PixelFarm.Drawing.GLES2.MyGLDrawBoard  (for GLES2)

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
        public abstract bool PushClipAreaRect(int width, int height, UpdateArea updateArea);
        public abstract bool PushClipAreaRect(int left, int top, int width, int height, UpdateArea updateArea);

        public abstract void PopClipAreaRect();
        public abstract void SetClipRect(Rectangle clip, CombineMode combineMode = CombineMode.Replace);
        public abstract Rectangle CurrentClipRect { get; }
        //------------------------------------------------------
        //buffer
        public abstract void Clear(Color c); //TODO: add SetClearColor(), Clear(), 
        public abstract void RenderTo(System.IntPtr destHdc, int sourceX, int sourceY, Rectangle destArea); //TODO: review here
        public virtual void RenderTo(Image destImg, int srcX, int srcYy, int srcW, int srcH) { }
        //------------------------------------------------------- 

        //lines         
        public abstract void DrawLine(float x1, float y1, float x2, float y2);
        //-------------------------------------------------------
        //rects
        public abstract void FillRectangle(Color color, float left, float top, float width, float height);
        public abstract void FillRectangle(Brush brush, float left, float top, float width, float height);
        public abstract void DrawRectangle(Color color, float left, float top, float width, float height);
        //------------------------------------------------------- 

        public abstract void FillPolygon(Brush brush, PointF[] points);
        public abstract void FillPolygon(Color color, PointF[] points);
        //-------------------------------------------------------  
        //images
        public abstract void DrawImage(Image image, RectangleF dest, RectangleF src);
        public abstract void DrawImage(Image image, RectangleF dest);
        public abstract void DrawImages(Image image, RectangleF[] destAndSrcPairs);
        public abstract void DrawImage(Image image, int x, int y);//draw image unscaled at specific pos
        //---------------------------------------------------------------------------
        //text ,font, strings 
        //TODO: review these funcs
        public abstract RequestFont CurrentFont { get; set; }
        public abstract Color CurrentTextColor { get; set; }

        public abstract TextDrawingTech TextDrawingTech { get; set; }
        //TODO: review here again
        public abstract Color TextBackgroundColorHint { get; set; }//explicit set current text background color hint

        public abstract void DrawText(char[] buffer, int x, int y);
        public abstract void DrawText(char[] buffer, Rectangle logicalTextBox, int textAlignment);
        public abstract void DrawText(char[] buffer, int startAt, int len, Rectangle logicalTextBox, int textAlignment);

        /// <summary>
        /// create formatted string base on current font,font-size, font style
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public abstract RenderVxFormattedString CreateFormattedString(char[] buffer, int startAt, int len, bool delay);
        public abstract RenderVxFormattedString CreateFormattedString(int[] buffer, int startAt, int len, bool delay);

        public abstract void DrawRenderVx(RenderVx renderVx, float x, float y);
        public abstract void Dispose();
        //--
        public abstract Painter GetPainter();
        /// <summary>
        /// get software rendering surface drawboard
        /// </summary>
        /// <returns></returns>
        public abstract DrawBoard GetCpuBlitDrawBoard();
        //
        public abstract DrawboardBuffer CreateBackbuffer(int w, int h);
        public abstract void EnterNewDrawboardBuffer(DrawboardBuffer backbuffer);
        public abstract void ExitCurrentDrawboardBuffer();
        //
        public abstract bool IsGpuDrawBoard { get; }
        public abstract void BlitFrom(DrawBoard src, float srcX, float srcY, float srcW, float srcH, float dstX, float dstY);
        public abstract ImageBinder GetInternalBitmapProvider();
    }

    public enum TextDrawingTech : byte
    {
        Stencil,//default
        LcdSubPix,
        Copy,
    }
    public abstract class DrawboardBuffer : System.IDisposable
    {
        public abstract Image GetImage();
        public bool IsValid { get; set; }
        public abstract int Width { get; }
        public abstract int Height { get; }
        public abstract void Dispose();
        public abstract Image CopyToNewMemBitmap();
    }





    public enum RenderQuality
    {
        HighQuality,
        Fast,
    }

    /// <summary>
    /// image filter
    /// </summary>
    public interface IImageFilter
    {
        //implementation for cpu-based and gpu-based may be different
        void Apply();
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
    public enum RenderSurfaceOriginKind
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

    public static class DrawBoardExtensionMethods
    {

        public static SmoothingModeState SaveSmoothMode(this DrawBoard drawBoard)
        {
            //TODO: review offset function
            return new SmoothingModeState(drawBoard, drawBoard.SmoothingMode);
        }
        public static SmoothingModeState SetSmoothMode(this DrawBoard drawBoard, SmoothingMode value)
        {
            //TODO: review offset function
            var saveState = new SmoothingModeState(drawBoard, drawBoard.SmoothingMode);
            drawBoard.SmoothingMode = value;
            return saveState;
        }
        public static StrokeState SaveStroke(this DrawBoard drawBoard)
        {
            return new StrokeState(drawBoard);
        }
        public struct SmoothingModeState : System.IDisposable
        {
            readonly DrawBoard _drawBoard;
            readonly SmoothingMode _latestSmoothMode;
            internal SmoothingModeState(DrawBoard drawBoard, SmoothingMode state)
            {
                _latestSmoothMode = state;
                _drawBoard = drawBoard; 

            }
            public void Dispose()
            {
                _drawBoard.SmoothingMode = _latestSmoothMode;
            }
        }
        
        public struct StrokeState : System.IDisposable
        {
            readonly DrawBoard _d;
            readonly Color _stokeColor;
            readonly float _strokeW;
            public StrokeState(DrawBoard d)
            {
                _d = d;
                _stokeColor = d.StrokeColor;
                _strokeW = d.StrokeWidth;
            }
            public void Dispose()
            {
                _d.StrokeColor = _stokeColor;
                _d.StrokeWidth = _strokeW;
            }
        }


    }




}



