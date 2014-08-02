using System;
using System.Collections.Generic;
using System.ComponentModel;

using System.Drawing;

using System.Text;
using System.Windows.Forms;
using MatterHackers.Agg;
using MatterHackers.Agg.Image;

namespace Mini
{
    partial class FormTestBed1 : Form
    {
        ExampleBase exampleBase;
        List<ExampleConfigDesc> configList;
        public FormTestBed1()
        {
            InitializeComponent();
        }
        public void LoadExample(ExampleAndDesc exAndDesc)
        {
            ExampleBase exBase = Activator.CreateInstance(exAndDesc.Type) as ExampleBase;

            if (exBase != null)
            {
                this.exampleBase = exBase;
                exampleBase.Init();
                this.softAggControl2.LoadExample(exampleBase);

                //-------------------------------------------
                this.configList = exAndDesc.GetConfigList();
                if (configList != null)
                {
                    int j = configList.Count;
                    for (int i = 0; i < j; ++i)
                    {
                        ExampleConfigDesc config = configList[i];
                        switch (config.PresentaionHint)
                        {
                            case PresentaionHint.CheckBox:
                                {
                                    CheckBox checkBox = new CheckBox();
                                    checkBox.Text = config.Name;
                                    checkBox.Width = 400;

                                    bool currentValue = (bool)config.InvokeGet(exampleBase);
                                    checkBox.Checked = currentValue; 

                                    checkBox.CheckedChanged += (s, e) =>
                                    {
                                        config.InvokeSet(exBase, checkBox.Checked);
                                        this.softAggControl2.Invalidate();
                                    };

                                    this.flowLayoutPanel1.Controls.Add(checkBox);
                                } break;
                            case PresentaionHint.SlideBar:
                                {



                                } break;
                            case PresentaionHint.TextBox:
                                {
                                    Label descLabel = new Label();
                                    descLabel.Width = 400;
                                    descLabel.Text = config.Name;

                                    this.flowLayoutPanel1.Controls.Add(descLabel);
                                    TextBox textBox = new TextBox();
                                    textBox.Width = 400;
                                    this.flowLayoutPanel1.Controls.Add(textBox);

                                } break;
                        }

                    }

                }
            }
        }

    }
}
