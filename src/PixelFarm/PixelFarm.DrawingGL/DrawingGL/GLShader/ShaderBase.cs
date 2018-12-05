//MIT, 2016-present, WinterDev

using OpenTK.Graphics.ES20;
namespace PixelFarm.DrawingGL
{
    abstract class ShaderBase
    {
        protected readonly ShaderSharedResource _shareRes;
        protected readonly MiniShaderProgram _shaderProgram = new MiniShaderProgram();
        public ShaderBase(ShaderSharedResource shareRes)
        {
            _shareRes = shareRes;
        }
        /// <summary>
        /// set as current shader
        /// </summary>
        internal void SetCurrent()
        {
            if (_shareRes._currentShader != this)
            {
                _shaderProgram.UseProgram();
                _shareRes._currentShader = this;
                this.OnSwitchToThisShader();
            }
        }
        protected virtual void OnSwitchToThisShader()
        {
        }
    }
}