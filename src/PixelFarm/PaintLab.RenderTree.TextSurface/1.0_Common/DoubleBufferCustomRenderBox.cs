//MIT, 2019-present, WinterDev

using System.Collections.Generic;
using System.Text;

using LayoutFarm.RenderBoxes;
using PixelFarm.Drawing;

namespace LayoutFarm.TextEditing
{
    public class DoubleBufferCustomRenderBox : RenderBoxBase
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

        public RenderBoxBase ContentBox { get; set; }

        public bool EnableDoubleBuffer
        {
            get => _enableDoubleBuffer;
            set
            {
                _enableDoubleBuffer = value;
            }
        }
        protected override PlainLayer CreateDefaultLayer()
        {
            return new PlainLayer(this);
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
            if (ContentBox == null) return;
            //
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
                    RenderElement.Render(ContentBox, d, updateArea2);
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
                RenderElement.Render(ContentBox, d, updateArea);
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