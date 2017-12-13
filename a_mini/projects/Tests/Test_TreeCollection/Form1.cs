//MIT, 2017, WinterDev
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using PixelFarm.TreeCollection;

namespace Test_TreeCollection
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //test segment tree, 
            //with overlapped segment ...
            TreeSegment t1 = new TreeSegment(0, 10);
            TreeSegment t2 = new TreeSegment(8, 20);
            SegmentTree<TreeSegment> tree1 = new SegmentTree<TreeSegment>();
            tree1.Add(t1);
            tree1.Add(t2);
            foreach (var seg in tree1.GetSegmentsAt(9))
            {

            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            RedBlackTreeTests tests = new RedBlackTreeTests();
            tests.TestAddBug();
            tests.TestRemoveBug();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            HeightTests heightTest = new HeightTests();
            heightTest.Setup();
            heightTest.TestHeightChanged();
        }
    }
}
