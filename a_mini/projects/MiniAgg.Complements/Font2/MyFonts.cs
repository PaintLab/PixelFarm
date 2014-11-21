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
using PixelFarm.Agg;

namespace PixelFarm.Font2
{

    public class MyFontGlyph
    {
        public VertexStore vxs;
    }

    public class FontFace
    {
        IntPtr unmanagedMem;
        Dictionary<char, MyFontGlyph> dicGlyphs = new Dictionary<char, MyFontGlyph>();

        public FontFace(IntPtr unmanagedMem)
        {
            this.unmanagedMem = unmanagedMem;
        }
        public MyFontGlyph GetGlyph(char c)
        {
            MyFontGlyph found;
            if (!dicGlyphs.TryGetValue(c, out found))
            { 
                found = MyFonts.GetGlyph(c);
                this.dicGlyphs.Add(c, found);
            }
            return found;
        }
    }


    public static class MyFonts
    {
        static Dictionary<string, FontFace> fonts = new Dictionary<string, FontFace>();
        
        public static int InitLib()
        {
            return NativeMyFonts.MyFtInitLib();
        }
        internal static MyFontGlyph GetGlyph(char unicodeChar)
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
                int startContour = 0;
                int cpoint_index = 0;
                int todoContourCount = outline.n_contours;

                PixelFarm.Agg.VertexSource.PathStore ps = new Agg.VertexSource.PathStore();

                while (todoContourCount > 0)
                {
                    int nextContour = outline.contours[startContour] + 1;
                    bool isFirstPoint = true;
                    FT_Vector secondControlPoint = new FT_Vector();
                    FT_Vector thirdControlPoint = new FT_Vector();
                    bool hasThirdControlPoint = false;
                    bool justFromCurveMode = false;
                    for (; cpoint_index < nextContour; ++cpoint_index)
                    {
                        FT_Vector vpoint = outline.points[cpoint_index];
                        byte vtag = outline.tags[cpoint_index];
                        bool has_dropout = (((vtag >> 2) & 0x1) != 0);
                        int dropoutMode = vtag >> 3;

                        if ((vtag & 0x1) != 0)
                        {
                            //on curve
                            if (justFromCurveMode)
                            {
                                if (hasThirdControlPoint)
                                {
                                    ps.Curve4(secondControlPoint.x / 64, secondControlPoint.y / 64,
                                        thirdControlPoint.x / 64, thirdControlPoint.y / 64,
                                        vpoint.x / 64, vpoint.y / 64);
                                }
                                else
                                {
                                    ps.Curve3(secondControlPoint.x / 64, secondControlPoint.y / 64,
                                        vpoint.x / 64, vpoint.y / 64);

                                }
                                justFromCurveMode = false;
                            }
                            else
                            {
                                if (isFirstPoint)
                                {
                                    isFirstPoint = false;
                                    ps.MoveTo(vpoint.x / 64, vpoint.y / 64);
                                }
                                else
                                {
                                    ps.LineTo(vpoint.x / 64, vpoint.y / 64);
                                }
                                if (has_dropout)
                                {
                                    //printf("[%d] on,dropoutMode=%d: %d,y:%d \n", mm, dropoutMode, vpoint.x, vpoint.y);
                                }
                                else
                                {
                                    //printf("[%d] on,x: %d,y:%d \n", mm, vpoint.x, vpoint.y);
                                }
                            }

                        }
                        else
                        {
                            //bit 1 set=> off curve, this is a control point
                            //if this is a 2nd order or 3rd order control point
                            if (((vtag >> 1) & 0x1) != 0)
                            {
                                //printf("[%d] bzc3rd,  x: %d,y:%d \n", mm, vpoint.x, vpoint.y);
                                thirdControlPoint = vpoint;
                                hasThirdControlPoint = true;

                            }
                            else
                            {
                                //printf("[%d] bzc2nd,  x: %d,y:%d \n", mm, vpoint.x, vpoint.y);
                                secondControlPoint = vpoint;
                                hasThirdControlPoint = false;
                            }
                            justFromCurveMode = true;
                        }
                    }

                    ps.ClosePolygon();
                    startContour++;
                    todoContourCount--;
                }

                fontGlyph.vxs = ps.Vxs;

                return fontGlyph;
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
        public static FontFace LoadFont(string filename, int pixelSize)
        {
            //load font from specific file 
            FontFace fontFace;
            if (!fonts.TryGetValue(filename, out fontFace))
            {

                //if not found
                //then load it
                byte[] fontFileContent = File.ReadAllBytes(filename);
                int fileContent = fontFileContent.Length;
                IntPtr unmanagedMem = Marshal.AllocHGlobal(fileContent);
                Marshal.Copy(fontFileContent, 0, unmanagedMem, fileContent);
                fontFace = new FontFace(unmanagedMem);
                fonts.Add(filename, fontFace);

                PixelFarm.Font2.NativeMyFonts.MyFtNewMemoryFace(unmanagedMem, fileContent, pixelSize);
                
            }
            return fontFace;

        }
    }

}
