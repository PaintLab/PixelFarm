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
// The Stack Blur Algorithm was invented by Mario Klingemann, 
// mario@quasimondo.com and described here:
// http://incubator.quasimondo.com/processing/fast_blur_deluxe.php
// (search phrase "Stackblur: Fast But Goodlooking"). 
// The major improvement is that there's no more division table
// that was very expensive to create for large blur radii. Insted, 
// for 8-bit per channel and radius not exceeding 254 the division is 
// replaced by multiplication and shift. 
//
//----------------------------------------------------------------------------
using System;
using MatterHackers.Agg.Image;

namespace MatterHackers.Agg.Image
{
#if true
    public struct RGBA_Ints
    {
        public int r;
        public int g;
        public int b;
        public int a;
    };

    struct stack_blur_tables
    {
        public static ushort[] g_stack_blur8_mul = 
        {
            512,512,456,512,328,456,335,512,405,328,271,456,388,335,292,512,
            454,405,364,328,298,271,496,456,420,388,360,335,312,292,273,512,
            482,454,428,405,383,364,345,328,312,298,284,271,259,496,475,456,
            437,420,404,388,374,360,347,335,323,312,302,292,282,273,265,512,
            497,482,468,454,441,428,417,405,394,383,373,364,354,345,337,328,
            320,312,305,298,291,284,278,271,265,259,507,496,485,475,465,456,
            446,437,428,420,412,404,396,388,381,374,367,360,354,347,341,335,
            329,323,318,312,307,302,297,292,287,282,278,273,269,265,261,512,
            505,497,489,482,475,468,461,454,447,441,435,428,422,417,411,405,
            399,394,389,383,378,373,368,364,359,354,350,345,341,337,332,328,
            324,320,316,312,309,305,301,298,294,291,287,284,281,278,274,271,
            268,265,262,259,257,507,501,496,491,485,480,475,470,465,460,456,
            451,446,442,437,433,428,424,420,416,412,408,404,400,396,392,388,
            385,381,377,374,370,367,363,360,357,354,350,347,344,341,338,335,
            332,329,326,323,320,318,315,312,310,307,304,302,299,297,294,292,
            289,287,285,282,280,278,275,273,271,269,267,265,263,261,259
        };

        public static byte[] g_stack_blur8_shr = 
        {
              9, 11, 12, 13, 13, 14, 14, 15, 15, 15, 15, 16, 16, 16, 16, 17, 
             17, 17, 17, 17, 17, 17, 18, 18, 18, 18, 18, 18, 18, 18, 18, 19, 
             19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 20, 20, 20,
             20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 21,
             21, 21, 21, 21, 21, 21, 21, 21, 21, 21, 21, 21, 21, 21, 21, 21,
             21, 21, 21, 21, 21, 21, 21, 21, 21, 21, 22, 22, 22, 22, 22, 22, 
             22, 22, 22, 22, 22, 22, 22, 22, 22, 22, 22, 22, 22, 22, 22, 22,
             22, 22, 22, 22, 22, 22, 22, 22, 22, 22, 22, 22, 22, 22, 22, 23, 
             23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23,
             23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23,
             23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 
             23, 23, 23, 23, 23, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 
             24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24,
             24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24,
             24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24,
             24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24
        };
    };

    //==============================================================stack_blur
    public class stack_blur
    {
        //private VectorPOD<RGBA_Ints> m_buff;
        //private VectorPOD<int> m_stack;
        //int base_mask = 255;

        enum order_e
        {
            R = 2,
            G = 1,
            B = 0,
            A = 3
        };

        public void blur_x(IImage img, int radius)
        {

            throw new NotSupportedException();
#if false
            if(radius < 1) return;

            int x, y, xp, i;
            int stack_ptr;
            int stack_start;

            color_type      pix;
            color_type*     stack_pix;
            calculator_type sum;
            calculator_type sum_in;
            calculator_type sum_out;

            int w   = img.width();
            int h   = img.height();
            int wm  = w - 1;
            int div = radius * 2 + 1;

            int div_sum = (radius + 1) * (radius + 1);
            int mul_sum = 0;
            int shr_sum = 0;
            int max_val = base_mask;

            if(max_val <= 255 && radius < 255)
            {
                mul_sum = stack_blur_tables.g_stack_blur8_mul[radius];
                shr_sum = stack_blur_tables.g_stack_blur8_shr[radius];
            }

            m_buf.allocate(w, 128);
            m_stack.allocate(div, 32);

            for(y = 0; y < h; y++)
            {
                sum.clear();
                sum_in.clear();
                sum_out.clear();

                pix = img.pixel(0, y);
                for(i = 0; i <= radius; i++)
                {
                    m_stack[i] = pix;
                    sum.add(pix, i + 1);
                    sum_out.add(pix);
                }
                for(i = 1; i <= radius; i++)
                {
                    pix = img.pixel((i > wm) ? wm : i, y);
                    m_stack[i + radius] = pix;
                    sum.add(pix, radius + 1 - i);
                    sum_in.add(pix);
                }

                stack_ptr = radius;
                for(x = 0; x < w; x++)
                {
                    if(mul_sum) sum.calc_pix(m_buf[x], mul_sum, shr_sum);
                    else        sum.calc_pix(m_buf[x], div_sum);

                    sum.sub(sum_out);
           
                    stack_start = stack_ptr + div - radius;
                    if(stack_start >= div) stack_start -= div;
                    stack_pix = &m_stack[stack_start];

                    sum_out.sub(*stack_pix);

                    xp = x + radius + 1;
                    if(xp > wm) xp = wm;
                    pix = img.pixel(xp, y);
            
                    *stack_pix = pix;
            
                    sum_in.add(pix);
                    sum.add(sum_in);
            
                    ++stack_ptr;
                    if(stack_ptr >= div) stack_ptr = 0;
                    stack_pix = &m_stack[stack_ptr];

                    sum_out.add(*stack_pix);
                    sum_in.sub(*stack_pix);
                }
                img.copy_color_hspan(0, y, w, &m_buf[0]);
            }
#endif
        }

        public void blur_y(IImage img, int radius)
        {
            FormatTransposer img2 = new FormatTransposer(img);
            blur_x(img2, radius);
        }

        public void blur(ImageBase img, int radius)
        {
            blur_x(img, radius);
            FormatTransposer img2 = new FormatTransposer(img);
            blur_x(img2, radius);
        }


        void stack_blur_gray8(ImageBase img, int rx, int ry)
        {
            throw new NotImplementedException();
#if false
            int x, y, xp, yp, i;
            int stack_ptr;
            int stack_start;

            byte* src_pix_ptr;
                  byte* dst_pix_ptr;
            int pix;
            int stack_pix;
            int sum;
            int sum_in;
            int sum_out;

            int w   = img.width();
            int h   = img.height();
            int wm  = w - 1;
            int hm  = h - 1;

            int div;
            int mul_sum;
            int shr_sum;

            pod_vector<byte> stack;

            if(rx > 0)
            {
                if(rx > 254) rx = 254;
                div = rx * 2 + 1;
                mul_sum = stack_blur_tables.g_stack_blur8_mul[rx];
                shr_sum = stack_blur_tables.g_stack_blur8_shr[rx];
                stack.allocate(div);

                for(y = 0; y < h; y++)
                {
                    sum = sum_in = sum_out = 0;

                    src_pix_ptr = img.pix_ptr(0, y);
                    pix = *src_pix_ptr;
                    for(i = 0; i <= rx; i++)
                    {
                        stack[i] = pix;
                        sum     += pix * (i + 1);
                        sum_out += pix;
                    }
                    for(i = 1; i <= rx; i++)
                    {
                        if(i <= wm) src_pix_ptr += Img::pix_step; 
                        pix = *src_pix_ptr; 
                        stack[i + rx] = pix;
                        sum    += pix * (rx + 1 - i);
                        sum_in += pix;
                    }

                    stack_ptr = rx;
                    xp = rx;
                    if(xp > wm) xp = wm;
                    src_pix_ptr = img.pix_ptr(xp, y);
                    dst_pix_ptr = img.pix_ptr(0, y);
                    for(x = 0; x < w; x++)
                    {
                        *dst_pix_ptr = (sum * mul_sum) >> shr_sum;
                        dst_pix_ptr += Img::pix_step;

                        sum -= sum_out;
           
                        stack_start = stack_ptr + div - rx;
                        if(stack_start >= div) stack_start -= div;
                        sum_out -= stack[stack_start];

                        if(xp < wm) 
                        {
                            src_pix_ptr += Img::pix_step;
                            pix = *src_pix_ptr;
                            ++xp;
                        }
            
                        stack[stack_start] = pix;
            
                        sum_in += pix;
                        sum    += sum_in;
            
                        ++stack_ptr;
                        if(stack_ptr >= div) stack_ptr = 0;
                        stack_pix = stack[stack_ptr];

                        sum_out += stack_pix;
                        sum_in  -= stack_pix;
                    }
                }
            }

            if(ry > 0)
            {
                if(ry > 254) ry = 254;
                div = ry * 2 + 1;
                mul_sum = stack_blur_tables.g_stack_blur8_mul[ry];
                shr_sum = stack_blur_tables.g_stack_blur8_shr[ry];
                stack.allocate(div);

                int stride = img.stride();
                for(x = 0; x < w; x++)
                {
                    sum = sum_in = sum_out = 0;

                    src_pix_ptr = img.pix_ptr(x, 0);
                    pix = *src_pix_ptr;
                    for(i = 0; i <= ry; i++)
                    {
                        stack[i] = pix;
                        sum     += pix * (i + 1);
                        sum_out += pix;
                    }
                    for(i = 1; i <= ry; i++)
                    {
                        if(i <= hm) src_pix_ptr += stride; 
                        pix = *src_pix_ptr; 
                        stack[i + ry] = pix;
                        sum    += pix * (ry + 1 - i);
                        sum_in += pix;
                    }

                    stack_ptr = ry;
                    yp = ry;
                    if(yp > hm) yp = hm;
                    src_pix_ptr = img.pix_ptr(x, yp);
                    dst_pix_ptr = img.pix_ptr(x, 0);
                    for(y = 0; y < h; y++)
                    {
                        *dst_pix_ptr = (sum * mul_sum) >> shr_sum;
                        dst_pix_ptr += stride;

                        sum -= sum_out;
           
                        stack_start = stack_ptr + div - ry;
                        if(stack_start >= div) stack_start -= div;
                        sum_out -= stack[stack_start];

                        if(yp < hm) 
                        {
                            src_pix_ptr += stride;
                            pix = *src_pix_ptr;
                            ++yp;
                        }
            
                        stack[stack_start] = pix;
            
                        sum_in += pix;
                        sum    += sum_in;
            
                        ++stack_ptr;
                        if(stack_ptr >= div) stack_ptr = 0;
                        stack_pix = stack[stack_ptr];

                        sum_out += stack_pix;
                        sum_in  -= stack_pix;
                    }
                }
            }
#endif
        }

        public void Blur(ImageBase img, int rx, int ry)
        {
            switch (img.BitDepth)
            {
                case 24:
                    stack_blur_bgr24(img, rx, ry);
                    break;

                case 32:
                    stack_blur_bgra32(img, rx, ry);
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        private void stack_blur_bgr24(ImageBase img, int rx, int ry)
        {
            throw new NotImplementedException();
#if false
            //typedef typename Img::color_type color_type;
            //typedef typename Img::order_type order_type;

            int x, y, xp, yp, i;
            int stack_ptr;
            int stack_start;

            byte* src_pix_ptr;
                  byte* dst_pix_ptr;
            color_type*  stack_pix_ptr;

            int sum_r;
            int sum_g;
            int sum_b;
            int sum_in_r;
            int sum_in_g;
            int sum_in_b;
            int sum_out_r;
            int sum_out_g;
            int sum_out_b;

            int w   = img.width();
            int h   = img.height();
            int wm  = w - 1;
            int hm  = h - 1;

            int div;
            int mul_sum;
            int shr_sum;

            pod_vector<color_type> stack;

            if(rx > 0)
            {
                if(rx > 254) rx = 254;
                div = rx * 2 + 1;
                mul_sum = stack_blur_tables.g_stack_blur8_mul[rx];
                shr_sum = stack_blur_tables.g_stack_blur8_shr[rx];
                stack.allocate(div);

                for(y = 0; y < h; y++)
                {
                    sum_r = 
                    sum_g = 
                    sum_b = 
                    sum_in_r = 
                    sum_in_g = 
                    sum_in_b = 
                    sum_out_r = 
                    sum_out_g = 
                    sum_out_b = 0;

                    src_pix_ptr = img.pix_ptr(0, y);
                    for(i = 0; i <= rx; i++)
                    {
                        stack_pix_ptr    = &stack[i];
                        stack_pix_ptr->r = src_pix_ptr[R];
                        stack_pix_ptr->g = src_pix_ptr[G];
                        stack_pix_ptr->b = src_pix_ptr[B];
                        sum_r           += src_pix_ptr[R] * (i + 1);
                        sum_g           += src_pix_ptr[G] * (i + 1);
                        sum_b           += src_pix_ptr[B] * (i + 1);
                        sum_out_r       += src_pix_ptr[R];
                        sum_out_g       += src_pix_ptr[G];
                        sum_out_b       += src_pix_ptr[B];
                    }
                    for(i = 1; i <= rx; i++)
                    {
                        if(i <= wm) src_pix_ptr += Img::pix_width; 
                        stack_pix_ptr = &stack[i + rx];
                        stack_pix_ptr->r = src_pix_ptr[R];
                        stack_pix_ptr->g = src_pix_ptr[G];
                        stack_pix_ptr->b = src_pix_ptr[B];
                        sum_r           += src_pix_ptr[R] * (rx + 1 - i);
                        sum_g           += src_pix_ptr[G] * (rx + 1 - i);
                        sum_b           += src_pix_ptr[B] * (rx + 1 - i);
                        sum_in_r        += src_pix_ptr[R];
                        sum_in_g        += src_pix_ptr[G];
                        sum_in_b        += src_pix_ptr[B];
                    }

                    stack_ptr = rx;
                    xp = rx;
                    if(xp > wm) xp = wm;
                    src_pix_ptr = img.pix_ptr(xp, y);
                    dst_pix_ptr = img.pix_ptr(0, y);
                    for(x = 0; x < w; x++)
                    {
                        dst_pix_ptr[R] = (sum_r * mul_sum) >> shr_sum;
                        dst_pix_ptr[G] = (sum_g * mul_sum) >> shr_sum;
                        dst_pix_ptr[B] = (sum_b * mul_sum) >> shr_sum;
                        dst_pix_ptr   += Img::pix_width;

                        sum_r -= sum_out_r;
                        sum_g -= sum_out_g;
                        sum_b -= sum_out_b;
           
                        stack_start = stack_ptr + div - rx;
                        if(stack_start >= div) stack_start -= div;
                        stack_pix_ptr = &stack[stack_start];

                        sum_out_r -= stack_pix_ptr->r;
                        sum_out_g -= stack_pix_ptr->g;
                        sum_out_b -= stack_pix_ptr->b;

                        if(xp < wm) 
                        {
                            src_pix_ptr += Img::pix_width;
                            ++xp;
                        }
            
                        stack_pix_ptr->r = src_pix_ptr[R];
                        stack_pix_ptr->g = src_pix_ptr[G];
                        stack_pix_ptr->b = src_pix_ptr[B];
            
                        sum_in_r += src_pix_ptr[R];
                        sum_in_g += src_pix_ptr[G];
                        sum_in_b += src_pix_ptr[B];
                        sum_r    += sum_in_r;
                        sum_g    += sum_in_g;
                        sum_b    += sum_in_b;
            
                        ++stack_ptr;
                        if(stack_ptr >= div) stack_ptr = 0;
                        stack_pix_ptr = &stack[stack_ptr];

                        sum_out_r += stack_pix_ptr->r;
                        sum_out_g += stack_pix_ptr->g;
                        sum_out_b += stack_pix_ptr->b;
                        sum_in_r  -= stack_pix_ptr->r;
                        sum_in_g  -= stack_pix_ptr->g;
                        sum_in_b  -= stack_pix_ptr->b;
                    }
                }
            }

            if(ry > 0)
            {
                if(ry > 254) ry = 254;
                div = ry * 2 + 1;
                mul_sum = stack_blur_tables.g_stack_blur8_mul[ry];
                shr_sum = stack_blur_tables.g_stack_blur8_shr[ry];
                stack.allocate(div);

                int stride = img.stride();
                for(x = 0; x < w; x++)
                {
                    sum_r = 
                    sum_g = 
                    sum_b = 
                    sum_in_r = 
                    sum_in_g = 
                    sum_in_b = 
                    sum_out_r = 
                    sum_out_g = 
                    sum_out_b = 0;

                    src_pix_ptr = img.pix_ptr(x, 0);
                    for(i = 0; i <= ry; i++)
                    {
                        stack_pix_ptr    = &stack[i];
                        stack_pix_ptr->r = src_pix_ptr[R];
                        stack_pix_ptr->g = src_pix_ptr[G];
                        stack_pix_ptr->b = src_pix_ptr[B];
                        sum_r           += src_pix_ptr[R] * (i + 1);
                        sum_g           += src_pix_ptr[G] * (i + 1);
                        sum_b           += src_pix_ptr[B] * (i + 1);
                        sum_out_r       += src_pix_ptr[R];
                        sum_out_g       += src_pix_ptr[G];
                        sum_out_b       += src_pix_ptr[B];
                    }
                    for(i = 1; i <= ry; i++)
                    {
                        if(i <= hm) src_pix_ptr += stride; 
                        stack_pix_ptr = &stack[i + ry];
                        stack_pix_ptr->r = src_pix_ptr[R];
                        stack_pix_ptr->g = src_pix_ptr[G];
                        stack_pix_ptr->b = src_pix_ptr[B];
                        sum_r           += src_pix_ptr[R] * (ry + 1 - i);
                        sum_g           += src_pix_ptr[G] * (ry + 1 - i);
                        sum_b           += src_pix_ptr[B] * (ry + 1 - i);
                        sum_in_r        += src_pix_ptr[R];
                        sum_in_g        += src_pix_ptr[G];
                        sum_in_b        += src_pix_ptr[B];
                    }

                    stack_ptr = ry;
                    yp = ry;
                    if(yp > hm) yp = hm;
                    src_pix_ptr = img.pix_ptr(x, yp);
                    dst_pix_ptr = img.pix_ptr(x, 0);
                    for(y = 0; y < h; y++)
                    {
                        dst_pix_ptr[R] = (sum_r * mul_sum) >> shr_sum;
                        dst_pix_ptr[G] = (sum_g * mul_sum) >> shr_sum;
                        dst_pix_ptr[B] = (sum_b * mul_sum) >> shr_sum;
                        dst_pix_ptr += stride;

                        sum_r -= sum_out_r;
                        sum_g -= sum_out_g;
                        sum_b -= sum_out_b;
           
                        stack_start = stack_ptr + div - ry;
                        if(stack_start >= div) stack_start -= div;

                        stack_pix_ptr = &stack[stack_start];
                        sum_out_r -= stack_pix_ptr->r;
                        sum_out_g -= stack_pix_ptr->g;
                        sum_out_b -= stack_pix_ptr->b;

                        if(yp < hm) 
                        {
                            src_pix_ptr += stride;
                            ++yp;
                        }
            
                        stack_pix_ptr->r = src_pix_ptr[R];
                        stack_pix_ptr->g = src_pix_ptr[G];
                        stack_pix_ptr->b = src_pix_ptr[B];
            
                        sum_in_r += src_pix_ptr[R];
                        sum_in_g += src_pix_ptr[G];
                        sum_in_b += src_pix_ptr[B];
                        sum_r    += sum_in_r;
                        sum_g    += sum_in_g;
                        sum_b    += sum_in_b;
            
                        ++stack_ptr;
                        if(stack_ptr >= div) stack_ptr = 0;
                        stack_pix_ptr = &stack[stack_ptr];

                        sum_out_r += stack_pix_ptr->r;
                        sum_out_g += stack_pix_ptr->g;
                        sum_out_b += stack_pix_ptr->b;
                        sum_in_r  -= stack_pix_ptr->r;
                        sum_in_g  -= stack_pix_ptr->g;
                        sum_in_b  -= stack_pix_ptr->b;
                    }
                }
            }
#endif
        }


        class BlurStack
        {
            public int r;
            public int g;
            public int b;
            public int a;
            public BlurStack() { }
            public BlurStack(byte r, byte g, byte b, byte a)
            {
                this.r = r;
                this.g = g;
                this.b = b;
                this.a = a;
            }
        }

        class CircularBlurStack
        {
            int currentHeadIndex;
            int currentTailIndex;

            int size;
            BlurStack[] blurValues;
            public CircularBlurStack(int size)
            {
                this.size = size;
                this.blurValues = new BlurStack[size];
                this.currentHeadIndex = 0;
                this.currentTailIndex = size - 1;
                for (int i = size - 1; i >= 0; --i)
                {
                    blurValues[i] = new BlurStack();

                }
            }
            public void Prepare(int count, int r, int g, int b, int a)
            {

                this.currentHeadIndex = 0;
                this.currentTailIndex = size - 1;
                for (int i = 0; i < count; ++i)
                {
                    blurValues[i] = new BlurStack((byte)r, (byte)g, (byte)b, (byte)a);
                    this.Next();
                }

            }
            public void ResetHeadTailPosition()
            {
                this.currentHeadIndex = 0;
                this.currentTailIndex = size - 1;
            }
            public void Next()
            {

                //--------------------------
                if (currentHeadIndex + 1 < size)
                {
                    currentHeadIndex++;
                }
                else
                {
                    currentHeadIndex = 0;
                }
                //--------------------------

                if (currentTailIndex + 1 < size)
                {
                    currentTailIndex++;
                }
                else
                {
                    currentTailIndex = 0;
                }
            }
            public BlurStack CurrentHeadColor
            {
                get
                {
                    return this.blurValues[this.currentHeadIndex];
                }
            }
            public BlurStack CurrentTailColor
            {
                get
                {
                    return this.blurValues[this.currentTailIndex];
                }
            }


        }

        private void stack_blur_bgra32(ImageBase img, int radius, int ry)
        {

            int width = img.Width;
            int w4 = img.Width * 4;
            int height = img.Height;
            int[] srcBuffer = new int[width * height];

            ImageBase.CopySubBufferToInt32Array(img, 0, 0, width, height, srcBuffer);
            //int i = 0; 
            //for (int y = 0; y < height; ++y)
            //{
            //    for (int x = 0; x < width; ++x)
            //    {
            //        RGBA_Bytes px = img.GetPixel(x, y);
            //        srcBuffer[i] = px.blue |
            //                       (px.green << 8) |
            //                       (px.red << 16);
            //        i++;
            //    }
            //}


            //int[] destBuffer = new int[srcBuffer.Length];
            StackBlurARGB.FastBlur32ARGB(srcBuffer, srcBuffer, img.Width, img.Height, radius);


            int i = 0;
            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    int dest = srcBuffer[i];
                    img.SetPixel(x, y,
                       new ColorRGBA(
                           (byte)((dest >> 16) & 0xff),
                           (byte)((dest >> 8) & 0xff),
                           (byte)((dest) & 0xff)));
                    i++;
                }
            }

            ////n = 0;
            //for (i = 0; i < lim; ++i)
            //{
            //    int dest = destBuffer[n];
            //    //buffer1[i] = (byte)((dest >> 24) & 0xff); //R
            //    //buffer1[i + 1] = (byte)((dest >> 16) & 0xff); //G
            //    //buffer1[i + 2] = (byte)((dest >> 8) & 0xff);//B
            //    //buffer1[i + 3] = (byte)((dest) & 0xff); //A 

            //    //from ARGB
            //    byte b_ = (byte)((dest) & 0xff);
            //    byte g_ = (byte)((dest >> 8) & 0xff);
            //    byte r_ = (byte)((dest >> 16) & 0xff);
            //    byte a_ = (byte)((dest >> 24) & 0xff);

            //    buffer1[i] = b_;
            //    buffer1[i + 1] = g_;
            //    buffer1[i + 2] = r_;
            //    buffer1[i + 3] = a_;

            //    i += 4;
            //    n++;
            //}
            // Buffer.BlockCopy(destBuffer, 0, buffer1, 0, buffer1.Length);
        }


        private void stack_blur_bgra32_2(ImageBase img, int radius, int ry)
        {


            //------------------------------------------------------------ 
            //Original StackBlue  Author:Mario Klingemann 
            //StackBlur - a fast almost Gaussian Blur For Canvas

            //Version: 	0.5
            //Author:		Mario Klingemann
            //Contact: 	mario@quasimondo.com
            //Website:	http://www.quasimondo.com/StackBlurForCanvas
            //Twitter:	@quasimondo
            //(port from js version)

            //------------------------------------------------------------
            if (radius < 1)
            {
                return;
            }
            if (radius > 254) radius = 254;


            byte[] imgBuffer = img.GetBuffer();
            //------------------------------------------------------------
            int r_sum, g_sum, b_sum, a_sum;
            int r_in_sum, g_in_sum, b_in_sum, a_in_sum;
            int r_out_sum, g_out_sum, b_out_sum, a_out_sum;
            //------------------------------------------------------------
            int p = 0;
            int width = img.Width; //original img width
            int height = img.Height;//original img height
            int div = (radius + radius) + 1;

            int widthMinus1 = width - 1;
            int heightMinus1 = height - 1;
            int radiusPlus1 = radius + 1;
            int sumFactor = radiusPlus1 * (radiusPlus1 + 1) / 2;

            //prepare blur stack             
            CircularBlurStack circularBlurStack = new CircularBlurStack(div);

            int y = 0;
            int yp = 0;

            int mul_sum = stack_blur_tables.g_stack_blur8_mul[radius];
            int shg_sum = stack_blur_tables.g_stack_blur8_shr[radius];

            int yi = 0;
            int yw = 0;


            for (y = 0; y < height; ++y)
            {
                int pr, pg, pb, pa;//pixel rgba
                int rbs;

                //---------------------------------------------------
                //reset
                r_in_sum = g_in_sum = b_in_sum = a_in_sum =
                     r_sum = g_sum = b_sum = a_sum = 0;


                r_out_sum = radiusPlus1 * (pr = imgBuffer[yi + (int)order_e.R]);
                g_out_sum = radiusPlus1 * (pg = imgBuffer[yi + (int)order_e.G]);
                b_out_sum = radiusPlus1 * (pb = imgBuffer[yi + (int)order_e.B]);
                a_out_sum = radiusPlus1 * (pa = imgBuffer[yi + (int)order_e.A]);

                r_sum += sumFactor * pr;
                g_sum += sumFactor * pg;
                b_sum += sumFactor * pb;
                a_sum += sumFactor * pa;

                //---------------------------------------------------  
                //reset to start point and prepare  color values
                circularBlurStack.ResetHeadTailPosition();
                circularBlurStack.Prepare(radiusPlus1, pr, pg, pb, pa);
                //--------------------------------------------------- 

                for (int i = 1; i < radiusPlus1; ++i)
                {
                    p = yi + ((widthMinus1 < i ? widthMinus1 : i) << 2);
                    var stackColor = circularBlurStack.CurrentHeadColor;

                    r_sum += (stackColor.r = (pr = imgBuffer[p + (int)order_e.R])) * (rbs = radiusPlus1 - i);
                    g_sum += (stackColor.g = (pg = imgBuffer[p + (int)order_e.G])) * rbs;
                    b_sum += (stackColor.b = (pb = imgBuffer[p + (int)order_e.B])) * rbs;
                    a_sum += (stackColor.a = (pa = imgBuffer[p + (int)order_e.A])) * rbs;

                    r_in_sum += pr;
                    g_in_sum += pg;
                    b_in_sum += pb;
                    a_in_sum += pa;

                    circularBlurStack.Next();
                }

                circularBlurStack.ResetHeadTailPosition();

                for (int x = 0; x < width; ++x)
                {
                    pa = (a_sum * mul_sum) >> shg_sum;
                    if (pa != 0)
                    {
                        //has alpha 
                        int pa_under_255 = 255 / 255;
                        imgBuffer[yi + (int)order_e.R] = (byte)(((r_sum * mul_sum) >> shg_sum) * pa_under_255);
                        imgBuffer[yi + (int)order_e.G] = (byte)(((g_sum * mul_sum) >> shg_sum) * pa_under_255);
                        imgBuffer[yi + (int)order_e.B] = (byte)(((b_sum * mul_sum) >> shg_sum) * pa_under_255);
                        imgBuffer[yi + (int)order_e.A] = (byte)pa;

                    }
                    else
                    {
                        //not visible if alpha =0
                        imgBuffer[yi + (int)order_e.R] =
                            imgBuffer[yi + (int)order_e.G] =
                            imgBuffer[yi + (int)order_e.B] =
                            imgBuffer[yi + (int)order_e.A] = 0;
                    }

                    r_sum -= r_out_sum;
                    g_sum -= g_out_sum;
                    b_sum -= b_out_sum;
                    a_sum -= a_out_sum;

                    var stackInColor = circularBlurStack.CurrentHeadColor;

                    r_out_sum -= stackInColor.r;
                    g_out_sum -= stackInColor.g;
                    b_out_sum -= stackInColor.b;
                    a_out_sum -= stackInColor.a;

                    p = (yw + ((p = x + radius + 1) < widthMinus1 ? p : widthMinus1)) << 2;

                    r_in_sum += (stackInColor.r = imgBuffer[p + (int)order_e.R]);
                    g_in_sum += (stackInColor.g = imgBuffer[p + (int)order_e.G]);
                    b_in_sum += (stackInColor.b = imgBuffer[p + (int)order_e.B]);
                    a_in_sum += (stackInColor.a = imgBuffer[p + (int)order_e.A]);

                    r_sum += r_in_sum;
                    g_sum += g_in_sum;
                    b_sum += b_in_sum;
                    a_sum += a_in_sum;


                    var stackOutColor = circularBlurStack.CurrentTailColor;
                    r_out_sum += (pr = stackOutColor.r);
                    g_out_sum += (pg = stackOutColor.g);
                    b_out_sum += (pb = stackOutColor.b);
                    a_out_sum += (pa = stackOutColor.a);

                    r_in_sum -= pr;
                    g_in_sum -= pg;
                    b_in_sum -= pb;
                    a_in_sum -= pa;

                    circularBlurStack.Next();
                    yi += 4;
                }
                yw += width;
            }

            //end y loop
            //------  
            //begin x loop  
            for (int x = 0; x < width; ++x)
            {

                r_in_sum = g_in_sum = b_in_sum = a_in_sum
                    = r_sum = g_sum = b_sum = a_sum = 0;

                yi = x << 2;

                int pr, pg, pb, pa;//pixel rgba
                int rbs;
                r_out_sum = radiusPlus1 * (pr = imgBuffer[yi + (int)order_e.R]);
                g_out_sum = radiusPlus1 * (pg = imgBuffer[yi + (int)order_e.G]);
                b_out_sum = radiusPlus1 * (pb = imgBuffer[yi + (int)order_e.B]);
                a_out_sum = radiusPlus1 * (pa = imgBuffer[yi + (int)order_e.A]);

                r_sum += sumFactor * pr;
                g_sum += sumFactor * pg;
                b_sum += sumFactor * pb;
                a_sum += sumFactor * pa;

                circularBlurStack.ResetHeadTailPosition();
                circularBlurStack.Prepare(radiusPlus1, pr, pg, pb, pa);

                yp = width;
                for (int i = 1; i <= radius; ++i)
                {
                    yi = (yp + x) << 2;

                    var stackColor = circularBlurStack.CurrentHeadColor;

                    r_sum += (stackColor.r = (pr = imgBuffer[yi + (int)order_e.R])) * (rbs = radiusPlus1 - i);
                    g_sum += (stackColor.g = (pg = imgBuffer[yi + (int)order_e.G])) * rbs;
                    b_sum += (stackColor.b = (pb = imgBuffer[yi + (int)order_e.B])) * rbs;
                    a_sum += (stackColor.a = (pa = imgBuffer[yi + (int)order_e.A])) * rbs;

                    r_in_sum += pr;
                    g_in_sum += pg;
                    b_in_sum += pb;
                    a_in_sum += pa;

                    circularBlurStack.Next();

                    if (i < heightMinus1)
                    {
                        yp += width;
                    }
                }
                yi = x;
                circularBlurStack.ResetHeadTailPosition();
                for (y = 0; y < height; ++y)
                {
                    pa = yi << 2;
                    pa = (a_sum * mul_sum) >> shg_sum;

                    if (pa != 0)
                    {
                        //has alpha 
                        int pa_under_255 = 255 / 255;
                        imgBuffer[p + (int)order_e.R] = (byte)(((r_sum * mul_sum) >> shg_sum) * pa_under_255);
                        imgBuffer[p + (int)order_e.G] = (byte)(((g_sum * mul_sum) >> shg_sum) * pa_under_255);
                        imgBuffer[p + (int)order_e.B] = (byte)(((b_sum * mul_sum) >> shg_sum) * pa_under_255);
                        imgBuffer[p + (int)order_e.A] = (byte)pa;
                    }
                    else
                    {
                        //not visible if alpha =0
                        imgBuffer[p + (int)order_e.R] = 0;
                        imgBuffer[p + (int)order_e.G] = 0;
                        imgBuffer[p + (int)order_e.B] = 0;
                        imgBuffer[p + (int)order_e.A] = 0;
                    }

                    r_sum -= r_out_sum;
                    g_sum -= g_out_sum;
                    b_sum -= b_out_sum;
                    a_sum -= a_out_sum;


                    var stackInColor = circularBlurStack.CurrentHeadColor;
                    r_out_sum -= stackInColor.r;
                    g_out_sum -= stackInColor.g;
                    b_out_sum -= stackInColor.b;
                    a_out_sum -= stackInColor.a;

                    p = (x + (((p = y + radiusPlus1) < heightMinus1 ? p : heightMinus1) * width)) << 2;

                    r_sum += (r_in_sum += (stackInColor.r = imgBuffer[p + (int)order_e.R]));
                    g_sum += (g_in_sum += (stackInColor.g = imgBuffer[p + (int)order_e.G]));
                    b_sum += (b_in_sum += (stackInColor.b = imgBuffer[p + (int)order_e.B]));
                    a_sum += (a_in_sum += (stackInColor.a = imgBuffer[p + (int)order_e.A]));

                    var stackOutColor = circularBlurStack.CurrentTailColor;
                    r_out_sum += (pr = stackOutColor.r);
                    g_out_sum += (pg = stackOutColor.g);
                    b_out_sum += (pb = stackOutColor.b);
                    a_out_sum += (pa = stackOutColor.a);

                    r_in_sum -= pr;
                    g_in_sum -= pg;
                    b_in_sum -= pb;
                    a_in_sum -= pa;

                    circularBlurStack.Next();

                    yi += width;

                }
            }
        }

    }

    //====================================================stack_blur_calc_rgba
    struct stack_blur_calc_rgba
    {
        int r, g, b, a;

        void clear()
        {
            r = g = b = a = 0;
        }

        void add(RGBA_Ints v)
        {
            r += v.r;
            g += v.g;
            b += v.b;
            a += v.a;
        }

        void add(RGBA_Ints v, int k)
        {
            r += v.r * k;
            g += v.g * k;
            b += v.b * k;
            a += v.a * k;
        }

        void sub(RGBA_Ints v)
        {
            r -= v.r;
            g -= v.g;
            b -= v.b;
            a -= v.a;
        }

        void calc_pix(RGBA_Ints v, int div)
        {
            v.r = (int)(r / div);
            v.g = (int)(g / div);
            v.b = (int)(b / div);
            v.a = (int)(a / div);
        }

        void calc_pix(RGBA_Ints v, int mul, int shr)
        {
            v.r = (int)((r * mul) >> shr);
            v.g = (int)((g * mul) >> shr);
            v.b = (int)((b * mul) >> shr);
            v.a = (int)((a * mul) >> shr);
        }
    };


    //=====================================================stack_blur_calc_rgb
    struct stack_blur_calc_rgb
    {
        int r, g, b;

        void clear()
        {
            r = g = b = 0;
        }

        void add(RGBA_Ints v)
        {
            r += v.r;
            g += v.g;
            b += v.b;
        }

        void add(RGBA_Ints v, int k)
        {
            r += v.r * k;
            g += v.g * k;
            b += v.b * k;
        }

        void sub(RGBA_Ints v)
        {
            r -= v.r;
            g -= v.g;
            b -= v.b;
        }

        void calc_pix(RGBA_Ints v, int div)
        {
            v.r = (int)(r / div);
            v.g = (int)(g / div);
            v.b = (int)(b / div);
        }

        void calc_pix(RGBA_Ints v, int mul, int shr)
        {
            v.r = (int)((r * mul) >> shr);
            v.g = (int)((g * mul) >> shr);
            v.b = (int)((b * mul) >> shr);
        }
    };


    //====================================================stack_blur_calc_gray
    struct stack_blur_calc_gray
    {
        int v;

        void clear()
        {
            v = 0;
        }

        void add(RGBA_Ints a)
        {
            v += a.r;
        }

        void add(RGBA_Ints a, int k)
        {
            v += a.r * k;
        }

        void sub(RGBA_Ints a)
        {
            v -= a.r;
        }

        void calc_pix(RGBA_Ints a, int div)
        {
            a.r = (int)(v / div);
        }

        void calc_pix(RGBA_Ints a, int mul, int shr)
        {
            a.r = (int)((v * mul) >> shr);
        }
    };
#endif

    public abstract class RecursizeBlurCalculator
    {
        public double r, g, b, a;

        public abstract RecursizeBlurCalculator CreateNew();

        public abstract void from_pix(ColorRGBA c);

        public abstract void calc(double b1, double b2, double b3, double b4,
            RecursizeBlurCalculator c1, RecursizeBlurCalculator c2, RecursizeBlurCalculator c3, RecursizeBlurCalculator c4);

        public abstract void to_pix(ref ColorRGBA c);
    };

    //===========================================================recursive_blur
    public sealed class RecursiveBlur
    {
        ArrayList<RecursizeBlurCalculator> m_sum1;
        ArrayList<RecursizeBlurCalculator> m_sum2;
        ArrayList<ColorRGBA> m_buf;
        RecursizeBlurCalculator m_RecursizeBlurCalculatorFactory;

        public RecursiveBlur(RecursizeBlurCalculator recursizeBluerCalculatorFactory)
        {
            m_sum1 = new ArrayList<RecursizeBlurCalculator>();
            m_sum2 = new ArrayList<RecursizeBlurCalculator>();
            m_buf = new ArrayList<ColorRGBA>();
            m_RecursizeBlurCalculatorFactory = recursizeBluerCalculatorFactory;
        }

        public void blur_x(IImage img, double radius)
        {
            if (radius < 0.62) return;
            if (img.Width < 3) return;

            double s = (double)(radius * 0.5);
            double q = (double)((s < 2.5) ?
                                    3.97156 - 4.14554 * Math.Sqrt(1 - 0.26891 * s) :
                                    0.98711 * s - 0.96330);

            double q2 = (double)(q * q);
            double q3 = (double)(q2 * q);

            double b0 = (double)(1.0 / (1.578250 +
                                            2.444130 * q +
                                            1.428100 * q2 +
                                            0.422205 * q3));

            double b1 = (double)(2.44413 * q +
                                      2.85619 * q2 +
                                      1.26661 * q3);

            double b2 = (double)(-1.42810 * q2 +
                                     -1.26661 * q3);

            double b3 = (double)(0.422205 * q3);

            double b = (double)(1 - (b1 + b2 + b3) * b0);

            b1 *= b0;
            b2 *= b0;
            b3 *= b0;

            int w = img.Width;
            int h = img.Height;
            int wm = (int)w - 1;
            int x, y;

            int startCreatingAt = (int)m_sum1.Count;
            m_sum1.AdjustSize(w);
            m_sum2.AdjustSize(w);
            m_buf.Allocate(w);

            RecursizeBlurCalculator[] Sum1Array = m_sum1.Array;
            RecursizeBlurCalculator[] Sum2Array = m_sum2.Array;
            ColorRGBA[] BufferArray = m_buf.Array;

            for (int i = startCreatingAt; i < w; i++)
            {
                Sum1Array[i] = m_RecursizeBlurCalculatorFactory.CreateNew();
                Sum2Array[i] = m_RecursizeBlurCalculatorFactory.CreateNew();
            }

            for (y = 0; y < h; y++)
            {
                RecursizeBlurCalculator c = m_RecursizeBlurCalculatorFactory;
                c.from_pix(img.GetPixel(0, y));
                Sum1Array[0].calc(b, b1, b2, b3, c, c, c, c);
                c.from_pix(img.GetPixel(1, y));
                Sum1Array[1].calc(b, b1, b2, b3, c, Sum1Array[0], Sum1Array[0], Sum1Array[0]);
                c.from_pix(img.GetPixel(2, y));
                Sum1Array[2].calc(b, b1, b2, b3, c, Sum1Array[1], Sum1Array[0], Sum1Array[0]);

                for (x = 3; x < w; ++x)
                {
                    c.from_pix(img.GetPixel(x, y));
                    Sum1Array[x].calc(b, b1, b2, b3, c, Sum1Array[x - 1], Sum1Array[x - 2], Sum1Array[x - 3]);
                }

                Sum2Array[wm].calc(b, b1, b2, b3, Sum1Array[wm], Sum1Array[wm], Sum1Array[wm], Sum1Array[wm]);
                Sum2Array[wm - 1].calc(b, b1, b2, b3, Sum1Array[wm - 1], Sum2Array[wm], Sum2Array[wm], Sum2Array[wm]);
                Sum2Array[wm - 2].calc(b, b1, b2, b3, Sum1Array[wm - 2], Sum2Array[wm - 1], Sum2Array[wm], Sum2Array[wm]);
                Sum2Array[wm].to_pix(ref BufferArray[wm]);
                Sum2Array[wm - 1].to_pix(ref BufferArray[wm - 1]);
                Sum2Array[wm - 2].to_pix(ref BufferArray[wm - 2]);

                for (x = wm - 3; x >= 0; --x)
                {
                    Sum2Array[x].calc(b, b1, b2, b3, Sum1Array[x], Sum2Array[x + 1], Sum2Array[x + 2], Sum2Array[x + 3]);
                    Sum2Array[x].to_pix(ref BufferArray[x]);
                }

                img.copy_color_hspan(0, y, w, BufferArray, 0);
            }
        }

        public void blur_y(IImage img, double radius)
        {
            FormatTransposer img2 = new FormatTransposer(img);
            blur_x(img2, radius);
        }

        public void blur(IImage img, double radius)
        {
            blur_x(img, radius);
            blur_y(img, radius);
        }
    };

    //=================================================recursive_blur_calc_rgb
    public sealed class recursive_blur_calc_rgb : RecursizeBlurCalculator
    {
        public override RecursizeBlurCalculator CreateNew()
        {
            return new recursive_blur_calc_rgb();
        }

        public override void from_pix(ColorRGBA c)
        {
            r = c.red;
            g = c.green;
            b = c.blue;
        }

        public override void calc(double b1, double b2, double b3, double b4,
            RecursizeBlurCalculator c1, RecursizeBlurCalculator c2, RecursizeBlurCalculator c3, RecursizeBlurCalculator c4)
        {
            r = b1 * c1.r + b2 * c2.r + b3 * c3.r + b4 * c4.r;
            g = b1 * c1.g + b2 * c2.g + b3 * c3.g + b4 * c4.g;
            b = b1 * c1.b + b2 * c2.b + b3 * c3.b + b4 * c4.b;
        }

        public override void to_pix(ref ColorRGBA c)
        {
            c.red = (byte)AggBasics.uround(r);
            c.green = (byte)AggBasics.uround(g);
            c.blue = (byte)AggBasics.uround(b);
        }
    };

    //=================================================recursive_blur_calc_rgba
    public sealed class recursive_blur_calc_rgba : RecursizeBlurCalculator
    {
        public override RecursizeBlurCalculator CreateNew()
        {
            return new recursive_blur_calc_rgba();
        }

        public override void from_pix(ColorRGBA c)
        {
            r = c.red;
            g = c.green;
            b = c.blue;
            a = c.alpha;
        }

        public override void calc(double b1, double b2, double b3, double b4,
            RecursizeBlurCalculator c1, RecursizeBlurCalculator c2, RecursizeBlurCalculator c3, RecursizeBlurCalculator c4)
        {
            r = b1 * c1.r + b2 * c2.r + b3 * c3.r + b4 * c4.r;
            g = b1 * c1.g + b2 * c2.g + b3 * c3.g + b4 * c4.g;
            b = b1 * c1.b + b2 * c2.b + b3 * c3.b + b4 * c4.b;
            a = b1 * c1.a + b2 * c2.a + b3 * c3.a + b4 * c4.a;
        }

        public override void to_pix(ref ColorRGBA c)
        {
            c.red = (byte)AggBasics.uround(r);
            c.green = (byte)AggBasics.uround(g);
            c.blue = (byte)AggBasics.uround(b);
            c.alpha = (byte)AggBasics.uround(a);
        }
    };

    //================================================recursive_blur_calc_gray
    public sealed class recursive_blur_calc_gray : RecursizeBlurCalculator
    {
        public override RecursizeBlurCalculator CreateNew()
        {
            return new recursive_blur_calc_gray();
        }

        public override void from_pix(ColorRGBA c)
        {
            r = c.red;
        }

        public override void calc(double b1, double b2, double b3, double b4,
            RecursizeBlurCalculator c1, RecursizeBlurCalculator c2, RecursizeBlurCalculator c3, RecursizeBlurCalculator c4)
        {
            r = b1 * c1.r + b2 * c2.r + b3 * c3.r + b4 * c4.r;
        }

        public override void to_pix(ref ColorRGBA c)
        {
            c.red = (byte)AggBasics.uround(r);
        }
    };
}
