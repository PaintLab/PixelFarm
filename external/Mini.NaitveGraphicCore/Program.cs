using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace NativeV8
{
    static class Program
    {
         
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Mini.GraphicCore.Form2());
           // Application.Run(new Mini.GraphicCore.Form3()); 
        }
    }
     
}
