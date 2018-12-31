//MIT, 2018-present, WinterDev
//from http://www.antigrain.com/research/bezier_interpolation/index.html#PAGE_BEZIER_INTERPOLATION

using System;
using System.Collections.Generic;
using PaintLab.Svg;
using PixelFarm.Drawing;
using PixelFarm.VectorMath;
using PixelFarm.CpuBlit.VertexProcessing;
using Mini;


namespace PixelFarm.CpuBlit.Samples
{
    
    [Info(OrderCode = "03")]
    public class LionCurveReconstruction : DemoBase
    {

        MyTestSprite _testSprite;
        public override void Init()
        {
            VgVisualElement renderVx = VgVisualDocHelper.CreateVgVisualDocFromFile(@"Samples\lion.svg").VgRootElem;
            var spriteShape = new SpriteShape(renderVx);
            _testSprite = new MyTestSprite(spriteShape);
            //lionFill.AutoFlipY = true;
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

            _testSprite.Render(p);
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

}