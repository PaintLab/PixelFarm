//MIT, 2017-present, WinterDev
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Mini
{
    partial class FormTestBed : Form
    {

        class ExampleConfigs
        {
            public List<ExampleAction> _exampleActionList;
            public List<ExampleConfigDesc> _configList;
            public void UpdateOtherPresentationValues()
            {

                int j = _configList.Count;
                for (int i = 0; i < j; ++i)
                {
                    _configList[i].InvokeUpdatePresentationValue();
                }
            }
        } 

        bool _globalUpdateOtherProperties; //prevent recursive loop while update other presentation properties
        LayoutFarm.UI.GraphicsViewRoot _cpuBlitControl;
        DemoBase _exampleBase;
        ExampleConfigs _mainConfigs;
        List<ExampleConfigs> _allConfigs = new List<ExampleConfigs>();

        public FormTestBed()
        {
            InitializeComponent(); 
        }

        void InvalidateSampleViewPort()
        {
            _cpuBlitControl?.Invalidate();
        }
        public void SetUISurfaceViewportControl(LayoutFarm.UI.GraphicsViewRoot cpuBlitControl)
        {
            _cpuBlitControl = cpuBlitControl;
        }
        public Control GetLandingControl()
        {
            return this.splitContainer1.Panel2;
        }


        void UpdateOtherPresentationValues()
        {
            _globalUpdateOtherProperties = true; //***
            _mainConfigs.UpdateOtherPresentationValues();
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
            _allConfigs.Clear();

            _mainConfigs = new ExampleConfigs();
            _mainConfigs._configList = exAndDesc.GetConfigList();
            _mainConfigs._exampleActionList = exAndDesc.GetActionList();
            _allConfigs.Add(_mainConfigs);

            CreateConfigPresentation(_exampleBase, _mainConfigs, this.flowLayoutPanel1);

            ////--------------------
            var _groupConfigs = exAndDesc.GetGroupConfigList();

            if (_groupConfigs != null)
            {
                int j = _groupConfigs.Count;
                for (int i = 0; i < j; ++i)
                {
                    ExampleGroupConfigDesc groupConfig = _groupConfigs[i];
                    object groupOwner = groupConfig.OwnerProperty.GetGetMethod().Invoke(_exampleBase, null);

                    var subConfig = new ExampleConfigs();

                    subConfig._configList = groupConfig._configList;
                    subConfig._exampleActionList = groupConfig._configActionList;
                    _allConfigs.Add(subConfig);
                    //
                    Panel subPanel = new Panel();
                    subPanel.Width = flowLayoutPanel1.Width;
                    subPanel.Height = 300;//example
                    flowLayoutPanel1.Controls.Add(subPanel);

                    CreateConfigPresentation(groupOwner, subConfig, subPanel); 
                }
            }
        }

        void CreateConfigPresentation(object configOwner, ExampleConfigs owner, Control parentControl)
        {

            var _configList = owner._configList;

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
                                bool currentValue = (bool)config.InvokeGet(configOwner);
                                checkBox.Checked = currentValue;
                                checkBox.CheckedChanged += delegate
                                {
                                    if (!_globalUpdateOtherProperties)
                                    {
                                        config.InvokeSet(configOwner, checkBox.Checked);
                                        InvalidateSampleViewPort();
                                    }
                                };
                                config.SetUpdatePresentationValueHandler(delegate
                                {
                                    //get latest current value
                                    checkBox.Checked = (bool)config.InvokeGet(configOwner);
                                });

                                parentControl.Controls.Add(checkBox);
                            }
                            break;
                        case DemoConfigPresentaionHint.SlideBarDiscrete:
                            {
                                Label descLabel = new Label();
                                descLabel.Width = 400;
                                parentControl.Controls.Add(descLabel);
                                var originalConfig = config.OriginalConfigAttribute;
                                HScrollBar hscrollBar = new HScrollBar();
                                hscrollBar.Width = parentControl.Width;
                                hscrollBar.Minimum = originalConfig.MinValue;
                                hscrollBar.Maximum = originalConfig.MaxValue + 10;
                                hscrollBar.SmallChange = 1;
                                //current value
                                int value = (int)config.InvokeGet(configOwner);
                                hscrollBar.Value = value;
                                //-------------
                                descLabel.Text = config.Name + ":" + hscrollBar.Value;
                                hscrollBar.ValueChanged += delegate
                                {
                                    if (!_globalUpdateOtherProperties)
                                    {
                                        config.InvokeSet(configOwner, hscrollBar.Value);
                                        descLabel.Text = config.Name + ":" + hscrollBar.Value;
                                        InvalidateSampleViewPort();
                                    }
                                };
                                config.SetUpdatePresentationValueHandler(delegate
                                {
                                    hscrollBar.Value = (int)config.InvokeGet(configOwner);
                                    descLabel.Text = config.Name + ":" + hscrollBar.Value;
                                });

                                parentControl.Controls.Add(hscrollBar);
                            }
                            break;
                        case DemoConfigPresentaionHint.SlideBarContinuous_R4:
                            {
                                Label descLabel = new Label();
                                descLabel.Width = 400;
                                parentControl.Controls.Add(descLabel);
                                var originalConfig = config.OriginalConfigAttribute;
                                HScrollBar hscrollBar = new HScrollBar();
                                //100 => for scale factor 

                                hscrollBar.Width = parentControl.Width;
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
                                        config.InvokeSet(configOwner, value);
                                        descLabel.Text = config.Name + ":" + value.ToString();
                                        InvalidateSampleViewPort();
                                    }
                                };
                                config.SetUpdatePresentationValueHandler(delegate
                                {
                                    hscrollBar.Value = (int)(((float)config.InvokeGet(configOwner) * 100));
                                    descLabel.Text = config.Name + ":" + ((float)hscrollBar.Value / 100d).ToString();
                                });

                                parentControl.Controls.Add(hscrollBar);
                            }
                            break;
                        case DemoConfigPresentaionHint.SlideBarContinuous_R8:
                            {
                                Label descLabel = new Label();
                                descLabel.Width = 400;
                                parentControl.Controls.Add(descLabel);
                                var originalConfig = config.OriginalConfigAttribute;
                                HScrollBar hscrollBar = new HScrollBar();
                                //100 => for scale factor 

                                hscrollBar.Width = parentControl.Width;
                                hscrollBar.Minimum = originalConfig.MinValue * 100;
                                hscrollBar.Maximum = (originalConfig.MaxValue * 100) + 10;
                                hscrollBar.SmallChange = 1;
                                //current value

                                double doubleValue = ((double)config.InvokeGet(configOwner) * 100);
                                hscrollBar.Value = (int)doubleValue;
                                //-------------
                                descLabel.Text = config.Name + ":" + ((double)hscrollBar.Value / 100d).ToString();
                                hscrollBar.ValueChanged += delegate
                                {
                                    if (!_globalUpdateOtherProperties)
                                    {
                                        double value = (double)hscrollBar.Value / 100d;
                                        config.InvokeSet(configOwner, value);
                                        descLabel.Text = config.Name + ":" + value.ToString();
                                        InvalidateSampleViewPort();
                                    }
                                };
                                config.SetUpdatePresentationValueHandler(delegate
                                {
                                    hscrollBar.Value = (int)(((double)config.InvokeGet(configOwner) * 100));
                                    descLabel.Text = config.Name + ":" + ((double)hscrollBar.Value / 100d).ToString();
                                });

                                parentControl.Controls.Add(hscrollBar);
                            }
                            break;
                        case DemoConfigPresentaionHint.OptionBoxes:
                            {
                                List<ExampleConfigValue> optionFields = config.GetOptionFields();
                                FlowLayoutPanel panelOption = new FlowLayoutPanel();
                                int totalHeight = 0;
                                int m = optionFields.Count;
                                //current value 
                                int currentValue = (int)config.InvokeGet(configOwner);

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
                                            ofield.InvokeSet(configOwner);
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
                                    int currentValue2 = (int)config.InvokeGet(configOwner);
                                    for (int n = 0; n < nn; ++n)
                                    {
                                        ExampleConfigValue ofield = optionFields[n];
                                        radioButtons[n].Checked = ofield.ValueAsInt32 == currentValue2;
                                    }
                                });

                                parentControl.Controls.Add(panelOption);
                            }
                            break;
                        case DemoConfigPresentaionHint.TextBox:
                            {
                                Label descLabel = new Label();
                                descLabel.Width = 400;
                                descLabel.Text = config.Name + ":";
                                parentControl.Controls.Add(descLabel);
                                TextBox textBox = new TextBox();
                                textBox.Width = 400;
                                textBox.Text = config.InvokeGet(configOwner).ToString();

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
                                    textBox.Text = config.InvokeGet(configOwner).ToString();
                                });
                                parentControl.Controls.Add(textBox);
                            }
                            break;
                        case DemoConfigPresentaionHint.ConfigGroup:
                            {
                                //extract more config data from this class
                                //?

                            }
                            break;
                    }
                }
            }

            //--------------------
            var _exampleActionList = owner._exampleActionList;
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
                        exAction.InvokeMethod(configOwner);
                        UpdateOtherPresentationValues();

                        InvalidateSampleViewPort();
                    };
                    parentControl.Controls.Add(button);
                }
            }
        }
    }
}
