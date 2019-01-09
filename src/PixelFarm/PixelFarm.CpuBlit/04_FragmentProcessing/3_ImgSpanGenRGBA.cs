//BSD, 2014-present, WinterDev
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
#define USE_UNSAFE_CODE

using System;
using PixelFarm.Drawing;
using PixelFarm.CpuBlit.Imaging;

using subpix_const = PixelFarm.CpuBlit.Imaging.ImageFilterLookUpTable.ImgSubPixConst;
using filter_const = PixelFarm.CpuBlit.Imaging.ImageFilterLookUpTable.ImgFilterConst;
using CO = PixelFarm.CpuBlit.PixelProcessing.CO;

namespace PixelFarm.CpuBlit.FragmentProcessing
{

    // it should be easy to write a 90 rotating or mirroring filter too. LBB 2012/01/14
    /// <summary>
    /// Nearest Neighbor,StepXBy1
    /// </summary>
    public class ImgSpanGenRGBA_NN_StepXBy1 : ImgSpanGen
    {
        //NN: nearest neighbor
        public ImgSpanGenRGBA_NN_StepXBy1()
        {

        }
        public sealed override void GenerateColors(Drawing.Color[] outputColors, int startIndex, int x, int y, int len)
        {
            ISpanInterpolator spanInterpolator = Interpolator;
            spanInterpolator.Begin(x + dx, y + dy, len);
            int x_hr;
            int y_hr;
            spanInterpolator.GetCoord(out x_hr, out y_hr);
            int x_lr = x_hr >> subpix_const.SHIFT;
            int y_lr = y_hr >> subpix_const.SHIFT;

            int bufferIndex = _bmpSrc.GetBufferOffsetXY32(x_lr, y_lr);

            unsafe
            {
                using (CpuBlit.Imaging.TempMemPtr srcBufferPtr = _bmpSrc.GetBufferPtr())
                {
                    int* pSource = (int*)srcBufferPtr.Ptr + bufferIndex;
                    do
                    {
                        int srcColor = *pSource;
                        //separate each component 
                        //TODO: review here, color from source buffer
                        //should be in 'pre-multiplied' format.
                        //so it should be converted to 'straight' color by call something like ..'FromPreMult()'  
                        outputColors[startIndex++] = Drawing.Color.FromArgb(
                              (srcColor >> CO.A_SHIFT) & 0xff, //a
                              (srcColor >> CO.R_SHIFT) & 0xff, //r
                              (srcColor >> CO.G_SHIFT) & 0xff, //g
                              (srcColor >> CO.B_SHIFT) & 0xff);//b 

                        pSource++;//move next

                    } while (--len != 0);

                }
            }
        }
    }


    //==============================================span_image_filter_rgba_nn
    /// <summary>
    /// Nearest Neighbor
    /// </summary>
    public class ImgSpanGenRGBA_NN : ImgSpanGen
    {
        //NN: nearest neighbor with/without tranformation***

        public override void GenerateColors(Drawing.Color[] outputColors, int startIndex, int x, int y, int len)
        {

            ISpanInterpolator spanInterpolator = Interpolator;
            spanInterpolator.Begin(x + dx, y + dy, len);
            unsafe
            {
                using (CpuBlit.Imaging.TempMemPtr.FromBmp(_bmpSrc, out int* srcBuffer))
                {
                    //TODO: if no any transformation,=> skip spanInterpolator (see above example)
                    do
                    {
                        int x_hr;
                        int y_hr;
                        spanInterpolator.GetCoord(out x_hr, out y_hr);
                        int x_lr = x_hr >> subpix_const.SHIFT;
                        int y_lr = y_hr >> subpix_const.SHIFT;

                        int bufferIndex = _bmpSrc.GetBufferOffsetXY32(x_lr, y_lr);
                        int srcColor = srcBuffer[bufferIndex++];

                        outputColors[startIndex] = Drawing.Color.FromArgb(
                              (srcColor >> CO.A_SHIFT) & 0xff, //a
                              (srcColor >> CO.R_SHIFT) & 0xff, //r
                              (srcColor >> CO.G_SHIFT) & 0xff, //g
                              (srcColor >> CO.B_SHIFT) & 0xff);//b 

                        ++startIndex;
                        spanInterpolator.Next();

                    } while (--len != 0);
                }
            }
        }
    }


    public class ImgSpanGenRGBA_BilinearClip : ImgSpanGen
    {

        bool _noTransformation = false;
        public ImgSpanGenRGBA_BilinearClip(Drawing.Color back_color)
        {
            BackgroundColor = back_color;
        }

        public override void Prepare()
        {
            base.Prepare();

            ISpanInterpolator spanInterpolator = base.Interpolator;

            _noTransformation = (spanInterpolator.GetType() == typeof(SpanInterpolatorLinear)
                && ((SpanInterpolatorLinear)spanInterpolator).Transformer.GetType() == typeof(VertexProcessing.Affine)
                && ((VertexProcessing.Affine)((SpanInterpolatorLinear)spanInterpolator).Transformer).IsIdentity());
        }


        public sealed override void GenerateColors(Drawing.Color[] outputColors, int startIndex, int x, int y, int len)
        {
#if DEBUG
            int tmp_len = len;
#endif
            unsafe
            {
                //TODO: review here 

                if (_noTransformation)
                {
                    using (CpuBlit.Imaging.TempMemPtr.FromBmp(_bmpSrc, out int* srcBuffer))
                    {
                        int bufferIndex = _bmpSrc.GetBufferOffsetXY32(x, y);
                        do
                        {
                            //TODO: review here, match component?
                            //ORDER IS IMPORTANT!
                            //TODO : use CO (color order instead)
                            int srcColor = srcBuffer[bufferIndex++];
                            outputColors[startIndex] = Drawing.Color.FromArgb(
                              (srcColor >> CO.A_SHIFT) & 0xff, //a
                              (srcColor >> CO.R_SHIFT) & 0xff, //r
                              (srcColor >> CO.G_SHIFT) & 0xff, //g
                              (srcColor >> CO.B_SHIFT) & 0xff);//b 

                            ++startIndex;
                        } while (--len != 0);
                    }
                }
                else
                {
                    //Bilinear interpolation, without lookup table
                    ISpanInterpolator spanInterpolator = base.Interpolator;
                    using (CpuBlit.Imaging.TempMemPtr srcBufferPtr = _bmpSrc.GetBufferPtr())
                    {
                        int* srcBuffer = (int*)srcBufferPtr.Ptr;

                        spanInterpolator.Begin(x + base.dx, y + base.dy, len);
                        int accColor0, accColor1, accColor2, accColor3;

                        Color bgColor = this.BackgroundColor;
                        int back_r = bgColor.red;
                        int back_g = bgColor.green;
                        int back_b = bgColor.blue;
                        int back_a = bgColor.alpha;
                        int maxx = _bmpSrc.Width - 1;
                        int maxy = _bmpSrc.Height - 1;
                        int srcColor = 0;


                        do
                        {
                            int x_hr;
                            int y_hr;
                            spanInterpolator.GetCoord(out x_hr, out y_hr);
                            x_hr -= base.dxInt;
                            y_hr -= base.dyInt;
                            int x_lr = x_hr >> subpix_const.SHIFT;
                            int y_lr = y_hr >> subpix_const.SHIFT;
                            int weight;
                            if (x_lr >= 0 && y_lr >= 0 &&
                               x_lr < maxx && y_lr < maxy)
                            {
                                int bufferIndex = _bmpSrc.GetBufferOffsetXY32(x_lr, y_lr);


                                accColor0 =
                                    accColor1 =
                                        accColor2 =
                                            accColor3 = subpix_const.SCALE * subpix_const.SCALE / 2;

                                x_hr &= subpix_const.MASK;
                                y_hr &= subpix_const.MASK;


                                weight = (subpix_const.SCALE - x_hr) * (subpix_const.SCALE - y_hr);

                                if (weight > BASE_MASK)
                                {
                                    srcColor = srcBuffer[bufferIndex];

                                    accColor3 += weight * ((srcColor >> CO.A_SHIFT) & 0xff); //a
                                    accColor0 += weight * ((srcColor >> CO.R_SHIFT) & 0xff); //r
                                    accColor1 += weight * ((srcColor >> CO.G_SHIFT) & 0xff); //g
                                    accColor2 += weight * ((srcColor >> CO.B_SHIFT) & 0xff); //b 

                                }

                                weight = (x_hr * (subpix_const.SCALE - y_hr));

                                if (weight > BASE_MASK)
                                {
                                    bufferIndex++;
                                    srcColor = srcBuffer[bufferIndex];
                                    //
                                    accColor3 += weight * ((srcColor >> CO.A_SHIFT) & 0xff); //a
                                    accColor0 += weight * ((srcColor >> CO.R_SHIFT) & 0xff); //r
                                    accColor1 += weight * ((srcColor >> CO.G_SHIFT) & 0xff); //g
                                    accColor2 += weight * ((srcColor >> CO.B_SHIFT) & 0xff); //b 
                                }

                                weight = ((subpix_const.SCALE - x_hr) * y_hr);

                                if (weight > BASE_MASK)
                                {
                                    ++y_lr;
                                    //
                                    bufferIndex = _bmpSrc.GetBufferOffsetXY32(x_lr, y_lr);
                                    srcColor = srcBuffer[bufferIndex];
                                    //
                                    accColor3 += weight * ((srcColor >> CO.A_SHIFT) & 0xff); //a
                                    accColor0 += weight * ((srcColor >> CO.R_SHIFT) & 0xff); //r
                                    accColor1 += weight * ((srcColor >> CO.G_SHIFT) & 0xff); //g
                                    accColor2 += weight * ((srcColor >> CO.B_SHIFT) & 0xff); //b 
                                }

                                weight = (x_hr * y_hr);

                                if (weight > BASE_MASK)
                                {
                                    bufferIndex++;
                                    srcColor = srcBuffer[bufferIndex];
                                    //
                                    accColor3 += weight * ((srcColor >> CO.A_SHIFT) & 0xff); //a
                                    accColor0 += weight * ((srcColor >> CO.R_SHIFT) & 0xff); //r
                                    accColor1 += weight * ((srcColor >> CO.G_SHIFT) & 0xff); //g
                                    accColor2 += weight * ((srcColor >> CO.B_SHIFT) & 0xff); //b 
                                }
                                accColor0 >>= subpix_const.SHIFT * 2;
                                accColor1 >>= subpix_const.SHIFT * 2;
                                accColor2 >>= subpix_const.SHIFT * 2;
                                accColor3 >>= subpix_const.SHIFT * 2;
                            }
                            else
                            {
                                if (x_lr < -1 || y_lr < -1 ||
                                   x_lr > maxx || y_lr > maxy)
                                {
                                    accColor0 = back_r;
                                    accColor1 = back_g;
                                    accColor2 = back_b;
                                    accColor3 = back_a;
                                }
                                else
                                {

                                    accColor0 =
                                       accColor1 =
                                          accColor2 =
                                            accColor3 = subpix_const.SCALE * subpix_const.SCALE / 2;

                                    x_hr &= subpix_const.MASK;
                                    y_hr &= subpix_const.MASK;

                                    weight = (subpix_const.SCALE - x_hr) * (subpix_const.SCALE - y_hr);

                                    if (weight > BASE_MASK)
                                    {

                                        if ((uint)x_lr <= (uint)maxx && (uint)y_lr <= (uint)maxy)
                                        {
                                            srcColor = srcBuffer[_bmpSrc.GetBufferOffsetXY32(x_lr, y_lr)];
                                            //
                                            accColor3 += weight * ((srcColor >> CO.A_SHIFT) & 0xff); //a
                                            accColor0 += weight * ((srcColor >> CO.R_SHIFT) & 0xff); //r
                                            accColor1 += weight * ((srcColor >> CO.G_SHIFT) & 0xff); //g
                                            accColor2 += weight * ((srcColor >> CO.B_SHIFT) & 0xff); //b 
                                        }
                                        else
                                        {
                                            accColor0 += back_r * weight;
                                            accColor1 += back_g * weight;
                                            accColor2 += back_b * weight;
                                            accColor3 += back_a * weight;
                                        }

                                    }

                                    x_lr++;
                                    weight = x_hr * (subpix_const.SCALE - y_hr);
                                    if (weight > BASE_MASK)
                                    {
                                        if ((uint)x_lr <= (uint)maxx && (uint)y_lr <= (uint)maxy)
                                        {

                                            srcColor = srcBuffer[_bmpSrc.GetBufferOffsetXY32(x_lr, y_lr)];
                                            //
                                            accColor3 += weight * ((srcColor >> CO.A_SHIFT) & 0xff); //a
                                            accColor0 += weight * ((srcColor >> CO.R_SHIFT) & 0xff); //r
                                            accColor1 += weight * ((srcColor >> CO.G_SHIFT) & 0xff); //g
                                            accColor2 += weight * ((srcColor >> CO.B_SHIFT) & 0xff); //b 
                                        }
                                        else
                                        {
                                            accColor0 += back_r * weight;
                                            accColor1 += back_g * weight;
                                            accColor2 += back_b * weight;
                                            accColor3 += back_a * weight;
                                        }
                                    }

                                    x_lr--;
                                    y_lr++;
                                    weight = (subpix_const.SCALE - x_hr) * y_hr;
                                    if (weight > BASE_MASK)
                                    {
                                        if ((uint)x_lr <= (uint)maxx && (uint)y_lr <= (uint)maxy)
                                        {


                                            srcColor = srcBuffer[_bmpSrc.GetBufferOffsetXY32(x_lr, y_lr)];
                                            //
                                            accColor3 += weight * ((srcColor >> CO.A_SHIFT) & 0xff); //a
                                            accColor0 += weight * ((srcColor >> CO.R_SHIFT) & 0xff); //r
                                            accColor1 += weight * ((srcColor >> CO.G_SHIFT) & 0xff); //g
                                            accColor2 += weight * ((srcColor >> CO.B_SHIFT) & 0xff); //b 

                                        }
                                        else
                                        {
                                            accColor0 += back_r * weight;
                                            accColor1 += back_g * weight;
                                            accColor2 += back_b * weight;
                                            accColor3 += back_a * weight;
                                        }
                                    }

                                    x_lr++;
                                    weight = (x_hr * y_hr);
                                    if (weight > BASE_MASK)
                                    {
                                        if ((uint)x_lr <= (uint)maxx && (uint)y_lr <= (uint)maxy)
                                        {
                                            srcColor = srcBuffer[_bmpSrc.GetBufferOffsetXY32(x_lr, y_lr)];
                                            //
                                            accColor3 += weight * ((srcColor >> CO.A_SHIFT) & 0xff); //a
                                            accColor0 += weight * ((srcColor >> CO.R_SHIFT) & 0xff); //r
                                            accColor1 += weight * ((srcColor >> CO.G_SHIFT) & 0xff); //g
                                            accColor2 += weight * ((srcColor >> CO.B_SHIFT) & 0xff); //b 
                                        }
                                        else
                                        {
                                            accColor0 += back_r * weight;
                                            accColor1 += back_g * weight;
                                            accColor2 += back_b * weight;
                                            accColor3 += back_a * weight;
                                        }
                                    }

                                    accColor0 >>= subpix_const.SHIFT * 2;
                                    accColor1 >>= subpix_const.SHIFT * 2;
                                    accColor2 >>= subpix_const.SHIFT * 2;
                                    accColor3 >>= subpix_const.SHIFT * 2;
                                }
                            }

#if DEBUG
                            if (startIndex >= outputColors.Length)
                            {

                            }
#endif
                            outputColors[startIndex] = PixelFarm.Drawing.Color.FromArgb(
                                (byte)accColor3,
                                (byte)accColor0,
                                (byte)accColor1,
                                (byte)accColor2
                                );

                            ++startIndex;
                            spanInterpolator.Next();

                        } while (--len != 0);

                    }//using
                }//else
            }//unsafe
        }
    }


    public class ImgSpanGenRGBA_CustomFilter : ImgSpanGen
    {
        //span_image_filter_rgba
        ImageFilterLookUpTable _lut;
        public ImgSpanGenRGBA_CustomFilter()
        {
        }
        public void SetLookupTable(ImageFilterLookUpTable lut)
        {
            _lut = lut;
        }
        public override void GenerateColors(Color[] outputColors, int startIndex, int x, int y, int len)
        {
            ISpanInterpolator spanInterpolator = this.Interpolator;
            //int f_r, f_g, f_b, f_a;//accumulate color components
            int accColor0, accColor1, accColor2, accColor3;
            int diameter = _lut.Diameter;
            int start = _lut.Start;
            int[] weight_array = _lut.WeightArray;

            int x_count;
            int weight_y;

            unsafe
            {
                using (CpuBlit.Imaging.TempMemPtr srcBufferPtr = _bmpSrc.GetBufferPtr())
                {
                    int* srcBuffer = (int*)srcBufferPtr.Ptr;
                    spanInterpolator.Begin(x + base.dx, y + base.dy, len);
                    int src_color = 0;

                    do
                    {
                        spanInterpolator.GetCoord(out x, out y);

                        x -= base.dxInt;
                        y -= base.dyInt;

                        int x_hr = x;
                        int y_hr = y;

                        int x_lr = x_hr >> subpix_const.SHIFT;
                        int y_lr = y_hr >> subpix_const.SHIFT;

                        accColor0 =
                           accColor1 =
                              accColor2 =
                                accColor3 = filter_const.SCALE / 2;


                        int x_fract = x_hr & subpix_const.MASK;
                        int y_count = diameter;

                        y_hr = subpix_const.MASK - (y_hr & subpix_const.MASK);
                        int bufferIndex = _bmpSrc.GetBufferOffsetXY32(x_lr, y_lr);

                        int tmp_Y = y_lr;
                        for (; ; )
                        {
                            x_count = diameter;
                            weight_y = weight_array[y_hr];
                            x_hr = subpix_const.MASK - x_fract;

                            //-------------------
                            for (; ; )
                            {
                                int weight = (weight_y * weight_array[x_hr] +
                                              filter_const.SCALE / 2) >>
                                              filter_const.SHIFT;

                                int srcColor = srcBuffer[bufferIndex];

                                accColor3 += weight * ((srcColor >> CO.A_SHIFT) & 0xff); //a
                                accColor0 += weight * ((srcColor >> CO.R_SHIFT) & 0xff); //r
                                accColor1 += weight * ((srcColor >> CO.G_SHIFT) & 0xff); //g
                                accColor2 += weight * ((srcColor >> CO.B_SHIFT) & 0xff); //b 

                                if (--x_count == 0) break; //for

                                x_hr += subpix_const.SCALE;
                                bufferIndex++;
                            }
                            //-------------------

                            if (--y_count == 0) break;
                            y_hr += (int)subpix_const.SCALE;

                            tmp_Y++; //move down to next row-> and find start bufferIndex
                            bufferIndex = _bmpSrc.GetBufferOffsetXY32(x_lr, tmp_Y);
                        }

                        accColor0 >>= filter_const.SHIFT;
                        accColor1 >>= filter_const.SHIFT;
                        accColor2 >>= filter_const.SHIFT;
                        accColor3 >>= filter_const.SHIFT;

                        unchecked
                        {
                            if ((uint)accColor0 > BASE_MASK)
                            {
                                if (accColor0 < 0) accColor0 = 0;
                                if (accColor0 > BASE_MASK) accColor0 = BASE_MASK;
                            }

                            if ((uint)accColor1 > BASE_MASK)
                            {
                                if (accColor1 < 0) accColor1 = 0;
                                if (accColor1 > BASE_MASK) accColor1 = BASE_MASK;
                            }

                            if ((uint)accColor2 > BASE_MASK)
                            {
                                if (accColor2 < 0) accColor2 = 0;
                                if (accColor2 > BASE_MASK) accColor2 = BASE_MASK;
                            }

                            if ((uint)accColor3 > BASE_MASK)
                            {
                                if (accColor3 < 0) accColor3 = 0;
                                if (accColor3 > BASE_MASK) accColor3 = BASE_MASK;
                            }
                        }
                        outputColors[startIndex] = PixelFarm.Drawing.Color.FromArgb(
                               (byte)accColor3, //a
                               (byte)accColor2, //
                               (byte)accColor1,
                               (byte)accColor0
                               );
                        startIndex++;

                        spanInterpolator.Next();
                    } while (--len != 0);

                }
            }
        }
    }
     

}






