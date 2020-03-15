//MIT, 2016-present, WinterDev
//Apache2, https://xmlgraphics.apache.org/

using System;
using System.Collections.Generic;
using PixelFarm.Drawing;
using PixelFarm.CpuBlit;
using PixelFarm.CpuBlit.VertexProcessing;

namespace PixelFarm.DrawingGL
{
    public sealed partial class GLPainter : Painter, IDisposable
    {
        GLPainterContext _pcx;
        SmoothingMode _smoothingMode; //smoothing mode of this  painter
        RenderSurfaceOrientation _orientation = RenderSurfaceOrientation.LeftTop;

        int _width;
        int _height;

        FigureBuilder _pathRenderVxBuilder;
        PathRenderVxBuilder2 _pathRenderVxBuilder2;

        RequestFont _requestFont;
        ITextPrinter _textPrinter;
        RenderQuality _renderQuality;

        TargetBuffer _targetBuffer;
        PixelFarm.Drawing.GLES2.MyGLDrawBoard _drawBoard;


        public GLPainter()
        {
            CurrentFont = PixelFarm.Drawing.GLES2.GLES2Platform.DefaultFont;
            UseVertexBufferObjectForRenderVx = true;
            //tools
            _pathRenderVxBuilder = new FigureBuilder();
            _defaultBrush = _currentBrush = new SolidBrush(Color.Black); //default brush 
            _pathRenderVxBuilder2 = new PathRenderVxBuilder2();

        }
        public void SetDrawboard(PixelFarm.Drawing.GLES2.MyGLDrawBoard drawBoard)
        {
            _drawBoard = drawBoard;
        }
        public void Dispose()
        {
            if (_wordPlateMx != null)
            {
                _wordPlateMx.ClearAllPlates();
                _wordPlateMx = null;
            }
        }

        public void BindToPainterContext(GLPainterContext pcx)
        {
            if (_pcx == pcx)
            {
                return;
            }
            //
            _pcx = pcx;
            _width = pcx.CanvasWidth;
            _height = pcx.CanvasHeight;
            _clipBox = new RectInt(0, 0, _width, _height);
        }
        public void UpdatePainterContext()
        {
            _width = _pcx.CanvasWidth;
            _height = _pcx.CanvasHeight;
            _clipBox = new RectInt(0, 0, _width, _height);
        }
        public override ICoordTransformer CoordTransformer
        {
            get => _pcx.CoordTransformer;
            set => _pcx.CoordTransformer = value;
        }
        public override int Width => _width;
        public override int Height => _height;
        public override float OriginX => _pcx.OriginX;
        public override float OriginY => _pcx.OriginY;

        public override void Render(RenderVx renderVx)
        {
            throw new NotImplementedException();
        }

        public void DetachCurrentShader() => _pcx.DetachCurrentShader();

        public override RenderSurfaceOrientation Orientation
        {
            get => _orientation;
            set => _orientation = value;
        }

        public bool UseVertexBufferObjectForRenderVx { get; set; }

        public override void SetOrigin(float ox, float oy)
        {
            _pcx.SetCanvasOrigin((int)ox, (int)oy);
        }
        //
        public GLPainterContext PainterContext => _pcx;
        //
        public override RenderQuality RenderQuality
        {
            get => _renderQuality;
            set => _renderQuality = value;
        }


        public override SmoothingMode SmoothingMode
        {
            get => _smoothingMode;
            set
            {
                switch (_smoothingMode = value)
                {
                    case SmoothingMode.HighQuality:
                    case SmoothingMode.AntiAlias:
                        _pcx.SmoothMode = SmoothMode.Smooth;
                        break;
                    default:
                        _pcx.SmoothMode = SmoothMode.No;
                        break;
                }
            }
        }
        public override bool UseSubPixelLcdEffect
        {
            get => _pcx.SmoothMode == SmoothMode.Smooth;
            set => _pcx.SmoothMode = value ? SmoothMode.Smooth : SmoothMode.No;
        }

        public override void Clear(Color color)
        {
            _pcx.Clear(color);
        }

        public override TargetBuffer TargetBuffer
        {
            get => _targetBuffer;
            set
            {
                if (_targetBuffer == value) return;
                //change target buffer
                _targetBuffer = value;

            }
        }
        //-----------------------------------------------------------------------------------------------------------------
        public override RenderVx CreateRenderVx(VertexStore vxs)
        {
            //store internal gfx path inside render vx  
            return PathRenderVx.Create(_pathRenderVxBuilder.Build(vxs));
        }
        public RenderVx CreatePolygonRenderVx(float[] xycoords)
        {
            //store internal gfx path inside render vx
            return new PathRenderVx(new Figure(xycoords));
        }


        class PathRenderVxBuilder2
        {
            Msdfgen.MsdfGen3 _msdfGen;
            public PathRenderVxBuilder2()
            {
                _msdfGen = new Msdfgen.MsdfGen3();
                _msdfGen.MsdfGenParams = new Msdfgen.MsdfGenParams();
            }
            public TextureRenderVx CreateRenderVx(VertexStore vxs)
            {
#if DEBUG
                //_msdfGen.dbugWriteMsdfTexture = true;
#endif
                CpuBlit.BitmapAtlas.BitmapAtlasItemSource item = _msdfGen.GenerateMsdfTexture(vxs);
                var spriteTextureMap = new CpuBlit.BitmapAtlas.AtlasItemSource<MemBitmap>(item.Left, item.Top, item.Width, item.Height);
                spriteTextureMap.TextureXOffset = item.TextureXOffset;
                spriteTextureMap.TextureYOffset = item.TextureYOffset;
                spriteTextureMap.Source = MemBitmap.CreateFromCopy(item.Width, item.Height, item.Source);
                return new TextureRenderVx(spriteTextureMap);
            }
        }
    }
}