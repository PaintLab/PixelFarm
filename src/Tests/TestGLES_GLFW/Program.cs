//MIT, 2016-present, WinterDev
using System; 
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
            GLFWProgram2.Start();
            //GLFWProgram3.Start();
        }
    }
}
