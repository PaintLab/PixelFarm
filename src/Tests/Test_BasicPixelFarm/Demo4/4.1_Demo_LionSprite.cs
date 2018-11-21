//Apache2, 2014-present, WinterDev

using PixelFarm.Drawing;
using PaintLab.Svg;
using LayoutFarm.UI;

namespace LayoutFarm
{
    [DemoNote("4.1 Demo_LionSprite")]
    class Demo_LionSprite : App
    {
        VgVisualElement _renderVx;

        MyTestSprite _mySprite;
        AppHost _host;
        protected override void OnStart(AppHost host)
        {
            _host = host;
            //_renderVx = SvgRenderVxLoader.CreateSvgRenderVxFromFile(@"Samples\1f0cf.svg");
            _renderVx = SvgRenderVxLoader.CreateSvgRenderVxFromFile(@"Samples\lion.svg");
            _mySprite = new MyTestSprite(_renderVx);
            var evListener = new GeneralEventListener();
            evListener.MouseDown += e =>
            {
                if (e.Button == UIMouseButtons.Right)
                {
                    VgVisualElement foundE = _mySprite.HitTest(e.X, e.Y, true);
                    if (foundE != null)
                    {
                        foundE.VisualSpec.FillColor = Color.Red;
                        _mySprite.InvalidateLayout();
                    }
                }
            };
            evListener.MouseMove += e =>
            {
                if (e.Button == UIMouseButtons.Left && e.IsDragging)
                {

                    if (e.Ctrl)
                    {
                        //TODO: 
                        //classic Agg's move and rotate                         

                    }
                    else
                    {   //just move
                        _mySprite.SetLocation(_mySprite.X + e.XDiff, _mySprite.Y + e.YDiff);
                    }
                }
            };

            _mySprite.AttachExternalEventListener(evListener);

            host.AddChild(_mySprite);



            ////
            //var textbox = new LayoutFarm.CustomWidgets.TextBox(100, 30, false);
            //var textSpanStyle = new Text.TextSpanStyle();

            ////test with various font style
            //textSpanStyle.FontInfo = new PixelFarm.Drawing.RequestFont("tahoma", 18);
            //textSpanStyle.FontColor = new PixelFarm.Drawing.Color(255, 0, 0);
            ////set default style
            //textbox.DefaultSpanStyle = textSpanStyle;

            //host.AddChild(textbox);


            //var box1 = new LayoutFarm.CustomWidgets.Box(50, 50);
            //box1.BackColor = Color.Red;
            //box1.SetLocation(10, 10);
            //host.AddChild(box1);
            ////--------------------------------
            //var box2 = new LayoutFarm.CustomWidgets.Box(30, 30);
            //box2.SetLocation(50, 50);
            //host.AddChild(box2);
            ////1. mouse down         
            //box1.MouseDown += (s, e) =>
            //{
            //    box1.BackColor = KnownColors.FromKnownColor(KnownColor.DeepSkyBlue);
            //    box2.Visible = false;
            //};
            //box1.MouseUp += (s, e) =>
            //{
            //    box1.BackColor = Color.Red;
            //    box2.Visible = true;
            //}; 
        }
    }
}