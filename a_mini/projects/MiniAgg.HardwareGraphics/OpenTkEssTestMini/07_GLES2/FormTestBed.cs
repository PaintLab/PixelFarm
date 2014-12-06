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
        DerivedMiniGLControl miniGLControl;
        public FormTestBed()
        {
            InitializeComponent();
            InitMiniGLControl();

        }
        void InitMiniGLControl()
        {
            miniGLControl = new DerivedMiniGLControl();
            miniGLControl.Width = 800;
            miniGLControl.Height = 600;
            miniGLControl.ClearColor = LayoutFarm.Drawing.Color.Blue;
            this.Controls.Add(miniGLControl);
        }
        public DerivedMiniGLControl MiniGLControl
        {
            get { return this.miniGLControl; }
        }
         

    }
}
