//2014 BSD, WinterDev

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

using System.Text;
using System.Windows.Forms;

namespace Mini
{
    public partial class FormDev : Form
    {
        public FormDev()
        {
            InitializeComponent();
            this.Load += new EventHandler(DevForm_Load);
            this.listBox1.DoubleClick += new EventHandler(listBox1_DoubleClick);
        }

        void listBox1_DoubleClick(object sender, EventArgs e)
        {
            //load sample form
            ExampleAndDesc exAndDesc = this.listBox1.SelectedItem as ExampleAndDesc;
            if (exAndDesc != null)
            {
                ExampleBase exBase = Activator.CreateInstance(exAndDesc.Type) as ExampleBase;
                if (exBase != null)
                {
                    FormTestBed1 testBed = new FormTestBed1();
                    testBed.WindowState = FormWindowState.Maximized;
                    testBed.Show();
                    testBed.LoadExample(exBase);
                }
            }

        }
        void DevForm_Load(object sender, EventArgs e)
        {
            //load examples
            Type[] allTypes = this.GetType().Assembly.GetTypes();
            Type exBase = typeof(Mini.ExampleBase);
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
            this.listBox1.Items.Clear();
            j = exlist.Count;
            for (int i = 0; i < j; ++i)
            {
                this.listBox1.Items.Add(exlist[i]);
            }
        }
        class ExampleAndDesc
        {
            public ExampleAndDesc(Type t, string desc)
            {
                this.Type = t;
                this.Desc = desc;
            }
            public Type Type { get; set; }
            public string Desc { get; set; }
            public override string ToString()
            {
                return this.Desc;
            }
        }
    }
}
