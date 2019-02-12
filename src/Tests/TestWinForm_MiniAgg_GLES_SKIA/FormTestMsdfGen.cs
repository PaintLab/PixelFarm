//MIT, 2019-present, WinterDev
using System;
using System.Collections.Generic;
using System.Drawing; 
using System.IO;
using System.Windows.Forms;

using Typography.OpenFont;
using Typography.Rendering;
using Typography.Contours;

using PixelFarm.Drawing;
using PixelFarm.CpuBlit;
using PixelFarm.CpuBlit.VertexProcessing;

namespace Mini
{
    public partial class FormTestMsdfGen : Form
    {
        public FormTestMsdfGen()
        {
            InitializeComponent();
        }



        void GetExampleVxs(VertexStore outputVxs)
        {
            //counter-clockwise 
            //a triangle
            //outputVxs.AddMoveTo(10, 20);
            //outputVxs.AddLineTo(50, 60);
            //outputVxs.AddLineTo(70, 20);
            //outputVxs.AddCloseFigure();

            //a quad
            //outputVxs.AddMoveTo(10, 20);
            //outputVxs.AddLineTo(50, 60);
            //outputVxs.AddLineTo(70, 20);
            //outputVxs.AddLineTo(50, 10);
            //outputVxs.AddCloseFigure();



            //curve4
            //outputVxs.AddMoveTo(5, 5);
            //outputVxs.AddLineTo(50, 60);
            //outputVxs.AddCurve4To(70, 20, 50, 10, 10, 5);
            //outputVxs.AddCloseFigure();

            //curve3
            outputVxs.AddMoveTo(5, 5);
            outputVxs.AddLineTo(50, 60);
            outputVxs.AddCurve3To(70, 20, 10, 5);
            outputVxs.AddCloseFigure();


            //a quad with hole
            //outputVxs.AddMoveTo(10, 20);
            //outputVxs.AddLineTo(50, 60);
            //outputVxs.AddLineTo(70, 20);
            //outputVxs.AddLineTo(50, 10);
            //outputVxs.AddCloseFigure();

            //outputVxs.AddMoveTo(30, 30);
            //outputVxs.AddLineTo(40, 30);
            //outputVxs.AddLineTo(40, 35);
            //outputVxs.AddLineTo(30, 35);
            //outputVxs.AddCloseFigure();



        }


        private void button2_Click(object sender, EventArgs e)
        {
            //test fake msdf (this is not real msdf gen)
            //--------------------  
            using (VxsTemp.Borrow(out var v1))
            using (VectorToolBox.Borrow(v1, out PathWriter w))
            {
                //--------
                GetExampleVxs(v1);
                //--------

                ExtMsdfGen.MsdfGen3 gen3 = new ExtMsdfGen.MsdfGen3();
#if DEBUG
                gen3.dbugWriteMsdfTexture = true;
#endif
                gen3.GenerateMsdfTexture(v1); 
            }
        }
    }
}
