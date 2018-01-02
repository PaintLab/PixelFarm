//MIT, 2016-2018, WinterDev

using OpenTK.Graphics.ES20;
namespace PixelFarm.DrawingGL
{
    abstract class ShaderBase
    {
        protected readonly ShaderSharedResource _shareRes;
        protected readonly MiniShaderProgram shaderProgram = new MiniShaderProgram();
        public ShaderBase(ShaderSharedResource shareRes)
        {
            _shareRes = shareRes;
        }
        /// <summary>
        /// set as current shader
        /// </summary>
        protected void SetCurrent()
        {
            if (_shareRes._currentShader != this)
            {
                shaderProgram.UseProgram();
                _shareRes._currentShader = this;
                this.OnSwithToThisShader();
            }
        }
        protected virtual void OnSwithToThisShader()
        {
        }
    }
}