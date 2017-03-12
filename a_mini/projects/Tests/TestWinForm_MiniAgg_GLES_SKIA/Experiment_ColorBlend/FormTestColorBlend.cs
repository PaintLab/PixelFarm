//MIT, 2017, WinterDev
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Mini
{
    public partial class FormTestColorBlend : Form
    {

        public FormTestColorBlend()
        {
            InitializeComponent();


        }

        private void FormTestColorBlend_Load(object sender, EventArgs e)
        {
            this.colorCompoBox1.SetColor(System.Drawing.Color.FromArgb(255, 125, 125, 125));
        }
    }
}
