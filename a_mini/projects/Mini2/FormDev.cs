using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

using PixelFarm.Agg;
using PixelFarm.DrawingGL;
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
 



    }
}
