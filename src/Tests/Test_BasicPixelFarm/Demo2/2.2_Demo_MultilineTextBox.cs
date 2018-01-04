//Apache2, 2014-2018, WinterDev
using LayoutFarm.CustomWidgets;
namespace LayoutFarm
{
    [DemoNote("2.2 MultiLineTextBox")]
    class Demo_MultiLineTextBox : DemoBase
    {
        protected override void OnStartDemo(SampleViewport viewport)
        {

            var textbox1 = new LayoutFarm.CustomWidgets.TextBox(400, 100, true);
            var style1 = new Text.TextSpanStyle();
            style1.FontInfo = new PixelFarm.Drawing.RequestFont("tahoma", 18);
            //test with various font style
            style1.FontColor = new PixelFarm.Drawing.Color(255, 0, 0);
            textbox1.DefaultSpanStyle = style1;
            viewport.AddContent(textbox1);
            //-------------------
            //this version we need to set a style font each textbox
            var textbox2 = new LayoutFarm.CustomWidgets.TextBox(400, 500, true);
            var style2 = new Text.TextSpanStyle();
            style2.FontInfo = new PixelFarm.Drawing.RequestFont("tahoma", 10);
            style2.FontColor = new PixelFarm.Drawing.Color(0, 0, 0);
            textbox2.SetLocation(20, 120);
            textbox2.DefaultSpanStyle = style2;

            viewport.AddContent(textbox2);
            var textSplitter = new ContentTextSplitter();
            textbox2.TextSplitter = textSplitter;
            textbox2.Text = "Hello World!";
        }
    }
}