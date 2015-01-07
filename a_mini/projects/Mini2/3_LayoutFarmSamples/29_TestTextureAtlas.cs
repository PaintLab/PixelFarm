using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

using PixelFarm.Agg;
using PixelFarm.DrawingGL;
namespace Mini2
{
    [Info(OrderCode = "09")]
    [Info("Drawing")]
    public class TestTextureAtlas : DemoBase
    {

        public override void Load()
        {

            //----------------------------------------------------
            //test only
            TextureAtlas textureAtlas = new TextureAtlas(800, 600);
            int areaId, x, y;
            for (int i = 0; i < 100; ++i)
            {
                var result2 = textureAtlas.AllocNewRectArea(100, 200, out areaId, out x, out y);
                if (result2 != TextureAtlasAllocResult.Ok)
                {
                }
            }
            //----------------------------------------------------
            FormTestWinGLControl form = new FormTestWinGLControl();
            CanvasGL2d canvas = new CanvasGL2d(this.Width, this.Height);
            form.SetGLPaintHandler((o, s) =>
            {
                canvas.Clear(PixelFarm.Drawing.Color.White);
            });
            form.Show();
        }
    }
}