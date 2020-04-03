//MIT, 2014-present,WinterDev

using System;
using PixelFarm.Drawing;
using PixelFarm.DrawingGL;
using PixelFarm.CpuBlit;
using PixelFarm.CpuBlit.VertexProcessing;

using Mini;
namespace OpenTkEssTest
{
    [Info(OrderCode = "106")]
    [Info("T106_SampleBrushes", AvailableOn = AvailableOn.GLES)]
    public class T106_SampleBrushes : DemoBase
    {
        GLPainterContext _pcx;
        GLPainter _painter;
        RenderVx _polygon1;
        RenderVx _polygon2;
        RenderVx _polygon3;


        GLBitmap _glBmp;
        GLBitmap _glBmp2;
        TextureBrush _textureBrush;
        TextureBrush _textureBrush2;
        LinearGradientBrush _linearGradient;

        protected override void OnGLPainterReady(GLPainter painter)
        {
            _pcx = painter.PainterContext;
            _painter = painter;
            _glBmp = DemoHelper.LoadTexture(RootDemoPath.Path + @"\logo-dark.jpg");            
            _glBmp2 = DemoHelper.LoadTexture(RootDemoPath.Path + @"\lion1_v2.png");
            
            _textureBrush = new TextureBrush(_glBmp);
            _textureBrush2 = new TextureBrush(_glBmp2);

            _linearGradient = new LinearGradientBrush(
              new PointF(10, 50), new PointF(10, 100),
              Color.Red, Color.White);
        }
        protected override void OnReadyForInitGLShaderProgram()
        {

            using (Tools.BorrowVxs(out var v1, out var v2, out var v3))
            using (Tools.BorrowPathWriter(v1, out PathWriter p))
            {
                p.MoveTo(0, 50);
                p.LineTo(50, 50);
                p.LineTo(10, 100);
                p.CloseFigure();

                _polygon1 = _painter.CreateRenderVx(v1.CreateTrim());
                AffineMat tx = AffineMat.Iden;
                tx.Translate(200, 0);

                tx.TransformToVxs(v1, v2); //v1=>v2
                _polygon2 = _painter.CreateRenderVx(v2.CreateTrim());

                tx.TransformToVxs(v2, v3); //v2=>v3
                _polygon3 = _painter.CreateRenderVx(v3.CreateTrim());
            }

        }
        protected override void DemoClosing()
        {
            _pcx.Dispose();
        }
        protected override void OnGLRender(object sender, EventArgs args)
        {
            _pcx.SmoothMode = SmoothMode.Smooth;
            _pcx.StrokeColor = PixelFarm.Drawing.Color.Blue;
            _pcx.ClearColorBuffer();
            _painter.FillColor = PixelFarm.Drawing.Color.Black;
            _painter.FillRect(0, 0, 150, 150);

            //-------------------------------------------------------------------------  
            _painter.FillRenderVx(_textureBrush, _polygon1);
            //_painter.FillRenderVx(_linearGradient, _polygon1);
            ////------------------------------------------------------------------------- 


            //fill polygon with gradient brush  
            _painter.FillColor = Color.Yellow;
            _painter.FillRect(200, 0, 150, 150);
            _painter.FillRenderVx(_textureBrush2, _polygon2);
            //_painter.FillRenderVx(_linearGradient, _polygon2);


            ////-------------------------------------------------------------------------  
            //_painter.FillColor = Color.Black;
            //_painter.FillRect(400, 0, 150, 150);
            ////another  ...                 
            //_painter.FillRenderVx(linearGrBrush2, _polygon3);
            ////------------------------------------------------------------------------- 


            SwapBuffers();
        }
    }
}

