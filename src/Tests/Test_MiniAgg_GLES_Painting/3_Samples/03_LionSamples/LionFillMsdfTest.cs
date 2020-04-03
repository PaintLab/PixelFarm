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
using System.Collections.Generic;
using PaintLab.Svg;
using Mini;
namespace PixelFarm.CpuBlit.Samples
{

    [Info(OrderCode = "03")]
    public class LionFillExampleMsdf : DemoBase
    {
        PixelFarm.CpuBlit.VertexProcessing.Affine _currentTx;
        PixelFarm.CpuBlit.VertexProcessing.Affine _iden = PixelFarm.CpuBlit.VertexProcessing.Affine.IdentityMatrix;

        MyTestSprite _testSprite;
        public override void Init()
        {
            VgVisualElement renderVx = VgVisualDocHelper.CreateVgVisualDocFromFile(@"Samples\lion.svg").VgRootElem;
            var spriteShape = new SpriteShape(renderVx);
            _testSprite = new MyTestSprite(spriteShape);

            //lionFill.AutoFlipY = true;

            PixelFarm.CpuBlit.VertexProcessing.AffineMat mat = PixelFarm.CpuBlit.VertexProcessing.AffineMat.Iden();
            mat.RotateDeg(30);
            mat.Scale(2);

            _currentTx = new PixelFarm.CpuBlit.VertexProcessing.Affine(mat);

        }


        public override void Draw(PixelFarm.Drawing.Painter p)
        {

            p.CoordTransformer = _currentTx;
            p.Clear(Drawing.Color.White);
            if (UseBitmapExt)
            {
                p.RenderQuality = Drawing.RenderQuality.Fast;
            }
            else
            {
                p.RenderQuality = Drawing.RenderQuality.HighQuality;
            }

            _testSprite.Render(p);

            p.CoordTransformer = _iden;
        }
        public override void MouseDrag(int x, int y)
        {
            //move to specific position
            _testSprite.Move(x, y);
        }
        [DemoConfig]
        public bool UseBitmapExt { get; set; }
        [DemoConfig(MaxValue = 20)]
        public int SharpRadius
        {
            //test
            get => _testSprite.SharpenRadius;
            set { _testSprite.SharpenRadius = value; }

        }
        [DemoConfig(MaxValue = 255)]
        public int AlphaValue
        {
            get => _testSprite.AlphaValue;
            set
            {
                _testSprite.AlphaValue = (byte)value;
            }
        }
    }

}