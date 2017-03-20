//Apache2, 2014-2017, WinterDev

using System;
using System.Collections.Generic;
using System.Windows.Forms;
namespace TestGraphicPackage2
{
    static class Program
    {
        static LayoutFarm.Dev.FormDemoList formDemoList;
        [STAThread]
        static void Main()
        {


            //-------------------------------
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //temp
            //TODO: fix this , 
            //LayoutFarm.Composers.Default.TextBreaker = new LayoutFarm.Composers.MyManagedTextBreaker();
            //LayoutFarm.Composers.Default.TextBreaker = new LayoutFarm.Composers.MyNativeTextBreaker();

            ////------------------------------- 
            formDemoList = new LayoutFarm.Dev.FormDemoList();
            formDemoList.LoadDemoList(typeof(Program));

            Application.Run(formDemoList);
        }


    }


}
