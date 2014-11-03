//2014 BSD,WinterDev  

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
using System.Collections.Generic;

using PixelFarm.Agg.Image;
using PixelFarm.Agg.VertexSource;
using PixelFarm.Agg.Transform;
using PixelFarm.VectorMath;

namespace PixelFarm.Agg
{
    class ImageGraphics2D : Graphics2D
    {
        ImageReaderWriterBase destImageReaderWriter;
        ScanlinePacked8 sclinePack8;
        PathStorage drawImageRectPath = new PathStorage();

        ScanlineRasToDestBitmapRenderer sclineRasToBmp;
        PixelBlenderBGRA pixBlenderRGBA32;
        IPixelBlender currentBlender;

        double ox; //canvas origin x
        double oy; //canvas origin y
        int destWidth;
        int destHeight;
        RectInt clipBox;

        public ImageGraphics2D(ActualImage destImage)
        {
            //create from actual image
            this.destActualImage = destImage;
            this.destImageReaderWriter = new MyImageReaderWriter(destImage);
            this.sclineRas = new ScanlineRasterizer();
            this.sclineRasToBmp = new ScanlineRasToDestBitmapRenderer();
            this.destWidth = destImage.Width;
            this.destHeight = destImage.Height;

            this.clipBox = new RectInt(0, 0, destImage.Width, destImage.Height);
            this.sclineRas.SetClipBox(this.clipBox);

            this.sclinePack8 = new ScanlinePacked8();
            this.currentBlender = this.pixBlenderRGBA32 = new PixelBlenderBGRA();

        }
        public override ScanlinePacked8 ScanlinePacked8
        {
            get { return this.sclinePack8; }
        }
        public override IPixelBlender PixelBlender
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
        public override ImageReaderWriterBase DestImage
        {
            get { return this.destImageReaderWriter; }
        }
        public override ScanlineRasToDestBitmapRenderer ScanlineRasToDestBitmap
        {
            get { return this.sclineRasToBmp; }
        }
        public override void SetClippingRect(RectInt rect)
        {
            ScanlineRasterizer.SetClipBox(rect);
        }
        public override RectInt GetClippingRect()
        {
            return ScanlineRasterizer.GetVectorClipBox();
        }
        //--------------------------
        public override void Render(ActualImage actualImage, int x, int y)
        {

        }
        public override void Render(VertexStoreSnap vertextSnap, ColorRGBA color)
        {
            //reset rasterizer before render each vertextSnap

            //-----------------------------
            sclineRas.Reset();
            Affine transform = this.CurrentTransformMatrix;
            if (!transform.IsIdentity())
            {
                sclineRas.AddPath(transform.Tranform(vertextSnap));
            }
            else
            {
                sclineRas.AddPath(vertextSnap);
            }
            sclineRasToBmp.RenderScanlineSolidAA(destImageReaderWriter, sclineRas, sclinePack8, color);
            unchecked { destImageChanged++; };
            //-----------------------------
        }
        void DrawImageGetDestBounds(IImageReaderWriter sourceImage,
            double destX, double destY,
            double hotspotOffsetX, double hotSpotOffsetY,
            double scaleX, double scaleY,
            double angleRad, out Affine destRectTransform)
        {

            AffinePlan[] plan = new AffinePlan[4];
            int i = 0;
            if (hotspotOffsetX != 0.0f || hotSpotOffsetY != 0.0f)
            {
                plan[i] = AffinePlan.Translate(-hotspotOffsetX, -hotSpotOffsetY);
                i++;
            }

            if (scaleX != 1 || scaleY != 1)
            {

                plan[i] = AffinePlan.Scale(scaleX, scaleY);
                i++;
            }

            if (angleRad != 0)
            {

                plan[i] = AffinePlan.Rotate(angleRad);
                i++;
            }

            if (destX != 0 || destY != 0)
            {
                plan[i] = AffinePlan.Translate(destX, destY);
                i++;
            }

            destRectTransform = Affine.NewMatix(plan);

            int srcBuffWidth = sourceImage.Width;
            int srcBuffHeight = sourceImage.Height;

            drawImageRectPath.Clear();

            drawImageRectPath.MoveTo(0, 0);
            drawImageRectPath.LineTo(srcBuffWidth, 0);
            drawImageRectPath.LineTo(srcBuffWidth, srcBuffHeight);
            drawImageRectPath.LineTo(0, srcBuffHeight);
            drawImageRectPath.ClosePolygon();
        }

        void DrawImage(IImageReaderWriter sourceImage, ISpanGenerator spanImageFilter, Affine destRectTransform)
        {  
            VertexStoreSnap sp1 = destRectTransform.TransformToVertexSnap(drawImageRectPath);
            ScanlineRasterizer.AddPath(sp1);
            sclineRasToBmp.GenerateAndRender(
                new ChildImage(destImageReaderWriter, destImageReaderWriter.GetRecieveBlender()),
                ScanlineRasterizer,
                sclinePack8,
                spanImageFilter);

        }

        public override void Render(IImageReaderWriter source,
            double destX, double destY,
            double angleRadians,
            double inScaleX, double inScaleY)
        {
            {   // exit early if the dest and source bounds don't touch.
                // TODO: <BUG> make this do rotation and scalling
                RectInt sourceBounds = source.GetBounds();
                RectInt destBounds = this.destImageReaderWriter.GetBounds();
                sourceBounds.Offset((int)destX, (int)destY);

                if (!RectInt.DoIntersect(sourceBounds, destBounds))
                {
                    if (inScaleX != 1 || inScaleY != 1 || angleRadians != 0)
                    {
                        throw new NotImplementedException();
                    }
                    return;
                }
            }

            double scaleX = inScaleX;
            double scaleY = inScaleY;

            Affine graphicsTransform = this.CurrentTransformMatrix;
            if (!graphicsTransform.IsIdentity())
            {
                if (scaleX != 1 || scaleY != 1 || angleRadians != 0)
                {
                    throw new NotImplementedException();
                }
                graphicsTransform.Transform(ref destX, ref destY);
            }

#if false // this is an optomization that eliminates the drawing of images that have their alpha set to all 0 (happens with generated images like explosions).
	        MaxAlphaFrameProperty maxAlphaFrameProperty = MaxAlphaFrameProperty::GetMaxAlphaFrameProperty(source);

	        if((maxAlphaFrameProperty.GetMaxAlpha() * color.A_Byte) / 256 <= ALPHA_CHANNEL_BITS_DIVISOR)
	        {
		        m_OutFinalBlitBounds.SetRect(0,0,0,0);
	        }
#endif
            bool isScale = (scaleX != 1 || scaleY != 1);

            bool isRotated = true;
            if (Math.Abs(angleRadians) < (0.1 * MathHelper.Tau / 360))
            {
                isRotated = false;
                angleRadians = 0;
            }

            //bool IsMipped = false;
            //double ox, oy;
            //source.GetOriginOffset(out ox, out oy);

            bool canUseMipMaps = isScale;
            if (scaleX > 0.5 || scaleY > 0.5)
            {
                canUseMipMaps = false;
            }

            bool renderRequriesSourceSampling = isScale || isRotated || destX != (int)destX || destY != (int)destY;

            // this is the fast drawing path
            if (renderRequriesSourceSampling)
            {
#if false // if the scalling is small enough the results can be improved by using mip maps
	        if(CanUseMipMaps)
	        {
		        CMipMapFrameProperty* pMipMapFrameProperty = CMipMapFrameProperty::GetMipMapFrameProperty(source);
		        double OldScaleX = scaleX;
		        double OldScaleY = scaleY;
		        const CFrameInterface* pMippedFrame = pMipMapFrameProperty.GetMipMapFrame(ref scaleX, ref scaleY);
		        if(pMippedFrame != source)
		        {
			        IsMipped = true;
			        source = pMippedFrame;
			        sourceOriginOffsetX *= (OldScaleX / scaleX);
			        sourceOriginOffsetY *= (OldScaleY / scaleY);
		        }

			    HotspotOffsetX *= (inScaleX / scaleX);
			    HotspotOffsetY *= (inScaleY / scaleY);
	        }
#endif
                Affine destRectTransform;
                DrawImageGetDestBounds(source, destX, destY, ox, oy, scaleX, scaleY, angleRadians, out destRectTransform);

                Affine sourceRectTransform = destRectTransform.CreateInvert();
                // We invert it because it is the transform to make the image go to the same position as the polygon. LBB [2/24/2004]


                ImgSpanGen spanImageFilter;
                var interpolator = new SpanInterpolatorLinear(sourceRectTransform);
                ImageBufferAccessorClip sourceAccessor = new ImageBufferAccessorClip(source, ColorRGBAf.rgba_pre(0, 0, 0, 0).ToColorRGBA());

                spanImageFilter = new ImgSpanGenRGBA_BilinearClip(sourceAccessor, ColorRGBAf.rgba_pre(0, 0, 0, 0).ToColorRGBA(), interpolator);

                DrawImage(source, spanImageFilter, destRectTransform);
#if false // this is some debug you can enable to visualize the dest bounding box
		        LineFloat(BoundingRect.left, BoundingRect.top, BoundingRect.right, BoundingRect.top, WHITE);
		        LineFloat(BoundingRect.right, BoundingRect.top, BoundingRect.right, BoundingRect.bottom, WHITE);
		        LineFloat(BoundingRect.right, BoundingRect.bottom, BoundingRect.left, BoundingRect.bottom, WHITE);
		        LineFloat(BoundingRect.left, BoundingRect.bottom, BoundingRect.left, BoundingRect.top, WHITE);
#endif
            }
            else // TODO: this can be even faster if we do not use an intermediat buffer
            {
                Affine destRectTransform;
                DrawImageGetDestBounds(source, destX, destY, ox, oy, scaleX, scaleY, angleRadians, out destRectTransform);

                Affine sourceRectTransform = destRectTransform.CreateInvert();
                // We invert it because it is the transform to make the image go to the same position as the polygon. LBB [2/24/2004]


                var interpolator = new SpanInterpolatorLinear(sourceRectTransform);
                ImageBufferAccessorClip sourceAccessor = new ImageBufferAccessorClip(source, ColorRGBAf.rgba_pre(0, 0, 0, 0).ToColorRGBA());

                ImgSpanGen spanImageFilter = null;
                switch (source.BitDepth)
                {
                    case 32:
                        spanImageFilter = new ImgSpanGenRGBA_NN_StepXBy1(sourceAccessor, interpolator);
                        break;

                    case 24:
                        spanImageFilter = new ImgSpanGenRGB_NNStepXby1(sourceAccessor, interpolator);
                        break;

                    case 8:
                        spanImageFilter = new ImgSpanGenGray_NNStepXby1(sourceAccessor, interpolator);
                        break;

                    default:
                        throw new NotImplementedException();
                }
                //spanImageFilter = new span_image_filter_rgba_nn(sourceAccessor, interpolator); 
                DrawImage(source, spanImageFilter, destRectTransform);

                unchecked { destImageChanged++; };
            }
        }
        int destImageChanged = 0;

        public override void Render(IImageReaderWriter source, double destX, double destY)
        {
            int inScaleX = 1;
            int inScaleY = 1;
            int angleRadians = 0;

            {   // exit early if the dest and source bounds don't touch.
                // TODO: <BUG> make this do rotation and scalling
                RectInt sourceBounds = source.GetBounds();
                RectInt destBounds = this.destImageReaderWriter.GetBounds();
                sourceBounds.Offset((int)destX, (int)destY);

                if (!RectInt.DoIntersect(sourceBounds, destBounds))
                {
                    //if (inScaleX != 1 || inScaleY != 1 || angleRadians != 0)
                    //{
                    //    throw new NotImplementedException();
                    //}
                    return;
                }
            }

            double scaleX = inScaleX;
            double scaleY = inScaleY;

            Affine graphicsTransform = this.CurrentTransformMatrix;
            if (!graphicsTransform.IsIdentity())
            {
                if (scaleX != 1 || scaleY != 1 || angleRadians != 0)
                {
                    throw new NotImplementedException();
                }
                graphicsTransform.Transform(ref destX, ref destY);
            }


#if false // this is an optomization that eliminates the drawing of images that have their alpha set to all 0 (happens with generated images like explosions).
	        MaxAlphaFrameProperty maxAlphaFrameProperty = MaxAlphaFrameProperty::GetMaxAlphaFrameProperty(source);

	        if((maxAlphaFrameProperty.GetMaxAlpha() * color.A_Byte) / 256 <= ALPHA_CHANNEL_BITS_DIVISOR)
	        {
		        m_OutFinalBlitBounds.SetRect(0,0,0,0);
	        }
#endif
            bool isScale = (scaleX != 1 || scaleY != 1);

            bool isRotated = true;
            if (Math.Abs(angleRadians) < (0.1 * MathHelper.Tau / 360))
            {
                isRotated = false;
                angleRadians = 0;
            }

            //bool IsMipped = false;
            //double ox, oy;
            //source.GetOriginOffset(out ox, out oy);

            bool canUseMipMaps = isScale;
            if (scaleX > 0.5 || scaleY > 0.5)
            {
                canUseMipMaps = false;
            }

            bool renderRequriesSourceSampling = isScale || isRotated || destX != (int)destX || destY != (int)destY;

            // this is the fast drawing path
            if (renderRequriesSourceSampling)
            {

#if false // if the scalling is small enough the results can be improved by using mip maps
	        if(CanUseMipMaps)
	        {
		        CMipMapFrameProperty* pMipMapFrameProperty = CMipMapFrameProperty::GetMipMapFrameProperty(source);
		        double OldScaleX = scaleX;
		        double OldScaleY = scaleY;
		        const CFrameInterface* pMippedFrame = pMipMapFrameProperty.GetMipMapFrame(ref scaleX, ref scaleY);
		        if(pMippedFrame != source)
		        {
			        IsMipped = true;
			        source = pMippedFrame;
			        sourceOriginOffsetX *= (OldScaleX / scaleX);
			        sourceOriginOffsetY *= (OldScaleY / scaleY);
		        }

			    HotspotOffsetX *= (inScaleX / scaleX);
			    HotspotOffsetY *= (inScaleY / scaleY);
	        }
#endif
                Affine destRectTransform;
                DrawImageGetDestBounds(source, destX, destY,
                    ox, oy,
                    scaleX, scaleY, angleRadians,
                    out destRectTransform);

                Affine sourceRectTransform = destRectTransform.CreateInvert();
                // We invert it because it is the transform to make the image go to the same position as the polygon. LBB [2/24/2004]

                var spanImageFilter = new ImgSpanGenRGBA_BilinearClip(
                    new ImageBufferAccessorClip(source, ColorRGBAf.rgba_pre(0, 0, 0, 0).ToColorRGBA()),
                    ColorRGBAf.rgba_pre(0, 0, 0, 0).ToColorRGBA(),
                    new SpanInterpolatorLinear(sourceRectTransform));

                DrawImage(source, spanImageFilter, destRectTransform);
#if false // this is some debug you can enable to visualize the dest bounding box
		        LineFloat(BoundingRect.left, BoundingRect.top, BoundingRect.right, BoundingRect.top, WHITE);
		        LineFloat(BoundingRect.right, BoundingRect.top, BoundingRect.right, BoundingRect.bottom, WHITE);
		        LineFloat(BoundingRect.right, BoundingRect.bottom, BoundingRect.left, BoundingRect.bottom, WHITE);
		        LineFloat(BoundingRect.left, BoundingRect.bottom, BoundingRect.left, BoundingRect.top, WHITE);
#endif
            }
            else // TODO: this can be even faster if we do not use an intermediat buffer
            {
                Affine destRectTransform;
                DrawImageGetDestBounds(source, destX, destY,
                    ox, oy,
                    scaleX, scaleY, angleRadians, out destRectTransform);

                Affine sourceRectTransform = destRectTransform.CreateInvert();
                // We invert it because it is the transform to make the image go to the same position as the polygon. LBB [2/24/2004]


                var interpolator = new SpanInterpolatorLinear(sourceRectTransform);
                ImageBufferAccessorClip sourceAccessor = new ImageBufferAccessorClip(source, ColorRGBAf.rgba_pre(0, 0, 0, 0).ToColorRGBA());

                ImgSpanGen spanImageFilter = null;
                switch (source.BitDepth)
                {
                    case 32:
                        spanImageFilter = new ImgSpanGenRGBA_NN_StepXBy1(sourceAccessor, interpolator);
                        break;

                    case 24:
                        spanImageFilter = new ImgSpanGenRGB_NNStepXby1(sourceAccessor, interpolator);
                        break;

                    case 8:
                        spanImageFilter = new ImgSpanGenGray_NNStepXby1(sourceAccessor, interpolator);
                        break;

                    default:
                        throw new NotImplementedException();
                }
                //spanImageFilter = new span_image_filter_rgba_nn(sourceAccessor, interpolator); 
                DrawImage(source, spanImageFilter, destRectTransform);
                unchecked { destImageChanged++; };
            }
        }
        public override void Clear(ColorRGBA color)
        {

            RectInt clippingRectInt = GetClippingRect();

            var destImage = this.DestImage;
            int width = destImage.Width;
            int height = destImage.Height;
            byte[] buffer = destImage.GetBuffer();

            switch (destImage.BitDepth)
            {
                case 8:
                    {
                        //int bytesBetweenPixels = destImage.BytesBetweenPixelsInclusive;
                        //byte byteColor = color.Red0To255;
                        //int clipRectLeft = clippingRectInt.Left;

                        //for (int y = clippingRectInt.Bottom; y < clippingRectInt.Top; ++y)
                        //{
                        //    int bufferOffset = destImage.GetBufferOffsetXY(clipRectLeft, y);
                        //    for (int x = 0; x < clippingRectInt.Width; ++x)
                        //    {
                        //        buffer[bufferOffset] = color.blue;
                        //        bufferOffset += bytesBetweenPixels;
                        //    }
                        //}
                        throw new NotSupportedException("temp");
                    }
                case 24:
                    {
                        //int bytesBetweenPixels = destImage.BytesBetweenPixelsInclusive;
                        //int clipRectLeft = clippingRectInt.Left;
                        //for (int y = clippingRectInt.Bottom; y < clippingRectInt.Top; y++)
                        //{
                        //    int bufferOffset = destImage.GetBufferOffsetXY(clipRectLeft, y);
                        //    for (int x = 0; x < clippingRectInt.Width; ++x)
                        //    {
                        //        buffer[bufferOffset + 0] = color.blue;
                        //        buffer[bufferOffset + 1] = color.green;
                        //        buffer[bufferOffset + 2] = color.red;
                        //        bufferOffset += bytesBetweenPixels;
                        //    }
                        //}
                        throw new NotSupportedException("temp");
                    }
                    break;
                case 32:
                    {

                        //------------------------------
                        //fast clear buffer
                        //skip clipping ****
                        //TODO: reimplement clipping***
                        //------------------------------


                        if (color == ColorRGBA.White)
                        {
                            //fast cleat with white color
                            int n = buffer.Length / 4;
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
                        else if (color == ColorRGBA.Black)
                        {
                            //fast cleat with black color
                            int n = buffer.Length / 4;
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
                            uint colorARGB = (uint)((color.alpha << 24) | ((color.red << 16) | (color.green << 8) | color.blue));
                            int n = buffer.Length / 4;
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

                            //int bytesBetweenPixels = 4;
                            //int clipRectLeft = clippingRectInt.Left;
                            //for (int y = clippingRectInt.Bottom; y < clippingRectInt.Top; ++y)
                            //{
                            //    int bufferOffset = destImage.GetBufferOffsetXY(clipRectLeft, y);
                            //    for (int x = 0; x < clippingRectInt.Width; ++x)
                            //    {
                            //        //b g, r, a
                            //        buffer[bufferOffset + 0] = color.blue;
                            //        buffer[bufferOffset + 1] = color.green;
                            //        buffer[bufferOffset + 2] = color.red;
                            //        buffer[bufferOffset + 3] = color.alpha;
                            //        bufferOffset += bytesBetweenPixels;
                            //    }
                            //}
                        }
                    }
                    break;

                default:
                    throw new NotImplementedException();
            }


        }
    }
}
