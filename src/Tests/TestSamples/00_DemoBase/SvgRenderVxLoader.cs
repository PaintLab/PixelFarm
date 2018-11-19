//BSD, 2014-present, WinterDev
//MattersHackers
//AGG 2.4


 
using PaintLab.Svg;
namespace PixelFarm.CpuBlit
{
    public class SvgRenderVxLoader
    {
        public static VgRenderVx CreateSvgRenderVxFromFile(string filename)
        {
            SvgDocBuilder docBuilder = new SvgDocBuilder();
            SvgParser svg = new SvgParser(docBuilder);
            VgRenderVxDocBuilder builder = new VgRenderVxDocBuilder();

            //svg.ReadSvgFile("d:\\WImageTest\\lion.svg");
            //svg.ReadSvgFile("d:\\WImageTest\\tiger001.svg");
            svg.ReadSvgFile(filename);
            return builder.CreateRenderVx(docBuilder.ResultDocument);
        }

    }

}