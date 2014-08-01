//2014 BSD, WinterDev

/*
Copyright (c) 2013, Lars Brubaker
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met: 

1. Redistributions of source code must retain the above copyright notice, this
   list of conditions and the following disclaimer. 
2. Redistributions in binary form must reproduce the above copyright notice,
   this list of conditions and the following disclaimer in the documentation
   and/or other materials provided with the distribution. 

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR
ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

The views and conclusions contained in the software and documentation are those
of the authors and should not be interpreted as representing official policies, 
either expressed or implied, of the FreeBSD Project.
*/

using System;
using MatterHackers.Agg.Transform;
using MatterHackers.Agg.Image;
using MatterHackers.Agg.VertexSource;
using MatterHackers.VectorMath;

namespace MatterHackers.Agg
{
    public abstract class BasicSprite
    {
        protected double angle = 0;
        protected double spriteScale = 1.0;
        protected double skewX = 0;
        protected double skewY = 0;
        
        public int Width { get; set; }
        public int Height { get; set; }

        public virtual void OnDraw(Graphics2D graphics2D)
        {
            //for (int i = 0; i < Children.Count; i++)
            //{
            //    GuiWidget child = Children[i];
            //    if (child.Visible)
            //    {
            //        RectangleDouble oldClippingRect = graphics2D.GetClippingRect();
            //        graphics2D.PushTransform();
            //        {
            //            Affine currentGraphics2DTransform = graphics2D.GetTransform();
            //            Affine accumulatedTransform = currentGraphics2DTransform * child.ParentToChildTransform;
            //            graphics2D.SetTransform(accumulatedTransform);

            //            RectangleDouble currentScreenClipping;
            //            if (child.CurrentScreenClipping(out currentScreenClipping))
            //            {
            //                currentScreenClipping.Left = Math.Floor(currentScreenClipping.Left);
            //                currentScreenClipping.Right = Math.Ceiling(currentScreenClipping.Right);
            //                currentScreenClipping.Bottom = Math.Floor(currentScreenClipping.Bottom);
            //                currentScreenClipping.Top = Math.Ceiling(currentScreenClipping.Top);
            //                if (currentScreenClipping.Right < currentScreenClipping.Left || currentScreenClipping.Top < currentScreenClipping.Bottom)
            //                {
            //                    //ThrowExceptionInDebug("Right is less than Left or Top is less than Bottom");
            //                }

            //                graphics2D.SetClippingRect(currentScreenClipping);

            //                if (child.DoubleBuffer)
            //                {
            //                    Vector2 offsetToRenderSurface = new Vector2(currentGraphics2DTransform.tx, currentGraphics2DTransform.ty);
            //                    offsetToRenderSurface += child.OriginRelativeParent;

            //                    double yFraction = offsetToRenderSurface.y - (int)offsetToRenderSurface.y;
            //                    double xFraction = offsetToRenderSurface.x - (int)offsetToRenderSurface.x;
            //                    int xOffset = (int)Math.Floor(child.LocalBounds.Left);
            //                    int yOffset = (int)Math.Floor(child.LocalBounds.Bottom);
            //                    if (child.isCurrentlyInvalid)
            //                    {
            //                        Graphics2D childBackBufferGraphics2D = child.backBuffer.NewGraphics2D();
            //                        childBackBufferGraphics2D.Clear(new RGBA_Bytes(0, 0, 0, 0));
            //                        Affine transformToBuffer = Affine.NewTranslation(-xOffset + xFraction, -yOffset + yFraction);
            //                        childBackBufferGraphics2D.SetTransform(transformToBuffer);
            //                        child.OnDrawBackground(childBackBufferGraphics2D);
            //                        child.OnDraw(childBackBufferGraphics2D);

            //                        child.backBuffer.MarkImageChanged();
            //                        child.isCurrentlyInvalid = false;
            //                    }

            //                    offsetToRenderSurface.x = (int)offsetToRenderSurface.x + xOffset;
            //                    offsetToRenderSurface.y = (int)offsetToRenderSurface.y + yOffset;
            //                    // The transform to draw the backbuffer to the graphics2D must not have a factional amount
            //                    // or we will get aliasing in the image and we want our back buffer pixels to map 1:1 to the next buffer
            //                    if (offsetToRenderSurface.x - (int)offsetToRenderSurface.x != 0
            //                        || offsetToRenderSurface.y - (int)offsetToRenderSurface.y != 0)
            //                    {
            //                        //ThrowExceptionInDebug("The transform for a back buffer must be integer to avoid aliasing.");
            //                    }
            //                    graphics2D.SetTransform(Affine.NewTranslation(offsetToRenderSurface));

            //                    graphics2D.Render(child.backBuffer, 0, 0);
            //                }
            //                else
            //                {
            //                    child.OnDrawBackground(graphics2D);
            //                    child.OnDraw(graphics2D);
            //                }
            //            }
            //        }
            //        graphics2D.PopTransform();
            //        graphics2D.SetClippingRect(oldClippingRect);
            //    }
            //}

            //if (DebugShowBounds)
            //{
            //    graphics2D.Line(LocalBounds.Left, LocalBounds.Bottom, LocalBounds.Right, LocalBounds.Top, RGBA_Bytes.Green);
            //    graphics2D.Line(LocalBounds.Left, LocalBounds.Top, LocalBounds.Right, LocalBounds.Bottom, RGBA_Bytes.Green);
            //    graphics2D.Rectangle(LocalBounds, RGBA_Bytes.Red);
            //}
            //if (showSize)
            //{
            //    graphics2D.DrawString(string.Format("{4} {0}, {1} : {2}, {3}", (int)MinimumSize.x, (int)MinimumSize.y, (int)LocalBounds.Width, (int)LocalBounds.Height, Name),
            //        Width / 2, Math.Max(Height - 16, Height / 2 - 16 * graphics2D.TransformStackCount), color: RGBA_Bytes.Magenta, justification: Font.Justification.Center);
            //}
        }
        protected void UpdateTransform(double width, double height, double x, double y)
        {
            x -= width / 2;
            y -= height / 2;
            angle = Math.Atan2(y, x);
            spriteScale = Math.Sqrt(y * y + x * x) / 100.0;
        }

        public bool Move(int mouseX, int mouseY)
        {
            double x = mouseX;
            double y = mouseY;

            int width = (int)Width;
            int height = (int)Height;
            UpdateTransform(width, height, x, y);

            return true;

        }
    } 
}