// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// (from https://github.com/wieslawsoltes/ColorBlender)
//MIT, 2017, WinterDev

using System;
using ColorBlender;
using ColorBlender.Algorithms;
using LayoutFarm.CustomWidgets;

namespace LayoutFarm.ColorBlenderSample
{
    [DemoNote("1.13 ColorBlenderExample")]
    class DemoColorBlender : DemoBase
    {
        ColorMatch colorMatch;
        SimpleBox r_sampleBox, g_sampleBox, b_sampleBox;
        SimpleBox[] rgb_varBoxes;
        SimpleBox[] hsv_varBoxes;
        SimpleBox[] swatch_Boxes;

        //
        SimpleBox pure_rgbBox;

        ScrollBar r_sc, g_sc, b_sc;
        ListView lstvw_blendAlgo;

        IAlgorithm blenderAlgo;
        protected override void OnStartDemo(SampleViewport viewport)
        {

            colorMatch = new ColorMatch();
            colorMatch.VariationsRGB = new RGB[7];
            colorMatch.VariationsHSV = new RGB[9];
            blenderAlgo = colorMatch.Algorithms[0];
            //

            {
                lstvw_blendAlgo = new ListView(200, 400);
                lstvw_blendAlgo.SetLocation(500, 20);
                viewport.AddContent(lstvw_blendAlgo);
                lstvw_blendAlgo.ListItemMouseEvent += (s, e) =>
                {
                    if (lstvw_blendAlgo.SelectedIndex > -1)
                    {
                        blenderAlgo = colorMatch.Algorithms[lstvw_blendAlgo.SelectedIndex];
                        UpdateAllComponents();
                    }
                };

                //add item
                foreach (IAlgorithm algo in colorMatch.Algorithms)
                {
                    ListItem listItem = new ListItem(200, 20);
                    listItem.Text = algo.GetType().Name;
                    listItem.Tag = algo;
                    lstvw_blendAlgo.AddItem(listItem);
                }
            }

            //start RGB value
            byte r_value = 200;
            byte g_value = 46;
            byte b_value = 49;


            CreateRBGVarBoxes(viewport, 20, 250);
            CreateHsvVarBoxes(viewport, 20, 300);
            CreateSwatchBoxes(viewport, 20, 350);

            {
                pure_rgbBox = new SimpleBox(50, 50);
                pure_rgbBox.BackColor = new PixelFarm.Drawing.Color(
                    (byte)r_value,
                    (byte)b_value,
                    (byte)g_value);
                pure_rgbBox.SetLocation(0, 0);
                viewport.AddContent(pure_rgbBox);
            }

            //R
            {

                CreateRBGScrollBarAndSampleColorBox(80, 80, out r_sc, out r_sampleBox, (n_scrollBar, n_sampleBox) =>
                {
                    if (_component_ready)
                    {
                        n_sampleBox.BackColor = new PixelFarm.Drawing.Color((byte)(n_scrollBar.ScrollValue / 10), 0, 0);
                        UpdateAllComponents();
                    }

                });
                viewport.AddContent(r_sc);
                viewport.AddContent(r_sampleBox);
            }
            //G 
            {

                CreateRBGScrollBarAndSampleColorBox(80, 120, out g_sc, out g_sampleBox, (n_scrollBar, n_sampleBox) =>
                {
                    if (_component_ready)
                    {
                        n_sampleBox.BackColor = new PixelFarm.Drawing.Color(0, (byte)(n_scrollBar.ScrollValue / 10), 0);
                        UpdateAllComponents();
                    }
                });
                viewport.AddContent(g_sc);
                viewport.AddContent(g_sampleBox);
            }
            //B
            {
                CreateRBGScrollBarAndSampleColorBox(80, 160, out b_sc, out b_sampleBox, (n_scrollBar, n_sampleBox) =>
                {
                    if (_component_ready)
                    {
                        n_sampleBox.BackColor = new PixelFarm.Drawing.Color(0, 0, (byte)(n_scrollBar.ScrollValue / 10));
                        UpdateAllComponents();
                    }
                });
                viewport.AddContent(b_sc);
                viewport.AddContent(b_sampleBox);
            }
            _component_ready = true;
        }

        void CreateRBGVarBoxes(SampleViewport viewport, int x, int y)
        {
            rgb_varBoxes = new SimpleBox[7];
            for (int i = 0; i < 7; ++i)
            {
                SimpleBox rgb_varBox = new SimpleBox(40, 40);
                rgb_varBox.SetLocation(x + (i * 40), y);
                rgb_varBoxes[i] = rgb_varBox;
                viewport.AddContent(rgb_varBox);
            }
        }
        void CreateSwatchBoxes(SampleViewport viewport, int x, int y)
        {
            swatch_Boxes = new SimpleBox[6];
            for (int i = 0; i < 6; ++i)
            {
                SimpleBox swatchBox = new SimpleBox(40, 40);
                swatchBox.SetLocation(x + (i * 40), y);
                swatch_Boxes[i] = swatchBox;
                viewport.AddContent(swatchBox);
            }
        }
        void CreateHsvVarBoxes(SampleViewport viewport, int x, int y)
        {
            hsv_varBoxes = new SimpleBox[9];
            for (int i = 0; i < 9; ++i)
            {
                SimpleBox hsv_varBox = new SimpleBox(40, 40);
                hsv_varBox.SetLocation(x + (i * 40), y);
                hsv_varBoxes[i] = hsv_varBox;
                viewport.AddContent(hsv_varBox);
            }
        }
        void CreateRBGScrollBarAndSampleColorBox(
           int x, int y,
           out ScrollBar scBar,
           out SimpleBox sampleBox,
           SimpleAction<ScrollBar, SimpleBox> pairAction
           )
        {
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
            sampleBox = new SimpleBox(30, 30);
            sampleBox.SetLocation(x + 350, y);
            // 
            var n_scBar = scBar;
            var n_sampleBox = sampleBox;
            scBar.UserScroll += (s, e) => pairAction(n_scBar, n_sampleBox);

            pairAction(n_scBar, n_sampleBox);
        }
        bool _component_ready = false;

        void UpdateAllComponents()
        {
            byte r = (byte)(r_sc.ScrollValue / 10);
            byte g = (byte)(g_sc.ScrollValue / 10);
            byte b = (byte)(b_sc.ScrollValue / 10);

            pure_rgbBox.BackColor = new PixelFarm.Drawing.Color(r, g, b);

            //the update ColorMatch
            colorMatch.CurrentAlgorithm = blenderAlgo;
            colorMatch.CurrentRGB = new RGB(r, g, b);
            colorMatch.CurrentHSV = colorMatch.CurrentRGB.ToHSV();
            colorMatch.CurrentRGB = colorMatch.CurrentHSV.ToRGB();//?
            colorMatch.Update();
            //then present color match results
            //1. rgb variants
            for (int i = 0; i < 7; ++i)
            {
                rgb_varBoxes[i].BackColor = colorMatch.VariationsRGB[i].ToPixelFarmColor();
            }
            //2. hsv variants
            for (int i = 0; i < 9; ++i)
            {
                hsv_varBoxes[i].BackColor = colorMatch.VariationsHSV[i].ToPixelFarmColor();
            }
            //3. swatch box
            Blend blend = colorMatch.CurrentBlend;
            for (int i = 0; i < 6; ++i)
            {
                swatch_Boxes[i].BackColor = blend.Colors[i].ToRGB().ToPixelFarmColor();
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