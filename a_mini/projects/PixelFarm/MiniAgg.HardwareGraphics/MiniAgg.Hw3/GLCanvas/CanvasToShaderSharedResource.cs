//MIT 2014-2016, WinterDev

namespace PixelFarm.DrawingGL
{
    /// <summary>
    /// sharing data between canvas and shaders
    /// </summary>
    class CanvasToShaderSharedResource
    {
        internal float _strokeWidth = 1;
        internal Drawing.Color _strokeColor;
        internal float _fillColor;
        OpenTK.Graphics.ES20.MyMat4 _orthoView;
        internal ShaderBase _currentShader;
        int _orthoViewVersion = 0;
        internal OpenTK.Graphics.ES20.MyMat4 OrthoView
        {
            get { return _orthoView; }
            set
            {
                _orthoView = value;
                _orthoViewVersion++;
            }
        }
        public int OrthoViewVersion
        {
            get { return this._orthoViewVersion; }
        }
    }
}