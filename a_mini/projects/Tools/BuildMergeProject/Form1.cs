//2016, MIT WinterDev

using System;
using System.IO;

using System.Windows.Forms;
namespace BuildMergeProject
{
    public partial class Form1 : Form
    {
        static string rootProjectFolders = @"D:\projects\PixelFarm-dev\a_mini\projects\PixelFarm";

        public Form1()
        {
            InitializeComponent();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            //merge all
            //need Gdi+ and WinForms
            MergeProject mergePro = CreateMergePixelFarmOneProject(MergeOption.All);
            mergePro.MergeAndSave(rootProjectFolders + @"\PixelFarm.One.csproj",
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
        private void cmdMergePixelFarmMiniAggOne_Click(object sender, EventArgs e)
        {
            //-----------
            //PixelFarm's MiniAgg.One
            //-----------
            MergeProject mergePro = CreateMiniAggOneProject();
            mergePro.MergeAndSave(rootProjectFolders + @"\PixelFarm.MiniAgg.One.csproj",
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
            mergePro.MergeAndSave(rootProjectFolders + @"\PixelFarm.One.Portable.csproj",
            "PixelFarm.One.Portable",
               "v4.5",
               "",//additional define constant
               new string[0]);
        }

        enum MergeOption
        {
            All,
            Windows_NoCustomNativeDll,
            Windows_NoWinFormNoGdiPlus,
            Cross,
        }
        static MergeProject CreateMergePixelFarmOneProject(MergeOption mergeOption)
        {
            MergeProject mergePro = new MergeProject();
            mergePro.LoadSubProject(rootProjectFolders + @"\PixelFarm.Drawing.Core\PixelFarm.Drawing.Core.csproj");
            mergePro.LoadSubProject(rootProjectFolders + @"\TypeMirror\TypeMirror.csproj");
            mergePro.LoadSubProject(rootProjectFolders + @"\Win32Utils\Win32Utils.csproj");
            //
            mergePro.LoadSubProject(rootProjectFolders + @"\MiniAgg\MiniAgg.csproj");
            mergePro.LoadSubProject(rootProjectFolders + @"\MiniAgg.Complements\MiniAgg.Complements.csproj");
            mergePro.LoadSubProject(rootProjectFolders + @"\NOpenType\N20\PixelFarm.OpenType\PixelFarm.OpenType.csproj");
            mergePro.LoadSubProject(rootProjectFolders + @"\PixelFarm.DataProvider.Managed\PixelFarm.DataProvider.Managed.csproj");

            switch (mergeOption)
            {
                default:
                case MergeOption.Cross:
                    throw new NotImplementedException();
                case MergeOption.Windows_NoCustomNativeDll:
                    mergePro.LoadSubProject(rootProjectFolders + @"\PixelFarm.Drawing.GdiPlus\PixelFarm.Drawing.GdiPlus.csproj");
                    break;

                case MergeOption.All:
                    mergePro.LoadSubProject(rootProjectFolders + @"\PixelFarm.Drawing.GdiPlus\PixelFarm.Drawing.GdiPlus.csproj");
                    mergePro.LoadSubProject(rootProjectFolders + @"\Tesselate\Tesselate.csproj");
                    mergePro.LoadSubProject(rootProjectFolders + @"\PixelFarm.DataProvider.Native\PixelFarm.DataProvider.Native.csproj");
                    //                                   
                    mergePro.LoadSubProject(rootProjectFolders + @"\PixelFarm.MiniOpenTK\PixelFarm.MiniOpenTK.csproj");
                    mergePro.LoadSubProject(rootProjectFolders + @"\PixelFarm.Drawing.GLES2\PixelFarm.Drawing.GLES2.csproj");
                    mergePro.LoadSubProject(rootProjectFolders + @"\PixelFarm.NativeWindows\PixelFarm.NativeWindows.csproj");
                    mergePro.LoadSubProject(rootProjectFolders + @"\PixelFarm.OpenTKWinForms\PixelFarm.OpenTKWinForms.csproj");
                    break;

                case MergeOption.Windows_NoWinFormNoGdiPlus:
                    mergePro.LoadSubProject(rootProjectFolders + @"\Tesselate\Tesselate.csproj");
                    mergePro.LoadSubProject(rootProjectFolders + @"\PixelFarm.MiniOpenTK\PixelFarm.MiniOpenTK.csproj");
                    mergePro.LoadSubProject(rootProjectFolders + @"\PixelFarm.NativeWindows\PixelFarm.NativeWindows.csproj");
                    mergePro.LoadSubProject(rootProjectFolders + @"\PixelFarm.Drawing.GLES2\PixelFarm.Drawing.GLES2.csproj");
                    break;
            }
            // 

            return mergePro;
        }
        static MergeProject CreateMiniAggOneProject()
        {
            MergeProject mergePro = new MergeProject();
            mergePro.LoadSubProject(rootProjectFolders + @"\PixelFarm.Drawing.Core\PixelFarm.Drawing.Core.csproj");
            //
            mergePro.LoadSubProject(rootProjectFolders + @"\MiniAgg\MiniAgg.csproj");
            mergePro.LoadSubProject(rootProjectFolders + @"\MiniAgg.Complements\MiniAgg.Complements.csproj");

            return mergePro;
        }
        static MergeProject CreateMergePixelFarmMiniAggProject()
        {
            MergeProject mergePro = new MergeProject();
            mergePro.LoadSubProject(rootProjectFolders + @"\MiniAgg\MiniAgg.csproj");
            mergePro.LoadSubProject(rootProjectFolders + @"\MiniAgg.Complements\MiniAgg.Complements.csproj");
            return mergePro;
        }
        static MergeProject CreateMergePixelFarmPortableProject()
        {
            //*** portable project for html renderer ***
            MergeProject mergePro = new MergeProject(true);
            mergePro.LoadSubProject(rootProjectFolders + @"\PixelFarm.Drawing.Core\PixelFarm.Drawing.Core.csproj");
            //
            mergePro.LoadSubProject(rootProjectFolders + @"\MiniAgg\MiniAgg.csproj");
            mergePro.LoadSubProject(rootProjectFolders + @"\MiniAgg.Complements\MiniAgg.Complements.csproj");
            //
            //mergePro.LoadSubProject(@"D:\projects\agg-sharp\a_mini\projects\PixelFarm\MiniAgg.HardwareGraphics\OpenTK.ES\OpenTK.ES.csproj");
            mergePro.LoadSubProject(rootProjectFolders + @"\MiniTesselate\Tesselate.csproj");
            // 
            //mergePro.LoadSubProject(@"D:\projects\agg-sharp\a_mini\projects\PixelFarm\MiniAgg.HardwareGraphics\MiniAgg.Hw2\MiniAgg.Hw2.csproj");

            return mergePro;
        }

        private void cmd_Windows_OnlyGdiPlus_Click(object sender, EventArgs e)
        {
            //Windows:
            //-------------------------
            //no glfw.dll,
            //no myft.dll,
            //no gles libs
            //-------------------------
            MergeProject mergePro = CreateMergePixelFarmOneProject(MergeOption.Windows_NoCustomNativeDll);

            mergePro.MergeAndSave(rootProjectFolders + @"\PixelFarm.One.OnlyGdiPlus.csproj",
               "PixelFarm.One.OnlyGdiPlus",
               "v2.0",
               "",//additional define constant
               new string[] {
                  "System",
                  "System.Drawing",
                  "System.Windows.Forms",
                  "System.Xml",
               });
        }
        private void cmd_Windows_NoGdiPlus_NoWinForms_Click(object sender, EventArgs e)
        {
            //Windows
            //----------------------------
            //no System.Drawing, no WinForms
            //----------------------------
            //may use Win32 api
            //----------------------------
            //need glfw, myft, gles

            MergeProject mergePro = CreateMergePixelFarmOneProject(MergeOption.Windows_NoWinFormNoGdiPlus);
            mergePro.MergeAndSave(rootProjectFolders + @"\PixelFarm.One.NoGdiPlusNoWinForms.csproj",
               "PixelFarm.One.NoGdiPlusNoWinForms",
               "v2.0",
               "",//additional define constant
               new string[] {
                  "System",
                  "System.Xml",
               });
        }

        private void cmd_Cross_Click(object sender, EventArgs e)
        {
            //cross platform 
            //no WinForms, no GdiPlus
            //MergeProject mergePro = CreateMergePixelFarmOneProject(MergeOption.Cross);
            //mergePro.MergeAndSave(rootProjectFolders + @"\PixelFarm.One.csproj",
            //   "PixelFarm.One",
            //   "v2.0",
            //   "",//additional define constant
            //   new string[] {
            //      "System",
            //      "System.Drawing",
            //      "System.Windows.Forms",
            //      "System.Xml",
            //   });
        }

        private void cmdCopyNativeLibs_Click(object sender, EventArgs e)
        {

            //this copy essential native lib to target folder

            //event we select native files from debug folder
            //theses native dll must be in release mode
            //TODO: review here=> //choose file from release folder


            string srcFolder = @"D:\projects\PixelFarm-dev\a_mini\projects\Tests\Debug\";

            string[] nativeFileList = new string[]{
                srcFolder+"d3dcompiler_47.dll",// angle project
                srcFolder+"libEGL.dll", // angle project
                srcFolder+"libGLESv2.dll",// angle project
                srcFolder +"myft.dll", //font, icu, native image lib
                srcFolder +"natives32/glfw3.dll", //glfw 
                @"D:\WImageTest\icudt57l\icudt57l.dat",//icu data

            };
            //------
            string targetFolder = rootProjectFolders + @"\ess_natives\";
            foreach (string srcFile in nativeFileList)
            {
                //copy to target folder

                File.Copy(srcFile,
                    targetFolder + Path.GetFileName(srcFile),
                    true);
            }

        }

        private void cmdForTestWithHtmlRenderer_Click(object sender, EventArgs e)
        {
            //this for test ****
            //Windows:
            //-------------------------
            //no glfw.dll,
            //no myft.dll,
            //no gles libs
            //-------------------------
            MergeProject mergePro = CreateMergePixelFarmOneProject(MergeOption.Windows_NoCustomNativeDll);

            mergePro.MergeAndSave(rootProjectFolders + @"\PixelFarm.One.HtmlRenderer.csproj",
               "PixelFarm.One.HtmlRenderer",
               "v2.0",
               "",//additional define constant
               new string[] {
                  "System",
                  "System.Drawing",
                  "System.Windows.Forms",
                  "System.Xml",
               });
        }

        private void cmdBuildMergePixelFarmMiniAgg_Click(object sender, EventArgs e)
        {
            //-----------
            //PixelFarm.MiniAgg
            //-----------
            MergeProject mergePro = CreateMergePixelFarmMiniAggProject();
            mergePro.MergeAndSave(rootProjectFolders + @"\PixelFarm.MiniAgg.csproj",
               "PixelFarm.MiniAgg",
               "v2.0",
               "",//additional define constant
               new string[] {
                  "System" ,
               });
        }
    }
}
