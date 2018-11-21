//MIT, 2014-present,WinterDev
//creadit : http://learningwebgl.com/lessons/lesson16/index.html

using System;
using Mini;
using PixelFarm.DrawingGL;
namespace OpenTkEssTest
{

    public enum T107_2_DrawImageSet
    {
        //for test only!
        Plain,
        LcdEffect1,
    }

    public enum T107_2_GlyphImages
    {   //for test only!
        Blank,
        Img1,
        Img2,
        Img3,
        Img4,

    }
    [Info(OrderCode = "107")]
    [Info("T107_2_DrawGlyphs_from_GlyphTextureAtlas")]
    public class T107_2_DrawGlyphs : DemoBase
    {
        GLRenderSurface _glsx;
        GLPainter _painter;
        GLBitmap _glbmp;
        bool _isInit;

        T107_2_GlyphImages _selectedGlyphImage;
        bool _needImgUpdate;

        protected override void OnGLSurfaceReady(GLRenderSurface glsx, GLPainter painter)
        {
            this._glsx = glsx;
            this._painter = painter;
            SelectedGlyphImage = T107_2_GlyphImages.Img1;
        }
        [DemoConfig]
        public T107_2_DrawImageSet DrawSet
        {
            get;
            set;
        }
        [DemoConfig]
        public T107_2_GlyphImages SelectedGlyphImage
        {
            get => _selectedGlyphImage;
            set
            {
                if (value != _selectedGlyphImage)
                {
                    //img changed
                    _needImgUpdate = true;
                }
                _selectedGlyphImage = value;
            }
        }
        protected override void OnReadyForInitGLShaderProgram()
        {
        }
        protected override void DemoClosing()
        {
            _glsx.Dispose();
        }
        protected override void OnGLRender(object sender, EventArgs args)
        {
            _glsx.SmoothMode = SmoothMode.Smooth;
            _glsx.StrokeColor = PixelFarm.Drawing.Color.Blue;
            _glsx.Clear(PixelFarm.Drawing.Color.White); //set clear color and clear all buffer
            _glsx.ClearColorBuffer(); //test , clear only color buffer
            //-------------------------------
            if (!_isInit || _needImgUpdate)
            {
                //for test only!
                string imgName = null;
                switch (SelectedGlyphImage)
                {
                    default: return;//

                    case T107_2_GlyphImages.Img1:
                        imgName = RootDemoPath.Path + @"\tahoma -293093872.info.png";
                        break;
                    case T107_2_GlyphImages.Img2:
                        imgName = RootDemoPath.Path + @"\tahoma -358105584.info.png";
                        break;
                    case T107_2_GlyphImages.Img3:
                        imgName = RootDemoPath.Path + @"\tahoma -455623152.info.png";
                        break;
                    case T107_2_GlyphImages.Img4:
                        imgName = RootDemoPath.Path + @"\tahoma -455623152.info.png";
                        break;
                }
                //
                _glbmp = DemoHelper.LoadTexture(imgName);
                _isInit = true;
                _needImgUpdate = false;
            }

            GLRenderSurfaceOrigin prevOrgKind = _glsx.OriginKind; //save
            switch (DrawSet)
            {
                default:
                case T107_2_DrawImageSet.Plain:
                    {
                        _glsx.OriginKind = GLRenderSurfaceOrigin.LeftTop;
                        for (int i = 0; i < 400;)
                        {
                            _glsx.DrawImage(_glbmp, i, i); //left,top (NOT x,y)
                            i += 50;
                        }
                        //
                        _glsx.OriginKind = GLRenderSurfaceOrigin.LeftBottom;
                        for (int i = 0; i < 400;)
                        {
                            _glsx.DrawImage(_glbmp, i, i); //left,top (NOT x,y)
                            i += 50;
                        }
                    }
                    break;
                case T107_2_DrawImageSet.LcdEffect1:
                    {
                        _glsx.OriginKind = GLRenderSurfaceOrigin.LeftTop;
                        _glsx.FontFillColor = PixelFarm.Drawing.Color.Black;
                        for (int i = 0; i < 400;)
                        {
                            _glsx.DrawGlyphImageWithSubPixelRenderingTechnique(_glbmp, i, i); //left,top (NOT x,y)
                            i += 50;
                        }
                        //
                        _glsx.OriginKind = GLRenderSurfaceOrigin.LeftBottom;
                        for (int i = 0; i < 400;)
                        {
                            _glsx.DrawGlyphImageWithSubPixelRenderingTechnique(_glbmp, i, i); //left,top (NOT x,y)
                            i += 50;
                        }
                    }
                    break;
            }
            _glsx.OriginKind = prevOrgKind;//restore  

        }
    }

}

