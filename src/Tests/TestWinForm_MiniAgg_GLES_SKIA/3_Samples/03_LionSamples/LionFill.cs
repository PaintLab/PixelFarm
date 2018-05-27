//BSD, 2014-2018, WinterDev

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

using Mini;
namespace PixelFarm.Agg.Samples
{
    [Info(OrderCode = "03")]
    [Info("Affine transformer, and basic renderers. You can rotate and scale the “Lion” with the"
      + " left mouse button. Right mouse button adds “skewing” transformations, proportional to the “X” "
      + "coordinate. The image is drawn over the old one with a cetrain opacity value. Change “Alpha” "
      + "to draw funny looking “lions”. Change window size to clear the window.")]
    public class LionFillExample : DemoBase
    {
        MyTestSprite lionFill;
        public override void Init()
        {
            lionFill = new MyTestSprite(new SpriteShape(SvgRenderVxLoader.CreateSvgRenderVxFromFile(@"Samples\lion.svg")));
            lionFill.AutoFlipY = true;
        }

        public override void Draw(PixelFarm.Drawing.Painter p)
        {
            p.Clear(Drawing.Color.White);

            if (UseBitmapExt)
            {
                p.RenderQuality = Drawing.RenderQualtity.Fast;
            }
            else
            {
                p.RenderQuality = Drawing.RenderQualtity.HighQuality;
            }

            lionFill.Draw(p);
        }
        public override void MouseDrag(int x, int y)
        {
            lionFill.Move(x, y);
        }
        [DemoConfig]
        public bool UseBitmapExt
        {
            get;
            set;
        }
        [DemoConfig(MaxValue = 20)]
        public int SharpRadius
        {
            //test
            get { return lionFill.SharpenRadius; }
            set { lionFill.SharpenRadius = value; }

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



    public enum LionMoveOption
    {
        ZoomAndRotate,
        Move,
    }
    //----------
    [Info(OrderCode = "03.1")]
    [Info("HitTest and Selection")]
    public class LionFillExample_HitTest : DemoBase
    {

        MyTestSprite _hitLion;
        bool hitOnLion;
        List<MyTestSprite> lionList = new List<MyTestSprite>();
        public override void Init()
        {
            // lion
            SpriteShape s = new SpriteShape(SvgRenderVxLoader.CreateSvgRenderVxFromFile(@"Samples\arrow2.svg"));
            lionList.Add(new MyTestSprite(s));
            //
            //lionFill.AutoFlipY = true;           
        }
        public override void KeyDown(int keycode)
        {
            //temp 
            System.Windows.Forms.Keys k = (System.Windows.Forms.Keys)keycode;
            switch (k)
            {
                case System.Windows.Forms.Keys.A:
                    {
                        SpriteShape s = new SpriteShape(SvgRenderVxLoader.CreateSvgRenderVxFromFile(@"Samples\arrow2.svg"));
                        lionList.Add(new MyTestSprite(s) { JustMove = true });
                    }
                    break;
                case System.Windows.Forms.Keys.Q:
                    {
                        //test add box control ...


                    }
                    break;
            }


            base.KeyDown(keycode);
        }
        public override void Draw(PixelFarm.Drawing.Painter p)
        {
            p.Clear(Drawing.Color.White);

            if (UseBitmapExt)
            {
                p.RenderQuality = Drawing.RenderQualtity.Fast;
            }
            else
            {
                p.RenderQuality = Drawing.RenderQualtity.HighQuality;
            }

            foreach (var lion in lionList)
            {
                lion.Draw(p);
            }
        }

        public override void MouseDown(int x, int y, bool isRightButton)
        {

            //check if we hit a lion or not 
            //this is example => if right button=>test with path

            if (isRightButton)
            {
                if (LionMoveOption == LionMoveOption.Move)
                {
                    LionMoveOption = LionMoveOption.ZoomAndRotate;
                }
                else
                {
                    LionMoveOption = LionMoveOption.Move;
                }
            }

            //-----------------------------------------------------
            _hitLion = null;
            hitOnLion = false;

            for (int i = lionList.Count - 1; i >= 0; --i)
            {
                MyTestSprite lion = lionList[i];
                if (lion.HitTest(x, y, isRightButton))
                {
                    hitOnLion = true;
                    _hitLion = lion;
                    break;
                }
            }


            base.MouseDown(x, y, isRightButton);
        }
        public override void MouseUp(int x, int y)
        {
            hitOnLion = false;
            base.MouseUp(x, y);
        }
        public override void MouseDrag(int x, int y)
        {
            if (hitOnLion && _hitLion != null)
            {
                _hitLion.Move(x, y);
            }
        }
        [DemoConfig]
        public bool UseBitmapExt
        {
            get;
            set;
        }

        LionMoveOption _moveOption;
        [DemoConfig]
        public LionMoveOption LionMoveOption
        {
            get
            {
                return _moveOption;
            }
            set
            {
                switch (_moveOption = value)
                {
                    default: break;
                    case LionMoveOption.Move:
                        foreach (var lion in lionList)
                        {
                            lion.JustMove = true;
                        }

                        break;
                    case LionMoveOption.ZoomAndRotate:
                        foreach (var lion in lionList)
                        {
                            lion.JustMove = false;
                        }
                        break;
                }
            }
        }

    }

}

