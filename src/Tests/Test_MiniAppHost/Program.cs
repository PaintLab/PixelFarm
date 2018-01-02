//Apache2, 2014-2018, WinterDev

using System;
namespace TestGraphicPackage2
{
    static class Program
    {

        [STAThread]
        static void Main(string[] args)
        {
            YourImplementation.TestBedStartup.Setup();
            YourImplementation.TestBedStartup.RunDemoList(typeof(Program));
        }
    }
}
