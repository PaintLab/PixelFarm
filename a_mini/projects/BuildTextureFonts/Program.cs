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
        const string MYFT = "myft.dll";
        [System.Runtime.InteropServices.DllImport(MYFT, CharSet = System.Runtime.InteropServices.CharSet.Ansi, CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern int MyFtMSDFGEN(int argc, string[] argv);

        [System.Runtime.InteropServices.DllImport(MYFT, CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern void DeleteUnmanagedObj(IntPtr unmanagedPtr);

        [System.Runtime.InteropServices.DllImport(MYFT, CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern IntPtr CreateShape();
        [System.Runtime.InteropServices.DllImport(MYFT, CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern IntPtr ShapeAddBlankContour(IntPtr shape);
        [System.Runtime.InteropServices.DllImport(MYFT, CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern void ContourAddLinearSegment(IntPtr cnt,
            double x0, double y0,
            double x1, double y1);
        [System.Runtime.InteropServices.DllImport(MYFT, CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern void ContourAddQuadraticSegment(IntPtr cnt,
            double x0, double y0,
            double ctrl0X, double ctrl0Y,
            double x1, double y1);
        [System.Runtime.InteropServices.DllImport(MYFT, CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern void ContourAddCubicSegment(IntPtr cnt,
            double x0, double y0,
            double ctrl0X, double ctrl0Y,
            double ctrl1X, double ctrl1Y,
            double x1, double y1);
        [System.Runtime.InteropServices.DllImport(MYFT, CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern unsafe void MyFtGenerateMsdf(IntPtr shape, int width, int height, double range,
            double scale, double tx, double ty, double edgeThreshold, double angleThreshold, int* outputBitmap);

        [System.Runtime.InteropServices.DllImport(MYFT)]
        public static extern bool ShapeValidate(IntPtr shape);
        [System.Runtime.InteropServices.DllImport(MYFT)]
        public static extern void ShapeNormalize(IntPtr shape);
        [System.Runtime.InteropServices.DllImport(MYFT)]
        public static extern void SetInverseYAxis(IntPtr shape, bool inverseYAxis);


        [System.Runtime.InteropServices.DllImport(MYFT)]
        public static extern int MyFtLibGetVersion();
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
            args.Add("-font"); args.Add(fontName);
            args.Add("0x" + ((int)character).ToString("X")); //accept unicode char
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
