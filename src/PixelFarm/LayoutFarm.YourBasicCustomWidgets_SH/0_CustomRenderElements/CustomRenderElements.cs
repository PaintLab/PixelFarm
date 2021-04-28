//Apache2, 2014-present, WinterDev

using System.Collections.Generic;

using PixelFarm.Drawing;
using LayoutFarm.RenderBoxes;


namespace LayoutFarm.CustomWidgets
{

    public class CustomRenderBox : RenderBoxBase
    {
        //LIMITATION:
        //these are NOT CSS borders/margins/paddings***
        //we use pixel unit for our RenderBox

        //if we want a sophisticate render element
        //we can create it in another render element class

        Color _backColor;
        Color _borderColor;
        bool _hasSomeBorderW;


        //...for rendering only, not for layout...
        ushort _contentLeft_offset; //border left + padding left,
        ushort _contentTop_offset; //border top + pading top
        ushort _contentRight_offset; //border right + padding right
        ushort _contentBottom_offset; //botrder bottom + padding bottom

        byte _borderLeft; //only border left,
        byte _borderTop; //only border top
        byte _borderRight; //only border right
        byte _borderBottom; //only border bottom 


        public CustomRenderBox(int width, int height)
            : base(width, height)
        {
            this.BackColor = KnownColors.LightGray;

        }

        public void RenderItsContent(DrawBoard d, UpdateArea updateArea)
        {
            RenderClientContent(d, updateArea);
        }


        internal List<RenderElemLineBox> Lines { get; set; }

        public ushort PaddingLeft
        {
            get => (ushort)(_contentLeft_offset - _borderLeft);
            set
            {
                _contentLeft_offset = (ushort)(value + _borderLeft);
            }
        }
        public ushort PaddingTop
        {
            get => (ushort)(_contentTop_offset - _borderTop);
            set
            {
                _contentTop_offset = (ushort)(value + _borderTop);
            }
        }
        public ushort PaddingRight
        {
            get => (ushort)(_contentRight_offset - _borderRight);
            set
            {
                _contentRight_offset = (ushort)(value + _borderRight);
            }
        }
        public ushort PaddingBottom

        {
            get => (ushort)(_contentBottom_offset - _borderBottom);
            set
            {
                _contentBottom_offset = (ushort)(value + _borderBottom);
            }
        }
        public void SetPadding(ushort left, ushort top, ushort right, ushort bottom)
        {
            _contentLeft_offset = (ushort)(left + _borderLeft);
            _contentTop_offset = (ushort)(top + _borderTop);
            _contentRight_offset = (ushort)(right + _borderRight);
            _contentBottom_offset = (ushort)(bottom + _borderBottom);
        }
        public void SetPadding(ushort sameValue)
        {
            _contentLeft_offset = (ushort)(sameValue + _borderLeft);
            _contentTop_offset = (ushort)(sameValue + _borderTop);
            _contentRight_offset = (ushort)(sameValue + _borderRight);
            _contentBottom_offset = (ushort)(sameValue + _borderBottom);
        }
        //------------------ 
        public byte BorderTop
        {
            get => _borderTop;
            set
            {
                _borderTop = (byte)value;
                if (!_hasSomeBorderW) _hasSomeBorderW = value > 0;
            }
        }
        public byte BorderBottom
        {
            get => _borderBottom;
            set
            {
                _borderBottom = (byte)value;
                if (!_hasSomeBorderW) _hasSomeBorderW = value > 0;
            }
        }
        public byte BorderRight
        {
            get => _borderRight;
            set
            {
                _borderRight = (byte)value;
                if (!_hasSomeBorderW) _hasSomeBorderW = value > 0;
            }
        }
        public byte BorderLeft
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

        public int ContentWidth => Width - (_contentLeft_offset + _contentRight_offset);
        public int ContentHeight => Height - (_contentTop_offset + _contentBottom_offset);

        public ushort ContentLeft
        {
            get => _contentLeft_offset;
            set => _contentLeft_offset = (byte)value;
        }
        public ushort ContentTop
        {
            get => _contentTop_offset;
            set => _contentTop_offset = (ushort)value;
        }
        public ushort ContentRight
        {
            get => _contentRight_offset;
            set => _contentRight_offset = (ushort)value;
        }
        public ushort ContentBottom
        {
            get => _contentBottom_offset;
            set => _contentBottom_offset = (ushort)value;
        }


        public Color BackColor
        {
            get => _backColor;
            set
            {
                if (_backColor == value) return;

                _backColor = value;

                BgIsNotOpaque = value.A < 255;


                if (this.HasParentLink && !this.BlockGraphicUpdateBubble)
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

        protected override void RenderClientContent(DrawBoard d, UpdateArea updateArea)
        {
#if DEBUG
            if (this.dbugBreak)
            {
            }
#endif
            //if (this.Width < 30 && this.BackColor == Color.White)
            //{

            //}

            //this render element dose not have child node, so
            //if WaitForStartRenderElement == true,
            //then we skip rendering its content
            //else if this renderElement has more child, we need to walk down)

            Color prevTextColorHint = d.TextBackgroundColorHint;

#if DEBUG
            if (prevTextColorHint.A == 0)
            {

            }
#endif


            bool setNewTextColorHint = false;
            if (!WaitForStartRenderElement)
            {
                if (BackColor.A > 0 && (Width > 0 && Height > 0))
                {
                    d.FillRectangle(BackColor, ViewportLeft, ViewportTop, this.Width, this.Height);
                    d.TextBackgroundColorHint = BackColor;
                    setNewTextColorHint = true;
                }
                //border is over background color          
            }

            //default content layer
            //check if we use multiline or not
            if (Lines != null)
            {

                List<RenderElemLineBox> lineboxes = Lines;
                int j = lineboxes.Count;
                int enter_canvas_x = d.OriginX;
                int enter_canvas_y = d.OriginY;

                int update_a_top = updateArea.Top;
                int update_a_bottom = updateArea.Bottom;

                for (int i = 0; i < j; ++i)
                {
                    RenderElemLineBox linebox = lineboxes[i];
                    if (linebox.IsIntersect(update_a_top, update_a_bottom))
                    {
                        //offset to this client

                        //if the child not need clip
                        //its children (if exist) may intersect 
                        int x = 0;
                        int y = linebox.LineTop;

                        d.SetCanvasOrigin(enter_canvas_x + x, enter_canvas_y + y);
                        updateArea.Offset(-x, -y);

                        linebox.Render(d, updateArea);

                        updateArea.Offset(x, y);
                    }
                }
                d.SetCanvasOrigin(enter_canvas_x, enter_canvas_y); //restore                
            }
            else
            {
                base.RenderClientContent(d, updateArea);
            }
            //

            if (!WaitForStartRenderElement &&
                _hasSomeBorderW && _borderColor.A > 0)
            {
                d.DrawRectangle(_borderColor, 0, 0, this.Width, this.Height);//test
            }

            if (setNewTextColorHint)
            {
                d.TextBackgroundColorHint = prevTextColorHint;
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

        public override void ChildrenHitTestCore(HitChain hitChain)
        {

            if (Lines != null)
            {
                //check if it's overlap line or not
                //find a properline 
                //then offset and test at that line
                List<RenderElemLineBox> lineboxes = Lines;
                int j = lineboxes.Count;
                for (int i = 0; i < j; ++i)
                {
                    RenderElemLineBox linebox = lineboxes[i];
                    if (linebox.HitTestCore(hitChain))
                    {
                        return;
                    }
                    else if (linebox.LineTop > hitChain.TestPointY)
                    {
                        //we iterate from top to bottom
                        //we should stop and return
                        return;
                    }
                }
            }
            else
            {
                //use default machanism
                base.ChildrenHitTestCore(hitChain);
            }
        }
    }

    public class DoubleBufferCustomRenderBox : CustomRenderBox
    {

        bool _hasAccumRect;
        Rectangle _invalidateRect;

        class BackBufferPlate
        {
            public int no;
            public int left;
            public int right;
            public int top;
            public int bottom;
            public DrawboardBuffer _backBuffer;
            public int Width => right - left;
            public int Height => bottom - top;
            public void SetBounds(int left, int top, int width, int height)
            {
                this.left = left;
                this.right = left + width;
                this.top = top;
                this.bottom = top + height;
            }
            public override string ToString()
            {
                //plate no
                return no.ToString() + ", (left,top)=" + left + "," + top;
            }
        }


        public DoubleBufferCustomRenderBox(int width, int height)
          : base(width, height)
        {
            NeedInvalidateRectEvent = true;
        }
        public bool EnableDoubleBuffer { get; set; }


        BackBufferPlate _p0;
        BackBufferPlate _p1;
        TriplePlateMx _triplePlateMx;

        class TriplePlateMx
        {
            BackBufferPlate[] _plates;
            int _width;
            int _height;

            public void Init(DrawBoard d, int w, int h)
            {
                if (_plates != null) return;
                _width = w;
                _height = h;
                _plates = new BackBufferPlate[3];

                _plates[0] = new BackBufferPlate() { _backBuffer = d.CreateBackbuffer(w, h) };
                _plates[1] = new BackBufferPlate() { _backBuffer = d.CreateBackbuffer(w, h) };
                _plates[2] = new BackBufferPlate() { _backBuffer = d.CreateBackbuffer(w, h) };

                //update init plate-no and bounds
                int ypos = 0;
                for (int i = 0; i < _plates.Length; ++i)
                {
                    BackBufferPlate p = _plates[i];
                    p.no = i;
                    p.SetBounds(0, ypos, w, h);
                    ypos += h;
                }
            }


            public void SetCurrentArea(UpdateArea updateArea, out BackBufferPlate _p0, out BackBufferPlate _p1)
            {
                int topPlateNo = updateArea.Top / _height;
                int bottomPlateNo = updateArea.Bottom / _height;

                //find top plate
                BackBufferPlate foundPlate = null;
                int foundAtIndex = 0;
                for (int i = 0; i < _plates.Length; ++i)
                {
                    BackBufferPlate p = _plates[i];
                    if (p.no == topPlateNo) //match plate number
                    {
                        //found- page
                        foundPlate = p;
                        foundAtIndex = i;
                        break;
                    }
                }

                if (foundPlate == null)
                {
                    //not found matching plate                     
                    //swap up or down?
                    if (_plates[0].no - 1 == topPlateNo)
                    {

                        BackBufferPlate a0 = _plates[0];
                        BackBufferPlate a1 = _plates[1];
                        BackBufferPlate a2 = _plates[2];

                        //move a0, a1 down
                        //move a2 up
                        _plates[0] = a2;
                        _plates[1] = a0;
                        _plates[2] = a1;
                        //
                        a2.no = a0.no - 1;
                        a2._backBuffer.IsValid = false;
                        a2.SetBounds(0, a2.no * _height, _width, _height);

                        foundAtIndex = 0;
                        foundPlate = a2;
                    }
                    else if (_plates[2].no + 1 == topPlateNo)
                    {
                        BackBufferPlate a0 = _plates[0];
                        BackBufferPlate a1 = _plates[1];
                        BackBufferPlate a2 = _plates[2];

                        //move a1,a2 up
                        //move a0 to last elem
                        _plates[0] = a1;
                        _plates[1] = a2;
                        _plates[2] = a0;
                        //
                        a0.no = a0.no + 1;
                        a0._backBuffer.IsValid = false;
                        a0.SetBounds(0, a2.no * _height, _width, _height);

                        foundAtIndex = 2;
                        foundPlate = a0;
                    }
                    else
                    {
                        //reset all plates
                        int ypos = topPlateNo * _height;
                        for (int i = 0; i < _plates.Length; ++i)
                        {
                            BackBufferPlate p = _plates[i];
                            p.no = i + topPlateNo;
                            p._backBuffer.IsValid = false;
                            p.SetBounds(0, ypos, _width, _height);
                            ypos += _height;
                        }
                        foundPlate = _plates[0];
                        foundAtIndex = 0;
                    }
                }


                //found
                if (topPlateNo == bottomPlateNo)
                {
                    _p0 = foundPlate;
                    _p1 = null;
                }
                else
                {
                    //need 2 page
                    _p0 = foundPlate;
                    if (foundAtIndex + 1 < 3)
                    {
                        //0-1-2
                        _p1 = _plates[foundAtIndex + 1];
                    }
                    else
                    {
                        //we need to swap
                        BackBufferPlate a0 = _plates[0];
                        BackBufferPlate a1 = _plates[1];
                        BackBufferPlate a2 = _plates[2];

                        BackBufferPlate tmp1 = a0;//***
                        _plates[0] = a1;
                        _plates[1] = a2;
                        _plates[2] = tmp1;

                        tmp1._backBuffer.IsValid = false;//**
                        tmp1.no = a2.no + 1;
                        tmp1.SetBounds(0, a2.bottom, tmp1.Width, tmp1.Height);

                        _p1 = tmp1;
                    }
                }
            }
        }

        protected override void RenderClientContent(DrawBoard d, UpdateArea updateArea)
        {
            if (!EnableDoubleBuffer)
            {
                base.RenderClientContent(d, updateArea);
                return;//early exit***
            }
            //------------------------------------------

            //helper struct
            var painter = new PixelFarm.Drawing.Internal.MicroPainter(d);
            int backBufferHeight = this.Height * 2;

            //from updateArea we should calculate a proper page 
            int topPlateNo = updateArea.Top / backBufferHeight;
            int bottomPlateNo = updateArea.Bottom / backBufferHeight;
            if (_triplePlateMx == null)
            {
                _triplePlateMx = new TriplePlateMx();
                _triplePlateMx.Init(d, Width, backBufferHeight);
            }

            _triplePlateMx.SetCurrentArea(updateArea, out _p0, out _p1);
            //------------------------------------------
            bool also_drawWithBackBuffer1 = _p1 != null;


            if (also_drawWithBackBuffer1 && !_p1._backBuffer.IsValid)
            {
                //update
#if DEBUG
                System.Diagnostics.Debug.WriteLine("double_buffer_update:" + this.dbug_obj_id + "," + _invalidateRect.ToString());
#endif
                float backupViewportW = painter.ViewportWidth; //backup
                float backupViewportH = painter.ViewportHeight; //backup
                painter.AttachTo(_p1._backBuffer); //*** switch to builtInBackbuffer 
                painter.SetViewportSize(this.Width, backBufferHeight);//after to attach new buffer**
                if (!_hasAccumRect)
                {
                    _invalidateRect = new Rectangle(0, 0, Width, Height);
                }
#if DEBUG
                d.Clear(Color.Blue);//dbug
#else
                 d.Clear(Color.White);
#endif
                Rectangle backup = updateArea.CurrentRect;
                updateArea.CurrentRect = new Rectangle(0, _p1.top, _p1.Width, _p1.Height);
                d.SetCanvasOrigin(0, -_p1.top);//minus**
                base.RenderClientContent(d, updateArea);
                d.SetCanvasOrigin(0, 0);

#if DEBUG
                //Image dbugImg = p1._backBuffer.CopyToNewMemBitmap();
                //PixelFarm.CpuBlit.MemBitmapExt.s_dbugEnableDebugImage = true;
                //PixelFarm.CpuBlit.MemBitmapExt.SaveImage((PixelFarm.CpuBlit.MemBitmap)dbugImg, "temp1.png", PixelFarm.CpuBlit.MemBitmapIO.OutputImageFormat.Png);
                //PixelFarm.CpuBlit.MemBitmapExt.s_dbugEnableDebugImage = false;
#endif

                updateArea.CurrentRect = backup;

                _p1._backBuffer.IsValid = true;

                painter.AttachToNormalBuffer();//*** switch back
                painter.SetViewportSize(backupViewportW, backupViewportH);//restore viewport size
            }

            //--------------
            if (!_p0._backBuffer.IsValid)
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine("double_buffer_update:" + this.dbug_obj_id + "," + _invalidateRect.ToString());
#endif
                float backupViewportW = painter.ViewportWidth; //backup
                float backupViewportH = painter.ViewportHeight; //backup
                painter.AttachTo(_p0._backBuffer); //*** switch to builtInBackbuffer 
                painter.SetViewportSize(this.Width, backBufferHeight);//after to attach new buffer**
                if (!_hasAccumRect)
                {
                    _invalidateRect = new Rectangle(0, 0, Width, Height);
                }

#if DEBUG
                //if (_invalidateRect.Width == 15)
                //{

                //}
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
                d.Clear(Color.White);
#endif

                Rectangle backup = updateArea.CurrentRect;
                updateArea.CurrentRect = new Rectangle(0, _p0.top, _p0.Width, _p0.Height);
                d.SetCanvasOrigin(0, -_p0.top);//minus**
                base.RenderClientContent(d, updateArea);
                updateArea.CurrentRect = backup;
                d.SetCanvasOrigin(0, 0);
                //}
                //painter.PopLocalClipArea();
                //
                _p0._backBuffer.IsValid = true;
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

            _hasAccumRect = false;

            if (also_drawWithBackBuffer1)
            {
                painter.DrawImage(_p0._backBuffer.GetImage(), 0, _p0.top);
                painter.DrawImage(_p1._backBuffer.GetImage(), 0, _p1.top);
            }
            else
            {
                painter.DrawImage(_p0._backBuffer.GetImage(), 0, _p0.top);
            }
        }
        protected override void OnInvalidateGraphicsNoti(bool fromMe, ref Rectangle totalBounds)
        {

            //find affected slide

            if (_p0 != null)
            {
                //TODO: review here,
                //in this case, we copy to another rect
                //since we don't want the offset to effect the total bounds 
#if DEBUG
                //if (totalBounds.Width == 150)
                //{

                //}
                System.Diagnostics.Debug.WriteLine("noti, fromMe=" + fromMe + ",bounds" + totalBounds);
#endif
                if (!fromMe)
                {
                    totalBounds.Offset(-this.X, -this.Y);
                }

                int actualTop = totalBounds.Top + this.ViewportTop;
                int actualBottom = totalBounds.Top + this.ViewportBottom;

                _p0._backBuffer.IsValid = false;

                if (!_hasAccumRect)
                {
                    _invalidateRect = totalBounds;
                    _hasAccumRect = true;
                }
                else
                {
                    _invalidateRect = Rectangle.Union(_invalidateRect, totalBounds);
                }

                //------
                if (_p1 != null)
                {

                    if (actualTop < _p1.top && actualBottom > _p1.top)
                    {
                        _p1._backBuffer.IsValid = false;
                    }
                    else if (actualTop > _p1.top && actualBottom > _p1.bottom)
                    {
                        _p1._backBuffer.IsValid = false;
                    }
                    else
                    {

                    }
                }
            }
            else
            {
                totalBounds.Offset(this.X, this.Y);
            }

            //base.OnInvalidateGraphicsNoti(totalBounds);//skip
        }
    }
}