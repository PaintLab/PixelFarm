using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;


using OpenTK;

namespace OpenTkEssTest
{
    public partial class FormTestBed : Form
    {
        MiniGLControl miniGLControl;
        public FormTestBed()
        {
            InitializeComponent();
            InitMiniGLControl();

        }
        void InitMiniGLControl()
        {
            miniGLControl = new MiniGLControl();
            miniGLControl.Width = 800;
            miniGLControl.Height = 600;
            this.Controls.Add(miniGLControl);
        }
    }
}
