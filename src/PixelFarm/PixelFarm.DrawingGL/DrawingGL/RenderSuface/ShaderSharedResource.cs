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

        internal LinkedListNode<TextureContainter> ActiveQueueLinkedNode { get; set; }
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
        
        readonly LinkedList<TextureContainter> _activeContainers = new LinkedList<TextureContainter>();
        readonly Queue<TextureContainter> _freeContainers = new Queue<TextureContainter>(32);

        public TextureContainerMx(int platformTextureUnitCount)
        {
            //GLES has 32 texture_units
            //texture unit 0,1 is not control by this
            //we let user to control them directly

            TextureUnit[] textureUnitNames = new TextureUnit[] {
                TextureUnit.Texture0,TextureUnit.Texture1,TextureUnit.Texture2, TextureUnit.Texture3,
                TextureUnit.Texture4,TextureUnit.Texture5,TextureUnit.Texture6, TextureUnit.Texture7,
                TextureUnit.Texture8,TextureUnit.Texture9,TextureUnit.Texture10,TextureUnit.Texture11,
                TextureUnit.Texture12,TextureUnit.Texture13,TextureUnit.Texture14, TextureUnit.Texture15,
                TextureUnit.Texture16,TextureUnit.Texture17,TextureUnit.Texture18, TextureUnit.Texture19,
                TextureUnit.Texture20,TextureUnit.Texture21,TextureUnit.Texture22,TextureUnit.Texture23,
                TextureUnit.Texture24,TextureUnit.Texture25,TextureUnit.Texture26,TextureUnit.Texture27,
                TextureUnit.Texture28,TextureUnit.Texture29,TextureUnit.Texture30,TextureUnit.Texture31,
            };

            //so we only suport 32-2            
            if (platformTextureUnitCount > 32) { platformTextureUnitCount = 32; }

            //old code we test when platformTextureUnitCount=4

            for (int i = 2; i < platformTextureUnitCount; ++i)
            {
                _freeContainers.Enqueue(new TextureContainter(this, textureUnitNames[i], i));
            }
        }

        internal TextureContainter CurrentActiveTextureContainer { get; set; }
        internal void ReleaseTextureContainer(TextureContainter textureContainer)
        {
#if DEBUG
            if (textureContainer.GLBitmap != null)
            {   //clear the glbmp first
                throw new System.NotSupportedException();
            }
            if (_freeContainers.Contains(textureContainer))
            {

            }
#endif
            //must remove from active
            if (textureContainer.ActiveQueueLinkedNode != null)
            {
                //remove specific node
                _activeContainers.Remove(textureContainer.ActiveQueueLinkedNode);
                textureContainer.ActiveQueueLinkedNode = null;//***
            }
            else
            {
#if DEBUG
                System.Diagnostics.Debugger.Break();//???
#endif
            }

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

#if DEBUG
                if (_activeContainers.Contains(container))
                {
                    //remove this from 
                    System.Diagnostics.Debugger.Break();
                }
#endif

                container.ActiveQueueLinkedNode = _activeContainers.AddLast(container);
#if DEBUG
                container._debugIsActive = true;
#endif
                return container;
            }
            else
            {
                //TODO: handle out-of-quota  texture unit here
                //in this version we unload from oldest active container

                TextureContainter container = _activeContainers.First.Value;
                container.UnloadGLBitmap();

#if DEBUG
                if (container.ActiveQueueLinkedNode != null)
                {
                    System.Diagnostics.Debugger.Break();
                }
                if (_freeContainers.Count < 2)
                {

                }
#endif
                //when container is unload, it will enqueue to the free pool
                //so we get the new one from the pool,DO NOT reuse current container
                container = _freeContainers.Dequeue();
                container.LoadGLBitmap(bmp);

                //enqueue
                container.ActiveQueueLinkedNode = _activeContainers.AddLast(container);

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

        TextureContainerMx _textureContainerMx;
        public ShaderSharedResource()
        {
            GL.GetInteger(GetPName.MaxCombinedTextureImageUnits, out int textunit_num);
            _textureContainerMx = new TextureContainerMx(textunit_num);
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
        public bool IsFlipAndPulldownHint
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