//Apache2, 2014-2017, WinterDev

using System;
using System.Windows.Forms;
namespace TestGraphicPackage2
{
    static class Program
    {
        static LayoutFarm.Dev.FormDemoList formDemoList;
        [STAThread]
        static void Main()
        {

            YourImplementation.TestBedStartup.StartSetup(); 
            //-------------------------------
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false); 
            ////------------------------------- 
            formDemoList = new LayoutFarm.Dev.FormDemoList();
            formDemoList.LoadDemoList(typeof(Program)); 
            Application.Run(formDemoList);
        }


    }


}
