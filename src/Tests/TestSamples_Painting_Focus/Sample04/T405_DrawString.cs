//MIT, 2014-present,WinterDev

using System;
using Mini;
using PixelFarm.Drawing;

namespace OpenTkEssTest
{
    [Info(OrderCode = "405")]
    [Info("T405_DrawString")]
    public class T405_DrawString : DemoBase
    {
        //HarfBuzzShapingService hbShapingService;
        PixelFarm.Drawing.RequestFont _font1;
        PixelFarm.Drawing.RequestFont _font2;
        float _font1_h;
        float _font2_h;
        protected override void OnReadyForInitGLShaderProgram()
        {

        }
        protected override void OnPainterReady(Painter painter)
        {
            _font1 = new PixelFarm.Drawing.RequestFont("tahoma", 15);
            _font2 = new PixelFarm.Drawing.RequestFont("tahoma", 11);

            _font1_h = Typography.Text.GlobalTextService.TxtClient.MeasureBlankLineHeight(_font1);
            _font2_h = Typography.Text.GlobalTextService.TxtClient.MeasureBlankLineHeight(_font2);

            painter.UseLcdEffectSubPixelRendering = true;
            painter.CurrentFont = _font1;
        }
        public override void Draw(Painter p)
        {
            //canvas2d.SmoothMode = CanvasSmoothMode.Smooth;
            //canvas2d.StrokeColor = PixelFarm.Drawing.Color.Blue;
            //canvas2d.ClearColorBuffer();
            //if (!resInit)
            //{
            //    resInit = true;
            //}
            //painter.Clear(PixelFarm.Drawing.Color.White);
            //painter.DrawString("hello world!", 100, 100);            
            //painter.DrawString("กิ่น", 100, 100);
            // string test_str = "อูญูอุบ่ป่กินกิ่นก็โก้";
            //string test_str = "ปู่";
            //string test_str = "ก็";
            //string test_str = "example";
            string test_str = "jhPfmnpqOo";
            //string test_str = "A";
            //string test_str = "012345";
            //string test_str = "กิน";
            p.Clear(PixelFarm.Drawing.Color.White);
            // p.Clear(PixelFarm.Drawing.Color.Blue); 
            p.FillColor = PixelFarm.Drawing.Color.Red;
            p.UseLcdEffectSubPixelRendering = true;

            int n = 3;
            float xpos2 = 0;
            float y_pos = 0;

            for (int i = 0; i < n; i++)
            {
                xpos2 += 1f;
                //  p.DrawString(test_str, i * 10, i * 10);
                float x_pos = xpos2;


                p.FillColor = Color.Yellow;
                p.FillRect(x_pos, y_pos, 200, _font1_h); //draw 5x5px marker

                p.FillColor = Color.Red;
                p.FillRect(x_pos, y_pos, 5, 5); //draw 5x5px marker

                y_pos += _font1_h;
            }

            p.FillColor = PixelFarm.Drawing.Color.Black;

            xpos2 = 0;
            y_pos = 0;
            for (int i = 0; i < n; i++)
            {
                xpos2 += 1f;
                float x_pos = xpos2;//i + .1f;

                //p.DrawString("(" + x_pos + "," + y_pos + ")", x_pos, y_pos);
                //if ((i % 2) == 0)
                //{
                p.CurrentFont = _font1;
                p.DrawString(test_str, x_pos, y_pos);
                y_pos += _font1_h;

                //}
                //else
                //{
                //    p.CurrentFont = _font2;
                //    p.DrawString(test_str, x_pos, y_pos);
                //    y_pos += _font2_h;
                //}

            }

        }
    }


    [Info(OrderCode = "405")]
    [Info("T405_1_DrawStringRenderVx")]
    public class T405_1_DrawStringRenderVx : DemoBase
    {
        //HarfBuzzShapingService hbShapingService;
        PixelFarm.Drawing.RequestFont _font1;
        PixelFarm.Drawing.RequestFont _font2;
        protected override void OnReadyForInitGLShaderProgram()
        {

        }
        protected override void OnPainterReady(Painter painter)
        {
            _font1 = new PixelFarm.Drawing.RequestFont("tahoma", 11);
            _font2 = new PixelFarm.Drawing.RequestFont("tahoma", 16);
            painter.UseLcdEffectSubPixelRendering = true;
            painter.CurrentFont = _font1;
            //-------------- 
        }
        RenderVxFormattedString _strRenderVx_1;
        RenderVxFormattedString _strRenderVx_2;
        public override void Draw(Painter p)
        {

            //painter.DrawString("hello world!", 100, 100);            
            //painter.DrawString("กิ่น", 100, 100);
            // string test_str = "อูญูอุบ่ป่กินกิ่นก็โก้";
            //string test_str = "ปู่";
            //string test_str = "ก็";
            string test_str = "abcd";
            //string test_str = "A";
            //string test_str = "012345";
            //string test_str = "กิน";
            p.Clear(PixelFarm.Drawing.Color.White);
            p.FillColor = PixelFarm.Drawing.Color.Red;

            p.UseLcdEffectSubPixelRendering = true;
            int n = 50;
            for (int i = 0; i < n; i++)
            {
                //  p.DrawString(test_str, i * 10, i * 10);
                float x_pos = i * 20;
                float y_pos = i * 20;
                p.FillRect(x_pos, y_pos, x_pos + 5, y_pos + 5);
            }
            p.FillColor = PixelFarm.Drawing.Color.Black;

            if (_strRenderVx_1 == null)
            {
                p.CurrentFont = _font1;
                _strRenderVx_1 = p.CreateRenderVx(test_str);
            }
            if (_strRenderVx_2 == null)
            {
                p.CurrentFont = _font2;
                _strRenderVx_2 = p.CreateRenderVx(test_str);
            }
            //
            for (int i = 0; i < n; i++)
            {
                float x_pos = i * 20;
                float y_pos = i * 20;
                //p.DrawString("(" + x_pos + "," + y_pos + ")", x_pos, y_pos);
                if ((i % 2) == 0)
                {
                    //since draw string may be slow
                    //we can convert it to a 'freezed' visual object (RenderVx) 
                    p.CurrentFont = _font1;
                    p.DrawString(_strRenderVx_1, x_pos, y_pos);
                }
                else
                {
                    //since draw string may be slow
                    //we can convert it to a 'freezed' visual object (RenderVx) 
                    p.CurrentFont = _font2;
                    p.DrawString(_strRenderVx_2, x_pos, y_pos);
                }
            }
        }
    }
}

