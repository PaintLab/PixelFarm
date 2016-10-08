//2016, MIT WinterDev

using System;
using System.Windows.Forms;
namespace BuildMergeProject
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            MergeProject mergePro = CreateMergePixelFarmProject();
            mergePro.MergeAndSave(@"D:\projects\PixelFarm-dev\a_mini\projects\PixelFarm\PixelFarm.One.csproj",
               "PixelFarm.One",
               "v2.0",
               "",//additional define constant
               new string[] {
                  "System",
                  "System.Drawing",
                  "System.Windows.Forms",
                  "System.Xml",
               });
        }

        private void cmdBuildMergePixelFarmPortable_Click(object sender, EventArgs e)
        {
            //config as portble library ***
            MergeProject mergePro = CreateMergePixelFarmPortableProject();
            mergePro.MergeAndSave(@"D:\projects\PixelFarm-dev\a_mini\projects\PixelFarm\PixelFarm.One.Portable.csproj",
            "PixelFarm.One.Portable",
               "v4.5",
               "",//additional define constant
               new string[0]);
        }
        static MergeProject CreateMergePixelFarmProject()
        {
            MergeProject mergePro = new MergeProject();
            mergePro.LoadSubProject(@"D:\projects\PixelFarm-dev\a_mini\projects\PixelFarm\PixelFarm.Drawing\PixelFarm.Drawing.csproj");
            //
            mergePro.LoadSubProject(@"D:\projects\PixelFarm-dev\a_mini\projects\PixelFarm\MiniAgg\MiniAgg.csproj");
            mergePro.LoadSubProject(@"D:\projects\PixelFarm-dev\a_mini\projects\PixelFarm\MiniAgg.Complements\MiniAgg.Complements.csproj");
            //
           //mergePro.LoadSubProject(@"D:\projects\PixelFarm-dev\a_mini\projects\PixelFarm\PixelFarm.Drawing.Hw\PixelFarm.Drawing.Hw.csproj");
            mergePro.LoadSubProject(@"D:\projects\PixelFarm-dev\a_mini\projects\PixelFarm\PixelFarm.MiniPlatforms\PixelFarm.MiniPlatforms.csproj");
            mergePro.LoadSubProject(@"D:\projects\PixelFarm-dev\a_mini\projects\PixelFarm\MiniTesselate\Tesselate.csproj");
            // 
            mergePro.LoadSubProject(@"D:\projects\PixelFarm-dev\a_mini\projects\PixelFarm\PixelFarm.MiniPlatforms.WindowsForms\PixelFarm.MiniPlatforms.WindowsForms.csproj");
            //
            mergePro.LoadSubProject(@"D:\projects\PixelFarm-dev\a_mini\projects\PixelFarm\PixelFarm.Drawing.GdiPlusPlatform\PixelFarm.Drawing.GdiPlusPlatform.csproj");
            //
            return mergePro;
        }
        static MergeProject CreateMergePixelFarmPortableProject()
        {
            //*** portable project for html renderer ***
            MergeProject mergePro = new MergeProject(true);
            mergePro.LoadSubProject(@"D:\projects\PixelFarm-dev\a_mini\projects\PixelFarm\PixelFarm.Drawing\PixelFarm.Drawing.csproj");
            //
            mergePro.LoadSubProject(@"D:\projects\PixelFarm-dev\a_mini\projects\PixelFarm\MiniAgg\MiniAgg.csproj");
            mergePro.LoadSubProject(@"D:\projects\PixelFarm-dev\a_mini\projects\PixelFarm\MiniAgg.Complements\MiniAgg.Complements.csproj");
            //
            //mergePro.LoadSubProject(@"D:\projects\agg-sharp\a_mini\projects\PixelFarm\MiniAgg.HardwareGraphics\OpenTK.ES\OpenTK.ES.csproj");
            mergePro.LoadSubProject(@"D:\projects\PixelFarm-dev\a_mini\projects\PixelFarm\MiniTesselate\Tesselate.csproj");
            // 
            //mergePro.LoadSubProject(@"D:\projects\agg-sharp\a_mini\projects\PixelFarm\MiniAgg.HardwareGraphics\MiniAgg.Hw2\MiniAgg.Hw2.csproj");

            return mergePro;
        }
    }
}
