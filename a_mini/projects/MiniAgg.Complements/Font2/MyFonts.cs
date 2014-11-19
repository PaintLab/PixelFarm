//MIT 2014,WinterDev

//-----------------------------------
//use FreeType and HarfBuzz wrapper
//native dll lib
//plan?: port  them to C#  :)
//-----------------------------------

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

namespace PixelFarm.Font2
{

    public class MyFontGlyph
    {
        public int contourCount;
        public int pointCount;
        public List<PixelFarm.VectorMath.Vector2> points;
    }

    class FontFile
    {
        IntPtr unmanagedMem;
        public FontFile(IntPtr unmanagedMem)
        {
            this.unmanagedMem = unmanagedMem;
        }
    }


    public static class MyFonts
    {
        static Dictionary<string, FontFile> fonts = new Dictionary<string, FontFile>();
        public static int InitLib()
        {
            return NativeMyFonts.MyFtInitLib();
        }
        public static void GetGlyph(char unicodeChar)
        {
            //--------------------------------------------------
            unsafe
            {
                FT_Outline outline = new FT_Outline(); 
                PixelFarm.Font2.NativeMyFonts.MyFtLoadChar(unicodeChar, ref outline);
                //then fetch outline data from ft_outline          
                MyFontGlyph fontGlyph = new MyFontGlyph(); 
                //------------------------------
                int npoints = outline.n_points;
                List<PixelFarm.VectorMath.Vector2> points = new List<PixelFarm.VectorMath.Vector2>(npoints);
                fontGlyph.points = points;
                int startContour = 0;
                int cpoint_index = 0;
                int todoContourCount = outline.n_contours;

                while (todoContourCount > 0)
                {
                    int nextContour = outline.contours[startContour] + 1;
                    for (; cpoint_index < nextContour; ++cpoint_index)
                    {
                        FT_Vector v = outline.points[cpoint_index];
                         

                        points.Add(new PixelFarm.VectorMath.Vector2(v.x, v.y));
                    }
                    startContour++;
                    todoContourCount--;
                }
            }
        }

        public static void SetShapingEngine()
        {
            PixelFarm.Font2.NativeMyFonts.MyFtSetupShapingEngine("en", 2, 1);
        }
        public static void ShapeText(string data)
        {
            ShapeText(data.ToCharArray());
        }
        public static void ShapeText(char[] data)
        {
            byte[] unicodeBuffer = Encoding.Unicode.GetBytes(data);
            unsafe
            {
                fixed (byte* u = &unicodeBuffer[0])
                {
                    PixelFarm.Font2.NativeMyFonts.MyFtShaping(u, 2);
                }
            }
        }
        public static void LoadFont(string filename, int pixelSize)
        {
            //load font from specific file 
            FontFile fontFile;
            if (!fonts.TryGetValue(filename, out fontFile))
            {

                //if not found
                //then load it
                byte[] fontFileContent = File.ReadAllBytes(filename);
                int fileContent = fontFileContent.Length;
                IntPtr unmanagedMem = Marshal.AllocHGlobal(fileContent);
                Marshal.Copy(fontFileContent, 0, unmanagedMem, fileContent);
                FontFile fontfile = new FontFile(unmanagedMem);
                fonts.Add(filename, fontfile);

                PixelFarm.Font2.NativeMyFonts.MyFtNewMemoryFace(unmanagedMem, fileContent, pixelSize);
            }

        }
    }

}
