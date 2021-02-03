//MIT, 2014-present, WinterDev

using System;
using System.Collections.Generic;
using OpenTK.Graphics.ES20;
using PixelFarm.Drawing;
using PixelFarm.CpuBlit;
using PixelFarm.CpuBlit.VertexProcessing;
namespace PixelFarm.DrawingGL
{

    public class GLRenderSurface : IDisposable
    {
        public readonly struct InnerGLData
        {
            public readonly GLBitmap GLBmp;
            public readonly int FramebufferId;
            public InnerGLData(int frameBufferId, GLBitmap glBmp)
            {
                GLBmp = glBmp;
                FramebufferId = frameBufferId;
            }
        }

        internal readonly MyMat4 _orthoView;
        internal readonly MyMat4 _orthoFlipY_and_PullDown;
        Framebuffer _frameBuffer;//default = null, system-provide-framebuffer  (primary) 
        internal GLRenderSurface(int width, int height, int viewportW, int viewportH, bool isPrimary)
        {
            Width = width;
            Height = height;
            ViewportW = viewportW;
            ViewportH = viewportH;
            IsPrimary = isPrimary;
            //setup viewport size,
            //we need W:H ratio= 1:1 , square viewport
            int max = Math.Max(width, height);
            _orthoView = MyMat4.ortho(0, max, 0, max, 0, 1); //this make our viewport W:H =1:1

            //init ortho 
            _orthoFlipY_and_PullDown = _orthoView *
                                     MyMat4.scale(1, -1) * //flip Y
                                     MyMat4.translate(new OpenTK.Vector3(0, -viewportH, 0)); //pull-down; //init 
            IsValid = true;
        }

        public GLRenderSurface(int width, int height, BitmapBufferFormat bufferFormat = BitmapBufferFormat.RGBA)
            : this(Math.Max(width, height), Math.Max(width, height), width, height, false)
        {
            //max int for 1:1 ratio

            //create seconday render surface (off-screen)
            _frameBuffer = new Framebuffer(new GLBitmap(width, height, bufferFormat), true);
            IsValid = _frameBuffer.FrameBufferId != 0;
        }
        public GLRenderSurface(GLBitmap bmp, bool isBmpOwner)
            : this(Math.Max(bmp.Width, bmp.Height), Math.Max(bmp.Width, bmp.Height), bmp.Width, bmp.Height, false)
        {
            //max int for 1:1 ratio
            //create seconday render surface (off-screen)
            _frameBuffer = new Framebuffer(bmp, isBmpOwner);
            IsValid = _frameBuffer.FrameBufferId != 0;
        }
        public void Dispose()
        {
            if (_frameBuffer != null)
            {
                _frameBuffer.Dispose();
                _frameBuffer = null;
            }
            IsValid = false;
        }
        public int Width { get; }
        public int Height { get; }
        public int ViewportW { get; }
        public int ViewportH { get; }
        public bool IsPrimary { get; }
        public bool IsValid { get; private set; }

        public GLBitmap GetGLBitmap() => _frameBuffer?.GetGLBitmap();

        public InnerGLData GetInnerGLData() => (_frameBuffer != null) ? new InnerGLData(_frameBuffer.FrameBufferId, _frameBuffer.GetGLBitmap()) : new InnerGLData();

        internal void SetAsCurrentSurface() => _frameBuffer?.MakeCurrent();

        internal void ReleaseCurrent(bool updateTexture)
        {
            if (_frameBuffer != null)
            {
                if (updateTexture)
                {
                    _frameBuffer.UpdateTexture();
                }
                _frameBuffer.ReleaseCurrent();
            }
        }
        public void CopySurface(int left, int top, int width, int height, PixelFarm.CpuBlit.MemBitmap outputMemBmp)
        {
            if (_frameBuffer != null)
            {
                _frameBuffer.MakeCurrent();
                GL.ReadPixels(left, top, width, height, OpenTK.Graphics.ES20.PixelFormat.Rgba, PixelType.UnsignedByte, PixelFarm.CpuBlit.MemBitmap.GetBufferPtr(outputMemBmp).Ptr);
                _frameBuffer.ReleaseCurrent();
            }
        }
    }

    /// <summary>
    /// GLES2 render Core, This is not intended to be used directly from your code
    /// </summary>
    public sealed class GLPainterCore
    {
        readonly int _id;

        readonly SolidColorFillShader _solidColorFillShader;
        readonly RectFillShader _rectFillShader;
        readonly RadialGradientFillShader _radialGradientShader;

        readonly SmoothLineShader _smoothLineShader;
        readonly InvertAlphaLineSmoothShader _invertedAlphaSmoothLineShader;

        readonly GlyphImageStecilShader _glyphStencilShader;
        readonly BGRImageTextureShader _bgrImgTextureShader;
        readonly BGRAImageTextureShader _bgraImgTextureShader;

        readonly LcdSubPixShader _lcdSubPixShader;
        readonly LcdSubPixShaderForSolidBg _lcdSubPixForSolidBgShader;
        readonly LcdSubPixShaderForWordStripCreation _lcdSubPixShaderForWordStripCreation;
        readonly LcdSubPixShaderV2 _lcdSubPixShaderV2;

        readonly RGBATextureShader _rgbaTextureShader;
        readonly RGBTextureShader _rgbTextureShader;
        readonly BlurShader _blurShader;
        readonly Conv3x3TextureShader _conv3x3TextureShader;
        readonly MsdfShader _msdfShader;
        readonly SingleChannelSdf _sdfShader;


        readonly OneColorMaskShader _maskShader_OneColor;
        readonly TwoColorMaskShader _maskShader_TwoColor;
        //-----------------------------------------------------------
        readonly ShaderSharedResource _shareRes;

        RenderSurfaceOriginKind _originKind;
        GLRenderSurface _rendersx;
        int _canvasOriginX = 0;
        int _canvasOriginY = 0;
        int _vwHeight = 0;

        ICoordTransformer _coordTransformer;
        MyMat4 _customCoordTransformer;

        //
        readonly TessTool _tessTool;
        readonly SmoothBorderBuilder _smoothBorderBuilder = new SmoothBorderBuilder();


        FillingRule _fillingRule;
        Tesselate.Tesselator.WindingRuleType _tessWindingRuleType = Tesselate.Tesselator.WindingRuleType.NonZero;//default

        internal GLPainterCore(int id, int w, int h, int viewportW, int viewportH)
        {
            //-------------
            //y axis points upward (like other OpenGL)
            //x axis points to right.
            //please NOTE: left lower corner of the canvas is (0,0)
            //------------- 
            _id = id;
            //1.
            _shareRes = new ShaderSharedResource();
            //-----------------------------------------------------------------------             
            //2. set primary render sx, similar to AttachToRenderSurface()
            var primRenderSx = new GLRenderSurface(w, h, viewportW, viewportH, true);
            _rendersx = primRenderSx;
            GL.Viewport(0, 0, primRenderSx.Width, primRenderSx.Height);
            _vwHeight = primRenderSx.ViewportH;

            if (_originKind == RenderSurfaceOriginKind.LeftTop)
            {
                _shareRes.OrthoView = _rendersx._orthoFlipY_and_PullDown;
                _shareRes.IsFlipAndPulldownHint = true;
            }
            else
            {
                _shareRes.OrthoView = _rendersx._orthoView;
            }


            //----------------------------------------------------------------------- 
            //3. shaders 
            CachedBinaryShaderIO currentBinCache = CachedBinaryShaderIO.GetCurrentImpl();
            currentBinCache?.Open(); //open and lock shader cache file

            _solidColorFillShader = new SolidColorFillShader(_shareRes);
            _smoothLineShader = new SmoothLineShader(_shareRes);
            _invertedAlphaSmoothLineShader = new InvertAlphaLineSmoothShader(_shareRes);

            _rectFillShader = new RectFillShader(_shareRes); //for gradient color fill, and  polygon-shape gradient fill
            _radialGradientShader = new RadialGradientFillShader(_shareRes);
            //
            _bgrImgTextureShader = new BGRImageTextureShader(_shareRes); //BGR eg. from Win32 surface
            _bgraImgTextureShader = new BGRAImageTextureShader(_shareRes);

            _rgbaTextureShader = new RGBATextureShader(_shareRes);
            _rgbTextureShader = new RGBTextureShader(_shareRes);
            //
            _glyphStencilShader = new GlyphImageStecilShader(_shareRes);

            _lcdSubPixShader = new LcdSubPixShader(_shareRes);
            _lcdSubPixForSolidBgShader = new LcdSubPixShaderForSolidBg(_shareRes);
            _lcdSubPixShaderForWordStripCreation = new LcdSubPixShaderForWordStripCreation(_shareRes);
            _lcdSubPixShaderV2 = new LcdSubPixShaderV2(_shareRes);
            _lcdSubPixShaderV2.SetFillColor(Color.White);

            _maskShader_OneColor = new OneColorMaskShader(_shareRes);
            _maskShader_TwoColor = new TwoColorMaskShader(_shareRes);


            _blurShader = new BlurShader(_shareRes);
            //             

            _conv3x3TextureShader = new Conv3x3TextureShader(_shareRes);

            _msdfShader = new MsdfShader(_shareRes);
            _sdfShader = new SingleChannelSdf(_shareRes);


            currentBinCache?.Close(); //close the cache, let other app use the shader cache file
            CachedBinaryShaderIO.ClearCurrentImpl();
            //-----------------------------------------------------------------------
            //tools
            _tessTool = new TessTool();
            //-----------------------------------------------------------------------

            //GL.Enable(EnableCap.CullFace);
            //GL.FrontFace(FrontFaceDirection.Cw);
            //GL.CullFace(CullFaceMode.Back); 

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);//original **

            //GL.BlendFunc(BlendingFactorSrc.SrcColor, BlendingFactorDest.One);// not apply alpha to src
            //GL.BlendFuncSeparate(BlendingFactorSrc.SrcColor, BlendingFactorDest.OneMinusSrcAlpha,
            //                     BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            //GL.BlendFuncSeparate(BlendingFactorSrc.SrcColor, BlendingFactorDest.OneMinusSrcColor, BlendingFactorSrc.SrcAlpha, BlendingFactorDest.Zero);

            GL.ClearColor(1, 1, 1, 1);
            //-------------------------------------------------------------------------------
            //GL.Viewport(0, 0, width, height);
            //-------------------------------------------------------------------------------
            //1. original GLES (0,0) is on left-lower.
            //2. but our GLRenderSurface use Html5Canvas/SvgCanvas coordinate model 
            // so (0,0) is on LEFT-UPPER => so we need to FlipY

            OriginKind = RenderSurfaceOriginKind.LeftTop;
            EnableClipRect();

        }



        readonly static Dictionary<int, GLPainterCore> s_registeredPainterCors = new Dictionary<int, GLPainterCore>();
        static int s_painterCoreId;

        /// <summary>
        /// create primary GL painter core
        /// </summary>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <param name="viewportW"></param>
        /// <param name="viewportH"></param>
        /// <returns></returns>
        public static GLPainterCore Create(int w, int h, int viewportW, int viewportH, bool register)
        {
            //the canvas may need some init modules
            //so we start the canvass internaly here
            if (!register)
            {
                return new GLPainterCore(0, w, h, viewportW, viewportH);
            }
            else
            {
                int coreId = ++s_painterCoreId;
                var newPainterCore = new GLPainterCore(coreId, w, h, viewportW, viewportH);
                s_registeredPainterCors.Add(coreId, newPainterCore);
                return newPainterCore;
            }
        }

        public static bool TryGetRegisteredPainterCore(int coreId, out GLPainterCore found)
        {
            return s_registeredPainterCors.TryGetValue(coreId, out found);
        }
        public void AttachToRenderSurface(GLRenderSurface rendersx, bool updateTextureResult = true)
        {
            if (_rendersx == rendersx)
            {
                //same
                return;
            }
#if DEBUG
            if (rendersx == null)
            {

            }
#endif

            _rendersx.ReleaseCurrent(updateTextureResult);
            _rendersx = rendersx;

            GL.Viewport(0, 0, rendersx.Width, rendersx.Height);
            _vwHeight = rendersx.ViewportH;

            if (_originKind == RenderSurfaceOriginKind.LeftTop)
            {
                //TODO: review here, 
                //essential here
                _shareRes.OrthoView = _rendersx._orthoFlipY_and_PullDown;
                _shareRes.IsFlipAndPulldownHint = true;
            }
            else
            {
                _shareRes.OrthoView = _rendersx._orthoView;
            }

            _shareRes.SetOrthoViewOffset(0, 0);
            rendersx.SetAsCurrentSurface();
            SetCanvasOrigin(0, 0);//reset
            //TODO: width, height?
            SetClipRect(0, 0, rendersx.Height, rendersx.Height);
        }



        public GLRenderSurface CurrentRenderSurface => _rendersx;
        public int OriginX => _canvasOriginX;
        public int OriginY => _canvasOriginY;
        public ICoordTransformer CoordTransformer
        {
            get => _coordTransformer;
            set
            {

                if (value != null)
                {
                    switch (value.Kind)
                    {
                        case CoordTransformerKind.Affine3x2:
                            //current version we support only 
                            {
                                Affine aff1 = (Affine)value;
                                float[] elems = aff1.Get3x3MatrixElements();

                                _customCoordTransformer = new MyMat4(
                                    elems[0], elems[1], elems[2], 0,
                                    elems[3], elems[4], elems[5], 0,
                                    elems[6], elems[7], elems[8], 0,
                                    0, 0, 0, 1
                                    );

                                _coordTransformer = value;
                            }
                            break;
                        default:
                            //this version support only affine 3x2
                            _coordTransformer = null;
                            break;
                    }
                }
                else
                {
                    _coordTransformer = null;
                }
            }
        }

        public FillingRule FillingRule
        {
            get => _fillingRule;
            set
            {
                _fillingRule = value;
                switch (value)
                {
                    default://??
                    case FillingRule.NonZero:
                        _tessWindingRuleType = Tesselate.Tesselator.WindingRuleType.NonZero;
                        break;
                    case FillingRule.EvenOdd:
                        _tessWindingRuleType = Tesselate.Tesselator.WindingRuleType.Odd;
                        break;
                }
            }
        }
        public RenderSurfaceOriginKind OriginKind
        {
            get => _originKind;
            set
            {
                _originKind = value;
                if (_rendersx != null)
                {
                    if (_originKind == RenderSurfaceOriginKind.LeftTop)
                    {
                        if (!_shareRes.IsFlipAndPulldownHint)
                        {
                            _shareRes.OrthoView = _rendersx._orthoFlipY_and_PullDown;
                            _shareRes.IsFlipAndPulldownHint = true;
                        }
                    }
                    else
                    {
                        _shareRes.OrthoView = _rendersx._orthoView;
                    }
                    _shareRes.SetOrthoViewOffset(0, 0);
                }
            }
        }
        internal GLBitmap ResolveForGLBitmap(Image image)
        {
            //1.

            if (image is GLBitmap glBmp)
            {
                return glBmp;
            }
            //2. 

            glBmp = Image.GetCacheInnerImage(image) as GLBitmap;
            if (glBmp != null)
            {
                return glBmp;
            }
            //

            if (image is ImageBinder bmpBuffProvider)
            {
                glBmp = new GLBitmap(bmpBuffProvider);
            }
            else if (image is CpuBlit.MemBitmap memBmp)
            {
                glBmp = new GLBitmap(memBmp, false);
            }
            else
            {
                ////TODO: review here
                ////we should create 'borrow' method ? => send direct exact ptr to img buffer 
                ////for now, create a new one -- after we copy we, don't use it 
                //var req = new Image.ImgBufferRequestArgs(32, Image.RequestType.Copy);
                //image.RequestInternalBuffer(ref req);
                //int[] copy = req.OutputBuffer32;
                //glBmp = new GLBitmap(image.Width, image.Height, copy, req.IsInvertedImage);
                return null;
            }

            Image.SetCacheInnerImage(image, glBmp, true);//***
            return glBmp;
        }

        public unsafe void CopyPixels(int x, int y, int w, int h, IntPtr outputBuffer)
        {
            //GL.ReadPixels(x, y, w, h,
            //   PixelFormat.AbgrExt,
            //   PixelType.UnsignedByte,
            //   outputBuffer);
            GL.ReadPixels(x, y, w, h,
                OpenTK.Graphics.ES20.PixelFormat.Rgba,
                PixelType.UnsignedByte,
                outputBuffer);

        }
        //
        public int ViewportWidth => _rendersx.ViewportW;
        public int ViewportHeight => _rendersx.ViewportH;
        //
        public int CanvasWidth => _rendersx.Width;
        public int CanvasHeight => _rendersx.Height;
        //
        public void Dispose()
        {
        }
        public void DetachCurrentShader()
        {
            _shareRes._currentShader = null;
        }
        //
        public SmoothMode SmoothMode { get; set; }
        public PixelFarm.Drawing.Color FontFillColor { get; set; }

        public void Clear()
        {
            GL.ClearStencil(0);
            //actual clear here !
            GL.Clear(ClearBufferMask.ColorBufferBit |
                ClearBufferMask.DepthBufferBit |
                ClearBufferMask.StencilBufferBit);
        }
        public void Clear(PixelFarm.Drawing.Color c)
        {

            GL.ClearColor(
               (float)c.R / 255f,
               (float)c.G / 255f,
               (float)c.B / 255f,
               (float)c.A / 255f);
            GL.ClearStencil(0);
            //actual clear here !
            GL.Clear(ClearBufferMask.ColorBufferBit |
                ClearBufferMask.DepthBufferBit |
                ClearBufferMask.StencilBufferBit);
        }

        public void ClearColorBuffer()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);
        }
        public float StrokeWidth
        {
            get => _shareRes._strokeWidth;
            set => _shareRes._strokeWidth = value;
        }
        public Drawing.Color StrokeColor
        {
            get => _shareRes.StrokeColor;
            set => _shareRes.StrokeColor = value;
        }
        public void DrawLine(float x1, float y1, float x2, float y2)
        {
            switch (this.SmoothMode)
            {
                case SmoothMode.Smooth:
                    {
                        if (y1 == y2)
                        {
                            _solidColorFillShader.DrawLine(x1, y1, x2, y2, StrokeColor);
                        }
                        else
                        {
                            _smoothLineShader.DrawLine(x1, y1, x2, y2);
                        }
                    }
                    break;
                default:
                    {
                        if (StrokeWidth == 1)
                        {
                            _solidColorFillShader.DrawLine(x1, y1, x2, y2, StrokeColor);
                        }
                        else
                        {
                            //TODO: review stroke with for smooth line shader again
                            _shareRes._strokeWidth = this.StrokeWidth;
                            _smoothLineShader.DrawLine(x1, y1, x2, y2);
                        }
                    }
                    break;
            }
        }
        //-----------------------------------------------------------------
        //public void BlitRenderSurface(GLRenderSurface srcRenderSx, float left, float top, bool isFlipped = true)
        //{
        //    ////IMPORTANT: (left,top) != (x,y) 
        //    ////IMPORTANT: left,top position need to be adjusted with 
        //    ////Canvas' origin kind
        //    ////see https://github.com/PaintLab/PixelFarm/issues/43
        //    ////-----------
        //    //if (OriginKind == RenderSurfaceOrientation.LeftTop)
        //    //{
        //    //    //***
        //    //    top += srcRenderSx.Height;
        //    //}
        //    ////...
        //    //_rgbaTextureShader.Render(srcRenderSx.TextureId, left, top, srcRenderSx.Width, srcRenderSx.Height, isFlipped);
        //}

        public void DrawImage(GLBitmap bmp, float left, float top)
        {
            DrawImage(bmp, left, top, bmp.Width, bmp.Height);
        }
        //-----------------------------------------------------------------

        public void DrawSubImage(GLBitmap bmp, float srcLeft, float srcTop, float srcW, float srcH, float targetLeft, float targetTop)
        {
            if (OriginKind == RenderSurfaceOriginKind.LeftTop) //***
            {
                targetTop += srcH; //***
            }
            switch (bmp.BitmapFormat)
            {
                default: throw new NotSupportedException();
                case BitmapBufferFormat.RGBO:
                    _rgbTextureShader.DrawSubImage(bmp, srcLeft, srcTop, srcW, srcH, targetLeft, targetTop);
                    break;
                case BitmapBufferFormat.RGBA:
                    _rgbaTextureShader.DrawSubImage(bmp, srcLeft, srcTop, srcW, srcH, targetLeft, targetTop);
                    break;
                case BitmapBufferFormat.BGR:
                    _bgrImgTextureShader.DrawSubImage(bmp, srcLeft, srcTop, srcW, srcH, targetLeft, targetTop);
                    break;
                case BitmapBufferFormat.BGRA:
                    _bgraImgTextureShader.DrawSubImage(bmp, srcLeft, srcTop, srcW, srcH, targetLeft, targetTop);
                    break;
            }
        }
        public void DrawSubImage(GLBitmap bmp, in PixelFarm.Drawing.Rectangle srcRect, float targetLeft, float targetTop)
        {
            DrawSubImage(bmp, srcRect.Left, srcRect.Top, srcRect.Width, srcRect.Height, targetLeft, targetTop);
        }

        public void DrawSubImage(GLBitmap bmp, in PixelFarm.Drawing.Rectangle srcRect, float targetLeft, float targetTop, float scale)
        {
            if (OriginKind == RenderSurfaceOriginKind.LeftTop) //***
            {
                //***
                targetTop += srcRect.Height * scale;  //***
            }
            switch (bmp.BitmapFormat)
            {
                default: throw new NotSupportedException();
                case BitmapBufferFormat.RGBA:
                    _rgbaTextureShader.DrawSubImage(bmp, srcRect.Left, srcRect.Top, srcRect.Width, srcRect.Height, targetLeft, targetTop, scale);
                    break;
                case BitmapBufferFormat.BGR:
                    _bgrImgTextureShader.DrawSubImage(bmp, srcRect.Left, srcRect.Top, srcRect.Width, srcRect.Height, targetLeft, targetTop, scale);
                    break;
                case BitmapBufferFormat.BGRA:
                    _bgraImgTextureShader.DrawSubImage(bmp, srcRect.Left, srcRect.Top, srcRect.Width, srcRect.Height, targetLeft, targetTop, scale);
                    break;
            }
        }

        //---------------------------------------------------------------------------------------------------------------------------------
        public void DrawSubImageWithMsdf(GLBitmap bmp, ref PixelFarm.Drawing.Rectangle r, float targetLeft, float targetTop)
        {
            if (OriginKind == RenderSurfaceOriginKind.LeftTop)
            {
                //***
                targetTop += r.Height;
            }

#if DEBUG
            // bitmap must be rgba ***
            //if (bmp.BitmapFormat != BitmapBufferFormat.RGBA)
            //{
            //    System.Diagnostics.Debug.WriteLine(nameof(DrawSubImageWithMsdf) + ":not a bgra");
            //}
#endif

            _msdfShader.SetColor(this.FontFillColor);
            _msdfShader.DrawSubImage(bmp, r.Left, r.Top, r.Width, r.Height, targetLeft, targetTop);
        }
        public void DrawImagesWithMsdf_VBO(GLBitmap bmp, TextureCoordVboBuilder vboBuilder)
        {
            _msdfShader.LoadGLBitmap(bmp);
            _msdfShader.SetColor(this.FontFillColor);
            _msdfShader.DrawWithVBO(vboBuilder);
        }
        public void DrawSubImageWithMsdf(GLBitmap bmp, in PixelFarm.Drawing.Rectangle srcRect, float targetLeft, float targetTop, float scale)
        {
            //we expect that the bmp supports alpha value

            if (OriginKind == RenderSurfaceOriginKind.LeftTop)
            {
                //***
                targetTop += srcRect.Height;
            }

#if DEBUG
            // bitmap must be rgba ***
            //if (bmp.BitmapFormat != BitmapBufferFormat.RGBA)
            //{
            //    System.Diagnostics.Debug.WriteLine(nameof(DrawSubImageWithMsdf) + ":not a bgra");
            //}
#endif

            _msdfShader.SetColor(this.FontFillColor);
            _msdfShader.DrawSubImage(bmp, srcRect.Left, srcRect.Top, srcRect.Width, srcRect.Height, targetLeft, targetTop, scale);
        }
        public void DrawSubImageWithMsdf(GLBitmap bmp, float[] coords, float scale)
        {
#if DEBUG
            // bitmap must be rgba ***
            //if (bmp.BitmapFormat != BitmapBufferFormat.RGBA)
            //{
            //    System.Diagnostics.Debug.WriteLine(nameof(DrawSubImageWithMsdf) + ":not a bgra");
            //}
#endif
            _msdfShader.SetColor(this.FontFillColor);
            _msdfShader.DrawSubImages(bmp, coords, scale);
        }

        bool _repeatTexture = true;

        public bool RepeatTexture
        {
            get => _repeatTexture;
            set
            {
                //review here 
                if (_repeatTexture)
                {
                    //default
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);//restore, assume org is default
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);//restore, assume org is default
                }
                else
                {

#if XAMARIN
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, ((int)0x812D)); //ClampToBorder
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, ((int)0x812D)); //ClampToBorder
#else
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToBorder);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToBorder);
#endif


                }
            }
        }

        public bool UseTwoColorsMask { get; set; }

        float _maskColorSrcOffsetX;
        float _maskColorSrcOffsetY;
        public void SetColorMaskColorSourceOffset(float dx, float dy)
        {
            //color source offset for DrawImageWithMask
            _maskColorSrcOffsetX = dx;
            _maskColorSrcOffsetY = dy;
        }
        public void DrawImageWithMask(GLBitmap mask, GLBitmap colorSrc, float targetLeft, float targetTop)
        {
            DrawImageWithMask(mask, colorSrc,
                new RectangleF(0, 0, mask.Width, mask.Height),
                0, 0,
                targetLeft, targetTop);
        }
        public void DrawImageWithMask(GLBitmap mask, GLBitmap colorSrc,
            in PixelFarm.Drawing.RectangleF maskSrcRect,
            float colorSrcX, float colorSrcY,
            float targetLeft, float targetTop)
        {

            if (OriginKind == RenderSurfaceOriginKind.LeftTop)
            {
                //***
                targetTop += mask.Height;
            }

            if (UseTwoColorsMask)
            {
                _maskShader_TwoColor.LoadGLBitmap(mask);
                _maskShader_TwoColor.LoadColorSourceBitmap(colorSrc);
                _maskShader_TwoColor.SetColorSourceOffset(_maskColorSrcOffsetX, _maskColorSrcOffsetY);
                _maskShader_TwoColor.DrawSubImage2(maskSrcRect,
                    -colorSrcX, -colorSrcY,
                    targetLeft, targetTop);
            }
            else
            {
                _maskShader_OneColor.LoadGLBitmap(mask);
                _maskShader_OneColor.LoadColorSourceBitmap(colorSrc);
                _maskShader_OneColor.SetColorSourceOffset(_maskColorSrcOffsetX, _maskColorSrcOffsetY);
                _maskShader_OneColor.DrawSubImage2(maskSrcRect,
                    -colorSrcX, -colorSrcY,
                    targetLeft, targetTop);
            }
        }

        public void DrawImage(GLBitmap bmp,
            float left, float top, float w, float h)
        {
            //IMPORTANT: (left,top) != (x,y) 
            //IMPORTANT: left,top position need to be adjusted with 
            //Canvas' origin kind
            //see https://github.com/PaintLab/PixelFarm/issues/43
            //-----------
            if (OriginKind == RenderSurfaceOriginKind.LeftTop)
            {
                //***
                top += h;
            }
            switch (bmp.BitmapFormat)
            {
                default: throw new NotSupportedException();
                case BitmapBufferFormat.RGBO:
                    _rgbTextureShader.Render(bmp, left, top, w, h);
                    break;
                case BitmapBufferFormat.RGBA:
                    _rgbaTextureShader.Render(bmp, left, top, w, h);
                    break;
                case BitmapBufferFormat.BGR:
                    _bgrImgTextureShader.Render(bmp, left, top, w, h);
                    break;
                case BitmapBufferFormat.BGRA:
                    _bgraImgTextureShader.Render(bmp, left, top, w, h);
                    break;
            }
        }
        public void DrawImageToQuad(GLBitmap bmp, Affine affine)
        {
            Quad2f quad = new Quad2f(bmp.Width, OriginKind == RenderSurfaceOriginKind.LeftTop ? bmp.Height : -bmp.Height);
            quad.Transform(affine);
            DrawImageToQuad(bmp, quad);
        }

        public void DrawImageToQuad(GLBitmap bmp, in AffineMat affine)
        {
            Quad2f quad = new Quad2f(bmp.Width, OriginKind == RenderSurfaceOriginKind.LeftTop ? bmp.Height : -bmp.Height);
            quad.Transform(affine);
            DrawImageToQuad(bmp, quad);
        }

        public void DrawImageToQuad(GLBitmap bmp, in Quad2f quad)
        {
            bool flipY = false;
            if (OriginKind == RenderSurfaceOriginKind.LeftTop)
            {
                flipY = true;
                //***
                //y_adjust = -bmp.Height;
            }
            switch (bmp.BitmapFormat)
            {
                case BitmapBufferFormat.RGBA:
                    _rgbaTextureShader.Render(bmp, quad, flipY);
                    break;
                case BitmapBufferFormat.BGR:
                    _bgrImgTextureShader.Render(bmp, quad, flipY);
                    break;
                case BitmapBufferFormat.BGRA:
                    _bgraImgTextureShader.Render(bmp, quad, flipY);
                    break;
            }
        }

        public void DrawGlyphImageWithSubPixelRenderingTechnique(GLBitmap bmp, float left, float top)
        {
            Rectangle srcRect = new Rectangle(bmp.Width, bmp.Height);
            DrawGlyphImageWithSubPixelRenderingTechnique(bmp, srcRect, left, top);
        }

        public void DrawGlyphImageWithStecil(GLBitmap bmp, in PixelFarm.Drawing.Rectangle srcRect, float targetLeft, float targetTop, float scale)
        {
            if (OriginKind == RenderSurfaceOriginKind.LeftTop) //***
            {
                //***
                targetTop += srcRect.Height;  //***
            }

            _glyphStencilShader.SetCurrent();
            _glyphStencilShader.SetColor(this.FontFillColor);
            _glyphStencilShader.DrawSubImage(bmp, srcRect.Left, srcRect.Top, srcRect.Width, srcRect.Height, targetLeft, targetTop);
        }

        public void DrawGlyphImageWithStecil_VBO(GLBitmap bmp, TextureCoordVboBuilder vboBuilder)
        {
            _glyphStencilShader.LoadGLBitmap(bmp);
            _glyphStencilShader.SetCurrent();
            _glyphStencilShader.SetColor(this.FontFillColor);
            _glyphStencilShader.DrawWithVBO(vboBuilder);
        }

        public void DrawGlyphImageWithCopy_VBO(GLBitmap bmp, TextureCoordVboBuilder vboBuilder)
        {
            _bgraImgTextureShader.LoadGLBitmap(bmp);
            _bgraImgTextureShader.DrawWithVBO(vboBuilder);
        }

        /// <summary>
        ///Technique2: draw glyph by glyph
        /// </summary>
        /// <param name="srcRect"></param>
        /// <param name="targetLeft"></param>
        /// <param name="targetTop"></param>
        /// <param name="scale"></param>
        public void DrawGlyphImageWithSubPixelRenderingTechnique2_GlyphByGlyph(
          GLBitmap glbmp,
          in Drawing.Rectangle srcRect,
          float targetLeft,
          float targetTop,
          float scale)
        {
            if (OriginKind == RenderSurfaceOriginKind.LeftTop) //***
            {
                //***
                targetTop += srcRect.Height;  //***
            }

            _lcdSubPixShader.SetColor(FontFillColor);
            _lcdSubPixShader.LoadGLBitmap(glbmp);
            _lcdSubPixShader.DrawSubImage(
                srcRect.Left,
                srcRect.Top,
                srcRect.Width,
                srcRect.Height, targetLeft, targetTop);
        }

        public void DrawGlyphImageWithSubPixelRenderingTechnique3_DrawElements(GLBitmap glBmp, TextureCoordVboBuilder vboBuilder)
        {
            //version 3            
            _lcdSubPixShader.SetColor(FontFillColor);
            _lcdSubPixShader.DrawSubImages(glBmp, vboBuilder);
        }
        public void DrawGlyphImageWithSubPixelRenderingTechnique4_FromVBO(GLBitmap glBmp, VertexBufferObject vbo, int count, float x, float y)
        {
            _lcdSubPixShader.SetColor(FontFillColor); //original
            _lcdSubPixShader.DrawSubImages(glBmp, vbo, count, x, y);
        }
        public void DrawGlyphImageWithSubPixelRenderingTechnique4_ForWordStrip_FromVBO(GLBitmap glBmp, VertexBufferObject vbo, int count, float x, float y)
        {
            //special optimization for WordStrip creation
            //this verion not support overlap glyph (since overlap glyphs worstrip is not consider a transparent bg)

            _lcdSubPixShaderForWordStripCreation.DrawSubImages(glBmp, vbo, count, x, y);
            //_lcdSubPixShaderV2.NewDrawSubImage4FromVBO(glBmp, vbo, count, x, y);
            //so, temp fix, swap to orignal 
            //DrawGlyphImageWithSubPixelRenderingTechnique4_FromVBO(glBmp, vbo, count, x, y);
        }
        public void DrawGlyphImageWithStencilRenderingTechnique4_FromVBO(GLBitmap glBmp, VertexBufferObject vbo, int count, float x, float y)
        {
            _lcdSubPixShader.SetColor(FontFillColor);
            _lcdSubPixShader.DrawSubImagesNoLcdEffect(glBmp, vbo, count, x, y);
        }

        public void DrawGlyphImageWithCopyTech_FromVBO(GLBitmap glBmp, VertexBufferObject vbo, int count, float x, float y)
        {

            //use RGBA
            _rgbaTextureShader.DrawSubImages(glBmp, vbo, count, x, y);
            //_bgraImgTextureShader.DrawSubImages(glBmp, vbo, count, x, y);
        }
        public void DrawGlyphImageWithCopyBGRATech_FromVBO(GLBitmap glBmp, VertexBufferObject vbo, int count, float x, float y)
        {

            //use RGBA
            //_rgbaTextureShader.DrawSubImages(glBmp, vbo, count, x, y);
            _bgraImgTextureShader.DrawSubImages(glBmp, vbo, count, x, y);
        }
        public void DrawWordSpanWithStencilTechnique(GLBitmap bmp, float srcLeft, float srcTop, float srcW, float srcH, float targetLeft, float targetTop)
        {

            //similar to DrawSubImage(), use this for debug
            //DrawSubImage(bmp,
            //   srcLeft, srcTop,
            //   srcW, srcH,
            //   targetLeft,
            //   targetTop);

            if (OriginKind == RenderSurfaceOriginKind.LeftTop) //***
            {
                targetTop += srcH; //***
            }
            _lcdSubPixShader.SetColor(FontFillColor);
            _lcdSubPixShader.DrawSubImageNoLcdEffect(bmp, srcLeft, srcTop, srcW, srcH, targetLeft, targetTop);
        }
        public void DrawWordSpanWithCopyTechnique(GLBitmap bmp, float srcLeft, float srcTop, float srcW, float srcH, float targetLeft, float targetTop)
        {

            //similar to DrawSubImage(), use this for debug
            //DrawSubImage(bmp,
            //   srcLeft, srcTop,
            //   srcW, srcH,
            //   targetLeft,
            //   targetTop);

            if (OriginKind == RenderSurfaceOriginKind.LeftTop) //***
            {
                targetTop += srcH; //***
            }

            //_rgbaTextureShader.DrawSubImage(bmp, srcLeft, srcTop, srcW, srcH, targetLeft, targetTop);
            _bgraImgTextureShader.DrawSubImage(bmp, srcLeft, srcTop, srcW, srcH, targetLeft, targetTop);
        }
        public void DrawWordSpanWithLcdSubpixForSolidBgColor(GLBitmap bmp, float srcLeft, float srcTop, float srcW, float srcH, float targetLeft, float targetTop, Color textBgColor)
        {
            //lcd-effect subpix rendering, optimized version for solid bg color
            //

            if (OriginKind == RenderSurfaceOriginKind.LeftTop) //***
            {
                targetTop += srcH; //***
            }

            _lcdSubPixForSolidBgShader.SetBackgroundColor(textBgColor);
            _lcdSubPixForSolidBgShader.SetTextColor(FontFillColor);
            _lcdSubPixForSolidBgShader.DrawSubImageWithStencil(bmp, srcLeft, srcTop, srcW, srcH, targetLeft, targetTop);
        }


        public void DrawGlyphImageWithSubPixelRenderingTechnique(
            GLBitmap bmp,
            in Rectangle srcRect,
            float targetLeft,
            float targetTop)
        {

            //
            if (OriginKind == RenderSurfaceOriginKind.LeftTop)
            {
                //***
                targetTop += bmp.Height;
            }

#if DEBUG
            if (bmp.BitmapFormat == BitmapBufferFormat.RGBA)
            {

            }
#endif
            _lcdSubPixShader.LoadGLBitmap(bmp);
            _lcdSubPixShader.SetColor(this.FontFillColor);

            unsafe
            {
                float* srcDestList = stackalloc float[]
                {
                    srcRect.Left,
                    srcRect.Top,
                    srcRect.Width,
                    srcRect.Height,
                    targetLeft,
                    targetTop
                };

                //------------
                //TODO: review performance here ***
                //1. B , cyan result
                GL.ColorMask(false, false, true, false);
                _lcdSubPixShader.SetCompo(LcdSubPixShader.ColorCompo.C0);
                _lcdSubPixShader.UnsafeDrawSubImages(srcDestList, 6, 1);

                //float subpixel_shift = 1 / 9f;
                //---------------------------------------------------

                //2. G , magenta result
                GL.ColorMask(false, true, false, false);
                _lcdSubPixShader.SetCompo(LcdSubPixShader.ColorCompo.C1);
                _lcdSubPixShader.UnsafeDrawSubImages(srcDestList, 6, 1);

                //3. R , yellow result 
                _lcdSubPixShader.SetCompo(LcdSubPixShader.ColorCompo.C2);
                GL.ColorMask(true, false, false, false);//             
                _lcdSubPixShader.UnsafeDrawSubImages(srcDestList, 6, 1);

                //enable all color component
                GL.ColorMask(true, true, true, true);
            }
        }
        //-----------------------------------
        public void DrawImageWithBlurY(GLBitmap bmp, float left, float top)
        {
            if (OriginKind == RenderSurfaceOriginKind.LeftTop)
            {
                //***
                top += bmp.Height;
            }
            //TODO: review here not complete 

            _blurShader.IsHorizontal = false;
            _blurShader.Render(bmp, left, top, bmp.Width, bmp.Height);
        }
        public void DrawImageWithBlurX(GLBitmap bmp, float left, float top)
        {

            if (OriginKind == RenderSurfaceOriginKind.LeftTop)
            {
                //***
                top += bmp.Height;
            }

#if DEBUG
            // bitmap must be rgba ***
            if (bmp.BitmapFormat != BitmapBufferFormat.RGBA)
            {
                System.Diagnostics.Debug.WriteLine(nameof(DrawSubImageWithMsdf) + ":not a bgra");
            }
#endif

            _blurShader.IsHorizontal = true;
            _blurShader.Render(bmp, left, top, bmp.Width, bmp.Height);
        }
        public void DrawImageWithConv3x3(GLBitmap bmp, float[] kernel3x3, float top, float left)
        {
            if (OriginKind == RenderSurfaceOriginKind.LeftTop)
            {
                //***
                top += bmp.Height;
            }
#if DEBUG
            // bitmap must be rgba ***
            if (bmp.BitmapFormat != BitmapBufferFormat.RGBA)
            {
                System.Diagnostics.Debug.WriteLine(nameof(DrawSubImageWithMsdf) + ":not a bgra");
            }
#endif
            _conv3x3TextureShader.SetBitmapSize(bmp.Width, bmp.Height);
            _conv3x3TextureShader.SetConvolutionKernel(kernel3x3);
            _conv3x3TextureShader.Render(bmp, left, top, bmp.Width, bmp.Height);
        }
        public void DrawImageWithMsdf(GLBitmap bmp, float left, float top, float scale, Color c)
        {
            if (OriginKind == RenderSurfaceOriginKind.LeftTop)
            {
                //***
                top += bmp.Height;
            }
            _msdfShader.SetColor(c);
            _msdfShader.Render(bmp, left * scale, top * scale, bmp.Width * scale, bmp.Height * scale);
        }
        public void DrawImageWithMsdf(GLBitmap bmp, Quad2f quad, Color c, bool flipY = false)
        {
            _msdfShader.SetColor(c);
            _msdfShader.Render(bmp, quad, flipY);
        }
        public void DrawImageWithSdf(GLBitmap bmp, float x, float y, float scale)
        {
            //TODO: review x,y or lef,top ***
            _sdfShader.SetColor(PixelFarm.Drawing.Color.Black);
            _sdfShader.Render(bmp, x, y, bmp.Width * scale, bmp.Height * scale);
        }

        //-------------------------------------------------------------------------------
        float[] _rect_coords = new float[8]; //resuable rect coord
        public void FillRect(Drawing.Color color, double left, double top, double width, double height)
        {
            //left,bottom,width,height
            //skip alpha may effect some sitution
            if (color.A == 0) { return; }

            SimpleTessTool.CreateRectTessCoordsTriStrip((float)left, (float)(top + height), (float)width, (float)height, _rect_coords);
            FillTriangleStrip(color, _rect_coords, 4);
        }
        public void ClearRect(Drawing.Color color, double left, double top, double width, double height)
        {
            //if (color.A == 0)=> must fill too***
            SimpleTessTool.CreateRectTessCoordsTriStrip((float)left, (float)(top + height), (float)width, (float)height, _rect_coords);
            _solidColorFillShader.FillTriangleStripWithVertexBuffer(_rect_coords, 4, color);
        }
        void FillTriangleStrip(Drawing.Color color, float[] coords, int n)
        {
            if (color.A == 0) { return; }
            _solidColorFillShader.FillTriangleStripWithVertexBuffer(coords, n, color);
        }
        public void FillTriangleFan(Drawing.Color color, float[] coords, int n)
        {
            if (color.A == 0) { return; }
            unsafe
            {
                fixed (float* head = &coords[0])
                {
                    _solidColorFillShader.FillTriangleFan(head, n, color);
                }
            }
        }
        //-------------------------------------------------------------------------------
        //RenderVx
        public void FillRenderVx(Drawing.Brush brush, Drawing.RenderVx renderVx)
        {
            if (!(renderVx is PathRenderVx glRenderVx)) return;
            //
            FillGfxPath(brush, glRenderVx);
        }
        public void FillRenderVx(Drawing.Color color, Drawing.RenderVx renderVx)
        {
            if (!(renderVx is PathRenderVx glRenderVx)) return;

            FillGfxPath(color, glRenderVx);

        }
        public void DrawRenderVx(Drawing.Color color, Drawing.RenderVx renderVx)
        {
            if (!(renderVx is PathRenderVx glRenderVx)) return;

            DrawGfxPath(color, glRenderVx);
        }
        internal void FillTessArea(Drawing.Color color, float[] coords, ushort[] indices)
        {
            _solidColorFillShader.FillTriangles(coords, indices, color);
        }

        VBOStream GetVBOStreamOrBuildIfNotExists(PathRenderVx pathRenderVx)
        {
            VBOStream tessVBOStream = pathRenderVx._tessVBOStream;
            if (tessVBOStream == null)
            {
                //create vbo for this render vx
                pathRenderVx._tessVBOStream = tessVBOStream = new VBOStream();
                pathRenderVx._isTessVBOStreamOwner = true;

                pathRenderVx.CreateAreaTessVBOSegment(tessVBOStream, _tessTool, _tessWindingRuleType);
                pathRenderVx.CreateSmoothBorderTessSegment(tessVBOStream, _smoothBorderBuilder);

                //then render with vbo 
                tessVBOStream.BuildBuffer();
            }
            return tessVBOStream;
        }

        public void FillGfxPath(Drawing.Color fillColor, Drawing.Color fineBorderColor, PathRenderVx pathRenderVx)
        {
            switch (SmoothMode)
            {
                case SmoothMode.No:
                    {
                        if (pathRenderVx._enableVBO)
                        {

                            VBOStream tessVBOStream = GetVBOStreamOrBuildIfNotExists(pathRenderVx);

                            tessVBOStream.Bind();

                            _solidColorFillShader.FillTriangles(
                                pathRenderVx._tessAreaVboSeg.startAt,
                                pathRenderVx._tessAreaVboSeg.vertexCount,
                                fillColor);

                            tessVBOStream.Unbind();
                        }
                        else
                        {
                            int subPathCount = pathRenderVx.FigCount;
                            //alll subpath use the same color setting 
                            if (subPathCount > 1)
                            {
                                float[] tessArea = pathRenderVx.GetAreaTess(_tessTool, _tessWindingRuleType);
                                if (tessArea != null)
                                {
                                    _solidColorFillShader.FillTriangles(tessArea, pathRenderVx.TessAreaVertexCount, fillColor);
                                }
                            }
                            else
                            {
                                for (int i = 0; i < subPathCount; ++i)
                                {
                                    Figure figure = pathRenderVx.GetFig(i);

                                    float[] tessArea = figure.GetAreaTess(_tessTool, _tessWindingRuleType, TessTriangleTechnique.DrawArray);
                                    if (tessArea != null)
                                    {
                                        _solidColorFillShader.FillTriangles(tessArea, figure.TessAreaVertexCount, fillColor);
                                    }
                                }
                            }
                        }

                    }
                    break;
                case SmoothMode.Smooth:
                    {

                        float saved_Width = StrokeWidth;
                        Drawing.Color saved_Color = StrokeColor;

                        //temp set stroke width to 2 amd stroke color
                        //to the same as bg color (for smooth border).
                        //and it will be set back later.
                        // 
                        StrokeColor = fillColor;
                        StrokeWidth = 1.5f; //TODO: review this *** 

                        if (pathRenderVx._enableVBO)
                        {
                            VBOStream tessVBOStream = GetVBOStreamOrBuildIfNotExists(pathRenderVx);

                            tessVBOStream.Bind();

                            _solidColorFillShader.FillTriangles(
                                pathRenderVx._tessAreaVboSeg.startAt,
                                pathRenderVx._tessAreaVboSeg.vertexCount,
                                fillColor);

                            StrokeColor = fineBorderColor;
                            _smoothLineShader.DrawTriangleStrips(
                                pathRenderVx._smoothBorderVboSeg.startAt,
                                pathRenderVx._smoothBorderVboSeg.vertexCount);
                            //
                            tessVBOStream.Unbind();
                        }
                        else
                        {
                            int subPathCount = pathRenderVx.FigCount;
                            //all subpath use the same color setting 
                            //merge all subpath

                            if (subPathCount > 1)
                            {
                                float[] tessArea = pathRenderVx.GetAreaTess(_tessTool, _tessWindingRuleType);
                                if (tessArea != null)
                                {
                                    _solidColorFillShader.FillTriangles(tessArea, pathRenderVx.TessAreaVertexCount, fillColor);
                                }
                                StrokeColor = fineBorderColor;
                                _smoothLineShader.DrawTriangleStrips(
                                    pathRenderVx.GetSmoothBorders(_smoothBorderBuilder),
                                    pathRenderVx.BorderTriangleStripCount);
                            }
                            else
                            {
                                Figure figure = pathRenderVx.GetFig(0);
                                float[] tessArea;
                                if ((tessArea = figure.GetAreaTess(_tessTool, _tessWindingRuleType, TessTriangleTechnique.DrawArray)) != null)
                                {
                                    //draw area
                                    _solidColorFillShader.FillTriangles(tessArea, figure.TessAreaVertexCount, fillColor);
                                    //draw smooth border
                                    StrokeColor = fineBorderColor;
                                    _smoothLineShader.DrawTriangleStrips(
                                        figure.GetSmoothBorders(_smoothBorderBuilder),
                                        figure.BorderTriangleStripCount);
                                }
                            }
                        }

                        //restore stroke width and color
                        StrokeWidth = saved_Width; //restore back
                        StrokeColor = saved_Color;
                    }
                    break;
            }
        }

        public void FillGfxPath(Drawing.Color color, PathRenderVx pathRenderVx)
        {
            FillGfxPath(color, color, pathRenderVx);
        }

        public void FillRect(Brush brush, double left, double top, double width, double height)
        {
            //left,bottom,width,height         

            SimpleTessTool.CreateRectTessCoordsTriStrip((float)left, (float)(top + height), (float)width, (float)height, _rect_coords);
            switch (brush.BrushKind)
            {
                default: throw new NotSupportedException();
                case BrushKind.Solid:
                    {
                        SolidBrush sb = (SolidBrush)brush;
                        _solidColorFillShader.FillTriangleStripWithVertexBuffer(_rect_coords, 4, sb.Color);
                    }
                    break;
                case BrushKind.LinearGradient:
                    {
                        LinearGradientBrush glGrBrush = LinearGradientBrush.Resolve((Drawing.LinearGradientBrush)brush);
                        _rectFillShader.Render(glGrBrush._v2f, glGrBrush._colors);
                    }
                    break;
                case BrushKind.CircularGradient:
                    {
                        RadialGradientBrush glGrBrush = RadialGradientBrush.Resolve((Drawing.RadialGradientBrush)brush);
                        _radialGradientShader.Render(
                                              glGrBrush._v2f,
                                              glGrBrush._cx,
                                              _vwHeight - glGrBrush._cy,
                                              glGrBrush._r,
                                              glGrBrush._invertedAff,
                                              glGrBrush._lookupBmp);
                    }
                    break;
                case BrushKind.PolygonGradient:
                    {
                        PolygonGradientBrush glGrBrush = PolygonGradientBrush.Resolve((Drawing.PolygonGradientBrush)brush, _tessTool);
                        _rectFillShader.Render(glGrBrush._v2f, glGrBrush._colors);
                    }
                    break;
                case BrushKind.Texture:
                    {
                        PixelFarm.Drawing.TextureBrush tbrush = (PixelFarm.Drawing.TextureBrush)brush;
                        //TODO: review here
                    }
                    break;
            }

        }


        public void FillGfxPath(Drawing.Brush brush, PathRenderVx pathRenderVx)
        {
            if (brush.BrushKind == BrushKind.Solid)
            {
                var solidBrush = (PixelFarm.Drawing.SolidBrush)brush;
                FillGfxPath(solidBrush.Color, pathRenderVx);
            }
            else
            {
                //TODO: review here again
                //use VBO?
                //TODO: create mask on another render suface and use shader+mask is more simple

                //we use mask technique
                //1. generate mask bitmap
                //find bound of path render vx

                RectangleF bounds = pathRenderVx.GetBounds();
                int bounds_left = (int)Math.Round(bounds.Left);
                int bounds_top = (int)Math.Round(bounds.Top);
                int size_w = (int)Math.Round(bounds.Width);
                int size_h = (int)Math.Round(bounds.Height);

                GLRenderSurface renderSx_mask = new GLRenderSurface(size_w, size_h); //mask color surface 
                using (TempSwitchToNewSurface(renderSx_mask))
                {
                    //after switch to a new surface
                    //canvas offset is reset to (0,0) of the new surface 

                    //for mask, fill bg with black
                    Clear(Color.Black);

                    //offset to (left,top) of bounds
                    SetCanvasOrigin(-bounds_left, -bounds_top);
                    FillGfxPath(Color.White, Color.Red, pathRenderVx); //
                    SetCanvasOrigin(0, 0);//offset back
                }

                //DrawImage(renderSx_mask.GetGLBitmap(), 0, 0);//for debug, show mask 

                //2. generate gradient color, (TODO: cache, NOT need to generate this every time)
                GLBitmap color_src;

                switch (brush.BrushKind)
                {
                    default: throw new NotSupportedException();

                    case BrushKind.LinearGradient:
                        {
                            //we can cache the gradient color for this bitmap
                            //so we don't need to create every time

                            LinearGradientBrush glGrBrush = LinearGradientBrush.Resolve((Drawing.LinearGradientBrush)brush);
                            if (glGrBrush.CacheGradientBitmap != null)
                            {
                                color_src = glGrBrush.CacheGradientBitmap;
                            }
                            else
                            {
                                //create a new one and cache
                                color_src = new GLBitmap(size_w, size_h);
                                var renderSx_color = new GLRenderSurface(color_src, false); //gradient color surface

                                ////brush origin?
                                ////this can be configured
                                ////1. relative to bounds of pathRenderVx
                                ////2. relative to other specific position 
                                using (TempSwitchToNewSurface(renderSx_color))
                                {
                                    _rectFillShader.Render(glGrBrush._v2f, glGrBrush._colors);
                                }
                                glGrBrush.SetCacheGradientBitmap(color_src, true);

                                //FillRect(Color.Yellow, 0, 0, 200, 100);
                                //for debug,                                                           
                                renderSx_color.Dispose();
                            }
                            //DrawImage(color_src, 0, 0);//for debug show color-gradient
                        }
                        break;
                    case BrushKind.CircularGradient:
                        {
                            RadialGradientBrush glGrBrush = RadialGradientBrush.Resolve((Drawing.RadialGradientBrush)brush);
                            if (glGrBrush.CacheGradientBitmap != null)
                            {
                                color_src = glGrBrush.CacheGradientBitmap;
                            }
                            else
                            {
                                color_src = new GLBitmap(size_w, size_h);
                                var renderSx_color = new GLRenderSurface(color_src, false); //gradient color surface
                                using (TempSwitchToNewSurface(renderSx_color))
                                {
                                    _radialGradientShader.Render(
                                                glGrBrush._v2f,
                                                glGrBrush._cx,
                                                _vwHeight - glGrBrush._cy,
                                                glGrBrush._r,
                                                glGrBrush._invertedAff,
                                                glGrBrush._lookupBmp);
                                    color_src = renderSx_color.GetGLBitmap();
                                }
                                glGrBrush.SetCacheGradientBitmap(color_src, true);
                                renderSx_color.Dispose();
                                //DrawImage(color_src, 0, 0);//for debug show color gradient 
                            }
                        }
                        break;
                    case BrushKind.PolygonGradient:
                        {
                            PolygonGradientBrush glGrBrush = PolygonGradientBrush.Resolve((Drawing.PolygonGradientBrush)brush, _tessTool);
                            if (glGrBrush.CacheGradientBitmap != null)
                            {
                                color_src = glGrBrush.CacheGradientBitmap;
                            }
                            else
                            {
                                color_src = new GLBitmap(size_w, size_h);
                                var renderSx_color = new GLRenderSurface(color_src, false); //gradient color surface
                                using (TempSwitchToNewSurface(renderSx_color))
                                {
                                    _rectFillShader.Render(glGrBrush._v2f, glGrBrush._colors);
                                    color_src = renderSx_color.GetGLBitmap();
                                }
                                glGrBrush.SetCacheGradientBitmap(color_src, true);
                                renderSx_color.Dispose();
                            }
                            //DrawImage(color_src, 0, 0);//for debug show color gradient 
                        }
                        break;
                    case BrushKind.Texture:
                        {
                            //TODO: implement here
                            //see mask
                            PixelFarm.Drawing.TextureBrush tbrush = (PixelFarm.Drawing.TextureBrush)brush;
                            color_src = ResolveForGLBitmap(tbrush.TextureImage);
                        }
                        break;
                }

                //SetColorMaskColorSourceOffset(bounds.Left, bounds.Top);

                //move origin to (left,top) of bounds
                int ox = OriginX;
                int oy = OriginY;
                SetCanvasOrigin(ox + bounds_left, oy + bounds_top);
                DrawImageWithMask(renderSx_mask.GetGLBitmap(), color_src, 0, 0);
                SetCanvasOrigin(ox, oy);//restore

                renderSx_mask.Dispose();
            }
        }

        public void EnableMask(PathRenderVx pathRenderVx)
        {


            GL.ClearStencil(0); //set value for clearing stencil buffer 
                                //actual clear here
            GL.Clear(ClearBufferMask.StencilBufferBit);
            //-------------------
            //disable rendering to color buffer
            GL.ColorMask(false, false, false, false);
            //start using stencil
            GL.Enable(EnableCap.StencilTest);
            //place a 1 where rendered
            GL.StencilFunc(StencilFunction.Always, 1, 1);
            //replace where rendered
            GL.StencilOp(StencilOp.Replace, StencilOp.Replace, StencilOp.Replace);
            //render  to stencill buffer
            //-----------------

            int m = pathRenderVx.FigCount;
            for (int b = 0; b < m; ++b)
            {
                Figure fig = pathRenderVx.GetFig(b);
                float[] tessArea = fig.GetAreaTess(_tessTool, _tessWindingRuleType, TessTriangleTechnique.DrawArray);
                //-------------------------------------   
                if (tessArea != null)
                {
                    _solidColorFillShader.FillTriangles(tessArea, fig.TessAreaVertexCount, PixelFarm.Drawing.Color.Black);
                }
            }

            //-------------------------------------- 
            //render color
            //--------------------------------------  
            //reenable color buffer 
            GL.ColorMask(true, true, true, true);
            //where a 1 was not rendered
            GL.StencilFunc(StencilFunction.Equal, 1, 1);
            //freeze stencill buffer
            GL.StencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Keep);
            //------------------------------------------
            //we already have valid ps from stencil step
            //------------------------------------------

            //-------------------------------------------------------------------------------------
            //1.  we draw only alpha chanel of this black color to destination color
            //so we use  BlendFuncSeparate  as follow ... 
            //-------------------------------------------------------------------------------------
            //1.  we draw only alpha channel of this black color to destination color
            //so we use  BlendFuncSeparate  as follow ... 
            GL.ColorMask(false, false, false, true);
            //GL.BlendFuncSeparate(
            //     BlendingFactorSrc.DstColor, BlendingFactorDest.DstColor, //the same
            //     BlendingFactorSrc.One, BlendingFactorDest.Zero);

            //use alpha chanel from source***
            GL.BlendFunc(BlendingFactorSrc.One, BlendingFactorDest.Zero);

            //
            //TODO: review smooth border filll here ***
            //
            //float[] smoothBorder = fig.GetSmoothBorders(_smoothBorderBuilder);
            Color prevStrokeColor = StrokeColor;
            float preStrokeW = StrokeWidth;
            StrokeColor = Color.White;
            StrokeWidth = 1.5f;
            for (int b = 0; b < m; ++b)
            {
                Figure f = pathRenderVx.GetFig(b);
                //-------------------------------------   
                _invertedAlphaSmoothLineShader.DrawTriangleStrips(
                               f.GetSmoothBorders(_smoothBorderBuilder),
                               f.BorderTriangleStripCount);

                //_smoothLineShader.DrawTriangleStrips(
                //               f.GetSmoothBorders(_smoothBorderBuilder),
                //               f.BorderTriangleStripCount);
            }
            StrokeColor = prevStrokeColor;
            StrokeWidth = preStrokeW;
            //at this point,alpha component is fill in to destination 
            //-------------------------------------------------------------------------------------
            //2. then fill again!, 
            //we use alpha information from dest, 
            //so we set blend func to ... GL.BlendFunc(BlendingFactorSrc.DstAlpha, BlendingFactorDest.OneMinusDstAlpha)    
            GL.ColorMask(true, true, true, true);
            GL.BlendFunc(BlendingFactorSrc.DstAlpha, BlendingFactorDest.OneMinusDstAlpha);
        }
        public void DisableMask()
        {

            ////restore back 
            ////3. switch to normal blending mode 
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.Disable(EnableCap.StencilTest);
        }
        public void DrawGfxPath(Drawing.Color color, PathRenderVx igpth)
        {
            //TODO: review here againerr
            //use VBO?
            //
            switch (SmoothMode)
            {
                case SmoothMode.No:
                    {

                        int subPathCount = igpth.FigCount;
                        for (int i = 0; i < subPathCount; ++i)
                        {
                            Figure f = igpth.GetFig(i);
                            float[] coordXYs = f.coordXYs;
                            unsafe
                            {
                                fixed (float* head = &coordXYs[0])
                                {
                                    _solidColorFillShader.DrawLineLoopWithVertexBuffer(head, coordXYs.Length / 2, StrokeColor);
                                }
                            }
                        }
                    }
                    break;
                case SmoothMode.Smooth:
                    {
                        //
                        StrokeColor = color;
                        float prevStrokeW = StrokeWidth;
                        if (prevStrokeW < 0.25f)
                        {
                            StrokeWidth = 0.25f;
                        }
                        //
                        int subPathCount = igpth.FigCount;
                        for (int i = 0; i < subPathCount; ++i)
                        {
                            Figure f = igpth.GetFig(i);
                            _smoothLineShader.DrawTriangleStrips(
                                f.GetSmoothBorders(_smoothBorderBuilder),
                                f.BorderTriangleStripCount);
                        }
                        StrokeWidth = prevStrokeW;
                        //
                    }
                    break;
            }
        }
        public void SetCanvasOrigin(int x, int y)
        {
            _canvasOriginX = x;
            _canvasOriginY = y;
            if (_rendersx == null)
            {
                return;
            }


            //TODO: review here again ***
            if (_coordTransformer == null)
            {
                if (!_shareRes.IsFlipAndPulldownHint)
                {
                    _shareRes.OrthoView = _rendersx._orthoFlipY_and_PullDown;
                    _shareRes.IsFlipAndPulldownHint = true;
                }
                _shareRes.SetOrthoViewOffset(x, y);
            }
            else
            {
                _shareRes.OrthoView = _rendersx._orthoFlipY_and_PullDown *
                                      MyMat4.translate(new OpenTK.Vector3(x, y, 0)) *
                                      _customCoordTransformer;

                _shareRes.SetOrthoViewOffset(0, 0);//*** we reset this because=> we have multiply (x,y) into the ortho view.
            }


            //old => not correct!   
            //leave here to study
            //: if we set viewport to (x,y,viewport_w,viewport_h)
            // then we draw image that larger (eg.img_h> viewport_h)
            // the image is crop! (eg. see example in scrollview example)
            // so we set ortho metrix instead
            //
            //GL.Viewport(x,
            //    (OriginKind == RenderSurfaceOrientation.LeftTop) ? -y : y,
            //    _width,
            //    _height);
        }
        public void EnableClipRect()
        {
            GL.Enable(EnableCap.ScissorTest);
        }
        public void DisableClipRect()
        {
            GL.Disable(EnableCap.ScissorTest);
        }

        int _scss_left;
        int _scss_bottom;
        int _scss_width;
        int _scss_height;

        public Rectangle GetClipRect()
        {
            //int bottom = OriginKind == RenderSurfaceOriginKind.LeftTop ? _vwHeight - _scss_bottom : _scss_bottom;
            //return Rectangle.FromLTRB(_scss_left - _canvasOriginX, bottom - _scss_height, _scss_width + _scss_left, bottom);

            int bottom = OriginKind == RenderSurfaceOriginKind.LeftTop ? _vwHeight - _scss_bottom : _scss_bottom;
            return Rectangle.FromLTRB(_scss_left - _canvasOriginX, bottom - _scss_height - _canvasOriginY, _scss_width + _scss_left, bottom);
        }
        public void SetClipRect(int left, int top, int width, int height)
        {

            //#if DEBUG
            //            System.Diagnostics.Debug.WriteLine("clip:" + left + "," + top + "," + width + "," + height);
            //#endif

            int new_left = left + _canvasOriginX;
            int bottom = _canvasOriginY + top + height;
            int new_bottom = (OriginKind == RenderSurfaceOriginKind.LeftTop) ? _vwHeight - bottom : bottom;

            if (_scss_left != new_left || _scss_bottom != new_bottom || _scss_width != width || _scss_height != height)
            {

                //#if DEBUG
                //                System.Diagnostics.Debug.WriteLine("clip_scissor:" + new_left + "," + new_bottom + "," + width + "," + height);
                //#endif
                GL.Scissor(
                    _scss_left = new_left,
                    _scss_bottom = new_bottom,
                    _scss_width = width,
                    _scss_height = height);
            }
        }
        internal TessTool GetTessTool() => _tessTool;
        internal SmoothBorderBuilder GetSmoothBorderBuilder() => _smoothBorderBuilder;

        public void SaveStates(out GLPainterStatesData stateData)
        {
            stateData = new GLPainterStatesData
            {
                renderSurface = CurrentRenderSurface,
                canvas_origin_X = _canvasOriginX,
                canvas_origin_Y = _canvasOriginY,
                originKind = OriginKind,
                clipRect = GetClipRect()
            };
        }
        public void RestoreStates(in GLPainterStatesData stateData)
        {
            AttachToRenderSurface(stateData.renderSurface);
            this.OriginKind = stateData.originKind;

            SetCanvasOrigin(stateData.canvas_origin_X, stateData.canvas_origin_Y);
            Rectangle clipRect = stateData.clipRect;
            SetClipRect(clipRect.Left, clipRect.Top, clipRect.Width, clipRect.Height);

        }
        /// <summary>
        /// temporary switch to another render surface,and switch back after exist using context
        /// </summary>
        /// <param name="newRenderSurface"></param>
        /// <returns></returns>
        public GLContextAutoSwitchBack TempSwitchToNewSurface(GLRenderSurface newRenderSurface)
        {
            GLContextAutoSwitchBack swBack = new GLContextAutoSwitchBack(this);

            AttachToRenderSurface(newRenderSurface);
            OriginKind = RenderSurfaceOriginKind.LeftTop;

            return swBack;
        }
    }


    public struct GLContextAutoSwitchBack : IDisposable
    {
        GLPainterStatesData _stateData;
        GLPainterCore _owner;
        public GLContextAutoSwitchBack(GLPainterCore core)
        {
            _owner = core;
            _owner.SaveStates(out _stateData);
        }
        public void Dispose()
        {
            _owner?.RestoreStates(_stateData);
            _owner = null;
        }
    }

    public struct GLPainterStatesData
    {
        internal GLRenderSurface renderSurface;
        internal int canvas_origin_X;
        internal int canvas_origin_Y;
        internal Rectangle clipRect;
        internal RenderSurfaceOriginKind originKind;
    }


    static class SimpleTessTool
    {
        /// <summary>
        /// create coord for left-bottom-origin canvas
        /// </summary>
        /// <param name="left"></param>
        /// <param name="bottom"></param>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <param name="output"></param>
        public static void CreateRectTessCoordsTriStrip(float left, float bottom, float w, float h, float[] output)
        {
            //use original GLES coord base => (0,0)= left,bottom 
            output[0] = left; output[1] = bottom - h;
            output[2] = left; output[3] = bottom;
            output[4] = left + w; output[5] = bottom - h;
            output[6] = left + w; output[7] = bottom;
        }
    }



    public class TextureCoordVboBuilder
    {

        int _orgBmpW;
        int _orgBmpH;
        bool _bmpYFlipped;
        RenderSurfaceOriginKind _pcxOrgKind;

        ArrayList<float> _vertexList;
        ArrayList<ushort> _indexList;

        int _v_startIndex;
        int _i_startIndex;

        ushort _indexCount = 0;

        public TextureCoordVboBuilder()
        {
        }
        public void SetArrayLists(ArrayList<float> vertexList, ArrayList<ushort> indexlist)
        {
            _v_startIndex = vertexList.Count;
            _i_startIndex = indexlist.Count;

            _indexCount = 0;//***
            _vertexList = vertexList;
            _indexList = indexlist;
        }
        public ArrayListSegment<float> CreateVertextListSpan() => new ArrayListSegment<float>(_vertexList, _v_startIndex, _vertexList.Count - _v_startIndex);
        public ArrayListSegment<ushort> CreateIndexListSpan() => new ArrayListSegment<ushort>(_indexList, _i_startIndex, _indexList.Count - _i_startIndex);

        public void SetTextureInfo(int width, int height, bool isYFlipped, RenderSurfaceOriginKind pcxOrgKind)
        {
            _orgBmpW = width;
            _orgBmpH = height;
            _bmpYFlipped = isYFlipped;
            _pcxOrgKind = pcxOrgKind;
        }


        public void WriteRect(
            in PixelFarm.Drawing.Rectangle srcRect,
            float targetLeft,
            float targetTop,
            float scale)
        {

#if DEBUG
            if (_orgBmpW == 0 || _orgBmpH == 0)
            {
                //please SetTextureInfo()
                System.Diagnostics.Debugger.Break();
            }
#endif

            if (_pcxOrgKind == RenderSurfaceOriginKind.LeftTop) //***
            {
                //***
                targetTop += srcRect.Height;  //***
            }

            // https://developer.apple.com/library/content/documentation/3DDrawing/Conceptual/OpenGLES_ProgrammingGuide/TechniquesforWorkingwithVertexData/TechniquesforWorkingwithVertexData.html

            //ushort indexCount = (ushort)_indexList.Count;

            ArrayList<float> vertexLst = _vertexList;
            ArrayList<ushort> indxLst = _indexList;
            if (_indexCount > 0)
            {

                //add degenerative triangle 
                int buff_count = vertexLst.Count;
                float prev_5 = vertexLst[buff_count - 5];
                float prev_4 = vertexLst[buff_count - 4];
                float prev_3 = vertexLst[buff_count - 3];
                float prev_2 = vertexLst[buff_count - 2];
                float prev_1 = vertexLst[buff_count - 1];

                vertexLst.Append(prev_5); vertexLst.Append(prev_4); vertexLst.Append(prev_3);
                vertexLst.Append(prev_2); vertexLst.Append(prev_1);


                indxLst.Append((ushort)(_indexCount));
                indxLst.Append((ushort)(_indexCount + 1));

                _indexCount += 2;
            }

            //---------
            RectangleF normalizedSrc = srcRect.CreateNormalizedRect(_orgBmpW, _orgBmpH);
            RectangleF target = new RectangleF(targetLeft, targetTop, srcRect.Width, srcRect.Height);
            WriteToVboStream(vertexLst, _indexCount > 0,
                normalizedSrc,
                target,
                _bmpYFlipped);

            indxLst.Append(_indexCount);
            indxLst.Append((ushort)(_indexCount + 1));
            indxLst.Append((ushort)(_indexCount + 2));
            indxLst.Append((ushort)(_indexCount + 3));

            _indexCount += 4;
        }


        public void WriteRectWithRotation(
            in PixelFarm.Drawing.Rectangle srcRect,
            float targetLeft,
            float targetTop,
            float srcRotation)
        {

#if DEBUG
            if (_orgBmpW == 0 || _orgBmpH == 0)
            {
                //please SetTextureInfo()
                System.Diagnostics.Debugger.Break();
            }
#endif

            if (_pcxOrgKind == RenderSurfaceOriginKind.LeftTop) //***
            {
                //***
                targetTop += srcRect.Height;  //***
            }

            // https://developer.apple.com/library/content/documentation/3DDrawing/Conceptual/OpenGLES_ProgrammingGuide/TechniquesforWorkingwithVertexData/TechniquesforWorkingwithVertexData.html

            ArrayList<float> vertexLst = _vertexList;
            ArrayList<ushort> indxLst = _indexList;
            if (_indexCount > 0)
            {

                //add degenerative triangle
                int buff_count = vertexLst.Count;
                float prev_5 = vertexLst[buff_count - 5];
                float prev_4 = vertexLst[buff_count - 4];
                float prev_3 = vertexLst[buff_count - 3];
                float prev_2 = vertexLst[buff_count - 2];
                float prev_1 = vertexLst[buff_count - 1];

                vertexLst.Append(prev_5); vertexLst.Append(prev_4); vertexLst.Append(prev_3);
                vertexLst.Append(prev_2); vertexLst.Append(prev_1);


                indxLst.Append((ushort)(_indexCount));
                indxLst.Append((ushort)(_indexCount + 1));

                _indexCount += 2;
            }

            //---------

            Quad2f quad = new Quad2f();
            quad.SetCornersFromRect(srcRect);

            AffineMat mat = AffineMat.Iden();
            mat.Translate(-srcRect.Left, -(srcRect.Top + srcRect.Bottom) / 2); //*** in this case we move to left-most x amd mid-y of the srcRect
            mat.Rotate(srcRotation);
            mat.Translate(targetLeft, targetTop);

            quad.Transform(mat);

            RectangleF normalizedSrcRect = srcRect.CreateNormalizedRect(_orgBmpW, _orgBmpH);

            WriteToVboStream(vertexLst,
                 _indexCount > 0,
                 normalizedSrcRect,
                 quad,
                 _bmpYFlipped);

            indxLst.Append(_indexCount);
            indxLst.Append((ushort)(_indexCount + 1));
            indxLst.Append((ushort)(_indexCount + 2));
            indxLst.Append((ushort)(_indexCount + 3));

            _indexCount += 4;
        }

        public void AppendDegenerativeTriangle()
        {

            if (_indexCount > 0)
            {
                ArrayList<float> vertexLst = _vertexList;
                ArrayList<ushort> indxLst = _indexList;
                //add degenerative triangle
                int buff_count = vertexLst.Count;
                float prev_5 = vertexLst[buff_count - 5];
                float prev_4 = vertexLst[buff_count - 4];
                float prev_3 = vertexLst[buff_count - 3];
                float prev_2 = vertexLst[buff_count - 2];
                float prev_1 = vertexLst[buff_count - 1];

                vertexLst.Append(prev_5); vertexLst.Append(prev_4); vertexLst.Append(prev_3);
                vertexLst.Append(prev_2); vertexLst.Append(prev_1);
                indxLst.Append((ushort)(_indexCount));
                indxLst.Append((ushort)(_indexCount + 1));

                _indexCount += 2;
            }
        }

        static void WriteToVboStream(
            PixelFarm.CpuBlit.ArrayList<float> vboList, bool duplicateFirst,
            in RectangleF normalizedSrc,
            in RectangleF targetRect,
            bool bmpYFlipped
        )
        {

            if (bmpYFlipped)
            {
                vboList.Append(targetRect.Left); vboList.Append(targetRect.Top); vboList.Append(0); //coord 0 (left,top)                                                                                                       
                vboList.Append(normalizedSrc.Left); vboList.Append(normalizedSrc.Top); //texture coord 0 (left,top)

                if (duplicateFirst)
                {
                    //for creating degenerative triangle 
                    vboList.Append(targetRect.Left); vboList.Append(targetRect.Top); vboList.Append(0); //coord 0 (left,top)                                                                                                       
                    vboList.Append(normalizedSrc.Left); vboList.Append(normalizedSrc.Top); //texture coord 0 (left,top)
                }
                //---------------------
                vboList.Append(targetRect.Left); vboList.Append(targetRect.Top - targetRect.Height); vboList.Append(0); //coord 1 (left,bottom)
                vboList.Append(normalizedSrc.Left); vboList.Append(normalizedSrc.Bottom); //texture coord 1 (left,bottom)

                //---------------------
                vboList.Append(targetRect.Right); vboList.Append(targetRect.Top); vboList.Append(0); //coord 2 (right,top)
                vboList.Append(normalizedSrc.Right); vboList.Append(normalizedSrc.Top); //texture coord 2 (right,top)

                //---------------------
                vboList.Append(targetRect.Right); vboList.Append(targetRect.Top - targetRect.Height); vboList.Append(0);//coord 3 (right, bottom)
                vboList.Append(normalizedSrc.Right); vboList.Append(normalizedSrc.Bottom); //texture coord 3  (right,bottom) 

            }
            else
            {


                vboList.Append(targetRect.Left); vboList.Append(targetRect.Top); vboList.Append(0); //coord 0 (left,top)
                vboList.Append(normalizedSrc.Left); vboList.Append(normalizedSrc.Bottom); //texture coord 0  (left,bottom) 
                if (duplicateFirst)
                {
                    //for creating degenerative triangle
                    //https://developer.apple.com/library/content/documentation/3DDrawing/Conceptual/OpenGLES_ProgrammingGuide/TechniquesforWorkingwithVertexData/TechniquesforWorkingwithVertexData.html

                    vboList.Append(targetRect.Left); vboList.Append(targetRect.Top); vboList.Append(0); //coord 0 (left,top)
                    vboList.Append(normalizedSrc.Left); vboList.Append(normalizedSrc.Bottom); //texture coord 0  (left,bottom)
                }

                //---------------------
                vboList.Append(targetRect.Left); vboList.Append(targetRect.Top - targetRect.Height); vboList.Append(0); //coord 1 (left,bottom)
                vboList.Append(normalizedSrc.Left); vboList.Append(normalizedSrc.Top); //texture coord 1  (left,top)

                //---------------------
                vboList.Append(targetRect.Right); vboList.Append(targetRect.Top); vboList.Append(0); //coord 2 (right,top)
                vboList.Append(normalizedSrc.Right); vboList.Append(normalizedSrc.Bottom); //texture coord 2  (right,bottom)

                //---------------------
                vboList.Append(targetRect.Right); vboList.Append(targetRect.Top - targetRect.Height); vboList.Append(0); //coord 3 (right, bottom)
                vboList.Append(normalizedSrc.Right); vboList.Append(normalizedSrc.Top); //texture coord 3 (right,top) 
            }
        }


        static void WriteToVboStream(
           PixelFarm.CpuBlit.ArrayList<float> vboList,
           bool duplicateFirst,
           in RectangleF normalizedSrcRect,
           in Quad2f quad,
           bool bmpYFlipped
        )
        {

            if (bmpYFlipped)
            {
                vboList.Append(quad.left_top_x); vboList.Append(quad.left_top_y); vboList.Append(0); //coord 0 (left,top)                                                                                                       
                vboList.Append(normalizedSrcRect.Left); vboList.Append(normalizedSrcRect.Top); //texture coord 0 (left,top)

                if (duplicateFirst)
                {
                    //for creating degenerative triangle 
                    vboList.Append(quad.left_top_x); vboList.Append(quad.left_top_y); vboList.Append(0); //coord 0 (left,top)                                                                                                       
                    vboList.Append(normalizedSrcRect.Left); vboList.Append(normalizedSrcRect.Top); //texture coord 0 (left,top)
                }
                //---------------------
                vboList.Append(quad.left_bottom_x); vboList.Append(quad.left_bottom_y); vboList.Append(0); //coord 1 (left,bottom)
                vboList.Append(normalizedSrcRect.Left); vboList.Append(normalizedSrcRect.Bottom); //texture coord 1 (left,bottom)

                //---------------------
                vboList.Append(quad.right_top_x); vboList.Append(quad.right_top_y); vboList.Append(0); //coord 2 (right,top)
                vboList.Append(normalizedSrcRect.Right); vboList.Append(normalizedSrcRect.Top); //texture coord 2 (right,top)

                //---------------------
                vboList.Append(quad.right_bottom_x); vboList.Append(quad.right_bottom_y); vboList.Append(0);//coord 3 (right, bottom)
                vboList.Append(normalizedSrcRect.Right); vboList.Append(normalizedSrcRect.Bottom); //texture coord 3  (right,bottom) 

            }
            else
            {

                vboList.Append(quad.left_top_x); vboList.Append(quad.left_top_y); vboList.Append(0); //coord 0 (left,top)
                vboList.Append(normalizedSrcRect.Left); vboList.Append(normalizedSrcRect.Bottom); //texture coord 0  (left,bottom) 
                if (duplicateFirst)
                {
                    //for creating degenerative triangle
                    //https://developer.apple.com/library/content/documentation/3DDrawing/Conceptual/OpenGLES_ProgrammingGuide/TechniquesforWorkingwithVertexData/TechniquesforWorkingwithVertexData.html

                    vboList.Append(quad.left_top_x); vboList.Append(quad.left_top_y); vboList.Append(0); //coord 0 (left,top)
                    vboList.Append(normalizedSrcRect.Left); vboList.Append(normalizedSrcRect.Bottom); //texture coord 0  (left,bottom)
                }

                //---------------------
                vboList.Append(quad.left_bottom_x); vboList.Append(quad.left_bottom_y); vboList.Append(0); //coord 1 (left,bottom)
                vboList.Append(normalizedSrcRect.Left); vboList.Append(normalizedSrcRect.Top); //texture coord 1  (left,top)

                //---------------------
                vboList.Append(quad.right_top_x); vboList.Append(quad.right_top_y); vboList.Append(0); //coord 2 (right,top)
                vboList.Append(normalizedSrcRect.Right); vboList.Append(normalizedSrcRect.Bottom); //texture coord 2  (right,bottom)

                //---------------------
                vboList.Append(quad.right_bottom_x); vboList.Append(quad.right_bottom_y); vboList.Append(0); //coord 3 (right, bottom)
                vboList.Append(normalizedSrcRect.Right); vboList.Append(normalizedSrcRect.Top); //texture coord 3 (right,top) 
            }

        }



    }
}