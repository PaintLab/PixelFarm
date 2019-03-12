//MIT, 2014-present, WinterDev

namespace PixelFarm.DrawingGL
{


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
        //
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