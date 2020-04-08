//MIT, 2016-present, WinterDev

using System;
using OpenTK.Graphics.ES20;
namespace PixelFarm.DrawingGL
{

    abstract class ColorFillShaderBase : ShaderBase
    {
        protected ShaderUniformMatrix4 u_matrix;
        protected ShaderUniformVar2 u_ortho_offset;

        int _orthoviewVersion = -1;
        protected float _orthov_offsetX = 0;
        protected float _orthov_offsetY = 0;

        public ColorFillShaderBase(ShaderSharedResource shareRes)
            : base(shareRes)
        {

        }
        protected void CheckViewMatrix()
        {
            //int version = 0;
            //if (_orthoviewVersion != (version = _shareRes.OrthoViewVersion))
            //{
            //    _orthoviewVersion = version;
            //    u_matrix.SetData(_shareRes.OrthoView.data);
            //} 
            if (_shareRes.GetOrthoViewVersion(ref _orthoviewVersion))
            {
                u_matrix.SetData(_shareRes.OrthoView.data);
            }

            _shareRes.GetOrthoViewOffset(out float dx, out float dy);
            if (dx != _orthov_offsetX || dy != _orthov_offsetY)
            {
                //change
                u_ortho_offset.SetValue(_orthov_offsetX = dx, _orthov_offsetY = dy);
            }
        }
    }


}