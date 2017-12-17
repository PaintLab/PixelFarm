//BSD, 2014-2017, WinterDev
//----------------------------------------------------------------------------
// Anti-Grain Geometry - Version 2.4
// Copyright (C) 2002-2005 Maxim Shemanarev (http://www.antigrain.com)
//
// C# Port port by: Lars Brubaker
//                  larsbrubaker@gmail.com
// Copyright (C) 2007
//
// Permission to copy, use, modify, sell and distribute this software 
// is granted provided this copyright notice appears in all copies. 
// This software is provided "as is" without express or implied
// warranty, and with no claim as to its suitability for any purpose.
//
//----------------------------------------------------------------------------
// Contact: mcseem@antigrain.com
//          mcseemagg@yahoo.com
//          http://www.antigrain.com
//----------------------------------------------------------------------------
//
// Adaptation for high precision colors has been sponsored by 
// Liberty Technology Systems, Inc., visit http://lib-sys.com
//
// Liberty Technology Systems, Inc. is the provider of
// PostScript and PDF technology for software developers.
// 
//----------------------------------------------------------------------------
#define USE_BLENDER

using PixelFarm.Drawing;
using System.Collections.Generic;
using System;

namespace PixelFarm.Agg.Imaging
{
    /// <summary>
    /// look up table helper for clamp value from 9 bits to 8 bits
    /// </summary>
    static class ClampFrom9To8Bits
    {
        internal static readonly int[] _ = new int[1 << 9];
        static ClampFrom9To8Bits()
        {
            //this is a clamp table
            //9 bits to 8 bits
            //if we don't use this clamp table
            for (int i = _.Length - 1; i >= 0; --i)
            {
                _[i] = Math.Min(i, 255);
            }
        }
    }



    public interface IPixelBlender
    {
        /// <summary>
        /// info 
        /// </summary>
        int NumPixelBits { get; }

        Color PixelToColorRGBA(byte[] buffer, int bufferOffset);

        /// <summary>
        /// copy source color and set to dest buffer
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="bufferOffset"></param>
        /// <param name="sourceColor"></param>
        /// <param name="count"></param>
        void CopyPixels(byte[] buffer, int bufferOffset, Color sourceColor, int count);
        /// <summary>
        /// copy source color and set to dest buffer
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="bufferOffset"></param>
        /// <param name="sourceColor"></param>
        void CopyPixel(byte[] buffer, int bufferOffset, Color sourceColor);
        /// <summary>
        /// blend source color to dest buffer with specific blend function of this PixelBlender
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="bufferOffset"></param>
        /// <param name="sourceColor"></param>
        void BlendPixel(byte[] buffer, int bufferOffset, Color sourceColor);

        /// <summary>
        /// blend source color to dest buffer with specific blend function of this PixelBlender
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="bufferOffset"></param>
        /// <param name="sourceColors"></param>
        /// <param name="sourceColorsOffset"></param>
        /// <param name="sourceCovers"></param>
        /// <param name="sourceCoversOffset"></param>
        /// <param name="firstCoverForAll"></param>
        /// <param name="count"></param>
        void BlendPixels(byte[] buffer, int bufferOffset, Color[] sourceColors, int sourceColorsOffset, byte[] sourceCovers, int sourceCoversOffset, bool firstCoverForAll, int count);
    }

    public abstract class PixelBlenderBGRABase
    {
        public int NumPixelBits { get { return 32; } }
        public const byte BASE_MASK = 255;
    }


    public sealed class PixelBlenderBGRA : PixelBlenderBGRABase, IPixelBlender
    {
        //from https://microsoft.github.io/Win2D/html/PremultipliedAlpha.htm
        //1. Straight alpha
        //result = (source.RGB* source.A) + (dest.RGB* (1 - source.A))
        //---
        //2. Premultiplied alpha
        //result = source.RGB + (dest.RGB * (1 - source.A))
        //---
        //3. Converting between alpha formats
        //3.1 from straight to premult
        //premultiplied.R = (byte) (straight.R* straight.A / 255);
        //premultiplied.G = (byte) (straight.G* straight.A / 255);
        //premultiplied.B = (byte) (straight.B* straight.A / 255);
        //premultiplied.A = straight.A;
        //3.2 from premult to strait
        //straight.R = premultiplied.R  * ((1/straight.A) * 255);
        //straight.G = premultiplied.G  * ((1/straight.A) * 255);
        //straight.B = premultiplied.B  * ((1/straight.A) * 255);
        //straight.A = premultiplied.A;


        public PixelBlenderBGRA()
        {
        }

        public void CopyPixels(byte[] buffer, int bufferOffset, Color sourceColor, int count)
        {
            do
            {
                buffer[bufferOffset + CO.A] = sourceColor.alpha;
                buffer[bufferOffset + CO.R] = sourceColor.red;
                buffer[bufferOffset + CO.G] = sourceColor.green;
                buffer[bufferOffset + CO.B] = sourceColor.blue;
                bufferOffset += 4;
            }
            while (--count != 0);
        }

        public void CopyPixel(byte[] buffer, int bufferOffset, Color sourceColor)
        {
            buffer[bufferOffset + CO.R] = sourceColor.red;
            buffer[bufferOffset + CO.G] = sourceColor.green;
            buffer[bufferOffset + CO.B] = sourceColor.blue;
            buffer[bufferOffset + CO.A] = sourceColor.alpha;
            bufferOffset += 4;
        }
        public void BlendPixel(byte[] buffer, int bufferOffset, Color sourceColor)
        {
            //unsafe
            {
                unchecked
                {
                    if (sourceColor.alpha == 255)
                    {
                        buffer[bufferOffset + CO.R] = (byte)(sourceColor.red);
                        buffer[bufferOffset + CO.G] = (byte)(sourceColor.green);
                        buffer[bufferOffset + CO.B] = (byte)(sourceColor.blue);
                        buffer[bufferOffset + CO.A] = (byte)(sourceColor.alpha);
                    }
                    else
                    {
                        byte r = buffer[bufferOffset + CO.R];
                        byte g = buffer[bufferOffset + CO.G];
                        byte b = buffer[bufferOffset + CO.B];
                        byte a = buffer[bufferOffset + CO.A];
                        buffer[bufferOffset + CO.R] = (byte)(((sourceColor.red - r) * sourceColor.alpha + (r << (int)ColorEx.BASE_SHIFT)) >> (int)ColorEx.BASE_SHIFT);
                        buffer[bufferOffset + CO.G] = (byte)(((sourceColor.green - g) * sourceColor.alpha + (g << (int)ColorEx.BASE_SHIFT)) >> (int)ColorEx.BASE_SHIFT);
                        buffer[bufferOffset + CO.B] = (byte)(((sourceColor.blue - b) * sourceColor.alpha + (b << (int)ColorEx.BASE_SHIFT)) >> (int)ColorEx.BASE_SHIFT);
                        buffer[bufferOffset + CO.A] = (byte)((sourceColor.alpha + a) - ((sourceColor.alpha * a + BASE_MASK) >> (int)ColorEx.BASE_SHIFT));
                    }
                }
            }
        }

        public void BlendPixels(byte[] destBuffer, int bufferOffset,
            Color[] sourceColors, int sourceColorsOffset,
            byte[] covers, int coversIndex, bool firstCoverForAll, int count)
        {
            if (firstCoverForAll)
            {
                int cover = covers[coversIndex];
                if (cover == 255)
                {
                    do
                    {
                        BlendPixel(destBuffer, bufferOffset, sourceColors[sourceColorsOffset++]);
                        bufferOffset += 4;
                    }
                    while (--count != 0);
                }
                else
                {
                    do
                    {
                        sourceColors[sourceColorsOffset].alpha = (byte)((sourceColors[sourceColorsOffset].alpha * cover + 255) >> 8);
                        BlendPixel(destBuffer, bufferOffset, sourceColors[sourceColorsOffset]);
                        bufferOffset += 4;
                        ++sourceColorsOffset;
                    }
                    while (--count != 0);
                }
            }
            else
            {
                do
                {
                    int cover = covers[coversIndex++];
                    if (cover == 255)
                    {
                        BlendPixel(destBuffer, bufferOffset, sourceColors[sourceColorsOffset]);
                    }
                    else
                    {
                        Color color = sourceColors[sourceColorsOffset];
                        color.alpha = (byte)((color.alpha * (cover) + 255) >> 8);
                        BlendPixel(destBuffer, bufferOffset, color);
                    }
                    bufferOffset += 4;
                    ++sourceColorsOffset;
                }
                while (--count != 0);
            }
        }

        public Color PixelToColorRGBA(byte[] buffer, int bufferOffset)
        {
            //TODO: review here ...
            //
            //change if the buffer is pre-mul or not
            return new Color(
                buffer[bufferOffset + CO.A],
                buffer[bufferOffset + CO.R],
                buffer[bufferOffset + CO.G],
                buffer[bufferOffset + CO.B]
                );
        }

    }


    public sealed class PixelBlenderGammaBGRA : PixelBlenderBGRABase, IPixelBlender
    {
        GammaLookUpTable m_gamma;
        static Dictionary<float, GammaLookUpTable> gammaTablePool = new Dictionary<float, GammaLookUpTable>();
        public PixelBlenderGammaBGRA(float gammaValue)
        {
            GammaLookUpTable found;
            if (!gammaTablePool.TryGetValue(gammaValue, out found))
            {
                found = new GammaLookUpTable(gammaValue);
                gammaTablePool.Add(gammaValue, found);
            }

            this.m_gamma = found;
        }
        public Color PixelToColorRGBA(byte[] buffer, int bufferOffset)
        {
            return new Color(
                buffer[bufferOffset + CO.A],
                buffer[bufferOffset + CO.R],
                buffer[bufferOffset + CO.G],
                buffer[bufferOffset + CO.B]
                );
        }

        public void CopyPixels(byte[] buffer, int bufferOffset, Color sourceColor, int count)
        {
            do
            {
                buffer[bufferOffset + CO.R] = m_gamma.inv(sourceColor.red);
                buffer[bufferOffset + CO.G] = m_gamma.inv(sourceColor.green);
                buffer[bufferOffset + CO.B] = m_gamma.inv(sourceColor.blue);
                buffer[bufferOffset + CO.A] = m_gamma.inv(sourceColor.alpha);
                bufferOffset += 4;
            }
            while (--count != 0);
        }

        public void CopyPixel(byte[] buffer, int bufferOffset, Color sourceColor)
        {
            buffer[bufferOffset + CO.R] = m_gamma.inv(sourceColor.red);
            buffer[bufferOffset + CO.G] = m_gamma.inv(sourceColor.green);
            buffer[bufferOffset + CO.B] = m_gamma.inv(sourceColor.blue);
            buffer[bufferOffset + CO.A] = m_gamma.inv(sourceColor.alpha);
        }
        public void BlendPixel(byte[] buffer, int bufferOffset, Color sourceColor)
        {
            unchecked
            {
                byte r = buffer[bufferOffset + CO.R];
                byte g = buffer[bufferOffset + CO.G];
                byte b = buffer[bufferOffset + CO.B];
                byte a = buffer[bufferOffset + CO.A];
                buffer[bufferOffset + CO.R] = m_gamma.inv((byte)(((sourceColor.red - r) * sourceColor.alpha + (r << (int)ColorEx.BASE_SHIFT)) >> (int)ColorEx.BASE_SHIFT));
                buffer[bufferOffset + CO.G] = m_gamma.inv((byte)(((sourceColor.green - g) * sourceColor.alpha + (g << (int)ColorEx.BASE_SHIFT)) >> (int)ColorEx.BASE_SHIFT));
                buffer[bufferOffset + CO.B] = m_gamma.inv((byte)(((sourceColor.blue - b) * sourceColor.alpha + (b << (int)ColorEx.BASE_SHIFT)) >> (int)ColorEx.BASE_SHIFT));
                buffer[CO.A] = (byte)((sourceColor.alpha + a) - ((sourceColor.alpha * a + BASE_MASK) >> (int)ColorEx.BASE_SHIFT));
            }
        }

        public void BlendPixels(byte[] buffer, int bufferOffset,
            Color[] sourceColors, int sourceColorsOffset,
            byte[] sourceCovers, int sourceCoversOffset, bool firstCoverForAll, int count)
        {
            throw new NotImplementedException();
        }
    }

    public sealed class PixelBlenderPreMultBGRA : PixelBlenderBGRABase, IPixelBlender
    {

        public PixelBlenderPreMultBGRA()
        {
        }

        public Color PixelToColorRGBA(byte[] buffer, int bufferOffset)
        {
            return new Color(buffer[bufferOffset + CO.A],
                buffer[bufferOffset + CO.R],
                buffer[bufferOffset + CO.G],
                buffer[bufferOffset + CO.B]);
        }

        public void CopyPixels(byte[] buffer, int bufferOffset, Color sourceColor, int count)
        {
            for (int i = 0; i < count; i++)
            {
                buffer[bufferOffset + CO.R] = sourceColor.red;
                buffer[bufferOffset + CO.G] = sourceColor.green;
                buffer[bufferOffset + CO.B] = sourceColor.blue;
                buffer[bufferOffset + CO.A] = sourceColor.alpha;
                bufferOffset += 4;
            }
        }
        public void CopyPixel(byte[] buffer, int bufferOffset, Color sourceColor)
        {
            buffer[bufferOffset + CO.R] = sourceColor.red;
            buffer[bufferOffset + CO.G] = sourceColor.green;
            buffer[bufferOffset + CO.B] = sourceColor.blue;
            buffer[bufferOffset + CO.A] = sourceColor.alpha;
        }

        public void BlendPixel(byte[] pDestBuffer, int bufferOffset, Color sourceColor)
        {
            //unsafe
            {
                int oneOverAlpha = BASE_MASK - sourceColor.alpha;
                unchecked
                {
#if false
					Vector4i sourceColors = new Vector4i(sourceColor.m_B, sourceColor.m_G, sourceColor.m_R, sourceColor.m_A);
					Vector4i destColors = new Vector4i(
						pDestBuffer[bufferOffset + ImageBuffer.OrderB],
					    pDestBuffer[bufferOffset + ImageBuffer.OrderG],
					    pDestBuffer[bufferOffset + ImageBuffer.OrderB],
					    pDestBuffer[bufferOffset + ImageBuffer.OrderA]);
					Vector4i oneOverAlphaV = new Vector4i(oneOverAlpha, oneOverAlpha, oneOverAlpha, oneOverAlpha);
					Vector4i rounding = new Vector4i(255, 255, 255, 255);
					Vector4i temp = destColors * oneOverAlphaV + rounding;
					temp = temp >> 8;
					temp = temp + sourceColors;
					Vector8us packed8Final = Vector4i.PackWithUnsignedSaturation(temp, temp);
					Vector16b packed16Final = Vector8us.SignedPackWithUnsignedSaturation(packed8Final, packed8Final);
					pDestBuffer[bufferOffset + ImageBuffer.OrderR] = packed16Final.V2;
					pDestBuffer[bufferOffset + ImageBuffer.OrderG] = packed16Final.V1;
					pDestBuffer[bufferOffset + ImageBuffer.OrderB] = packed16Final.V0;
					pDestBuffer[bufferOffset + ImageBuffer.OrderA] = 255;
					            
#else
                    int r = ClampFrom9To8Bits._[((pDestBuffer[bufferOffset + CO.R] * oneOverAlpha + 255) >> 8) + sourceColor.red];
                    int g = ClampFrom9To8Bits._[((pDestBuffer[bufferOffset + CO.G] * oneOverAlpha + 255) >> 8) + sourceColor.green];
                    int b = ClampFrom9To8Bits._[((pDestBuffer[bufferOffset + CO.B] * oneOverAlpha + 255) >> 8) + sourceColor.blue];
                    int a = pDestBuffer[bufferOffset + CO.A];
                    pDestBuffer[bufferOffset + CO.R] = (byte)r;
                    pDestBuffer[bufferOffset + CO.G] = (byte)g;
                    pDestBuffer[bufferOffset + CO.B] = (byte)b;
                    pDestBuffer[bufferOffset + CO.A] = (byte)(BASE_MASK - ClampFrom9To8Bits._[(oneOverAlpha * (BASE_MASK - a) + 255) >> 8]);
#endif
                }
            }
        }

        public void BlendPixels(byte[] pDestBuffer, int bufferOffset,
            Color[] sourceColors, int sourceColorsOffset,
            byte[] sourceCovers, int sourceCoversOffset, bool firstCoverForAll, int count)
        {
            if (firstCoverForAll)
            {
                //unsafe
                {
                    if (sourceCovers[sourceCoversOffset] == 255)
                    {
                        for (int i = 0; i < count; i++)
                        {
#if false
                           BlendPixel(pDestBuffer, bufferOffset, sourceColors[sourceColorsOffset]);
#else
                            Color sourceColor = sourceColors[sourceColorsOffset];
                            if (sourceColor.alpha == 255)
                            {
                                pDestBuffer[bufferOffset + CO.R] = (byte)sourceColor.red;
                                pDestBuffer[bufferOffset + CO.G] = (byte)sourceColor.green;
                                pDestBuffer[bufferOffset + CO.B] = (byte)sourceColor.blue;
                                pDestBuffer[bufferOffset + CO.A] = 255;
                            }
                            else
                            {
                                int OneOverAlpha = BASE_MASK - sourceColor.alpha;
                                unchecked
                                {
                                    int r = ClampFrom9To8Bits._[((pDestBuffer[bufferOffset + CO.R] * OneOverAlpha + 255) >> 8) + sourceColor.red];
                                    int g = ClampFrom9To8Bits._[((pDestBuffer[bufferOffset + CO.G] * OneOverAlpha + 255) >> 8) + sourceColor.green];
                                    int b = ClampFrom9To8Bits._[((pDestBuffer[bufferOffset + CO.B] * OneOverAlpha + 255) >> 8) + sourceColor.blue];
                                    int a = pDestBuffer[bufferOffset + CO.A];
                                    pDestBuffer[bufferOffset + CO.R] = (byte)r;
                                    pDestBuffer[bufferOffset + CO.G] = (byte)g;
                                    pDestBuffer[bufferOffset + CO.B] = (byte)b;
                                    pDestBuffer[bufferOffset + CO.A] = (byte)(BASE_MASK - ClampFrom9To8Bits._[(OneOverAlpha * (BASE_MASK - a) + 255) >> 8]);
                                }
                            }
#endif
                            sourceColorsOffset++;
                            bufferOffset += 4;
                        }
                    }
                    else
                    {
                        for (int i = 0; i < count; i++)
                        {
                            Color sourceColor = sourceColors[sourceColorsOffset];
                            int alpha = (sourceColor.alpha * sourceCovers[sourceCoversOffset] + 255) / 256;
                            if (alpha == 0)
                            {
                                continue;
                            }
                            else if (alpha == 255)
                            {
                                pDestBuffer[bufferOffset + CO.R] = (byte)sourceColor.red;
                                pDestBuffer[bufferOffset + CO.G] = (byte)sourceColor.green;
                                pDestBuffer[bufferOffset + CO.B] = (byte)sourceColor.blue;
                                pDestBuffer[bufferOffset + CO.A] = (byte)alpha;
                            }
                            else
                            {
                                int OneOverAlpha = BASE_MASK - alpha;
                                unchecked
                                {
                                    int r = ClampFrom9To8Bits._[((pDestBuffer[bufferOffset + CO.R] * OneOverAlpha + 255) >> 8) + sourceColor.red];
                                    int g = ClampFrom9To8Bits._[((pDestBuffer[bufferOffset + CO.G] * OneOverAlpha + 255) >> 8) + sourceColor.green];
                                    int b = ClampFrom9To8Bits._[((pDestBuffer[bufferOffset + CO.B] * OneOverAlpha + 255) >> 8) + sourceColor.blue];
                                    int a = pDestBuffer[bufferOffset + CO.A];
                                    pDestBuffer[bufferOffset + CO.R] = (byte)r;
                                    pDestBuffer[bufferOffset + CO.G] = (byte)g;
                                    pDestBuffer[bufferOffset + CO.B] = (byte)b;
                                    pDestBuffer[bufferOffset + CO.A] = (byte)(BASE_MASK - ClampFrom9To8Bits._[(OneOverAlpha * (BASE_MASK - a) + 255) >> 8]);
                                }
                            }
                            sourceColorsOffset++;
                            bufferOffset += 4;
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    Color sourceColor = sourceColors[sourceColorsOffset];
                    int alpha = (sourceColor.alpha * sourceCovers[sourceCoversOffset] + 255) / 256;
                    if (alpha == 255)
                    {
                        pDestBuffer[bufferOffset + CO.R] = (byte)sourceColor.red;
                        pDestBuffer[bufferOffset + CO.G] = (byte)sourceColor.green;
                        pDestBuffer[bufferOffset + CO.B] = (byte)sourceColor.blue;
                        pDestBuffer[bufferOffset + CO.A] = (byte)alpha;
                    }
                    else if (alpha > 0)
                    {
                        int OneOverAlpha = BASE_MASK - alpha;
                        unchecked
                        {
                            int r = ClampFrom9To8Bits._[((pDestBuffer[bufferOffset + CO.R] * OneOverAlpha + 255) >> 8) + sourceColor.red];
                            int g = ClampFrom9To8Bits._[((pDestBuffer[bufferOffset + CO.G] * OneOverAlpha + 255) >> 8) + sourceColor.green];
                            int b = ClampFrom9To8Bits._[((pDestBuffer[bufferOffset + CO.B] * OneOverAlpha + 255) >> 8) + sourceColor.blue];
                            int a = pDestBuffer[bufferOffset + CO.A];
                            pDestBuffer[bufferOffset + CO.R] = (byte)r;
                            pDestBuffer[bufferOffset + CO.G] = (byte)g;
                            pDestBuffer[bufferOffset + CO.B] = (byte)b;
                            pDestBuffer[bufferOffset + CO.A] = (byte)(BASE_MASK - ClampFrom9To8Bits._[(OneOverAlpha * (BASE_MASK - a) + 255) >> 8]);
                        }
                    }
                    sourceColorsOffset++;
                    sourceCoversOffset++;
                    bufferOffset += 4;
                }
            }
        }
    }



#if DEBUG
    public sealed class PixelBlenderPolyColorPreMultBGRA : PixelBlenderBGRABase, IPixelBlender
    {

        Color polyColor;
        public PixelBlenderPolyColorPreMultBGRA(Color polyColor)
        {
            this.polyColor = polyColor;
        }

        public Color PixelToColorRGBA(byte[] buffer, int bufferOffset)
        {
            return new Color(buffer[bufferOffset + CO.A], buffer[bufferOffset + CO.R], buffer[bufferOffset + CO.G], buffer[bufferOffset + CO.B]);
        }

        public void CopyPixels(byte[] buffer, int bufferOffset, Color sourceColor, int count)
        {
            for (int i = 0; i < count; i++)
            {
                buffer[bufferOffset + CO.R] = sourceColor.red;
                buffer[bufferOffset + CO.G] = sourceColor.green;
                buffer[bufferOffset + CO.B] = sourceColor.blue;
                buffer[bufferOffset + CO.A] = sourceColor.alpha;
                bufferOffset += 4;
            }
        }
        public void CopyPixel(byte[] buffer, int bufferOffset, Color sourceColor)
        {
            buffer[bufferOffset + CO.R] = sourceColor.red;
            buffer[bufferOffset + CO.G] = sourceColor.green;
            buffer[bufferOffset + CO.B] = sourceColor.blue;
            buffer[bufferOffset + CO.A] = sourceColor.alpha;
        }

        public void BlendPixel(byte[] pDestBuffer, int bufferOffset, Color sourceColor)
        {
            //unsafe
            {
                int sourceA = (byte)(ClampFrom9To8Bits._[(polyColor.Alpha0To255 * sourceColor.alpha + 255) >> 8]);
                int oneOverAlpha = BASE_MASK - sourceA;
                unchecked
                {
                    int sourceR = (byte)(ClampFrom9To8Bits._[(polyColor.Alpha0To255 * sourceColor.red + 255) >> 8]);
                    int sourceG = (byte)(ClampFrom9To8Bits._[(polyColor.Alpha0To255 * sourceColor.green + 255) >> 8]);
                    int sourceB = (byte)(ClampFrom9To8Bits._[(polyColor.Alpha0To255 * sourceColor.blue + 255) >> 8]);
                    int destR = ClampFrom9To8Bits._[((pDestBuffer[bufferOffset + CO.R] * oneOverAlpha + 255) >> 8) + sourceR];
                    int destG = ClampFrom9To8Bits._[((pDestBuffer[bufferOffset + CO.G] * oneOverAlpha + 255) >> 8) + sourceG];
                    int destB = ClampFrom9To8Bits._[((pDestBuffer[bufferOffset + CO.B] * oneOverAlpha + 255) >> 8) + sourceB];
                    // TODO: calculated the correct dest alpha
                    //int destA = pDestBuffer[bufferOffset + ImageBuffer.OrderA];

                    pDestBuffer[bufferOffset + CO.R] = (byte)destR;
                    pDestBuffer[bufferOffset + CO.G] = (byte)destG;
                    pDestBuffer[bufferOffset + CO.B] = (byte)destB;
                    //pDestBuffer[bufferOffset + ImageBuffer.OrderA] = (byte)(base_mask - m_Saturate9BitToByte[(oneOverAlpha * (base_mask - a) + 255) >> 8]);
                }
            }
        }

        public void BlendPixels(byte[] pDestBuffer, int bufferOffset,
            Color[] sourceColors, int sourceColorsOffset,
            byte[] sourceCovers, int sourceCoversOffset, bool firstCoverForAll, int count)
        {
            if (firstCoverForAll)
            {
                //unsafe
                {
                    if (sourceCovers[sourceCoversOffset] == 255)
                    {
                        for (int i = 0; i < count; i++)
                        {
                            BlendPixel(pDestBuffer, bufferOffset, sourceColors[sourceColorsOffset]);
                            sourceColorsOffset++;
                            bufferOffset += 4;
                        }
                    }
                    else
                    {
                        throw new NotImplementedException("need to consider the polyColor");
#if false
                        for (int i = 0; i < count; i++)
                        {
                            RGBA_Bytes sourceColor = sourceColors[sourceColorsOffset];
                            int alpha = (sourceColor.alpha * sourceCovers[sourceCoversOffset] + 255) / 256;
                            if (alpha == 0)
                            {
                                continue;
                            }
                            else if (alpha == 255)
                            {
                                pDestBuffer[bufferOffset + ImageBuffer.OrderR] = (byte)sourceColor.red;
                                pDestBuffer[bufferOffset + ImageBuffer.OrderG] = (byte)sourceColor.green;
                                pDestBuffer[bufferOffset + ImageBuffer.OrderB] = (byte)sourceColor.blue;
                                pDestBuffer[bufferOffset + ImageBuffer.OrderA] = (byte)alpha;
                            }
                            else
                            {
                                int OneOverAlpha = base_mask - alpha;
                                unchecked
                                {
                                    int r = m_Saturate9BitToByte[((pDestBuffer[bufferOffset + ImageBuffer.OrderR] * OneOverAlpha + 255) >> 8) + sourceColor.red];
                                    int g = m_Saturate9BitToByte[((pDestBuffer[bufferOffset + ImageBuffer.OrderG] * OneOverAlpha + 255) >> 8) + sourceColor.green];
                                    int b = m_Saturate9BitToByte[((pDestBuffer[bufferOffset + ImageBuffer.OrderB] * OneOverAlpha + 255) >> 8) + sourceColor.blue];
                                    int a = pDestBuffer[bufferOffset + ImageBuffer.OrderA];
                                    pDestBuffer[bufferOffset + ImageBuffer.OrderR] = (byte)r;
                                    pDestBuffer[bufferOffset + ImageBuffer.OrderG] = (byte)g;
                                    pDestBuffer[bufferOffset + ImageBuffer.OrderB] = (byte)b;
                                    pDestBuffer[bufferOffset + ImageBuffer.OrderA] = (byte)(base_mask - m_Saturate9BitToByte[(OneOverAlpha * (base_mask - a) + 255) >> 8]);
                                }
                            }
                            sourceColorsOffset++;
                            bufferOffset += 4;
                        }
#endif
                    }
                }
            }
            else
            {
                throw new NotImplementedException("need to consider the polyColor");
#if false
                for (int i = 0; i < count; i++)
                {
                    RGBA_Bytes sourceColor = sourceColors[sourceColorsOffset];
                    int alpha = (sourceColor.alpha * sourceCovers[sourceCoversOffset] + 255) / 256;
                    if (alpha == 255)
                    {
                        pDestBuffer[bufferOffset + ImageBuffer.OrderR] = (byte)sourceColor.red;
                        pDestBuffer[bufferOffset + ImageBuffer.OrderG] = (byte)sourceColor.green;
                        pDestBuffer[bufferOffset + ImageBuffer.OrderB] = (byte)sourceColor.blue;
                        pDestBuffer[bufferOffset + ImageBuffer.OrderA] = (byte)alpha;
                    }
                    else if (alpha > 0)
                    {
                        int OneOverAlpha = base_mask - alpha;
                        unchecked
                        {
                            int r = m_Saturate9BitToByte[((pDestBuffer[bufferOffset + ImageBuffer.OrderR] * OneOverAlpha + 255) >> 8) + sourceColor.red];
                            int g = m_Saturate9BitToByte[((pDestBuffer[bufferOffset + ImageBuffer.OrderG] * OneOverAlpha + 255) >> 8) + sourceColor.green];
                            int b = m_Saturate9BitToByte[((pDestBuffer[bufferOffset + ImageBuffer.OrderB] * OneOverAlpha + 255) >> 8) + sourceColor.blue];
                            int a = pDestBuffer[bufferOffset + ImageBuffer.OrderA];
                            pDestBuffer[bufferOffset + ImageBuffer.OrderR] = (byte)r;
                            pDestBuffer[bufferOffset + ImageBuffer.OrderG] = (byte)g;
                            pDestBuffer[bufferOffset + ImageBuffer.OrderB] = (byte)b;
                            pDestBuffer[bufferOffset + ImageBuffer.OrderA] = (byte)(base_mask - m_Saturate9BitToByte[(OneOverAlpha * (base_mask - a) + 255) >> 8]);
                        }
                    }
                    sourceColorsOffset++;
                    sourceCoversOffset++;
                    bufferOffset += 4;
                }
#endif
            }
        }
    }
#endif //DEBUG
}


