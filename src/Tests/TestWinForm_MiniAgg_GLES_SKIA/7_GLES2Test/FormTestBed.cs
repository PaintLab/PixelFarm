//MIT, 2017-present, WinterDev
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Mini
{
    partial class FormTestBed : Form
    {
        List<ExampleAction> _exampleActionList;
        List<ExampleConfigDesc> _configList;

        LayoutFarm.UI.UISurfaceViewportControl _cpuBlitControl;
        DemoBase _exampleBase;

        public FormTestBed()
        {
            InitializeComponent();

        }

        void InvalidateSampleViewPort()
        {
            _cpuBlitControl?.Invalidate();
        }
        public void SetUISurfaceViewportControl(LayoutFarm.UI.UISurfaceViewportControl cpuBlitControl)
        {
            _cpuBlitControl = cpuBlitControl;
        }
        public Control GetLandingControl()
        {
            return this.splitContainer1.Panel2;
        }

        bool _globalUpdateOtherProperties; //prevent recursive loop while update other presentation properties
        void UpdateOtherPresentationValues()
        {
            _globalUpdateOtherProperties = true; //***

            int j = _configList.Count;
            for (int i = 0; i < j; ++i)
            {
                _configList[i].InvokeUpdatePresentationValue();
            }

            _globalUpdateOtherProperties = false;//***
        }
        public void LoadExample(ExampleAndDesc exAndDesc, DemoBase exBase)
        {


            _exampleBase = exBase;
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
            _configList = exAndDesc.GetConfigList();
            if (_configList != null)
            {
                int j = _configList.Count;
                for (int i = 0; i < j; ++i)
                {
                    ExampleConfigDesc config = _configList[i];
                    switch (config.PresentaionHint)
                    {
                        case DemoConfigPresentaionHint.CheckBox:
                            {
                                CheckBox checkBox = new CheckBox();
                                checkBox.Text = config.Name;
                                checkBox.Width = 400;
                                bool currentValue = (bool)config.InvokeGet(_exampleBase);
                                checkBox.Checked = currentValue;
                                checkBox.CheckedChanged += delegate
                                {
                                    if (!_globalUpdateOtherProperties)
                                    {
                                        config.InvokeSet(_exampleBase, checkBox.Checked);
                                        InvalidateSampleViewPort();
                                    }
                                };
                                config.SetUpdatePresentationValueHandler(delegate
                                {
                                    //get latest current value
                                    checkBox.Checked = (bool)config.InvokeGet(_exampleBase);
                                });

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
                                int value = (int)config.InvokeGet(_exampleBase);
                                hscrollBar.Value = value;
                                //-------------
                                descLabel.Text = config.Name + ":" + hscrollBar.Value;
                                hscrollBar.ValueChanged += delegate
                                {
                                    if (!_globalUpdateOtherProperties)
                                    {
                                        config.InvokeSet(_exampleBase, hscrollBar.Value);
                                        descLabel.Text = config.Name + ":" + hscrollBar.Value;
                                        InvalidateSampleViewPort();
                                    }
                                };
                                config.SetUpdatePresentationValueHandler(delegate
                                {
                                    hscrollBar.Value = (int)config.InvokeGet(_exampleBase);
                                    descLabel.Text = config.Name + ":" + hscrollBar.Value;
                                });

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

                                float doubleValue = ((float)config.InvokeGet(_exampleBase) * 100);
                                hscrollBar.Value = (int)doubleValue;
                                //-------------
                                descLabel.Text = config.Name + ":" + ((float)hscrollBar.Value / 100d).ToString();
                                hscrollBar.ValueChanged += delegate
                                {
                                    if (!_globalUpdateOtherProperties)
                                    {
                                        float value = (float)(hscrollBar.Value / 100f);
                                        config.InvokeSet(_exampleBase, value);
                                        descLabel.Text = config.Name + ":" + value.ToString();
                                        InvalidateSampleViewPort();
                                    }
                                };
                                config.SetUpdatePresentationValueHandler(delegate
                                {
                                    hscrollBar.Value = (int)(((float)config.InvokeGet(_exampleBase) * 100));
                                    descLabel.Text = config.Name + ":" + ((float)hscrollBar.Value / 100d).ToString();
                                });

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

                                double doubleValue = ((double)config.InvokeGet(_exampleBase) * 100);
                                hscrollBar.Value = (int)doubleValue;
                                //-------------
                                descLabel.Text = config.Name + ":" + ((double)hscrollBar.Value / 100d).ToString();
                                hscrollBar.ValueChanged += delegate
                                {
                                    if (!_globalUpdateOtherProperties)
                                    {
                                        double value = (double)hscrollBar.Value / 100d;
                                        config.InvokeSet(_exampleBase, value);
                                        descLabel.Text = config.Name + ":" + value.ToString();
                                        InvalidateSampleViewPort();
                                    }
                                };
                                config.SetUpdatePresentationValueHandler(delegate
                                {
                                    hscrollBar.Value = (int)(((double)config.InvokeGet(_exampleBase) * 100));
                                    descLabel.Text = config.Name + ":" + ((double)hscrollBar.Value / 100d).ToString();
                                });

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
                                int currentValue = (int)config.InvokeGet(_exampleBase);

                                Label descLabel = new Label();
                                descLabel.Width = 400;
                                descLabel.Text = config.Name + ":";
                                panelOption.Controls.Add(descLabel);
                                totalHeight += descLabel.Height;

                                List<RadioButton> radioButtons = new List<RadioButton>();
                                for (int n = 0; n < m; ++n)
                                {
                                    ExampleConfigValue ofield = optionFields[n];
                                    RadioButton radio = new RadioButton();

                                    radioButtons.Add(radio);

                                    panelOption.Controls.Add(radio);
                                    radio.Text = ofield.Name;
                                    radio.Width = 400;
                                    radio.Checked = ofield.ValueAsInt32 == currentValue;
                                    radio.Click += delegate
                                    {
                                        if (radio.Checked)
                                        {
                                            ofield.InvokeSet(_exampleBase);
                                            InvalidateSampleViewPort();
                                        }
                                    };

                                    totalHeight += radio.Height + 10;
                                }
                                panelOption.Height = totalHeight;
                                panelOption.FlowDirection = FlowDirection.TopDown;


                                config.SetUpdatePresentationValueHandler(delegate
                                {
                                    int nn = radioButtons.Count;
                                    int currentValue2 = (int)config.InvokeGet(_exampleBase);
                                    for (int n = 0; n < nn; ++n)
                                    {
                                        ExampleConfigValue ofield = optionFields[n];
                                        radioButtons[n].Checked = ofield.ValueAsInt32 == currentValue;
                                    }
                                });

                                this.flowLayoutPanel1.Controls.Add(panelOption);
                            }
                            break;
                        case DemoConfigPresentaionHint.TextBox:
                            {
                                Label descLabel = new Label();
                                descLabel.Width = 400;
                                descLabel.Text = config.Name + ":";
                                this.flowLayoutPanel1.Controls.Add(descLabel);
                                TextBox textBox = new TextBox();
                                textBox.Width = 400;
                                textBox.Text = config.InvokeGet(_exampleBase).ToString();

                                if (config.DataType == typeof(string))
                                {
                                    textBox.TextChanged += delegate
                                    {
                                        if (!_globalUpdateOtherProperties)
                                        {
                                            config.InvokeSet(_exampleBase, textBox.Text);
                                            InvalidateSampleViewPort();
                                        }
                                    };
                                }
                                config.SetUpdatePresentationValueHandler(delegate
                                {
                                    textBox.Text = config.InvokeGet(_exampleBase).ToString();
                                });



                                this.flowLayoutPanel1.Controls.Add(textBox);
                            }
                            break;
                    }
                }
            }

            //--------------------
            _exampleActionList = exAndDesc.GetActionList();
            if (_exampleActionList != null)
            {
                int j = _exampleActionList.Count;
                for (int i = 0; i < j; ++i)
                {
                    ExampleAction exAction = _exampleActionList[i];
                    //present it with simple button
                    Button button = new Button();
                    button.Width = 200;
                    button.Text = exAction.Name;
                    button.Click += delegate
                    {
                        exAction.InvokeMethod(_exampleBase);
                        UpdateOtherPresentationValues();

                        InvalidateSampleViewPort();
                    };
                    this.flowLayoutPanel1.Controls.Add(button);
                }
            }

            //--------------------
        }
    }
}
