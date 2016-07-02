using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace BuildTextureFonts
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
            Application.Run(new Form1());
        }
    }
    class ScanLine
    {
        public List<ScanStrip> scanStrip = new List<ScanStrip>();

    }
    struct ScanStrip
    {
        public bool black;
        public int x;
        public int width;
    }
    static class MyFtLib
    {
        [System.Runtime.InteropServices.DllImport("myft.dll", CharSet = System.Runtime.InteropServices.CharSet.Ansi, CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static unsafe extern int MyFtMSDFGEN(int argc, string[] argv);
    }
}
