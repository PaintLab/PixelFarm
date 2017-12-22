//MIT, 2017, WinterDev
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

using Typography.OpenFont;
using Typography.TextLayout;
using Typography.Contours;
using Typography.Rendering;

namespace TypographyTest
{
    public enum TextureKind
    {
        Msdf,
        Stencil
    }
    public class GlyphTextureBitmapGenController
    { 
        static ushort[] GetUniqueGlyphIndexList(List<ushort> inputGlyphIndexList)
        {
            Dictionary<ushort, bool> uniqueGlyphIndices = new Dictionary<ushort, bool>(inputGlyphIndexList.Count);
            foreach (ushort glyphIndex in inputGlyphIndexList)
            {
                if (!uniqueGlyphIndices.ContainsKey(glyphIndex))
                {
                    uniqueGlyphIndices.Add(glyphIndex, true);
                }
            }
            //
            ushort[] uniqueGlyphIndexArray = new ushort[uniqueGlyphIndices.Count];
            int i = 0;
            foreach (ushort glyphIndex in uniqueGlyphIndices.Keys)
            {
                uniqueGlyphIndexArray[i] = glyphIndex;
                i++;
            }
            return uniqueGlyphIndexArray;
        }


        public static void CreateSampleTextureFontFromScriptLangs(
            Typeface typeface, float sizeInPoint,
            TextureKind textureKind,
            ScriptLang[] scLangs,
            string outputFile)
        { 
            //2. find associated glyph index base on input script langs
            List<ushort> outputGlyphIndexList = new List<ushort>();
            //
            foreach (ScriptLang scLang in scLangs)
            {
                typeface.CollectAllAssociateGlyphIndex(outputGlyphIndexList, scLang);
            }
            //
            ushort[] allAssocGlyphIndices = GetUniqueGlyphIndexList(outputGlyphIndexList);
            CreateSampleTextureFontFromGlyphIndices(typeface, sizeInPoint, textureKind, allAssocGlyphIndices, outputFile);

        }
        public static void CreateSampleTextureFontFromInputChars(
            Typeface typeface, float sizeInPoint,
            TextureKind textureKind,
            char[] chars, string outputFile)
        {
            
            //convert input chars into glyphIndex
            List<ushort> glyphIndices = new List<ushort>(chars.Length);
            int i = 0;
            foreach (char ch in chars)
            {
                glyphIndices.Add(typeface.LookupIndex(ch));
                i++;
            }
            ushort[] allAssocGlyphIndices = GetUniqueGlyphIndexList(glyphIndices);
            CreateSampleTextureFontFromGlyphIndices(typeface, sizeInPoint, textureKind, allAssocGlyphIndices, outputFile);
        }

        public static void CreateSampleTextureFontFromGlyphIndices(
            Typeface typeface, float sizeInPoint,
            TextureKind textureKind,
          ushort[] glyphIndices, string outputFile)
        {

            //sample: create sample msdf texture 
            //-------------------------------------------------------------
            var builder = new GlyphPathBuilder(typeface);
            builder.UseTrueTypeInstructions = true;
            //-------------------------------------------------------------
            var atlasBuilder = new SimpleFontAtlasBuilder();
            MsdfGenParams msdfGenParams = new MsdfGenParams();
            //
            AggGlyphTextureGen aggTextureGen = new AggGlyphTextureGen();
            float pxscale = typeface.CalculateScaleToPixelFromPointSize(sizeInPoint);
            int j = glyphIndices.Length;
            for (int i = 0; i < j; ++i)
            {
                //build glyph
                ushort gindex = glyphIndices[i];
                builder.BuildFromGlyphIndex(gindex, -1);
                GlyphImage glyphImg = null;
                if (textureKind == TextureKind.Msdf)
                {
                    var glyphToContour = new GlyphContourBuilder();
                    //glyphToContour.Read(builder.GetOutputPoints(), builder.GetOutputContours());
                    builder.ReadShapes(glyphToContour);
                    msdfGenParams.shapeScale = 1f / 64;
                    glyphImg = MsdfGlyphGen.CreateMsdfImage(glyphToContour, msdfGenParams);
                }
                else
                {
                    //create alpha channel texture                      
                    glyphImg = aggTextureGen.CreateGlyphImage(builder, pxscale);
                }
                atlasBuilder.AddGlyph(gindex, glyphImg);
                int w = glyphImg.Width;
                int h = glyphImg.Height;

                using (Bitmap bmp = new Bitmap(glyphImg.Width, glyphImg.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
                {
                    var bmpdata = bmp.LockBits(new System.Drawing.Rectangle(0, 0, w, h), System.Drawing.Imaging.ImageLockMode.ReadWrite, bmp.PixelFormat);
                    int[] imgBuffer = glyphImg.GetImageBuffer();
                    System.Runtime.InteropServices.Marshal.Copy(imgBuffer, 0, bmpdata.Scan0, imgBuffer.Length);
                    bmp.UnlockBits(bmpdata);
                    //bmp.Save("d:\\WImageTest\\glyph_gen\\a001_alpha_" + ((int)gindex) + ".png");
                }
            }

            var glyphImg2 = atlasBuilder.BuildSingleImage();
            using (Bitmap bmp = new Bitmap(glyphImg2.Width, glyphImg2.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
            {
                var bmpdata = bmp.LockBits(new System.Drawing.Rectangle(0, 0, glyphImg2.Width, glyphImg2.Height),
                    System.Drawing.Imaging.ImageLockMode.ReadWrite, bmp.PixelFormat);
                int[] intBuffer = glyphImg2.GetImageBuffer();

                System.Runtime.InteropServices.Marshal.Copy(intBuffer, 0, bmpdata.Scan0, intBuffer.Length);
                bmp.UnlockBits(bmpdata);
                bmp.Save(outputFile);
            }
            atlasBuilder.SaveFontInfo(outputFile + ".xml");
        }

    }
}