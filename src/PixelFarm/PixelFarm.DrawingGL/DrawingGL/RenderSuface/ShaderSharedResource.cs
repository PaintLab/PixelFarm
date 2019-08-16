//MIT, 2014-present, WinterDev
using OpenTK.Graphics.ES20;
using System.Collections.Generic;
namespace PixelFarm.DrawingGL
{

    class TextureContainter
    {
        GLBitmap _currentGLBitmap;
        public GLBitmap GLBitmap => _currentGLBitmap;
        readonly TextureUnit _textureUnit;
        readonly int _textureUnitNo;
        readonly TextureContainerMx _owner;
        public TextureContainter(TextureContainerMx owner, TextureUnit textureUnit, int textureUnitNo)
        {
            _owner = owner;
            _textureUnit = textureUnit;
            _textureUnitNo = textureUnitNo;
        }
        public int TextureUnitNo => _textureUnitNo;
        public void LoadGLBitmap(GLBitmap glbmp)
        {
            if (_currentGLBitmap != null)
            {
                //already has some gl bitmap
                //same
                if (_currentGLBitmap == glbmp) return;

                //if not then
                _currentGLBitmap.TextureContainer = null;
            }
            _currentGLBitmap = glbmp;
            if (glbmp != null)
            {
                //then bind glBmp to the texture unit
                _currentGLBitmap.TextureContainer = this;
                MakeActive();
            }
        }
        public void UnloadGLBitmap()
        {
            if (_currentGLBitmap != null)
            {
                _currentGLBitmap.TextureContainer = null;
                _currentGLBitmap = null;
                _owner.ReleaseTextureContainer(this);
            }
#if DEBUG
            if (!_debugIsActive)
            {
                throw new System.NotSupportedException();
            }

            _debugIsActive = false;
#endif
        }
        public void MakeActive()
        {
            //if (_owner.CurrentActiveTextureContainer != this)
            //{
            _owner.CurrentActiveTextureContainer = this;
            GL.ActiveTexture(_textureUnit);
            GL.BindTexture(TextureTarget.Texture2D, _currentGLBitmap.GetServerTextureId());
            //}
            //else
            //{

            //}
        }
#if DEBUG
        internal bool _debugIsActive;
        public override string ToString()
        {
            return _textureUnit.ToString();
        }
#endif
    }

    class TextureContainerMx
    {
        readonly TextureContainter[] _textureContainers;
        readonly Queue<TextureContainter> _activeContainers = new Queue<TextureContainter>(32);
        readonly Queue<TextureContainter> _freeContainers = new Queue<TextureContainter>(32);

        public TextureContainerMx()
        {
            //GLES has 32 texture_units
            //texture unit 0,1 is not control by this
            //we let user to control them directly
            //
            _textureContainers = new TextureContainter[]
            {
                new TextureContainter(this,TextureUnit.Texture2,2),
                new TextureContainter(this,TextureUnit.Texture3,3),
                //new TextureContainter(this,TextureUnit.Texture4,4),
                //new TextureContainter(this,TextureUnit.Texture5,5),
                //new TextureContainter(this,TextureUnit.Texture6,6),
                //new TextureContainter(this,TextureUnit.Texture7,7),
                //new TextureContainter(this,TextureUnit.Texture8,8),
                //new TextureContainter(this,TextureUnit.Texture9,9),
                //new TextureContainter(this,TextureUnit.Texture10,10),
                //new TextureContainter(this,TextureUnit.Texture11,11),
                //new TextureContainter(this,TextureUnit.Texture12,12),
                //new TextureContainter(this,TextureUnit.Texture13,13),
                //new TextureContainter(this,TextureUnit.Texture14,14),
                //new TextureContainter(this,TextureUnit.Texture15,15),
                //new TextureContainter(this,TextureUnit.Texture16,16),
                ////
                //new TextureContainter(this,TextureUnit.Texture17,17),
                //new TextureContainter(this,TextureUnit.Texture18,18),
                //new TextureContainter(this,TextureUnit.Texture19,19),
                //new TextureContainter(this,TextureUnit.Texture20,20),
                //new TextureContainter(this,TextureUnit.Texture21,21),
                //new TextureContainter(this,TextureUnit.Texture22,22),
                //new TextureContainter(this,TextureUnit.Texture23,23),
                //new TextureContainter(this,TextureUnit.Texture24,24),
                //new TextureContainter(this,TextureUnit.Texture25,25),
                //new TextureContainter(this,TextureUnit.Texture26,26),
                //new TextureContainter(this,TextureUnit.Texture27,27),
                //new TextureContainter(this,TextureUnit.Texture28,28),
                //new TextureContainter(this,TextureUnit.Texture29,29),
                //new TextureContainter(this,TextureUnit.Texture30,30),
                //new TextureContainter(this,TextureUnit.Texture31,31),
            };

            for (int i = 0; i < _textureContainers.Length; ++i)
            {
                _freeContainers.Enqueue(_textureContainers[i]);
            }
        }

        internal TextureContainter CurrentActiveTextureContainer
        {
            get;
            set;
        }
        internal void ReleaseTextureContainer(TextureContainter textureContainer)
        {
#if DEBUG
            if (textureContainer.GLBitmap != null)
            {   //clear the glbmp first
                throw new System.NotSupportedException();
            }
#endif
            _freeContainers.Enqueue(textureContainer);
        }

        GLBitmap _latestLoadGLBmp;
        /// <summary>
        /// load glbitmap to free texture unit
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        public TextureContainter LoadGLBitmap(GLBitmap bmp)
        {
            

            if (bmp.TextureContainer != null)
            {
                //bmp.TextureContainer.UnloadGLBitmap();

                if (_latestLoadGLBmp != bmp)
                {
                    bmp.TextureContainer.MakeActive();
                    return bmp.TextureContainer;
                }
                else
                {
                    _latestLoadGLBmp = bmp;
                    return bmp.TextureContainer;
                }                
            }
            //----------------
            if (_freeContainers.Count > 0)
            {
                //has some free texture unit   
                TextureContainter container = _freeContainers.Dequeue();
                container.LoadGLBitmap(bmp);
                _activeContainers.Enqueue(container);//move to active container queue
#if DEBUG
                container._debugIsActive = true;
#endif
                return container;
            }
            else
            {
                //TODO: handle out-of-quota  texture unit here
                //in this version we unload from oldest active container
                TextureContainter container = _activeContainers.Dequeue();
                container.UnloadGLBitmap();
#if DEBUG
                if (_freeContainers.Count < 2)
                {

                }
#endif
                //when container is unload, it will enqueue to the free pool
                //so we get the new one from the pool,DO NOT reuse current container
                container = _freeContainers.Dequeue();
                container.LoadGLBitmap(bmp);
                _activeContainers.Enqueue(container);
#if DEBUG
                container._debugIsActive = true;
#endif
                return container;
            }
        }

    }

    /// <summary>
    /// sharing data between GLRenderSurface and shaders
    /// </summary>
    sealed class ShaderSharedResource
    {
        /// <summary>
        /// stroke width here is the sum of both side of the line.
        /// </summary>
        internal float _strokeWidth = 1;
        Drawing.Color _strokeColor;
        MyMat4 _orthoView;
        internal ShaderBase _currentShader;
        int _orthoViewVersion = 0;

        float _orthoViewOffsetX;
        float _orthoViewOffsetY;
        bool _isFlipAndPulldownHint;

        TextureContainerMx _textureContainerMx = new TextureContainerMx();
        public ShaderSharedResource()
        {

        }
        public TextureContainter LoadGLBitmap(GLBitmap glBmp)
        {
            return _textureContainerMx.LoadGLBitmap(glBmp);
        }
        public MyMat4 OrthoView
        {
            get => _orthoView;
            set
            {
                _orthoView = value;
                unchecked { _orthoViewVersion++; }
                _isFlipAndPulldownHint = false;
            }
        }
        public bool IsFilpAndPulldownHint
        {
            get => _isFlipAndPulldownHint;
            set => _isFlipAndPulldownHint = value;
        }

        public float OrthoViewOffsetX => _orthoViewOffsetX;
        public float OrthoViewOffsetY => _orthoViewOffsetY;
        public void SetOrthoViewOffset(float dx, float dy)
        {
            _orthoViewOffsetX = dx;
            _orthoViewOffsetY = dy;
        }
        //
        public int OrthoViewVersion => _orthoViewVersion;
        public bool GetOrthoViewVersion(ref int existingVersion)
        {
            bool result = existingVersion != _orthoViewVersion;
            existingVersion = _orthoViewVersion;
            return result;
        }
        public void GetOrthoViewOffset(out float offsetX, out float offsetY)
        {
            offsetX = _orthoViewOffsetX;
            offsetY = _orthoViewOffsetY;
        }


        //-------- 
        internal Drawing.Color StrokeColor
        {
            get => _strokeColor;
            set
            {
                _strokeColor = value;
                _stroke_r = value.R / 255f;
                _stroke_g = value.G / 255f;
                _stroke_b = value.B / 255f;
                _stroke_a = value.A / 255f;
            }
        }

        float _stroke_r;
        float _stroke_g;
        float _stroke_b;
        float _stroke_a;
        internal void AssignStrokeColorToVar(OpenTK.Graphics.ES20.ShaderUniformVar4 color)
        {
            color.SetValue(_stroke_r, _stroke_g, _stroke_b, _stroke_a);
        }
    }
}