//MIT, 2017, WinterDev
using System;
using System.Collections.Generic;

using System.IO;

using Typography.OpenFont;
using Typography.TextLayout;
using Typography.Contours;
using Typography.Rendering;

namespace PixelFarm.Drawing.Fonts
{

    public class GlyphTextureBitmapGenerator
    {

        public delegate void OnEachFinishTotal(int glyphIndex, GlyphImage glyphImage, SimpleFontAtlasBuilder atlasBuilder);

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


        public static void CreateTextureFontFromScriptLangs(
            Typeface typeface, float sizeInPoint,
            TextureKind textureKind,
            ScriptLang[] scLangs,
            OnEachFinishTotal onFinishTotal)
        {
            //2. find associated glyph index base on input script langs
            List<ushort> outputGlyphIndexList = new List<ushort>();
            //
            foreach (ScriptLang scLang in scLangs)
            {
                typeface.CollectAllAssociateGlyphIndex(outputGlyphIndexList, scLang);
            }
            //

            CreateTextureFontFromGlyphIndices(typeface, sizeInPoint, textureKind, GetUniqueGlyphIndexList(outputGlyphIndexList), onFinishTotal);

        }
        public static void CreateTextureFontFromInputChars(
            Typeface typeface, float sizeInPoint,
            TextureKind textureKind,
            char[] chars, OnEachFinishTotal onFinishTotal)
        {

            //convert input chars into glyphIndex
            List<ushort> glyphIndices = new List<ushort>(chars.Length);
            int i = 0;
            foreach (char ch in chars)
            {
                glyphIndices.Add(typeface.LookupIndex(ch));
                i++;
            }

            CreateTextureFontFromGlyphIndices(typeface, sizeInPoint, textureKind, GetUniqueGlyphIndexList(glyphIndices), onFinishTotal);
        }

        public static void CreateTextureFontFromGlyphIndices(
            Typeface typeface, float sizeInPoint,
            TextureKind textureKind,
            ushort[] glyphIndices, OnEachFinishTotal onFinishTotal)
        {
            if (onFinishTotal == null)
            {
                return;
            }
            //sample: create sample msdf texture 
            //-------------------------------------------------------------
            var builder = new GlyphPathBuilder(typeface);
            builder.UseTrueTypeInstructions = true;
            //-------------------------------------------------------------
            var atlasBuilder = new SimpleFontAtlasBuilder();
            atlasBuilder.SetAtlasInfo(textureKind, sizeInPoint);
            //-------------------------------------------------------------

            //
            MsdfGenParams msdfGenParams = null;
            AggGlyphTextureGen aggTextureGen = null;

            if (textureKind == TextureKind.Msdf)
            {
                msdfGenParams = new MsdfGenParams();
            }
            else
            {
                aggTextureGen = new AggGlyphTextureGen();
            }


            float pxscale = typeface.CalculateScaleToPixelFromPointSize(sizeInPoint);
            int j = glyphIndices.Length;
            for (int i = 0; i < j; ++i)
            {
                //build glyph
                ushort gindex = glyphIndices[i];
                builder.BuildFromGlyphIndex(gindex, -1);
                GlyphImage glyphImg = null;
                switch (textureKind)
                {
                    case TextureKind.Msdf:
                        {
                            var glyphToContour = new GlyphContourBuilder();
                            //glyphToContour.Read(builder.GetOutputPoints(), builder.GetOutputContours());
                            builder.ReadShapes(glyphToContour);
                            msdfGenParams.shapeScale = 1f / 64; //as original
                            glyphImg = MsdfGlyphGen.CreateMsdfImage(glyphToContour, msdfGenParams);
                        }
                        break;
                    case TextureKind.StencilGreyScale:
                        {
                            //create alpha channel texture                      
                            glyphImg = aggTextureGen.CreateGlyphImage(builder, false, pxscale);
                        }
                        break;
                    case TextureKind.StencilLcdEffect:
                        {
                            //create alpha channel texture                      
                            glyphImg = aggTextureGen.CreateGlyphImage(builder, true, pxscale);
                        }
                        break;
                }

                atlasBuilder.AddGlyph(gindex, glyphImg);
                onFinishTotal(gindex, glyphImg, null);
            }
            onFinishTotal(0, null, atlasBuilder);
        }

    }
}