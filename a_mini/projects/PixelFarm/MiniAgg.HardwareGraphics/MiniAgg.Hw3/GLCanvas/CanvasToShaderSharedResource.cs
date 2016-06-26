//MIT 2014-2016, WinterDev

namespace PixelFarm.DrawingGL
{
    enum ShaderName
    {
        Unknown,
        BasicFill,
        RectGradientShader,
        LineSmoothShader,
        InvertAlphaLineSmoothShader,
        SimpleTextureShader
    }
    /// <summary>
    /// sharing data between canvas and shaders
    /// </summary>
    class CanvasToShaderSharedResource
    {
        internal ShaderName ShaderName;
        internal float _strokeWidth = 1;
        internal Drawing.Color _strokeColor;
        internal float _fillColor;
        internal OpenTK.Graphics.ES20.MyMat4 _orthoView;
        internal ShaderBase _currentShader;
    }
}