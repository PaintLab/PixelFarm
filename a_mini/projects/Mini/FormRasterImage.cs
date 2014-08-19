

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Mini
{
    public partial class FormRasterImage : Form
    {
        List<PointControl> controls = new List<PointControl>();

        public FormRasterImage()
        {
            InitializeComponent();

            this.Load += new EventHandler(FormRasterImage_Load);
        }

        void FormRasterImage_Load(object sender, EventArgs e)
        {
            //create control point 


        }




        class PointControl : Panel
        {

            public PointControl()
            {
            }
        }

    }


}
