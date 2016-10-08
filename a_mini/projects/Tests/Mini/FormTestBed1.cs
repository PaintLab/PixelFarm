using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using PixelFarm.Agg;
using PixelFarm.Agg.Image;
namespace Mini
{
    partial class FormTestBed1 : Form
    {
        DemoBase exampleBase;
        List<ExampleConfigDesc> configList;
        public FormTestBed1()
        {
            InitializeComponent();
        }
        void InvalidateSampleViewPort()
        {
            this.softAggControl2.Invalidate();
        }
        public bool UseGdiPlus
        {
            get { return softAggControl2.UseGdiPlus; }
            set { softAggControl2.UseGdiPlus = value; }
        }
        public bool UseGdiAntiAlias
        {
            get { return softAggControl2.UseGdiAntiAlias; }
            set { softAggControl2.UseGdiAntiAlias = value; }
        }
        public void LoadExample(ExampleAndDesc exAndDesc)
        {
            DemoBase exBase = Activator.CreateInstance(exAndDesc.Type) as DemoBase;
            if (exBase == null)
            {
                return;
            }

            this.exampleBase = exBase;
            exampleBase.Init();
            this.softAggControl2.LoadExample(exampleBase);
            this.Text = exAndDesc.ToString();
            //-------------------------------------------
            //description:
            if (!string.IsNullOrEmpty(exAndDesc.Description))
            {
                TextBox tt = new TextBox();
                tt.Width = this.flowLayoutPanel1.Width - 5;
                tt.Text = exAndDesc.Description;
                tt.Multiline = true;
                tt.ScrollBars = ScrollBars.Vertical;
                tt.Height = 250;
                tt.BackColor = Color.Gainsboro;
                tt.Font = new Font("tahoma", 10);
                this.flowLayoutPanel1.Controls.Add(tt);
            }
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
                        case DemoConfigPresentaionHint.CheckBox:
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
                            }
                            break;
                        case DemoConfigPresentaionHint.SlideBarDiscrete:
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
                                int value = (int)config.InvokeGet(exampleBase);
                                hscrollBar.Value = value;
                                //-------------
                                descLabel.Text = config.Name + ":" + hscrollBar.Value;
                                hscrollBar.ValueChanged += (s, e) =>
                                {
                                    config.InvokeSet(exampleBase, hscrollBar.Value);
                                    descLabel.Text = config.Name + ":" + hscrollBar.Value;
                                    InvalidateSampleViewPort();
                                };
                                this.flowLayoutPanel1.Controls.Add(hscrollBar);
                            }
                            break;
                        case DemoConfigPresentaionHint.SlideBarContinuous_R4:
                            {
                                Label descLabel = new Label();
                                descLabel.Width = 400;
                                this.flowLayoutPanel1.Controls.Add(descLabel);
                                var originalConfig = config.OriginalConfigAttribute;
                                HScrollBar hscrollBar = new HScrollBar();
                                //100 => for scale factor 

                                hscrollBar.Width = flowLayoutPanel1.Width;
                                hscrollBar.Minimum = originalConfig.MinValue * 100;
                                hscrollBar.Maximum = (originalConfig.MaxValue * 100) + 10;
                                hscrollBar.SmallChange = 1;
                                //current value

                                float doubleValue = ((float)config.InvokeGet(exampleBase) * 100);
                                hscrollBar.Value = (int)doubleValue;
                                //-------------
                                descLabel.Text = config.Name + ":" + ((float)hscrollBar.Value / 100d).ToString();
                                hscrollBar.ValueChanged += (s, e) =>
                                {
                                    float value = (float)(hscrollBar.Value / 100f);
                                    config.InvokeSet(exampleBase, value);
                                    descLabel.Text = config.Name + ":" + value.ToString();
                                    InvalidateSampleViewPort();
                                };
                                this.flowLayoutPanel1.Controls.Add(hscrollBar);
                            }
                            break;
                        case DemoConfigPresentaionHint.SlideBarContinuous_R8:
                            {
                                Label descLabel = new Label();
                                descLabel.Width = 400;
                                this.flowLayoutPanel1.Controls.Add(descLabel);
                                var originalConfig = config.OriginalConfigAttribute;
                                HScrollBar hscrollBar = new HScrollBar();
                                //100 => for scale factor 

                                hscrollBar.Width = flowLayoutPanel1.Width;
                                hscrollBar.Minimum = originalConfig.MinValue * 100;
                                hscrollBar.Maximum = (originalConfig.MaxValue * 100) + 10;
                                hscrollBar.SmallChange = 1;
                                //current value

                                double doubleValue = ((double)config.InvokeGet(exampleBase) * 100);
                                hscrollBar.Value = (int)doubleValue;
                                //-------------
                                descLabel.Text = config.Name + ":" + ((double)hscrollBar.Value / 100d).ToString();
                                hscrollBar.ValueChanged += (s, e) =>
                                {
                                    double value = (double)hscrollBar.Value / 100d;
                                    config.InvokeSet(exampleBase, value);
                                    descLabel.Text = config.Name + ":" + value.ToString();
                                    InvalidateSampleViewPort();
                                };
                                this.flowLayoutPanel1.Controls.Add(hscrollBar);
                            }
                            break;
                        case DemoConfigPresentaionHint.OptionBoxes:
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
                            }
                            break;
                        case DemoConfigPresentaionHint.TextBox:
                            {
                                Label descLabel = new Label();
                                descLabel.Width = 400;
                                descLabel.Text = config.Name;
                                this.flowLayoutPanel1.Controls.Add(descLabel);
                                TextBox textBox = new TextBox();
                                textBox.Width = 400;
                                this.flowLayoutPanel1.Controls.Add(textBox);
                            }
                            break;
                    }
                }
            }
        }
    }
}
