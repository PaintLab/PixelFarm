//Apache2, 2014-present, WinterDev
using LayoutFarm.TextFlow;
namespace LayoutFarm
{
    [DemoNote("2.1 SingleLineText")]
    public class Demo_SingleLineText : App
    {
        protected override void OnStart(AppHost host)
        {
            //simple textbox
            var reqFont = new PixelFarm.Drawing.RequestFont("Sarabun", 18);
            var textbox = new LayoutFarm.CustomWidgets.TextBox(200, 30, false);
            var textSpanStyle = new TextSpanStyle()
            {
                ReqFont = reqFont,
                FontColor = new PixelFarm.Drawing.Color(255, 0, 0)
            };
            //test with various font style             
            //set default style
            textbox.DefaultSpanStyle = textSpanStyle;
            //
            host.AddChild(textbox);


            //--------------
            //mask text box
            var maskTextBox = new LayoutFarm.CustomWidgets.MaskTextBox(200, 30);
            var textSpanStyle2 = new TextSpanStyle()
            {
                ReqFont = reqFont,
                FontColor = PixelFarm.Drawing.Color.Black
            };

            maskTextBox.SetLocation(0, 40);
            host.AddChild(maskTextBox);
        }
    }
}