using System;

using System.Windows.Forms;

using LayoutFarm.Text;
namespace LayoutFarm.TestTextFlow
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            EditableTextFlowLayer flowLayer = new EditableTextFlowLayer();
            EditableTextLine line1 = new EditableTextLine(flowLayer);


        }
    }
}
