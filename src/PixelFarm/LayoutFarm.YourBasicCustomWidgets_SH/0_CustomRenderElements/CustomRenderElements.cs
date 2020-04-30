//Apache2, 2014-present, WinterDev


using PixelFarm.Drawing;
using System.Collections.Generic;
using LayoutFarm.RenderBoxes;
using LayoutFarm.UI;

namespace LayoutFarm.CustomWidgets
{


    class LineBox : IParentLink
    {
        LinkedList<RenderElement> _linkList = new LinkedList<RenderElement>();

#if DEBUG
        static int dbugTotalId;
        public readonly int dbugId;
#endif
        public LineBox()
        {
#if DEBUG
            dbugId = dbugTotalId++;
#endif

        }
        public int LineTop { get; set; }
        public int LineHeight { get; set; }
        public int LineBottom => LineTop + LineHeight;
        public RenderElement ParentRenderElement { get; internal set; }

        public void AdjustLocation(ref int px, ref int py)
        {
            //TODO: line left ?
            py += LineTop;
        }
#if DEBUG
        public string dbugGetLinkInfo()
        {
            return "linkbox";
        }
#endif
        public RenderElement FindOverlapedChildElementAtPoint(RenderElement afterThisChild, Point point)
        {
            return null;
        }

        public bool IsIntersect(int minY, int maxY) => !((maxY < LineTop) || (minY > (LineTop + LineHeight)));

        //TODO: need to clear all parent link? 
        public void Clear() => _linkList.Clear();

        public void Add(RenderElement renderE)
        {
            _linkList.AddLast(renderE);
            if (ParentRenderElement != null)
            {
                RenderElement.SetParentLink(renderE, this);
            }
        }

        public int Count => _linkList.Count;

        public bool HitTestCore(HitChain hitChain)
        {

            if (LineTop <= hitChain.TestPointY &&
               (LineTop + LineHeight) > hitChain.TestPointY)
            {
                LinkedListNode<RenderElement> node = _linkList.First;
                hitChain.OffsetTestPoint(0, -LineTop);
                bool found = false;
                while (node != null)
                {
                    if (node.Value.HitTestCore(hitChain))
                    {
                        found = true;
                        break;
                    }
                    node = node.Next;
                }
                hitChain.OffsetTestPoint(0, LineTop);
                return found;
            }
            return false;
        }
        public void Render(DrawBoard d, UpdateArea updateArea)
        {
            LinkedListNode<RenderElement> renderNode = _linkList.First;
            Rectangle backup = updateArea.CurrentRect;
            int enter_canvas_x = d.OriginX;
            int enter_canvas_y = d.OriginY;

            while (renderNode != null)
            {
                //---------------------------
                //TODO: review here again
                RenderElement renderE = renderNode.Value;
                if (renderE.IntersectsWith(updateArea))
                {
                    int x = renderE.X;
                    int y = renderE.Y;

                    d.SetCanvasOrigin(enter_canvas_x + x, enter_canvas_y + y);
                    updateArea.Offset(-x, -y);
                    RenderElement.Render(renderE, d, updateArea);
                    updateArea.Offset(x, y);
                }

                renderNode = renderNode.Next;
            }
            updateArea.CurrentRect = backup;//restore  
            d.SetCanvasOrigin(enter_canvas_x, enter_canvas_y);//restore
        }

        public void AdjustHorizontalAlignment(RectUIAlignment alignment)
        {
            //line, and spread data inside this line


        }
        public void AdjustVerticalAlignment(VerticalAlignment vertAlignment)
        {
            LinkedListNode<RenderElement> node = _linkList.First;

            switch (vertAlignment)
            {
                case VerticalAlignment.Bottom:
                    while (node != null)
                    {
                        RenderElement r = node.Value;
                        if (r is CustomRenderBox box)
                        {
                            int diff = LineHeight - r.Height;
                            if (diff > 0)
                            {
                                //change location
                                r.SetLocation(r.Left, box.ContentTop + diff);
                            }
                            else
                            {
                                //
                            }
                        }
                        else
                        {
                            r.SetLocation(r.Left, 0); //***
                        }

                        node = node.Next;//**
                    }
                    break;
                case VerticalAlignment.Middle:
                    {
                        //middle height of this line                        

                        while (node != null)
                        {
                            RenderElement r = node.Value;
                            if (r is CustomRenderBox box)
                            {
                                int diff = LineHeight - r.Height;
                                if (diff > 0)
                                {
                                    //change location
                                    r.SetLocation(r.Left, box.ContentTop + (diff / 2));
                                }
                                else
                                {
                                    //
                                }
                            }
                            else
                            {
                                r.SetLocation(r.Left, 0); //***
                            }

                            node = node.Next;//**
                        }
                    }
                    break;
                case VerticalAlignment.Top:
                    while (node != null)
                    {
                        RenderElement r = node.Value;
                        if (r is CustomRenderBox box)
                        {
                            r.SetLocation(r.Left, box.ContentTop);
                        }
                        else
                        {
                            r.SetLocation(r.Left, 0); //***
                        }

                        node = node.Next;//**
                    }
                    break;
                case VerticalAlignment.UserSpecific://TODO
                    break;
            }
        }
    }


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

        ushort _contentLeft;
        ushort _contentTop;
        ushort _contentRight;
        ushort _contentBottom;

        byte _borderLeft;
        byte _borderTop;
        byte _borderRight;
        byte _borderBottom;

        public CustomRenderBox(int width, int height)
            : base(width, height)
        {
            this.BackColor = KnownColors.LightGray;
        }


        internal List<LineBox> Lines { get; set; }

        public ushort PaddingLeft
        {
            get => (ushort)(_contentLeft - _borderLeft);
            set
            {
                _contentLeft = (ushort)(value + _borderLeft);
            }
        }
        public ushort PaddingTop
        {
            get => (ushort)(_contentTop - _borderTop);
            set
            {
                _contentTop = (ushort)(value + _borderTop);
            }
        }
        public ushort PaddingRight
        {
            get => (ushort)(_contentRight - _borderRight);
            set
            {
                _contentRight = (ushort)(value + _borderRight);
            }
        }
        public ushort PaddingBottom

        {
            get => (ushort)(_contentBottom - _borderBottom);
            set
            {
                _contentBottom = (ushort)(value + _borderBottom);
            }
        }
        public void SetPadding(ushort left, ushort top, ushort right, ushort bottom)
        {
            _contentLeft = (ushort)(left + _borderLeft);
            _contentTop = (ushort)(top + _borderTop);
            _contentRight = (ushort)(right + _borderRight);
            _contentBottom = (ushort)(bottom + _borderBottom);
        }
        public void SetPadding(ushort sameValue)
        {
            _contentLeft = (ushort)(sameValue + _borderLeft);
            _contentTop = (ushort)(sameValue + _borderTop);
            _contentRight = (ushort)(sameValue + _borderRight);
            _contentBottom = (ushort)(sameValue + _borderBottom);
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

        public int ContentWidth => Width - (_contentLeft + _contentRight);
        public int ContentHeight => Height - (_contentTop + _contentBottom);

        public ushort ContentLeft
        {
            get => _contentLeft;
            set => _contentLeft = (byte)value;
        }
        public ushort ContentTop
        {
            get => _contentTop;
            set => _contentTop = (ushort)value;
        }
        public ushort ContentRight
        {
            get => _contentRight;
            set => _contentRight = (ushort)value;
        }
        public ushort ContentBottom
        {
            get => _contentBottom;
            set => _contentBottom = (ushort)value;
        }
        public void SetContentOffsets(ushort contentLeft, ushort contentTop, ushort contentRight, ushort contentBottom)
        {
            _contentLeft = contentLeft;
            _contentTop = contentTop;
            _contentRight = contentRight;
            _contentBottom = contentBottom;
        }
        public void SetContentOffsets(ushort allside)
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

                BgIsNotOpaque = value.A < 255;

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

            if (!WaitForStartRenderElement)
            {
                d.FillRectangle(BackColor, ViewportLeft, ViewportTop, this.Width, this.Height);
                d.SetLatestFillAsTextBackgroundColorHint();
                //border is over background color          
            }

            //default content layer
            //check if we use multiline or not
            if (Lines != null)
            {

                List<LineBox> lineboxes = Lines;
                int j = lineboxes.Count;
                int enter_canvas_x = d.OriginX;
                int enter_canvas_y = d.OriginY;

                int update_a_top = updateArea.Top;
                int update_a_bottom = updateArea.Bottom;

                for (int i = 0; i < j; ++i)
                {
                    LineBox linebox = lineboxes[i];
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

#if DEBUG
            //if (this.dbugBreak)
            //    canvas.FillRectangle(Color.Red, ViewportLeft, ViewportTop, this.Width, this.Height);
            //canvas.dbug_DrawCrossRect(PixelFarm.Drawing.Color.Black,
            //    new Rectangle(0, 0, this.Width, this.Height));

            //canvas.dbug_DrawCrossRect(PixelFarm.Drawing.Color.Black,
            //   new Rectangle(updateArea.Left, updateArea.Top, updateArea.Width, updateArea.Height));
#endif
        }


        protected override bool CustomHitTest(HitChain hitChain)
        {
            return base.CustomHitTest(hitChain);
        }
        public override void ChildrenHitTestCore(HitChain hitChain)
        {

            if (Lines != null)
            {
                //check if it's overlap line or not
                //find a properline 
                //then offset and test at that line
                List<LineBox> lineboxes = Lines;
                int j = lineboxes.Count;
                for (int i = 0; i < j; ++i)
                {
                    LineBox linebox = lineboxes[i];
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
        DrawboardBuffer _builtInBackBuffer;
        bool _hasAccumRect;
        Rectangle _invalidateRect;

        public DoubleBufferCustomRenderBox(int width, int height)
          : base(width, height)
        {
            NeedInvalidateRectEvent = true;
        }
        public bool EnableDoubleBuffer { get; set; }

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
        protected override void RenderClientContent(DrawBoard d, UpdateArea updateArea)
        {
            if (EnableDoubleBuffer)
            {
                var painter = new PixelFarm.Drawing.Internal.MicroPainter(d);
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
                    d.Clear(Color.White);
#endif



                    Rectangle backup = updateArea.CurrentRect;
                    updateArea.CurrentRect = new Rectangle(0, 0, _builtInBackBuffer.Width, _builtInBackBuffer.Height);
                    base.RenderClientContent(d, updateArea);
                    updateArea.CurrentRect = backup;
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

    }
}