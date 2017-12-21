//MIT, 2014-2017, WinterDev

using System.Windows.Forms;
using OpenTK;

namespace Mini
{
    partial class FormGLTest : Form
    {
        MyGLControl miniGLControl;
        MyGLControl miniGLControl2;

        public FormGLTest()
        {
            InitializeComponent();
        }

        public MyGLControl InitMiniGLControl(int w, int h)
        {
            if (miniGLControl == null)
            {
                miniGLControl = new MyGLControl();
                miniGLControl.Width = w;
                miniGLControl.Height = h;
                this.Controls.Add(miniGLControl);
            }
            return miniGLControl;
        }
        public MyGLControl InitMiniGLControl2(int w, int h)
        {
            //test more than 1 gl control in the same form

            if (miniGLControl2 == null)
            {
                miniGLControl2 = new MyGLControl();
                miniGLControl2.Width = w;
                miniGLControl2.Height = h;
                miniGLControl2.BackColor = System.Drawing.Color.Black;
                miniGLControl2.Top = 0;
                miniGLControl2.Left = miniGLControl.Right;
                this.Controls.Add(miniGLControl2);
            }
            return miniGLControl2;
        }
    }
}
