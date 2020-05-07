//BSD, 2014-present, WinterDev

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
using LayoutFarm;
using LayoutFarm.UI;

namespace PixelFarm.CpuBlit
{
    public abstract class BasicSprite : UIElement
    {
        protected double _angle = 0;
        protected double _spriteScale = 1.0;
        protected double _skewX = 0;
        protected double _skewY = 0;


        //------------------------------
        protected override bool HasReadyRenderElement
        {
            get
            {
                throw new NotSupportedException();
            }
        }
        public override RenderElement GetPrimaryRenderElement()
        {
            throw new NotImplementedException();
        }
        public override RenderElement CurrentPrimaryRenderElement
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override void InvalidateGraphics()
        {
            throw new NotImplementedException();
        }
        //------------------------------
        public int Width { get; set; }
        public int Height { get; set; }
        //***

        //***
        public abstract void Render(PixelFarm.Drawing.Painter p);
        protected void UpdateTransform(double width, double height, double x, double y)
        {
            x -= width / 2;
            y -= height / 2;
            _angle = Math.Atan2(y, x);
            _spriteScale = Math.Sqrt(y * y + x * x) / 100.0;
        }



        public virtual bool Move(int mouseX, int mouseY)
        {

            UpdateTransform((int)Width, (int)Height, mouseX, mouseY);
            return true;
        }

        
    }
}