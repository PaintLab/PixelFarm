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
            Test2();
        }
        void Test2()
        {
            MergeProject mergePro = CreateMergePixelFarmProject();
            mergePro.MergeAndSave(@"D:\projects\agg-sharp\a_mini\projects\PixelFarm\PixelFarm.One.csproj",
               "PixelFarm.One",
               "v2.0",
               new string[] {
                  "System",
                  "System.Drawing",
                  "System.Windows.Forms"
               });
        }
        private void cmdBuildMergePixelFarmPortable_Click(object sender, EventArgs e)
        {
            //config as portble library ***
            MergeProject mergePro = CreateMergePixelFarmPortableProject();
            mergePro.MergeAndSave(@"D:\projects\agg-sharp\a_mini\projects\PixelFarm\PixelFarm.One.Portable.csproj",
            "PixelFarm.One.Portable",
               "v4.5",
               new string[0]);
        }
        static MergeProject CreateMergePixelFarmProject()
        {
            MergeProject mergePro = new MergeProject();
            mergePro.LoadSubProject(@"D:\projects\agg-sharp\a_mini\projects\PixelFarm\PixelFarm.Drawing\PixelFarm.Drawing.csproj");
            //
            mergePro.LoadSubProject(@"D:\projects\agg-sharp\a_mini\projects\PixelFarm\MiniAgg\MiniAgg.csproj");
            mergePro.LoadSubProject(@"D:\projects\agg-sharp\a_mini\projects\PixelFarm\MiniAgg.Complements\MiniAgg.Complements.csproj");
            //
            mergePro.LoadSubProject(@"D:\projects\agg-sharp\a_mini\projects\PixelFarm\MiniAgg.HardwareGraphics\OpenTK.ES\OpenTK.ES.csproj");
            mergePro.LoadSubProject(@"D:\projects\agg-sharp\a_mini\projects\PixelFarm\MiniAgg.HardwareGraphics\OpenTK.PlatformMini\OpenTK.PlatformMini.csproj");
            mergePro.LoadSubProject(@"D:\projects\agg-sharp\a_mini\projects\PixelFarm\MiniTesselate\Tesselate.csproj");
            // 
            mergePro.LoadSubProject(@"D:\projects\agg-sharp\a_mini\projects\PixelFarm\MiniAgg.HardwareGraphics\MiniAgg.Hw2\MiniAgg.Hw2.csproj");
            //
            mergePro.LoadSubProject(@"D:\projects\agg-sharp\a_mini\projects\PixelFarm\PixelFarm.Drawing.Platforms\PixelFarm.Drawing.Platforms.csproj");
            //
            return mergePro;
        }
        static MergeProject CreateMergePixelFarmPortableProject()
        {
            //*** portable project for html renderer ***
            MergeProject mergePro = new MergeProject(true);
            mergePro.LoadSubProject(@"D:\projects\agg-sharp\a_mini\projects\PixelFarm\PixelFarm.Drawing\PixelFarm.Drawing.csproj");
            //
            mergePro.LoadSubProject(@"D:\projects\agg-sharp\a_mini\projects\PixelFarm\MiniAgg\MiniAgg.csproj");
            mergePro.LoadSubProject(@"D:\projects\agg-sharp\a_mini\projects\PixelFarm\MiniAgg.Complements\MiniAgg.Complements.csproj");
            //
            //mergePro.LoadSubProject(@"D:\projects\agg-sharp\a_mini\projects\PixelFarm\MiniAgg.HardwareGraphics\OpenTK.ES\OpenTK.ES.csproj");
            mergePro.LoadSubProject(@"D:\projects\agg-sharp\a_mini\projects\PixelFarm\MiniTesselate\Tesselate.csproj");
            // 
            //mergePro.LoadSubProject(@"D:\projects\agg-sharp\a_mini\projects\PixelFarm\MiniAgg.HardwareGraphics\MiniAgg.Hw2\MiniAgg.Hw2.csproj");

            return mergePro;
        }
    }
}
