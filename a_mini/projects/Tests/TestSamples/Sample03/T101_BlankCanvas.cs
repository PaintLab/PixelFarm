//MIT, 2014-2016,WinterDev

using System;
using Mini;

namespace OpenTkEssTest
{
    [Info(OrderCode = "101")]
    [Info("T101_BlankCanvas")]
    public class T101_BlankCanvas : DemoBase
    {

        protected override void OnReadyForInitGLShaderProgram()
        {

        }
        protected override void DemoClosing()
        {
        }
        protected override void OnGLRender(object sender, EventArgs args)
        {
            SwapBuffers();
        }
    }
}
