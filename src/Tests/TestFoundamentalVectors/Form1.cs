//MIT, 2017-present, WinterDev
using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace TestFoundamentalVectors
{
    public partial class Form1 : Form
    {
        Graphics _g;
        TestCases _testCases = new TestCases();
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _g = panel1.CreateGraphics();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //testCases.TestLines(g);
            _testCases.TestLineCut(_g);
        }
    }


}
