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
            MergeProject mergePro = CreateMergePixelFarmOneProject();
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
        private void cmdMergePixelFarmMiniAgg_Click(object sender, EventArgs e)
        {
            //-----------
            //PixelFarm's MiniAgg
            //-----------
            MergeProject mergePro = CreateMergePixelFarmMiniAggProject();
            mergePro.MergeAndSave(@"D:\projects\PixelFarm-dev\a_mini\projects\PixelFarm\PixelFarm.MiniAgg.One.csproj",
               "PixelFarm.MiniAgg.One",
               "v2.0",
               "",//additional define constant
               new string[] {
                  "System" ,
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
        static MergeProject CreateMergePixelFarmOneProject()
        {
            MergeProject mergePro = new MergeProject();
            mergePro.LoadSubProject(@"D:\projects\PixelFarm-dev\a_mini\projects\PixelFarm\PixelFarm.Drawing.Core\PixelFarm.Drawing.Core.csproj");
            //
            mergePro.LoadSubProject(@"D:\projects\PixelFarm-dev\a_mini\projects\PixelFarm\MiniAgg\MiniAgg.csproj");
            mergePro.LoadSubProject(@"D:\projects\PixelFarm-dev\a_mini\projects\PixelFarm\MiniAgg.Complements\MiniAgg.Complements.csproj");
            mergePro.LoadSubProject(@"D:\projects\PixelFarm-dev\a_mini\projects\PixelFarm\NOpenType\N20\PixelFarm.OpenType\PixelFarm.OpenType.csproj");
            mergePro.LoadSubProject(@"D:\projects\PixelFarm-dev\a_mini\projects\PixelFarm\Tesselate\Tesselate.csproj");
            //
            mergePro.LoadSubProject(@"D:\projects\PixelFarm-dev\a_mini\projects\PixelFarm\PixelFarm.Drawing.GdiPlusPlatform\PixelFarm.Drawing.GdiPlusPlatform.csproj");
            //
            mergePro.LoadSubProject(@"D:\projects\PixelFarm-dev\a_mini\projects\PixelFarm\PixelFarm.NativeWindows\PixelFarm.NativeWindows.csproj");
            mergePro.LoadSubProject(@"D:\projects\PixelFarm-dev\a_mini\projects\PixelFarm\PixelFarm.Drawing.GLES2\PixelFarm.Drawing.GLES2.csproj");
            mergePro.LoadSubProject(@"D:\projects\PixelFarm-dev\a_mini\projects\PixelFarm\PixelFarm.MiniOpenTK\PixelFarm.MiniOpenTK.csproj");
            mergePro.LoadSubProject(@"D:\projects\PixelFarm-dev\a_mini\projects\PixelFarm\PixelFarm.NativeWindows.WinForms\PixelFarm.NativeWindows.WinForms.csproj"); 
             
            
            return mergePro;
        }
        static MergeProject CreateMergePixelFarmMiniAggProject()
        {
            MergeProject mergePro = new MergeProject();
            mergePro.LoadSubProject(@"D:\projects\PixelFarm-dev\a_mini\projects\PixelFarm\PixelFarm.Drawing.Core\PixelFarm.Drawing.Core.csproj");
            //
            mergePro.LoadSubProject(@"D:\projects\PixelFarm-dev\a_mini\projects\PixelFarm\MiniAgg\MiniAgg.csproj");
            mergePro.LoadSubProject(@"D:\projects\PixelFarm-dev\a_mini\projects\PixelFarm\MiniAgg.Complements\MiniAgg.Complements.csproj");

            return mergePro;
        }

        static MergeProject CreateMergePixelFarmPortableProject()
        {
            //*** portable project for html renderer ***
            MergeProject mergePro = new MergeProject(true);
            mergePro.LoadSubProject(@"D:\projects\PixelFarm-dev\a_mini\projects\PixelFarm\PixelFarm.Drawing.Core\PixelFarm.Drawing.Core.csproj");
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
        ////---------------------------------------------------------------------------------------------------------------------------------------
        //static MergeProject CreateMergePixelFarmDrawingProject()
        //{

        //    MergeProject mergePro = new MergeProject();
        //    mergePro.LoadSubProject(@"D:\projects\PixelFarm-dev\a_mini\projects\PixelFarm\PixelFarm.Drawing.Core\PixelFarm.Drawing.Core.csproj");
        //    //
        //    mergePro.LoadSubProject(@"D:\projects\PixelFarm-dev\a_mini\projects\PixelFarm\MiniAgg\MiniAgg.csproj");
        //    mergePro.LoadSubProject(@"D:\projects\PixelFarm-dev\a_mini\projects\PixelFarm\MiniAgg.Complements\MiniAgg.Complements.csproj");
        //    mergePro.LoadSubProject(@"D:\projects\PixelFarm-dev\a_mini\projects\PixelFarm\NOpenType\N20\PixelFarm.OpenType\PixelFarm.OpenType.csproj");

        //    return mergePro;
        //}
        //private void cmdMergePixelFarm_Drawing_Click(object sender, EventArgs e)
        //{
        //    //-----------
        //    //PixelFarm's MiniAgg
        //    //-----------
        //    MergeProject mergePro = CreateMergePixelFarmDrawingProject();
        //    mergePro.MergeAndSave(@"D:\projects\PixelFarm-dev\a_mini\projects\PixelFarm\PixelFarm.Drawing.csproj",
        //       "PixelFarm.Drawing",
        //       "v2.0",
        //       "",//additional define constant
        //       new string[] {
        //          "System" ,
        //       });

        //}


    }
}
