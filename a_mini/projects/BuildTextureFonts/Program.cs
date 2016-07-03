using System;
using System.Collections.Generic;
using System.Text;

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
    class MsdfParameters
    {
        public string fontName;
        public bool useClassicSdf;
        public char character;
        public string outputFile;
        public int sizeW = 32;
        public int sizeH = 32;
        public int pixelRange = 4;
        public string testRenderFileName;
        public bool enableRenderTestFile = true;
        public MsdfParameters(string fontName, char character)
        {
            this.fontName = fontName;
            this.character = character;
        }
        public override string ToString()
        {
            string[] args = GetArgs();
            StringBuilder stbulder = new StringBuilder();
            stbulder.Append(args[0]);
            int j = args.Length;
            for (int i = 1; i < j; ++i)
            {
                stbulder.Append(' ');
                stbulder.Append(args[i]);
            }
            return stbulder.ToString();

        }
        public string[] GetArgs()
        {
            List<string> args = new List<string>();
            //0.
            args.Add("msdfgen");
            //1.
            string genMode = "msdf";
            if (useClassicSdf)
            {
                genMode = "sdf";
            }
            args.Add(genMode);
            //2.
            if (fontName == null) { throw new Exception(); }
            args.Add("-font"); args.Add(fontName); args.Add("'" + character + "'");
            //3.
            if (outputFile == null)
            {
                //use default
                outputFile = genMode + "_" + ((int)character).ToString() + ".png";
            }
            args.Add("-o"); args.Add(outputFile);
            //4.
            args.Add("-size"); args.Add(sizeW.ToString()); args.Add(sizeH.ToString());
            //5.
            args.Add("pxrange"); args.Add(pixelRange.ToString());
            //6.
            args.Add("-autoframe");//default
                                   //7.
            if (enableRenderTestFile)
            {
                if (testRenderFileName == null)
                {
                    testRenderFileName = "test_" + genMode + "_" + character + ".png";
                }
                args.Add("-testrender"); args.Add(testRenderFileName);
                args.Add("1024");
                args.Add("1024");
            }
            return args.ToArray();
        }
    }

}
