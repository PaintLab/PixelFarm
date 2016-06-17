using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
namespace Mini2
{
    public partial class FormDev : Form
    {
        public FormDev()
        {
            InitializeComponent();
            this.Load += new EventHandler(DevForm_Load);
            this.listBox1.DoubleClick += new EventHandler(listBox1_DoubleClick);
            this.Text = "DevForm: Double Click The Example!";
        }
        void listBox1_DoubleClick(object sender, EventArgs e)
        {
            //load sample form
            ExampleAndDesc exAndDesc = this.listBox1.SelectedItem as ExampleAndDesc;
            if (exAndDesc != null)
            {
                DemoBase demoBaseType = (DemoBase)Activator.CreateInstance(exAndDesc.Type);
                demoBaseType.Load();
            }
        }
        void DevForm_Load(object sender, EventArgs e)
        {
            //load examples
            Type[] allTypes = this.GetType().Assembly.GetTypes();
            Type exBase = typeof(Mini2.DemoBase);
            int j = allTypes.Length;
            List<ExampleAndDesc> exlist = new List<ExampleAndDesc>();
            for (int i = 0; i < j; ++i)
            {
                Type t = allTypes[i];
                if (exBase.IsAssignableFrom(t) && t != exBase)
                {
                    ExampleAndDesc ex = new ExampleAndDesc(t, t.Name);
                    exlist.Add(ex);
                }
            }
            //-------
            exlist.Sort((ex1, ex2) =>
            {
                return ex1.OrderCode.CompareTo(ex2.OrderCode);
            });
            this.listBox1.Items.Clear();
            j = exlist.Count;
            for (int i = 0; i < j; ++i)
            {
                this.listBox1.Items.Add(exlist[i]);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Graphics gfx = panel1.CreateGraphics();
            gfx.Clear(Color.White);
            TestDrawLine(gfx, 50, 50, -100, 70);
        }

        void TestDrawLine(Graphics gfx, float x1, float y1, float x2, float y2)
        {
            //line
            gfx.DrawLine(Pens.Black, x1, y1, x2, y2);
            //surrounding area
            double atan = Math.Atan((y2 - y1) / (x2 - x1));
            double sin_ = Math.Sin(atan);
            double cos_ = Math.Cos(atan);
            //vec4 vec_delta1 = vec4(0.01 * sin_angle, 0.01 * cos_angle, 0.0, 0.0);

            float w = 10;
            double nx = x1 - (w * sin_);
            double ny = y1 + (w * cos_);
            gfx.DrawLine(Pens.Red, (float)x1, (float)y1, (float)nx, (float)ny);
            nx = x1 + (w * sin_);
            ny = y1 - (w * cos_);
            gfx.DrawLine(Pens.Red, (float)x1, (float)y1, (float)nx, (float)ny);
            //----------------------------------------------------------------------

            nx = x2 - (w * sin_);
            ny = y2 + (w * cos_);
            gfx.DrawLine(Pens.Blue, (float)x2, (float)y2, (float)nx, (float)ny);
            nx = x2 + (w * sin_);
            ny = y2 - (w * cos_);
            gfx.DrawLine(Pens.Blue, (float)x2, (float)y2, (float)nx, (float)ny);
        }
    }
}
