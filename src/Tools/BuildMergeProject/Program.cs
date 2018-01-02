//MIT, 2017-2018, WinterDev

using System;
using System.Collections.Generic;
using System.Windows.Forms;
namespace BuildMergeProject
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            StartupConfig.defaultSln = @"D:\projects\PixelFarm\src\MiniDev.sln";

            Application.Run(new FormBuildMergeProject());
        }
    }

}

