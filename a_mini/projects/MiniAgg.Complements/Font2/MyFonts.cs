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
        /// <summary>
        /// original 8bpp image buffer
        /// </summary>
        public byte[] glyImgBuffer8;
        /// <summary>
        /// 32 bpp image for render
        /// </summary>
        public ActualImage glyphImage32;
        //----------------------------
        /// <summary>
        /// original glyph outline
        /// </summary>
        public VertexStore originalVxs;
        /// <summary>
        /// flaten version of original glyph outline
        /// </summary>
        public VertexStore flattenVxs;
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
        static Agg.VertexSource.CurveFlattener curveFlattener = new Agg.VertexSource.CurveFlattener();

        public static int InitLib()
        {
            return NativeMyFonts.MyFtInitLib();
        }
        static FT_Vector GetMidPoint(FT_Vector v1, FT_Vector v2)
        {

            return new FT_Vector(
                (v1.x + v2.x) / 2,
                (v1.y + v2.y) / 2);
        }

        unsafe static void CopyGlyphBitmap(ref ExportTypeFace exportTypeFace, MyFontGlyph fontGlyph)
        {
            FT_Bitmap* ftBmp = exportTypeFace.bitmap;
            //image is 8 bits grayscale
            int h = ftBmp->rows;
            int w = ftBmp->width;
            int stride = ftBmp->pitch;
            int size = stride * h;
            //copy it to array  
            byte[] buff = new byte[size];
            Marshal.Copy((IntPtr)ftBmp->buffer, buff, 0, size);
            //------------------------------------------------

            //convert to 32bpp
            //make gray value as alpha channel color value
            ActualImage actualImage = new ActualImage(w, h, Agg.Image.PixelFormat.Rgba32);
            int newstride = stride * 4;
            byte[] newBmp32Buffer = actualImage.GetBuffer();
            int src_p = 0;
            int target_p = 0;
            for (int r = 0; r < h; ++r)
            {
                for (int c = 0; c < w; ++c)
                {
                    byte srcColor = buff[src_p + c];
                    //expand to 4 channel
                    newBmp32Buffer[target_p] = 0; //R
                    newBmp32Buffer[target_p + 1] = 0; //G
                    newBmp32Buffer[target_p + 2] = 0; //B
                    newBmp32Buffer[target_p + 3] = srcColor; //A
                    target_p += 4;
                }
                src_p += stride;
            }
            fontGlyph.glyphImage32 = actualImage;

            ////------------------------------------------------
            //{
            //      //add System.Drawing to references 
            //      //save image for debug***
            //    
            //    byte[] buffer = new byte[size];
            //    Marshal.Copy((IntPtr)ftBmp->buffer, buffer, 0, size);
            //    ////save to 
            //    System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(w, h, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
            //    var bmpdata = bmp.LockBits(new System.Drawing.Rectangle(0, 0, w, h),
            //        System.Drawing.Imaging.ImageLockMode.ReadWrite,
            //        bmp.PixelFormat);
            //    Marshal.Copy(buffer, 0, bmpdata.Scan0, size);
            //    bmp.UnlockBits(bmpdata);
            //    bmp.Save("d:\\WImageTest\\glyph.png");
            //} 


        }
        internal static MyFontGlyph GetGlyph(char unicodeChar)
        {

            //--------------------------------------------------
            unsafe
            {
                ExportTypeFace exportTypeFace = new ExportTypeFace();

                PixelFarm.Font2.NativeMyFonts.MyFtLoadChar(unicodeChar, ref exportTypeFace);
                FT_Outline outline = *exportTypeFace.outline;
                MyFontGlyph fontGlyph = new MyFontGlyph();

                //copy raw image 
                CopyGlyphBitmap(ref exportTypeFace, fontGlyph);


                //outline version
                //------------------------------
                int npoints = outline.n_points;
                List<PixelFarm.VectorMath.Vector2> points = new List<PixelFarm.VectorMath.Vector2>(npoints);
                int startContour = 0;
                int cpoint_index = 0;
                int todoContourCount = outline.n_contours;

                PixelFarm.Agg.VertexSource.PathWriter ps = new Agg.VertexSource.PathWriter();
                fontGlyph.originalVxs = ps.Vxs;
                const int resize = 64;
                int controlPointCount = 0;

                while (todoContourCount > 0)
                {
                    int nextContour = outline.contours[startContour] + 1;
                    bool isFirstPoint = true;
                    FT_Vector secondControlPoint = new FT_Vector();
                    FT_Vector thirdControlPoint = new FT_Vector();


                    bool justFromCurveMode = false;
                    //FT_Vector vpoint = new FT_Vector();
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
                                switch (controlPointCount)
                                {
                                    case 1:
                                        {
                                            ps.Curve3(secondControlPoint.x / resize, secondControlPoint.y / resize,
                                                vpoint.x / resize, vpoint.y / resize);

                                        } break;
                                    case 2:
                                        {
                                            ps.Curve4(secondControlPoint.x / resize, secondControlPoint.y / resize,
                                               thirdControlPoint.x / resize, thirdControlPoint.y / resize,
                                               vpoint.x / resize, vpoint.y / resize);
                                        } break;
                                    default:
                                        {
                                            throw new NotSupportedException();
                                        }
                                }
                                controlPointCount = 0;
                                justFromCurveMode = false;
                            }
                            else
                            {

                                if (isFirstPoint)
                                {
                                    isFirstPoint = false;
                                    ps.MoveTo(vpoint.x / resize, vpoint.y / resize);
                                }
                                else
                                {
                                    ps.LineTo(vpoint.x / resize, vpoint.y / resize);
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
                            switch (controlPointCount)
                            {
                                case 0:
                                    {   //bit 1 set=> off curve, this is a control point
                                        //if this is a 2nd order or 3rd order control point
                                        if (((vtag >> 1) & 0x1) != 0)
                                        {
                                            //printf("[%d] bzc3rd,  x: %d,y:%d \n", mm, vpoint.x, vpoint.y);
                                            thirdControlPoint = vpoint;
                                        }
                                        else
                                        {
                                            //printf("[%d] bzc2nd,  x: %d,y:%d \n", mm, vpoint.x, vpoint.y);
                                            secondControlPoint = vpoint;
                                        }
                                    } break;
                                case 1:
                                    {
                                        if (((vtag >> 1) & 0x1) != 0)
                                        {
                                            //printf("[%d] bzc3rd,  x: %d,y:%d \n", mm, vpoint.x, vpoint.y);
                                            thirdControlPoint = vpoint;
                                        }
                                        else
                                        {
                                            //we already have prev second control point
                                            //so auto calculate line to 
                                            //between 2 point
                                            var mid = GetMidPoint(secondControlPoint, vpoint);
                                            //----------
                                            //generate curve3
                                            ps.Curve3(secondControlPoint.x / resize, secondControlPoint.y / resize,
                                                mid.x / resize, mid.y / resize);
                                            //------------------------
                                            controlPointCount--;
                                            //------------------------
                                            //printf("[%d] bzc2nd,  x: %d,y:%d \n", mm, vpoint.x, vpoint.y);
                                            secondControlPoint = vpoint;
                                        }
                                    } break;
                                default:
                                    {
                                        throw new NotSupportedException();
                                    } break;
                            }

                            controlPointCount++;
                            justFromCurveMode = true;
                        }
                    }
                    //--------
                    //close figure
                    //if in curve mode
                    if (justFromCurveMode)
                    {
                        switch (controlPointCount)
                        {
                            case 0: break;
                            case 1:
                                {
                                    ps.Curve3(secondControlPoint.x / resize, secondControlPoint.y / resize,
                                        ps.LastMoveX, ps.LastMoveY);

                                } break;
                            case 2:
                                {
                                    ps.Curve4(secondControlPoint.x / resize, secondControlPoint.y / resize,
                                       thirdControlPoint.x / resize, thirdControlPoint.y / resize,
                                       ps.LastMoveX, ps.LastMoveY);
                                } break;
                            default:
                                { throw new NotSupportedException(); }
                        }
                        justFromCurveMode = false;
                        controlPointCount = 0;
                    }
                    ps.CloseFigure();
                    //--------                   
                    startContour++;
                    todoContourCount--;
                }

                fontGlyph.flattenVxs = curveFlattener.MakeVxs(fontGlyph.originalVxs);
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
