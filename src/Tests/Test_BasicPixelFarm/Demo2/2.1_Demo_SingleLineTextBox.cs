//Apache2, 2014-present, WinterDev

namespace LayoutFarm
{
    [DemoNote("2.1 SingleLineText")]
    class Demo_SingleLineText : DemoBase
    {
        protected override void OnStartDemo(SampleViewport viewport)
        {
            var textbox = new LayoutFarm.CustomWidgets.TextBox(100, 30, false);
            var textSpanStyle = new Text.TextSpanStyle();

            //test with various font style
            textSpanStyle.FontInfo = new PixelFarm.Drawing.RequestFont("tahoma", 18);
            textSpanStyle.FontColor = new PixelFarm.Drawing.Color(255, 0, 0);
            //set default style
            textbox.DefaultSpanStyle = textSpanStyle;

            viewport.AddChild(textbox);
            textbox.InvalidateGraphics();
        }
    }
}