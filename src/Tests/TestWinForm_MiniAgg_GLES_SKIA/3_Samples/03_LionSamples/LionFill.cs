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
    public class LionFillExample : DemoBase
    {
        PixelFarm.CpuBlit.VertexProcessing.Affine _currentTx;
        PixelFarm.CpuBlit.VertexProcessing.Affine _iden = PixelFarm.CpuBlit.VertexProcessing.Affine.IdentityMatrix;

        MyTestSprite _testSprite;
        public override void Init()
        {
            //VgVisualElement renderVx = VgVisualDocHelper.CreateVgVisualDocFromFile(@"Samples\lion_foot2.svg").VgRootElem;
            VgVisualElement renderVx = VgVisualDocHelper.CreateVgVisualDocFromFile(@"Samples\star.svg").VgRootElem;
            var spriteShape = new SpriteShape(renderVx);
            _testSprite = new MyTestSprite(spriteShape);

            //lionFill.AutoFlipY = true;

            _currentTx = PixelFarm.CpuBlit.VertexProcessing.Affine.NewMatix(
                VertexProcessing.AffinePlan.RotateDeg(30),
                VertexProcessing.AffinePlan.Scale(2)
                );

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
        public bool UseBitmapExt
        {
            get;
            set;
        }
        [DemoConfig(MaxValue = 20)]
        public int SharpRadius
        {
            //test
            get { return _testSprite.SharpenRadius; }
            set { _testSprite.SharpenRadius = value; }

        }
        [DemoConfig(MaxValue = 255)]
        public int AlphaValue
        {
            get { return _testSprite.AlphaValue; }
            set
            {
                _testSprite.AlphaValue = (byte)value;
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


        bool hitOnLion;

        List<MyTestSprite> _spriteList = new List<MyTestSprite>();
        MyTestSprite _hitSprite;

        public override void Init()
        {
            // lion 

            VgVisualElement renderVx = VgVisualDocHelper.CreateVgVisualDocFromFile(@"Samples\arrow2.svg").VgRootElem;
            var spriteShape = new SpriteShape(renderVx);
            _spriteList.Add(new MyTestSprite(spriteShape));
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
                        SpriteShape s = new SpriteShape(VgVisualDocHelper.CreateVgVisualDocFromFile(@"Samples\arrow2.svg").VgRootElem);
                        _spriteList.Add(new MyTestSprite(s) { JustMove = true });
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
                p.RenderQuality = Drawing.RenderQuality.Fast;
            }
            else
            {
                p.RenderQuality = Drawing.RenderQuality.HighQuality;
            }

            foreach (MyTestSprite s in _spriteList)
            {
                s.Render(p);
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
            _hitSprite = null;
            hitOnLion = false;

            for (int i = _spriteList.Count - 1; i >= 0; --i)
            {
                MyTestSprite sprite = _spriteList[i];

                double testX = x;
                double testY = y;
                if (!sprite.JustMove && sprite.CurrentAffineTx != null)
                {
                    sprite.CurrentAffineTx.Transform(ref testX, ref testY);
                }

                if (sprite.HitTest((float)testX, (float)testY, isRightButton))
                {
                    hitOnLion = true;
                    _hitSprite = sprite;
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
            if (hitOnLion && _hitSprite != null)
            {
                _hitSprite.JustMove = _moveOption == LionMoveOption.Move;
                _hitSprite.Move(x, y);
            }
        }
        [DemoConfig]
        public bool UseBitmapExt
        {
            get;
            set;
        }

        LionMoveOption _moveOption = LionMoveOption.Move;
        [DemoConfig]
        public LionMoveOption LionMoveOption
        {
            get
            {
                return _moveOption;
            }
            set
            {
                //switch (_moveOption = value)
                //{
                //    default: break;
                //    case LionMoveOption.Move:
                //        foreach (MyTestSprite s in _spriteList)
                //        {
                //            s.JustMove = true;
                //        }

                //        break;
                //    case LionMoveOption.ZoomAndRotate:
                //        foreach (MyTestSprite s in _spriteList)
                //        {
                //            s.JustMove = false;
                //        }
                //        break;
                //}
            }
        }

    }

}

