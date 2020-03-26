// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// (from https://github.com/wieslawsoltes/ColorBlender)
//MIT, 2017-present, WinterDev

using System;
using LayoutFarm.CustomWidgets;
//
using PaintLab.ColorBlender;

namespace LayoutFarm.ColorBlenderSample
{
    [DemoNote("1.13 ColorBlenderExample")]
    class DemoColorBlender : App
    {
        ColorMatch _colorMatch;
        Box _r_sampleBox, _g_sampleBox, _b_sampleBox;
        Box[] _rgb_varBoxes;
        Box[] _hsv_varBoxes;
        Box[] _swatch_Boxes;

        //
        Box _pure_rgbBox;

        ScrollBar _r_sc, _g_sc, _b_sc;
        ListView _lstvw_blendAlgo;
        IAlgorithm _blenderAlgo;
        protected override void OnStart(AppHost host)
        {

            _colorMatch = new ColorMatch();
            _colorMatch.VariationsRGB = new RGB[7];
            _colorMatch.VariationsHSV = new RGB[9];
            _blenderAlgo = _colorMatch.Algorithms[0];
            //

            {
                _lstvw_blendAlgo = new ListView(200, 400);
                _lstvw_blendAlgo.SetLocation(500, 20);
                host.AddChild(_lstvw_blendAlgo);
                _lstvw_blendAlgo.ListItemMouseEvent += (s, e) =>
                {
                    if (_lstvw_blendAlgo.SelectedIndex > -1)
                    {
                        _blenderAlgo = _colorMatch.Algorithms[_lstvw_blendAlgo.SelectedIndex];
                        UpdateAllComponents();
                    }
                };

                //add item
                foreach (IAlgorithm algo in _colorMatch.Algorithms)
                {
                    ListItem listItem = new ListItem(200, 20);
                    listItem.Text = algo.GetType().Name;
                    listItem.Tag = algo;
                    _lstvw_blendAlgo.AddItem(listItem);
                }
            }

            //start RGB value
            byte r_value = 200;
            byte g_value = 46;
            byte b_value = 49;


            CreateRBGVarBoxes(host, 20, 250);
            CreateHsvVarBoxes(host, 20, 300);
            CreateSwatchBoxes(host, 20, 350);

            {
                _pure_rgbBox = new Box(50, 50);
                _pure_rgbBox.BackColor = new PixelFarm.Drawing.Color(
                    (byte)r_value,
                    (byte)b_value,
                    (byte)g_value);
                _pure_rgbBox.SetLocation(0, 0);
                host.AddChild(_pure_rgbBox);
            }

            //R
            {

                CreateRBGScrollBarAndSampleColorBox(80, 80, out _r_sc, out _r_sampleBox, (n_scrollBar, n_sampleBox) =>
                {
                    if (_component_ready)
                    {
                        n_sampleBox.BackColor = new PixelFarm.Drawing.Color((byte)(n_scrollBar.ScrollValue / 10), 0, 0);
                        UpdateAllComponents();
                    }

                });
                host.AddChild(_r_sc);
                host.AddChild(_r_sampleBox);
            }
            //G 
            {

                CreateRBGScrollBarAndSampleColorBox(80, 120, out _g_sc, out _g_sampleBox, (n_scrollBar, n_sampleBox) =>
                {
                    if (_component_ready)
                    {
                        n_sampleBox.BackColor = new PixelFarm.Drawing.Color(0, (byte)(n_scrollBar.ScrollValue / 10), 0);
                        UpdateAllComponents();
                    }
                });
                host.AddChild(_g_sc);
                host.AddChild(_g_sampleBox);
            }
            //B
            {
                CreateRBGScrollBarAndSampleColorBox(80, 160, out _b_sc, out _b_sampleBox, (n_scrollBar, n_sampleBox) =>
                {
                    if (_component_ready)
                    {
                        n_sampleBox.BackColor = new PixelFarm.Drawing.Color(0, 0, (byte)(n_scrollBar.ScrollValue / 10));
                        UpdateAllComponents();
                    }
                });
                host.AddChild(_b_sc);
                host.AddChild(_b_sampleBox);
            }
            _component_ready = true;

            //---------
            CreateChromaTestButtons(host, 20, 450);
        }
        void CreateChromaTestButtons(AppHost host, int x, int y)
        {

            void ShowColorBoxs(Box colorBoxPanel, PixelFarm.Drawing.Color[] colors)
            {
                //nested method
                colorBoxPanel.ClearChildren();
                for (int i = 0; i < colors.Length; ++i)
                {
                    Box colorBox = new Box(30, 30);

                    PixelFarm.Drawing.Color c = colors[i];
                    colorBox.BackColor = new PixelFarm.Drawing.Color(c.R, c.G, c.B);
                    colorBoxPanel.Add(colorBox);
                }
                colorBoxPanel.PerformContentLayout();
            }

            Box colorPanel = new Box(200, 40);
            colorPanel.ContentLayoutKind = BoxContentLayoutKind.HorizontalStack;
            colorPanel.BackColor = PixelFarm.Drawing.Color.White;
            colorPanel.SetLocation(x, y);
            host.AddChild(colorPanel);

            y += colorPanel.Height;


            //test1...
            var buttonBeh = new UI.GeneralUIElementBehaviour();
            buttonBeh.MouseMove += (b, e) =>
            {
                if (e.CurrentContextElement is Label lbl)
                {
                    lbl.BackColor = PixelFarm.Drawing.Color.Yellow;
                }
            };
            buttonBeh.MouseLeave += (b, e) =>
            {
                if (e.CurrentContextElement is Label lbl)
                {
                    lbl.BackColor = PixelFarm.Drawing.KnownColors.Gray;
                }
            };
            //----------------------------------
            //test2


            //----------------------------------
            {
                Label lblChromaDarken = new Label(50, 20);
                lblChromaDarken.BackColor = PixelFarm.Drawing.KnownColors.Gray;
                lblChromaDarken.Text = "Darken";
                lblChromaDarken.SetLocation(x, y);
                lblChromaDarken.AttachUIBehaviour(buttonBeh);

                UI.GeneralEventListener evListener = new UI.GeneralEventListener();
                evListener.MouseDown += e =>
                {
                    PixelFarm.Drawing.Color color = PixelFarm.Drawing.KnownColors.DeepPink;
                    PaintLab.ChromaJs.Chroma chroma = new PaintLab.ChromaJs.Chroma(color);

                    PixelFarm.Drawing.Color[] colors = new[] {
                        color,
                        chroma.Darken() ,
                        chroma.Darken(2),
                        chroma.Darken(2.6)
                    };
                    //present in the box                     
                    ShowColorBoxs(colorPanel, colors);
                };
                lblChromaDarken.AttachExternalEventListener(evListener);
                x += lblChromaDarken.Width + 5;

                host.AddChild(lblChromaDarken);
            }

            //----------------------------------
            {
                Label lblLighten = new Label(50, 20);
                lblLighten.AttachUIBehaviour(buttonBeh);

                lblLighten.Text = "Brighten";
                lblLighten.SetLocation(x, y);
                {
                    UI.GeneralEventListener evListener = new UI.GeneralEventListener();
                    evListener.MouseDown += e =>
                    {
                        PixelFarm.Drawing.Color color = PixelFarm.Drawing.KnownColors.DeepPink;
                        PaintLab.ChromaJs.Chroma chroma = new PaintLab.ChromaJs.Chroma(color);

                        PixelFarm.Drawing.Color[] colors = new[] {
                            color,
                            chroma.Brighten(),
                            chroma.Brighten(2),
                            chroma.Brighten(3)
                        };
                        //present in the box                     
                        ShowColorBoxs(colorPanel, colors);
                    };
                    lblLighten.AttachExternalEventListener(evListener);
                }
                x += lblLighten.Width + 5;

                host.AddChild(lblLighten);
            }
        }



        void CreateRBGVarBoxes(AppHost host, int x, int y)
        {
            _rgb_varBoxes = new Box[7];
            for (int i = 0; i < 7; ++i)
            {
                Box rgb_varBox = new Box(40, 40);
                rgb_varBox.SetLocation(x + (i * 40), y);
                _rgb_varBoxes[i] = rgb_varBox;
                host.AddChild(rgb_varBox);
            }
        }
        void CreateSwatchBoxes(AppHost host, int x, int y)
        {
            _swatch_Boxes = new Box[6];
            for (int i = 0; i < 6; ++i)
            {
                Box swatchBox = new Box(40, 40);
                swatchBox.SetLocation(x + (i * 40), y);
                _swatch_Boxes[i] = swatchBox;
                host.AddChild(swatchBox);
            }
        }
        void CreateHsvVarBoxes(AppHost host, int x, int y)
        {
            _hsv_varBoxes = new Box[9];
            for (int i = 0; i < 9; ++i)
            {
                Box hsv_varBox = new Box(40, 40);
                hsv_varBox.SetLocation(x + (i * 40), y);
                _hsv_varBoxes[i] = hsv_varBox;
                host.AddChild(hsv_varBox);
            }
        }
        void CreateRBGScrollBarAndSampleColorBox(
           int x, int y,
           out ScrollBar scBar,
           out Box sampleBox,
           Action<ScrollBar, Box> pairAction
           )
        {
            //Action<>
            //horizontal scrollbar
            scBar = new LayoutFarm.CustomWidgets.ScrollBar(300, 15);

            //TODO: add mx with layout engine
            scBar.ScrollBarType = CustomWidgets.ScrollBarType.Horizontal;
            scBar.SetLocation(x, y);
            scBar.MinValue = 0;
            scBar.MaxValue = 255 * 10;
            scBar.SmallChange = 1;
            //
            scBar.ScrollValue = 0;//init
                                  // 
            sampleBox = new Box(30, 30);
            sampleBox.SetLocation(x + 350, y);
            // 
            var n_scBar = scBar;
            var n_sampleBox = sampleBox;
            scBar.SliderBox.UserScroll += (s, e) => pairAction(n_scBar, n_sampleBox);

            pairAction(n_scBar, n_sampleBox);
        }
        bool _component_ready = false;

        void UpdateAllComponents()
        {
            byte r = (byte)(_r_sc.ScrollValue / 10);
            byte g = (byte)(_g_sc.ScrollValue / 10);
            byte b = (byte)(_b_sc.ScrollValue / 10);

            _pure_rgbBox.BackColor = new PixelFarm.Drawing.Color(r, g, b);

            //the update ColorMatch
            _colorMatch.CurrentAlgorithm = _blenderAlgo;
            _colorMatch.CurrentRGB = new RGB(r, g, b);
            _colorMatch.CurrentHSV = _colorMatch.CurrentRGB.ToHSV();
            _colorMatch.CurrentRGB = _colorMatch.CurrentHSV.ToRGB();//?
            _colorMatch.Update();
            //then present color match results
            //1. rgb variants
            for (int i = 0; i < 7; ++i)
            {
                _rgb_varBoxes[i].BackColor = _colorMatch.VariationsRGB[i].ToPixelFarmColor();
            }
            //2. hsv variants
            for (int i = 0; i < 9; ++i)
            {
                _hsv_varBoxes[i].BackColor = _colorMatch.VariationsHSV[i].ToPixelFarmColor();
            }
            //3. swatch box
            Blend blend = _colorMatch.CurrentBlend;
            for (int i = 0; i < 6; ++i)
            {
                _swatch_Boxes[i].BackColor = blend.Colors[i].ToRGB().ToPixelFarmColor();
            }

        }

    }

    static class ColorBlenderToPixelFarmExtensions
    {
        public static PixelFarm.Drawing.Color ToPixelFarmColor(this RGB rgbColor)
        {
            return new PixelFarm.Drawing.Color((byte)rgbColor.R, (byte)rgbColor.G, (byte)rgbColor.B);
        }
    }

    static class ListViewItemExtensions
    {
        public static void AddItem(this ListView lstView, string text)
        {
            ListItem listItem = new ListItem(lstView.Width, 20);
            listItem.Text = text;
            lstView.AddItem(listItem);
        }
    }

}