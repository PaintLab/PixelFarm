//MIT, 2014-2018, WinterDev

using PixelFarm.Drawing;
using PaintLab.Svg;
using LayoutFarm.UI;

namespace LayoutFarm
{
    [DemoNote("4.1 DemoSvgTiger")]
    class Demo_SvgTiger : DemoBase
    {
        protected override void OnStartDemo(SampleViewport viewport)
        {

            PaintLab.Svg.SvgParser parser = new SvgParser();

            //load lion svg
            string file = @"d:\\WImageTest\\lion.svg";
            string svgContent = System.IO.File.ReadAllText(file);
            WebLexer.TextSnapshot textSnapshot = new WebLexer.TextSnapshot(svgContent);
            parser.ParseDocument(textSnapshot);
            //
            var svgRenderVx = parser.GetResultAsRenderVx();

            var uiSprite = new UISprite(10, 10);
            uiSprite.LoadSvg(svgRenderVx);
            viewport.AddContent(uiSprite);

            //box2.SetLocation(50, 50);
           
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