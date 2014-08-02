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
        void InvalidateSampleViewPort()
        {
            this.softAggControl2.Invalidate();
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
                                        InvalidateSampleViewPort();
                                    };

                                    this.flowLayoutPanel1.Controls.Add(checkBox);
                                } break;
                            case PresentaionHint.SlideBar:
                                {


                                    Label descLabel = new Label();
                                    descLabel.Width = 400;

                                    this.flowLayoutPanel1.Controls.Add(descLabel);

                                    var originalConfig = config.OriginalConfigAttribute;
                                    HScrollBar hscrollBar = new HScrollBar();
                                    hscrollBar.Width = flowLayoutPanel1.Width;
                                    hscrollBar.Minimum = originalConfig.MinValue;
                                    hscrollBar.Maximum = originalConfig.MaxValue + 10;
                                    hscrollBar.SmallChange = 1;
                                    //current value
                                    hscrollBar.Value = (int)config.InvokeGet(exampleBase);
                                    //-------------
                                    descLabel.Text = config.Name + ":" + hscrollBar.Value;
                                    hscrollBar.ValueChanged += (s, e) =>
                                    {
                                        config.InvokeSet(exampleBase, hscrollBar.Value);
                                        descLabel.Text = config.Name + ":" + hscrollBar.Value; 
                                        InvalidateSampleViewPort(); 
                                    };

                                    this.flowLayoutPanel1.Controls.Add(hscrollBar);

                                } break;
                            case PresentaionHint.OptionBoxes:
                                {

                                    List<ExampleConfigValue> optionFields = config.GetOptionFields();
                                    FlowLayoutPanel panelOption = new FlowLayoutPanel();
                                    int totalHeight = 0;
                                    int m = optionFields.Count;

                                    //current value 
                                    int currentValue = (int)config.InvokeGet(exampleBase);

                                    for (int n = 0; n < m; ++n)
                                    {

                                        ExampleConfigValue ofield = optionFields[n];

                                        RadioButton radio = new RadioButton();
                                        panelOption.Controls.Add(radio);
                                        radio.Text = ofield.Name;
                                        radio.Width = 400;
                                        radio.Checked = ofield.ValueAsInt32 == currentValue;

                                        radio.Click += (s, e) =>
                                        {
                                            if (radio.Checked)
                                            {
                                                ofield.InvokeSet(this.exampleBase);
                                                InvalidateSampleViewPort();
                                            }
                                        };
                                        totalHeight += radio.Height + 10;
                                    }




                                    panelOption.Height = totalHeight;
                                    panelOption.FlowDirection = FlowDirection.TopDown;

                                    this.flowLayoutPanel1.Controls.Add(panelOption);
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
