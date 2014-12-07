using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

using PixelFarm.Agg;
using LayoutFarm.DrawingGL;
namespace Mini2
{
    [Info(OrderCode = "04")]
    [Info("LayoutFarmDrawDemo")]
    public class LayoutFarmDrawDemo : DemoBase
    {
        public override void Load()
        {
            //-----------------------------------------------
            //test draw with LayoutFarm.Canvas interface ***
            //with OpenGL backend ***
            //-----------------------------------------------
            FormTestWinGLControl form = new FormTestWinGLControl();
            form.SetGLPaintHandler((o, s) =>
            {



            });
            form.Show();
        }
    }
}