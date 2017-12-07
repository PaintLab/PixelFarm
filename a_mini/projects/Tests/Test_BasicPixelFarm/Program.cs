//Apache2, 2014-2017, WinterDev

using System;
namespace TestGraphicPackage2
{
    static class Program
    {

        [STAThread]
        static void Main()
        {

            YourImplementation.TestBedStartup.Setup();
            YourImplementation.TestBedStartup.RunDemoList(typeof(Program));

        }

    }


}
