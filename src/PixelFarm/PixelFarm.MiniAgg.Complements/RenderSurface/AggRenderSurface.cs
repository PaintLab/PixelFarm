//MIT, 2014-2017, WinterDev

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

using System;
using PixelFarm.Drawing;
using PixelFarm.Agg.Imaging;
using PixelFarm.Agg.Transform;
namespace PixelFarm.Agg
{
    public sealed partial class AggRenderSurface
    {
        MyImageReaderWriter destImageReaderWriter;
        ScanlinePacked8 sclinePack8;

        ScanlineRasToDestBitmapRenderer sclineRasToBmp;
        PixelBlenderBGRA pixBlenderRGBA32;
        IPixelBlender currentBlender;
        double ox; //canvas origin x
        double oy; //canvas origin y
        int destWidth;
        int destHeight;
        RectInt clipBox;
        ImageInterpolationQuality imgInterpolationQuality = ImageInterpolationQuality.Bilinear;

        ActualImage destImage;
        public AggRenderSurface(ActualImage destImage)
        {
            //create from actual image
            this.destImage = destImage;

            this.destActualImage = destImage;
            this.destImageReaderWriter = new MyImageReaderWriter();
            this.destImageReaderWriter.ReloadImage(destImage);
            //
            this.sclineRas = new ScanlineRasterizer(destImage.Width, destImage.Height);
            this.sclineRasToBmp = new ScanlineRasToDestBitmapRenderer();
            //
            this.destWidth = destImage.Width;
            this.destHeight = destImage.Height;
            //
            this.clipBox = new RectInt(0, 0, destImage.Width, destImage.Height);
            this.sclineRas.SetClipBox(this.clipBox);
            this.sclinePack8 = new ScanlinePacked8();
            this.currentBlender = this.pixBlenderRGBA32 = new PixelBlenderBGRA();
        }

        public int Width { get { return destWidth; } }
        public int Height { get { return destHeight; } }

        public ScanlineRasterizer ScanlineRasterizer
        {
            get { return sclineRas; }
        }
        public ActualImage DestActualImage
        {
            get { return this.destActualImage; }
        }
        public ScanlinePacked8 ScanlinePacked8
        {
            get { return this.sclinePack8; }
        }
        public IPixelBlender PixelBlender
        {
            get
            {
                return this.currentBlender;
            }
            set
            {
                this.currentBlender = value;
            }
        }
        public ImageReaderWriterBase DestImage
        {
            get { return this.destImageReaderWriter; }
        }
        public ScanlineRasToDestBitmapRenderer ScanlineRasToDestBitmap
        {
            get { return this.sclineRasToBmp; }
        }
        public void SetClippingRect(RectInt rect)
        {
            ScanlineRasterizer.SetClipBox(rect);
        }
        public RectInt GetClippingRect()
        {
            return ScanlineRasterizer.GetVectorClipBox();
        }
        public ImageInterpolationQuality ImageInterpolationQuality
        {
            get { return this.ImageInterpolationQuality; }
            set { this.imgInterpolationQuality = value; }
        }

        VertexStorePool _vxsPool = new VertexStorePool();
        VertexStore GetFreeVxs()
        {
            return _vxsPool.GetFreeVxs();
        }
        void ReleaseVxs(ref VertexStore vxs)
        {
            _vxsPool.Release(ref vxs);
        }
        public void Clear(Color color)
        {
            RectInt clippingRectInt = GetClippingRect();
            var destImage = this.DestImage;
            int width = destImage.Width;
            int height = destImage.Height;
            int[] buffer = destImage.GetBuffer32();
            switch (destImage.BitDepth)
            {
                default: throw new NotSupportedException();
                case 32:
                    {
                        //------------------------------
                        //fast clear buffer
                        //skip clipping ****
                        //TODO: reimplement clipping***
                        //------------------------------ 
                        if (color == Color.White)
                        {
                            //fast cleat with white color
                            int n = buffer.Length;
                            unsafe
                            {
                                fixed (void* head = &buffer[0])
                                {
                                    uint* head_i32 = (uint*)head;
                                    for (int i = n - 1; i >= 0; --i)
                                    {
                                        *head_i32 = 0xffffffff; //white (ARGB)
                                        head_i32++;
                                    }
                                }
                            }
                        }
                        else if (color == Color.Black)
                        {
                            //fast cleat with black color
                            int n = buffer.Length;
                            unsafe
                            {
                                fixed (void* head = &buffer[0])
                                {
                                    uint* head_i32 = (uint*)head;
                                    for (int i = n - 1; i >= 0; --i)
                                    {
                                        *head_i32 = 0xff000000; //black (ARGB)
                                        head_i32++;
                                    }
                                }
                            }
                        }
                        else
                        {
                            //other color
                            //#if WIN
                            //                            uint colorARGB = (uint)((color.alpha << 24) | ((color.red << 16) | (color.green << 8) | color.blue));
                            //#else
                            //                            uint colorARGB = (uint)((color.alpha << 24) | ((color.blue << 16) | (color.green << 8) | color.red));
                            //#endif

                            //ARGB
                            uint colorARGB = (uint)((color.alpha << 24) | ((color.red << 16) | (color.green << 8) | color.blue));
                            int n = buffer.Length;
                            unsafe
                            {
                                fixed (void* head = &buffer[0])
                                {
                                    uint* head_i32 = (uint*)head;
                                    for (int i = n - 1; i >= 0; --i)
                                    {
                                        *head_i32 = colorARGB;
                                        head_i32++;
                                    }
                                }
                            }
                        }
                    }
                    break;

            }

            //            switch (destImage.BitDepth)
            //            {
            //                case 8:
            //                    {
            //                        //int bytesBetweenPixels = destImage.BytesBetweenPixelsInclusive;
            //                        //byte byteColor = color.Red0To255;
            //                        //int clipRectLeft = clippingRectInt.Left;

            //                        //for (int y = clippingRectInt.Bottom; y < clippingRectInt.Top; ++y)
            //                        //{
            //                        //    int bufferOffset = destImage.GetBufferOffsetXY(clipRectLeft, y);
            //                        //    for (int x = 0; x < clippingRectInt.Width; ++x)
            //                        //    {
            //                        //        buffer[bufferOffset] = color.blue;
            //                        //        bufferOffset += bytesBetweenPixels;
            //                        //    }
            //                        //}
            //                        throw new NotSupportedException("temp");
            //                    }
            //                case 24:
            //                    {
            //                        //int bytesBetweenPixels = destImage.BytesBetweenPixelsInclusive;
            //                        //int clipRectLeft = clippingRectInt.Left;
            //                        //for (int y = clippingRectInt.Bottom; y < clippingRectInt.Top; y++)
            //                        //{
            //                        //    int bufferOffset = destImage.GetBufferOffsetXY(clipRectLeft, y);
            //                        //    for (int x = 0; x < clippingRectInt.Width; ++x)
            //                        //    {
            //                        //        buffer[bufferOffset + 0] = color.blue;
            //                        //        buffer[bufferOffset + 1] = color.green;
            //                        //        buffer[bufferOffset + 2] = color.red;
            //                        //        bufferOffset += bytesBetweenPixels;
            //                        //    }
            //                        //}
            //                        throw new NotSupportedException("temp");
            //                    }
            //                    break;
            //                case 32:
            //                    {
            //                        //------------------------------
            //                        //fast clear buffer
            //                        //skip clipping ****
            //                        //TODO: reimplement clipping***
            //                        //------------------------------ 
            //                        if (color == Color.White)
            //                        {
            //                            //fast cleat with white color
            //                            int n = buffer.Length / 4;
            //                            unsafe
            //                            {
            //                                fixed (void* head = &buffer[0])
            //                                {
            //                                    uint* head_i32 = (uint*)head;
            //                                    for (int i = n - 1; i >= 0; --i)
            //                                    {
            //                                        *head_i32 = 0xffffffff; //white (ARGB)
            //                                        head_i32++;
            //                                    }
            //                                }
            //                            }
            //                        }
            //                        else if (color == Color.Black)
            //                        {
            //                            //fast cleat with black color
            //                            int n = buffer.Length / 4;
            //                            unsafe
            //                            {
            //                                fixed (void* head = &buffer[0])
            //                                {
            //                                    uint* head_i32 = (uint*)head;
            //                                    for (int i = n - 1; i >= 0; --i)
            //                                    {
            //                                        *head_i32 = 0xff000000; //black (ARGB)
            //                                        head_i32++;
            //                                    }
            //                                }
            //                            }
            //                        }
            //                        else
            //                        {
            //                            //other color
            //#if WIN
            //                            uint colorARGB = (uint)((color.alpha << 24) | ((color.red << 16) | (color.green << 8) | color.blue));
            //#else
            //                            uint colorARGB = (uint)((color.alpha << 24) | ((color.blue << 16) | (color.green << 8) | color.red));
            //#endif
            //                            int n = buffer.Length / 4;
            //                            unsafe
            //                            {
            //                                fixed (void* head = &buffer[0])
            //                                {
            //                                    uint* head_i32 = (uint*)head;
            //                                    for (int i = n - 1; i >= 0; --i)
            //                                    {
            //                                        *head_i32 = colorARGB;
            //                                        head_i32++;
            //                                    }
            //                                }
            //                            }
            //                        }
            //                    }
            //                    break;
            //                default:
            //                    throw new NotImplementedException();
            //            }
        }


        /// <summary>
        /// we do NOT store vxs/vxsSnap
        /// </summary>
        /// <param name="vxsSnap"></param>
        /// <param name="color"></param>
        public void Render(VertexStoreSnap vxsSnap, Drawing.Color color)
        {
            //reset rasterizer before render each vertextSnap 
            //-----------------------------
            sclineRas.Reset();
            Affine transform = this.CurrentTransformMatrix;
            if (!transform.IsIdentity())
            {

                var v1 = transform.TransformToVxs(vxsSnap, GetFreeVxs());
                sclineRas.AddPath(v1);
                ReleaseVxs(ref v1);
                //-------------------------
                //since sclineRas do NOT store vxs
                //then we can reuse the vxs***
                //-------------------------
            }
            else
            {
                sclineRas.AddPath(vxsSnap);
            }
            sclineRasToBmp.RenderWithColor(destImageReaderWriter, sclineRas, sclinePack8, color);
            unchecked { destImageChanged++; };
            //-----------------------------
        }



        /// <summary>
        /// we do NOT store vxs
        /// </summary>
        /// <param name="vxs"></param>
        /// <param name="c"></param>
        public void Render(VertexStore vxs, Drawing.Color c)
        {
            Render(new VertexStoreSnap(vxs), c);
        }
        ActualImage destActualImage;
        ScanlineRasterizer sclineRas;
        Affine currentTxMatrix = Affine.IdentityMatrix;
        public Affine CurrentTransformMatrix
        {
            get { return this.currentTxMatrix; }
            set
            {
                this.currentTxMatrix = value;
            }
        }
#if DEBUG
        VertexStore dbug_v1 = new VertexStore(8);
        VertexStore dbug_v2 = new VertexStore();
        Stroke dbugStroke = new Stroke(1);
        public void dbugLine(double x1, double y1, double x2, double y2, Drawing.Color color)
        {


            dbug_v1.AddMoveTo(x1, y1);
            dbug_v1.AddLineTo(x2, y2);
            //dbug_v1.AddStop();

            dbugStroke.MakeVxs(dbug_v1, dbug_v2);
            Render(dbug_v2, color);
            dbug_v1.Clear();
            dbug_v2.Clear();
        }
#endif

    }
}
