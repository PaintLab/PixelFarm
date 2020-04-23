//MIT, 2020, WinterDev
using System;
using System.IO;
using LayoutFarm.CustomWidgets;
// 
using System.Collections.Generic;

namespace LayoutFarm.ColorBlenderSample
{

    class ColorBox : AbstractControlBox
    {
        string _note;
        public ColorBox(int w, int h) : base(w, h)
        {

        }
        public void SetColor(PixelFarm.Drawing.Color color)
        {

        }
        public string Note
        {
            get => _note;
            set => _note = value;
        }

    }

    class ColorSet
    {
        public List<ColorBox> colors = new List<ColorBox>();
        public string Name { get; set; }
        public override string ToString() => Name;
    }

    static class ColorUtil
    {
        internal static PixelFarm.Drawing.Color ParseHexRGBColor(string hexRGBString)
        {
            if (hexRGBString.StartsWith("#"))
            {
                //parse next 3 bytes of hex for rgb
                string onlyHex = hexRGBString.Substring(1);
                int intValue = int.Parse(onlyHex, System.Globalization.NumberStyles.HexNumber);
                return PixelFarm.Drawing.Color.FromArgb((intValue >> 16) & 0xff, (intValue >> 8) & 0xff, intValue & 0xff);
            }
            else
            {
                throw new NotSupportedException();
            }
        }
    }

    class GoogleMaterialDzColorSetParser
    {
        public Dictionary<string, ColorSet> _googleMaterialDzColors = new Dictionary<string, ColorSet>();
        public void Read()
        {
            //# from https://material.io/design/color/the-color-system.html#tools-for-picking-colors 
            //Red
            //50 #FFEBEE
            //100 #FFCDD2
            //200 #EF9A9A
            //300 #E57373
            //400 #EF5350
            //500 #F44336
            //600 #E53935
            //700 #D32F2F
            //800 #C62828
            //900 #B71C1C
            //A100 #FF8A80
            //A200 #FF5252
            //A400 #FF1744
            //A700 #D50000
            List<ColorBox> currentColorSet = null;
            string currentSetName = null;

            using (FileStream fs = new FileStream("Data\\GoogleMaterialDzColor.txt", FileMode.Open))
            using (StreamReader reader = new StreamReader(fs))
            {
                //simple parse the text file
                string line = reader.ReadLine();
                while (line != null)
                {

                    line = line.Trim();
                    if (line.Length > 0 && !line.StartsWith("#"))
                    {

                        //read first char of line
                        char c0 = line[0];
                        if (c0 == '%')
                        {
                            ColorSet colorSet = new ColorSet();
                            colorSet.Name = currentSetName = line.Substring(1);
                            currentColorSet = colorSet.colors;

                            _googleMaterialDzColors.Add(currentSetName = line.Substring(1), colorSet);

                        }
                        else
                        {
                            string[] parts = line.Split(' ');
                            ColorBox box = new ColorBox(30, 30);
                            box.Note = currentSetName + " " + parts[0] + "," + parts[1];
                            currentColorSet.Add(box);
                            //parse hex color value
                            box.SetColor(ColorUtil.ParseHexRGBColor(parts[1]));
                        }
                    }

                    line = reader.ReadLine();
                }
            }
        }
    }
    class ColorBrewDataParser
    {

        public Dictionary<string, ColorSet> _colorBrewColorSets = new Dictionary<string, ColorSet>();

        public void Read()
        {
            string currentSetName = null;

            using (FileStream fs = new FileStream("Data\\colorbrewer.txt", FileMode.Open))
            using (StreamReader reader = new StreamReader(fs))
            {
                //simple parse the text file
                string line = reader.ReadLine();
                char[] seps = new char[] { '\"', ',' };

                while (line != null)
                {

                    line = line.Trim();

                    if (line.Length > 0 && !line.StartsWith("/"))
                    {
                        //read first char of line
                        if (line.Contains("{"))
                        {
                            int colonIndex = line.IndexOf(':');
                            if (colonIndex > -1)
                            {
                                //start new set
                                int first = line.IndexOf(',');
                                if (first < colonIndex)
                                {
                                    currentSetName = line.Substring(first + 1, colonIndex - (first + 1));
                                }
                                else
                                {
                                    throw new NotSupportedException();
                                }
                            }
                        }
                        else
                        {
                            //other line should be color subset of current SetName
                            //{,YlGn: {
                            //3: ["#f7fcb9","#addd8e","#31a354"],
                            //4: ["#ffffcc","#c2e699","#78c679","#238443"],
                            //5: ["#ffffcc","#c2e699","#78c679","#31a354","#006837"],
                            //6: ["#ffffcc","#d9f0a3","#addd8e","#78c679","#31a354","#006837"],
                            //7: ["#ffffcc","#d9f0a3","#addd8e","#78c679","#41ab5d","#238443","#005a32"],
                            //8: ["#ffffe5","#f7fcb9","#d9f0a3","#addd8e","#78c679","#41ab5d","#238443","#005a32"],
                            //9: ["#ffffe5","#f7fcb9","#d9f0a3","#addd8e","#78c679","#41ab5d","#238443","#006837","#004529"]
                            int pos1 = line.IndexOf('[');

                            if (pos1 > -1)
                            {
                                int pos2 = line.IndexOf(']', pos1);
                                if (pos2 > -1)
                                {
                                    //split string inside this
                                    string[] hex_colors = line.Substring(pos1 + 1, pos2 - (pos1 + 1)).Split(',');
                                    ColorSet colorSet = new ColorSet();
                                    colorSet.Name = currentSetName + "-" + hex_colors.Length;

                                    List<ColorBox> colorSets = colorSet.colors;
                                    foreach (string color in hex_colors)
                                    {
                                        ColorBox colorBox = new ColorBox(30, 30);
                                        string value = color.Substring(1, color.Length - 2);
                                        colorBox.BackColor = ColorUtil.ParseHexRGBColor(value);
                                        colorBox.Note = currentSetName + "-" + hex_colors.Length + "," + value;
                                        colorSets.Add(colorBox);
                                    }

                                    _colorBrewColorSets.Add(currentSetName + "-" + hex_colors.Length, colorSet);
                                }
                            }
                        }

                    }
                    line = reader.ReadLine();
                }
            }
        }
    }
}