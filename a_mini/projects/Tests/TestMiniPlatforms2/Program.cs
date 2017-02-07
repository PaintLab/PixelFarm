//MIT, 2016-2017, WinterDev
using System;
using System.Runtime.InteropServices;
namespace TestGlfw
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
           
            Mini.RootDemoPath.Path = @"..\Data";
            //GLFWProgram2.Start();
            GLFWProgram3.Start();
        }
    }
}
