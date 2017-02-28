
using System.Windows.Forms;
namespace Mini
{
    public partial class FormTestBed : Form
    {
        OpenTK.MyGLControl miniGLControl;
        public FormTestBed()
        {
            InitializeComponent();
        }
        public OpenTK.MyGLControl InitMiniGLControl(int w, int h)
        {
            if (miniGLControl == null)
            {
                miniGLControl = new OpenTK.MyGLControl();
                miniGLControl.Width = w;
                miniGLControl.Height = h;
               // miniGLControl.ClearColor = PixelFarm.Drawing.Color.Blue;
                this.Controls.Add(miniGLControl);
            }
            return miniGLControl;
        }
        public OpenTK.MyGLControl MiniGLControl
        {
            get { return this.miniGLControl; }
        }
    }
}
