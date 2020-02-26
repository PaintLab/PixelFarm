//Apache2, 2014-present, WinterDev

using LayoutFarm.RenderBoxes;
using PixelFarm.Drawing;
namespace LayoutFarm.CustomWidgets
{
    public class CustomRenderBox : RenderBoxBase
    {
        Color _backColor;
        Color _borderColor;
        bool _hasSomeBorderW;

        //these are NOT CSS borders/margins/paddings***
        //we use pixel unit for our RenderBox
        //with limitation of int8 number

        byte _contentLeft;
        byte _contentTop;
        byte _contentRight;
        byte _contentBottom;

        byte _borderLeft;
        byte _borderTop;
        byte _borderRight;
        byte _borderBottom;
        BoxContentLayoutKind _layoutHint;

        public CustomRenderBox(RootGraphic rootgfx, int width, int height)
            : base(rootgfx, width, height)
        {
            this.BackColor = Color.LightGray;
        }
        protected override PlainLayer CreateDefaultLayer()
        {
#if DEBUG
            if (dbugBreak)
            {

            }
#endif
            PlainLayer layer = new PlainLayer(this);
            layer.LayoutHint = _layoutHint;
            return layer;
        }
        public BoxContentLayoutKind LayoutHint
        {
            get => _layoutHint;
            set
            {
                _layoutHint = value;
                if (_defaultLayer != null)
                {
                    _defaultLayer.LayoutHint = value;
                }
            }
        }
        public int PaddingLeft
        {
            get => _contentLeft - _borderLeft;
            set => _contentLeft = (byte)(value + _borderLeft);
        }

        public int PaddingTop
        {
            get => _contentTop - _borderTop;
            set => _contentTop = (byte)(value + _borderTop);
        }
        public int PaddingRight
        {
            get => _contentRight - _borderRight;
            set => _contentRight = (byte)(value + _borderRight);

        }
        public int PaddingBottom
        {
            get => _contentBottom - _borderBottom;
            set => _contentBottom = (byte)(value + _borderBottom);
        }
        public void SetPadding(byte left, byte top, byte right, byte bottom)
        {
            _contentLeft = (byte)(left + _borderLeft);
            _contentTop = (byte)(top + _borderTop);
            _contentRight = (byte)(right + _borderRight);
            _contentBottom = (byte)(bottom + _borderBottom);
        }
        public void SetPadding(byte sameValue)
        {
            _contentLeft = (byte)(sameValue + _borderLeft);
            _contentTop = (byte)(sameValue + _borderTop);
            _contentRight = (byte)(sameValue + _borderRight);
            _contentBottom = (byte)(sameValue + _borderBottom);
        }
        //------------------ 
        public int BorderTop
        {
            get => _borderTop;
            set
            {
                _borderTop = (byte)value;
                if (!_hasSomeBorderW) _hasSomeBorderW = value > 0;
            }
        }
        public int BorderBottom
        {
            get => _borderBottom;
            set
            {
                _borderBottom = (byte)value;
                if (!_hasSomeBorderW) _hasSomeBorderW = value > 0;
            }
        }
        public int BorderRight
        {
            get => _borderRight;
            set
            {
                _borderRight = (byte)value;
                if (!_hasSomeBorderW) _hasSomeBorderW = value > 0;
            }
        }
        public int BorderLeft
        {
            get => _borderLeft;
            set
            {
                _borderLeft = (byte)value;
                if (!_hasSomeBorderW) _hasSomeBorderW = value > 0;
            }

        }
        public void SetBorders(byte left, byte top, byte right, byte bottom)
        {
            _borderLeft = left;
            _borderTop = top;
            _borderRight = right;
            _borderBottom = bottom;

            _hasSomeBorderW = ((left | top | right | bottom) > 0);

        }
        public void SetBorders(byte sameValue)
        {
            _borderLeft =
                _borderTop =
                _borderRight =
                _borderBottom = sameValue;

            _hasSomeBorderW = sameValue > 0;
        }
        //-------------

        public int ContentWidth => Width - (_contentLeft + _contentRight);
        public int ContentHeight => Height - (_contentTop + _contentBottom);

        public int ContentLeft
        {
            get => _contentLeft;
            set => _contentLeft = (byte)value;
        }
        public int ContentTop
        {
            get => _contentTop;
            set => _contentTop = (byte)value;
        }
        public int ContentRight
        {
            get => _contentRight;
            set => _contentRight = (byte)value;
        }
        public int ContentBottom
        {
            get => _contentBottom;
            set => _contentBottom = (byte)value;
        }
        public void SetContentOffsets(byte contentLeft, byte contentTop, byte contentRight, byte contentBottom)
        {
            _contentLeft = contentLeft;
            _contentTop = contentTop;
            _contentRight = contentRight;
            _contentBottom = contentBottom;
        }
        public void SetContentOffsets(byte allside)
        {
            _contentLeft = allside;
            _contentTop = allside;
            _contentRight = allside;
            _contentBottom = allside;
        }

        public Color BackColor
        {
            get => _backColor;
            set
            {
                if (_backColor == value) return;

                _backColor = value;
                if (this.HasParentLink)
                {
                    this.InvalidateGraphics();
                }
            }
        }
#if DEBUG
        bool _dbugBorderBreak;
#endif
        public Color BorderColor
        {
            get => _borderColor;
            set
            {
                if (_borderColor == value) return;
                _borderColor = value;
#if DEBUG
                if (value.A > 0)
                {
                    _dbugBorderBreak = true;
                }
#endif

                if (this.HasParentLink)
                {
                    this.InvalidateGraphics();
                }
            }
        }
        protected override void RenderClientContent(DrawBoard d, Rectangle updateArea)
        {
#if DEBUG
            if (this.dbugBreak)
            {
            }
#endif

            if (GlobalRootGraphic.WaitForFirstRenderElement && this == GlobalRootGraphic.StartWithRenderElement)
            {
                GlobalRootGraphic.WaitForFirstRenderElement = false;
            }

            if (!GlobalRootGraphic.WaitForFirstRenderElement)
            {
                if (this.MayHasViewport)
                {
                    //TODO: review here
                    //start pos of background fill
                    //(0,0) 
                    //(viewportX,viewportY)
                    //tile or limit
                    d.FillRectangle(BackColor, ViewportLeft, ViewportTop, this.Width, this.Height);
                }
                else
                {
                    d.FillRectangle(BackColor, 0, 0, this.Width, this.Height);
                }

                //border is over background color
#if DEBUG
                if (_dbugBorderBreak)
                {
                }
#endif           
            }


            //default content layer
            this.DrawDefaultLayer(d, ref updateArea);

            if (!GlobalRootGraphic.WaitForFirstRenderElement)
            {
                if (_hasSomeBorderW && _borderColor.A > 0)
                {
                    d.DrawRectangle(_borderColor, 0, 0, this.Width, this.Height);//test
                }
            }
#if DEBUG
            //if (this.dbugBreak)
            //    canvas.FillRectangle(Color.Red, ViewportLeft, ViewportTop, this.Width, this.Height);
            //canvas.dbug_DrawCrossRect(PixelFarm.Drawing.Color.Black,
            //    new Rectangle(0, 0, this.Width, this.Height));

            //canvas.dbug_DrawCrossRect(PixelFarm.Drawing.Color.Black,
            //   new Rectangle(updateArea.Left, updateArea.Top, updateArea.Width, updateArea.Height));
#endif
        }
    }

    public class DoubleBufferCustomRenderBox : CustomRenderBox
    {
        DrawboardBuffer _builtInBackBuffer;
        bool _hasAccumRect;
        Rectangle _invalidateRect;
        bool _enableDoubleBuffer;
        public DoubleBufferCustomRenderBox(RootGraphic rootgfx, int width, int height)
          : base(rootgfx, width, height)
        {
            NeedInvalidateRectEvent = true;
        }
        public bool EnableDoubleBuffer
        {
            get => _enableDoubleBuffer;
            set
            {
                _enableDoubleBuffer = value;
            }
        }


        protected override void OnInvalidateGraphicsNoti(bool fromMe, ref Rectangle totalBounds)
        {
            if (_builtInBackBuffer != null)
            {
                //TODO: review here,
                //in this case, we copy to another rect
                //since we don't want the offset to effect the total bounds 
#if DEBUG
                if (totalBounds.Width == 150)
                {

                }
                System.Diagnostics.Debug.WriteLine("noti, fromMe=" + fromMe + ",bounds" + totalBounds);
#endif
                if (!fromMe)
                {
                    totalBounds.Offset(-this.X, -this.Y);
                }

                _builtInBackBuffer.IsValid = false;

                if (!_hasAccumRect)
                {
                    _invalidateRect = totalBounds;
                    _hasAccumRect = true;
                }
                else
                {
                    _invalidateRect = Rectangle.Union(_invalidateRect, totalBounds);
                }
            }
            else
            {
                totalBounds.Offset(this.X, this.Y);
            }
            //base.OnInvalidateGraphicsNoti(totalBounds);//skip
        }
        protected override void RenderClientContent(DrawBoard d, Rectangle updateArea)
        {
            if (_enableDoubleBuffer)
            {
                MicroPainter painter = new MicroPainter(d);
                if (_builtInBackBuffer == null)
                {
                    _builtInBackBuffer = painter.CreateOffscreenDrawBoard(this.Width, this.Height);
                }

                if (!_builtInBackBuffer.IsValid)
                {
#if DEBUG
                    System.Diagnostics.Debug.WriteLine("double_buffer_update:" + this.dbug_obj_id + "," + _invalidateRect.ToString());
#endif
                    float backupViewportW = painter.ViewportWidth; //backup
                    float backupViewportH = painter.ViewportHeight; //backup
                    painter.AttachTo(_builtInBackBuffer); //*** switch to builtInBackbuffer 
                    painter.SetViewportSize(this.Width, this.Height);
                    if (!_hasAccumRect)
                    {
                        _invalidateRect = new Rectangle(0, 0, Width, Height);
                    }

#if DEBUG
                    if (_invalidateRect.Width == 15)
                    {

                    }
#endif

                    //                    if (painter.PushLocalClipArea(
                    //                        _invalidateRect.Left, _invalidateRect.Top,
                    //                        _invalidateRect.Width, _invalidateRect.Height))
                    //                    {
                    //#if DEBUG
                    //                        //for debug , test clear with random color
                    //                        //another useful technique to see latest clear area frame-by-frame => use random color
                    //                        //painter.Clear(Color.FromArgb(255, dbugRandom.Next(0, 255), dbugRandom.Next(0, 255), dbugRandom.Next(0, 255)));

                    //                        canvas.Clear(Color.White);
                    //#else
                    //                        canvas.Clear(Color.White);
                    //#endif

                    //                        base.RenderBoxContent(canvas, updateArea);
                    //                    }

                    //if (painter.PushLocalClipArea(
                    //    _invalidateRect.Left, _invalidateRect.Top,
                    //    _invalidateRect.Width, _invalidateRect.Height))
                    //{
#if DEBUG
                    //for debug , test clear with random color
                    //another useful technique to see latest clear area frame-by-frame => use random color
                    //painter.Clear(Color.FromArgb(255, dbugRandom.Next(0, 255), dbugRandom.Next(0, 255), dbugRandom.Next(0, 255)));

                    d.Clear(Color.White);
#else
                    canvas.Clear(Color.White);
#endif


                    Rectangle updateArea2 = new Rectangle(0, 0, _builtInBackBuffer.Width, _builtInBackBuffer.Height);
                    base.RenderClientContent(d, updateArea2);

                    //}
                    //painter.PopLocalClipArea();
                    //
                    _builtInBackBuffer.IsValid = true;
                    _hasAccumRect = false;

                    painter.AttachToNormalBuffer();//*** switch back
                    painter.SetViewportSize(backupViewportW, backupViewportH);//restore viewport size
                }

#if DEBUG
                else
                {
                    System.Diagnostics.Debug.WriteLine("double_buffer_update:" + dbug_obj_id + " use cache");
                }
#endif
                painter.DrawImage(_builtInBackBuffer.GetImage(), 0, 0, this.Width, this.Height);
            }
            else
            {
                base.RenderClientContent(d, updateArea);
            }
        }

        //        protected override void RenderBoxContent(DrawBoard canvas, Rectangle updateArea)
        //        {
        //            if (_enableDoubleBuffer)
        //            {
        //                MicroPainter painter = new MicroPainter(canvas);
        //                if (_builtInBackBuffer == null)
        //                {
        //                    _builtInBackBuffer = painter.CreateOffscreenDrawBoard(this.Width, this.Height);
        //                }

        //                //                if (!_builtInBackBuffer.IsValid)
        //                //                {
        //                //#if DEBUG
        //                //                    System.Diagnostics.Debug.WriteLine("double_buffer_update:" + this.dbug_obj_id + "," + _invalidateRect.ToString());
        //                //#endif
        //                //                    float backupViewportW = painter.ViewportWidth; //backup
        //                //                    float backupViewportH = painter.ViewportHeight; //backup
        //                //                    painter.AttachTo(_builtInBackBuffer); //*** switch to builtInBackbuffer 
        //                //                    painter.SetViewportSize(this.Width, this.Height);
        //                //                    if (!_hasAccumRect)
        //                //                    {
        //                //                        _invalidateRect = new Rectangle(0, 0, Width, Height);
        //                //                    }

        //                //#if DEBUG
        //                //                    if (_invalidateRect.Width == 15)
        //                //                    {

        //                //                    }
        //                //#endif

        //                //                    //                    if (painter.PushLocalClipArea(
        //                //                    //                        _invalidateRect.Left, _invalidateRect.Top,
        //                //                    //                        _invalidateRect.Width, _invalidateRect.Height))
        //                //                    //                    {
        //                //                    //#if DEBUG
        //                //                    //                        //for debug , test clear with random color
        //                //                    //                        //another useful technique to see latest clear area frame-by-frame => use random color
        //                //                    //                        //painter.Clear(Color.FromArgb(255, dbugRandom.Next(0, 255), dbugRandom.Next(0, 255), dbugRandom.Next(0, 255)));

        //                //                    //                        canvas.Clear(Color.White);
        //                //                    //#else
        //                //                    //                        canvas.Clear(Color.White);
        //                //                    //#endif

        //                //                    //                        base.RenderBoxContent(canvas, updateArea);
        //                //                    //                    }

        //                //                    //if (painter.PushLocalClipArea(
        //                //                    //    _invalidateRect.Left, _invalidateRect.Top,
        //                //                    //    _invalidateRect.Width, _invalidateRect.Height))
        //                //                    //{
        //                //#if DEBUG
        //                //                    //for debug , test clear with random color
        //                //                    //another useful technique to see latest clear area frame-by-frame => use random color
        //                //                    //painter.Clear(Color.FromArgb(255, dbugRandom.Next(0, 255), dbugRandom.Next(0, 255), dbugRandom.Next(0, 255)));

        //                //                    canvas.Clear(Color.White);
        //                //#else
        //                //                        canvas.Clear(Color.White);
        //                //#endif


        //                //                    Rectangle updateArea2 = new Rectangle(0, 0, _builtInBackBuffer.Width, _builtInBackBuffer.Height);
        //                //                    base.RenderBoxContent(canvas, updateArea2);

        //                //                    //}
        //                //                    //painter.PopLocalClipArea();
        //                //                    //
        //                //                    _builtInBackBuffer.IsValid = true;
        //                //                    _hasAccumRect = false;

        //                //                    painter.AttachToNormalBuffer();//*** switch back
        //                //                    painter.SetViewportSize(backupViewportW, backupViewportH);//restore viewport size
        //                //                }
        //                if (!_builtInBackBuffer.IsValid)
        //                {
        //#if DEBUG
        //                    System.Diagnostics.Debug.WriteLine("double_buffer_update:" + this.dbug_obj_id + "," + _invalidateRect.ToString());
        //#endif
        //                    float backupViewportW = painter.ViewportWidth; //backup
        //                    float backupViewportH = painter.ViewportHeight; //backup
        //                    painter.AttachTo(_builtInBackBuffer); //*** switch to builtInBackbuffer 
        //                                                          // painter.SetViewportSize(this.Width, this.Height);
        //                    if (!_hasAccumRect)
        //                    {
        //                        _invalidateRect = new Rectangle(0, 0, Width, Height);
        //                    }
        //                    else
        //                    {

        //                    }

        //                    if (painter.PushLocalClipArea(
        //                        _invalidateRect.Left, _invalidateRect.Top,
        //                        _invalidateRect.Width, _invalidateRect.Height))
        //                    {
        //#if DEBUG
        //                        //for debug , test clear with random color
        //                        //another useful technique to see latest clear area frame-by-frame => use random color
        //                        //painter.Clear(Color.FromArgb(255, dbugRandom.Next(0, 255), dbugRandom.Next(0, 255), dbugRandom.Next(0, 255)));

        //                        canvas.Clear(Color.White);
        //#else
        //                        painter.Clear(Color.White);
        //#endif

        //                        base.RenderBoxContent(canvas, updateArea);
        //                    }

        //                    painter.PopLocalClipArea();
        //                    //
        //                    _builtInBackBuffer.IsValid = true;
        //                    _hasAccumRect = false;

        //                    painter.AttachToNormalBuffer();//*** switch back
        //                    //painter.SetViewportSize(backupViewportW, backupViewportH);//restore viewport size
        //                }
        //#if DEBUG
        //                else
        //                {
        //                    System.Diagnostics.Debug.WriteLine("double_buffer_update:" + dbug_obj_id + " use cache");
        //                }
        //#endif
        //                if (painter.PushLocalClipArea(
        //                        _invalidateRect.Left, _invalidateRect.Top,
        //                        _invalidateRect.Width, _invalidateRect.Height))
        //                {
        //                    painter.DrawImage(_builtInBackBuffer.GetImage(), 0, 0, this.Width, this.Height);
        //                }
        //                painter.PopLocalClipArea();
        //            }
        //            else
        //            {
        //                base.RenderBoxContent(canvas, updateArea);
        //            }
        //        }
    }
}