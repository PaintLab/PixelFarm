//2014 MIT,WinterDev  

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
    partial class ImageGraphics2D
    {
        public ImageInterpolationQuality ImageInterpolationQuality
        {
            get { return this.ImageInterpolationQuality; }
            set { this.imgInterpolationQuality = value; }
        }
        //--------------------------
        public override void Render(ActualImage actualImage, int x, int y)
        {
            MyImageReaderWriter imgRW = new MyImageReaderWriter(actualImage);
            Render(imgRW, (double)x, (double)y);

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
        Affine GetImageDestBounds(IImageReaderWriter sourceImage,
            PathStorage drawImageRectPath,
            double destX, double destY,
            double hotspotOffsetX, double hotSpotOffsetY,
            double scaleX, double scaleY,
            double angleRad)
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

            Affine destRectTransform = Affine.NewMatix(plan);

            int srcW = sourceImage.Width;
            int srcH = sourceImage.Height;

            drawImageRectPath.Clear();
            drawImageRectPath.MoveTo(0, 0);
            drawImageRectPath.LineTo(srcW, 0);
            drawImageRectPath.LineTo(srcW, srcH);
            drawImageRectPath.LineTo(0, srcH);
            drawImageRectPath.ClosePolygon();

            return destRectTransform;
        }

        void DrawImage(IImageReaderWriter sourceImage, ISpanGenerator spanImageFilter, VertexStore vxs)
        {

            ScanlineRasterizer.AddPath(vxs);
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

            PathStorage imgPathBounds = GetFreePathStorage();
            // this is the fast drawing path
            if (renderRequriesSourceSampling)
            {



                // if the scalling is small enough the results can be improved by using mip maps
                //if(CanUseMipMaps)
                //{
                //    CMipMapFrameProperty* pMipMapFrameProperty = CMipMapFrameProperty::GetMipMapFrameProperty(source);
                //    double OldScaleX = scaleX;
                //    double OldScaleY = scaleY;
                //    const CFrameInterface* pMippedFrame = pMipMapFrameProperty.GetMipMapFrame(ref scaleX, ref scaleY);
                //    if(pMippedFrame != source)
                //    {
                //        IsMipped = true;
                //        source = pMippedFrame;
                //        sourceOriginOffsetX *= (OldScaleX / scaleX);
                //        sourceOriginOffsetY *= (OldScaleY / scaleY);
                //    }

                //    HotspotOffsetX *= (inScaleX / scaleX);
                //    HotspotOffsetY *= (inScaleY / scaleY);
                //}
               
                Affine destRectTransform = GetImageDestBounds(source, imgPathBounds, destX, destY, ox, oy, scaleX, scaleY, angleRadians);

                Affine sourceRectTransform = destRectTransform.CreateInvert();
                // We invert it because it is the transform to make the image go to the same position as the polygon. LBB [2/24/2004]


                ImgSpanGen spanImageFilter;
                var interpolator = new SpanInterpolatorLinear(sourceRectTransform);
                ImageBufferAccessorClip sourceAccessor = new ImageBufferAccessorClip(source);

                spanImageFilter = new ImgSpanGenRGBA_BilinearClip(sourceAccessor, ColorRGBAf.rgba_pre(0, 0, 0, 0).ToColorRGBA(), interpolator);

                DrawImage(source, spanImageFilter, destRectTransform.TransformToVxs(imgPathBounds));


               
                // this is some debug you can enable to visualize the dest bounding box
                //LineFloat(BoundingRect.left, BoundingRect.top, BoundingRect.right, BoundingRect.top, WHITE);
                //LineFloat(BoundingRect.right, BoundingRect.top, BoundingRect.right, BoundingRect.bottom, WHITE);
                //LineFloat(BoundingRect.right, BoundingRect.bottom, BoundingRect.left, BoundingRect.bottom, WHITE);
                //LineFloat(BoundingRect.left, BoundingRect.bottom, BoundingRect.left, BoundingRect.top, WHITE);

            }
            else // TODO: this can be even faster if we do not use an intermediat buffer
            {
                 
                Affine destRectTransform = GetImageDestBounds(source, imgPathBounds, destX, destY, ox, oy, scaleX, scaleY, angleRadians);

                Affine sourceRectTransform = destRectTransform.CreateInvert();
                // We invert it because it is the transform to make the image go to the same position as the polygon. LBB [2/24/2004]


                var interpolator = new SpanInterpolatorLinear(sourceRectTransform);
                ImageBufferAccessorClip sourceAccessor = new ImageBufferAccessorClip(source);

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

                DrawImage(source, spanImageFilter, destRectTransform.TransformToVxs(imgPathBounds));               
                unchecked { destImageChanged++; };

            }
            ReleasePathStorage(imgPathBounds);
        }

        int destImageChanged = 0;

        public override void Render(IImageReaderWriter source, double destX, double destY)
        {
            int inScaleX = 1;
            int inScaleY = 1;
            int angleRadians = 0;

            // exit early if the dest and source bounds don't touch.
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

                var imgPathBounds = GetFreePathStorage();
                Affine destRectTransform = GetImageDestBounds(source, imgPathBounds, destX, destY,
                    ox, oy, scaleX, scaleY, angleRadians);

                Affine sourceRectTransform = destRectTransform.CreateInvert();
                // We invert it because it is the transform to make the image go to the same position as the polygon. LBB [2/24/2004]

                var spanImageFilter = new ImgSpanGenRGBA_BilinearClip(
                    new ImageBufferAccessorClip(source),
                    ColorRGBA.Black,
                    new SpanInterpolatorLinear(sourceRectTransform));

                DrawImage(source, spanImageFilter, destRectTransform.TransformToVxs(imgPathBounds));
                ReleasePathStorage(imgPathBounds);

#if false // this is some debug you can enable to visualize the dest bounding box
		        LineFloat(BoundingRect.left, BoundingRect.top, BoundingRect.right, BoundingRect.top, WHITE);
		        LineFloat(BoundingRect.right, BoundingRect.top, BoundingRect.right, BoundingRect.bottom, WHITE);
		        LineFloat(BoundingRect.right, BoundingRect.bottom, BoundingRect.left, BoundingRect.bottom, WHITE);
		        LineFloat(BoundingRect.left, BoundingRect.bottom, BoundingRect.left, BoundingRect.top, WHITE);
#endif
            }
            else // TODO: this can be even faster if we do not use an intermediat buffer
            {

                var imgPathBounds = GetFreePathStorage();
                Affine destRectTransform = GetImageDestBounds(source, imgPathBounds,
                    destX, destY,
                    ox, oy, scaleX, scaleY, angleRadians);

                // We invert it because it is the transform to make the image go to the same position as the polygon. LBB [2/24/2004]
                Affine sourceRectTransform = destRectTransform.CreateInvert();

                var interpolator = new SpanInterpolatorLinear(sourceRectTransform);
                var sourceAccessor = new ImageBufferAccessorClip(source);

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

                DrawImage(source, spanImageFilter, destRectTransform.TransformToVxs(imgPathBounds));

                ReleasePathStorage(imgPathBounds);

                unchecked { destImageChanged++; };
            }
        }



    }
}