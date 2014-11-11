using System;
using System.Collections.Generic;
using System.ComponentModel;
 
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using OpenTK.Graphics.OpenGL;
using Mini;

namespace OpenTkEssTest
{
    [Info(OrderCode = "21")]
    [Info("T21_TestWinGLControl")]
    public class T21_TestWinGLControl : DemoBase
    {
        public override void Init()
        {
            FormTestWinGLControl form = new FormTestWinGLControl();
            form.Show();

        }
    }
    [Info(OrderCode = "22")]
    [Info("T22_DemoWinGLControl")]
    public class T22_FormTestWinGLControlDemo2 : DemoBase
    {
        public override void Init()
        {
            FormGLControlSimple form = new FormGLControlSimple();
            form.Show(); 
        }
    }
    
    [Info(OrderCode = "23")]
    [Info("T23_FormMultipleGLControlsFormDemo")]
    public class T23_FormMultipleGLControlsFormDemo : DemoBase
    {
        public override void Init()
        {
            FormMultipleGLControlsForm form = new FormMultipleGLControlsForm();
            form.Show();
        }
    }

}