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

using Mini;
namespace MatterHackers.Agg.Sample_LionFill
{
    [Info(OrderCode = "03")]
    [Info("Affine transformer, and basic renderers. You can rotate and scale the “Lion” with the"
      + " left mouse button. Right mouse button adds “skewing” transformations, proportional to the “X” "
      + "coordinate. The image is drawn over the old one with a cetrain opacity value. Change “Alpha” "
      + "to draw funny looking “lions”. Change window size to clear the window.")]
    public class LionFillExample : DemoBase
    {
        LionFill lionFill;
        public override void Init()
        {
            lionFill = new LionFill();
        }
        public override void Draw(Graphics2D g)
        {
            lionFill.OnDraw(g);
        }
        public override void MouseDrag(int x, int y)
        {
            lionFill.Move(x, y);
        }

        [DemoConfig(MaxValue = 255)]
        public int AlphaValue
        {
            get { return lionFill.AlphaValue; }
            set
            {
                lionFill.AlphaValue = (byte)value;
            }
        }
    }

    //--------------------------------------------------
    public class LionFill : BasicSprite
    {

        LionShape lionShape;
        VertexStorage vxStorage;

        VertexSourceApplyTransform transformedPathStorage;
        byte alpha;
        public LionFill()
        {
            lionShape = new LionShape();
            lionShape.ParseLion();
            this.Width = 500;
            this.Height = 500;
            AlphaValue = 255;
        }
        public byte AlphaValue
        {
            get { return this.alpha; }
            set
            {

                this.alpha = value;

                //change alpha value
                int j = lionShape.NumPaths;
                var colorBuffer = lionShape.Colors;
                for (int i = lionShape.NumPaths - 1; i >= 0; --i)
                {
                    colorBuffer[i].Alpha0To255 = alpha;
                }
            }
        }

        public override bool Move(int mouseX, int mouseY)
        {
            bool result = base.Move(mouseX, mouseY);
            vxStorage = null;
            return result;
        }
        public override void OnDraw(Graphics2D graphics2D)
        {

            if (vxStorage == null)
            {
                var transform = Affine.NewMatix(
                        AffinePlan.Translate(-lionShape.Center.x, -lionShape.Center.y),
                        AffinePlan.Scale(spriteScale, spriteScale),
                        AffinePlan.Rotate(angle + Math.PI),
                        AffinePlan.Skew(skewX / 1000.0, skewY / 1000.0),
                        AffinePlan.Translate(Width / 2, Height / 2)
                );

                //convert
                //System.Collections.Generic.List<VertexData> list = new System.Collections.Generic.List<VertexData>();
                transformedPathStorage = new VertexSourceApplyTransform(lionShape.Path, transform);
                //transformedPathStorage.DoTransform(list);
                //vxStorage = new VertexStorage(list);
                vxStorage = transformedPathStorage.DoTransformToNewVxStorage();
            }

            //graphics2D.Render(transformedPathStorage, lionShape.Colors, lionShape.PathIndexList, lionShape.NumPaths);
            graphics2D.Render(vxStorage, lionShape.Colors, lionShape.PathIndexList, lionShape.NumPaths);

            base.OnDraw(graphics2D);
        }




    }


}

