//Apache2, 2014-2018, WinterDev

using PixelFarm.Drawing;
using PaintLab.Svg;


namespace LayoutFarm
{
    [DemoNote("4 Demo_SvgDoc")]
    class Demo_SvgDoc : DemoBase
    {
        protected override void OnStartDemo(SampleViewport viewport)
        {
            SvgParser svg = new SvgParser();
            svg.ReadSvgDocument("d:\\WImageTest\\tiger.svg");
            

        }
    }
}