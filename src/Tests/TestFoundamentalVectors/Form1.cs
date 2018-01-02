//MIT, 2017-2018, WinterDev
using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace TestFoundamentalVectors
{
    public partial class Form1 : Form
    {
        Graphics g;
        TestCases testCases = new TestCases();
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            g = panel1.CreateGraphics();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //testCases.TestLines(g);
            testCases.TestLineCut(g);
        }
    }


}
