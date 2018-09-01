//Apache2, 2014-present, WinterDev

namespace LayoutFarm
{
    [DemoNote("2.1 SingleLineText")]
    class Demo_SingleLineText : App
    {
        protected override void OnStart(AppHost host)
        {
            //simple textbox
            var textbox = new LayoutFarm.CustomWidgets.TextBox(100, 30, false);
            var textSpanStyle = new Text.TextSpanStyle()
            {
                ReqFont = new PixelFarm.Drawing.RequestFont("tahoma", 18),
                FontColor = new PixelFarm.Drawing.Color(255, 0, 0)
            };
            //test with various font style             
            //set default style
            textbox.DefaultSpanStyle = textSpanStyle;
            //
            host.AddChild(textbox);


            //--------------
            //mask text box
            var maskTextBox = new LayoutFarm.CustomWidgets.MaskTextBox(100, 30);
            var textSpanStyle2 = new Text.TextSpanStyle()
            {
                ReqFont = new PixelFarm.Drawing.RequestFont("tahoma", 18),
                FontColor = PixelFarm.Drawing.Color.Black
            };

            maskTextBox.SetLocation(0, 40);
            host.AddChild(maskTextBox);




        }
    }
}