
#region Using Directives

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading;
using System.Drawing;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.ES20;
using Examples.Tutorial;
using Mini;

#endregion

using LayoutFarm.DrawingGL;
namespace OpenTkEssTest
{
    [Info(OrderCode = "102")]
    [Info("T102_BasicDraw")]
    public class T102_BasicDraw : PrebuiltGLControlDemoBase
    {

        CanvasGL2d canvas2d;
        protected override void OnInitGLProgram(object sender, EventArgs args)
        {

            //--------------------------------------------------------------------------------
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.ClearColor(1, 1, 1, 1);
            //setup viewport size
            int max = Math.Max(this.Width, this.Height);

            canvas2d = new CanvasGL2d(max, max);

            //square viewport
            GL.Viewport(0, 0, max, max);
        }
        protected override void DemoClosing()
        {

        }
        protected override void OnGLRender(object sender, EventArgs args)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);

            canvas2d.StrokeColor = LayoutFarm.Drawing.Color.Blue;

            canvas2d.DrawLine(50, 50, 200, 200);

            canvas2d.DrawRect(2, 1, 50, 50);
            canvas2d.FillRect(LayoutFarm.Drawing.Color.Green, 50, 50, 50, 50);
            //--------------------------------------------
            
            canvas2d.DrawCircle(100, 100, 25);
            canvas2d.DrawEllipse(200, 200, 25, 50);
            //--------------------------------------------


            miniGLControl.SwapBuffers();
        }

    }


}
