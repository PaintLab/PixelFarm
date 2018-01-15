using SampleTests;
using System;
using System.Collections.Generic;
using System.Text;
using Hjg.Pngcs;


namespace SamplesTests
{

    class MainProgram
    {

        static void Main(string[] args)
        {


            PngReader png = FileHelper.CreatePngReader("d:/WImageTest/test1.png");
            Console.Out.WriteLine(png);


            long t0 = Environment.TickCount;
            //testX();
            myTestSuite();
            //testTextChunks();
            long t1 = Environment.TickCount;

            Console.Out.WriteLine("Done. (" + (t1 - t0) + " msecs) " + "Net version: " + Environment.Version + " Press ENTER to close");
            Console.In.ReadLine();
        }

        static void testX()
        { // placeholder method for misc tests
            PngReader png = FileHelper.CreatePngReader("C:/temp/map.png");
            Console.Out.WriteLine(png);
        }

        static void myTestSuite()
        {
            testSuite(new string[] { "d:/devel/repositories/pnjgs/pnjg/resources/testsuite1/", "D:/temp/testcs" });
        }

        /// <summary>
        /// textual chunks
        /// </summary>
        static void testTextChunks()
        {
            TestTextChunks.test();
        }


        static void sampleShowChunks(string[] args)
        {
            if (args.Length < 1)
            {
                Console.Error.WriteLine("expected [inputfile]");
                return;
            }
            SampleShowChunks.showChunks(args[0]);
        }

        static void sampleConvertTrueColor(string file)
        {
            SampleConvertToTrueCol.doit(file);
        }


        static void sampleMirror(string[] args)
        {
            if (args.Length < 2)
            {
                Console.Error.WriteLine("expected [inputfile] [outputfile]");
                return;
            }
            SampleMirrorImage.mirror(args[0], args[1]);
            Console.Out.WriteLine("sampleMirror done " + args[0] + " ->" + args[1]);
        }

        static void decreaseRed(string[] args)
        {
            if (args.Length < 2)
            {
                Console.Error.WriteLine("expected [inputfile] [outputfile]");
                return;
            }
            SampleDecreaseRed.DecreaseRed(args[0], args[1]);
            Console.Out.WriteLine("decreaseRed done " + args[0] + " ->" + args[1]);
        }

        static void customChunk(string[] args)
        {
            if (args.Length < 2)
            {
                Console.Error.WriteLine("expected [inputfile] [outputfile]");
                return;
            }
            Console.Out.WriteLine("custom chunk write : " + args[0] + " ->" + args[1]);
            SampleCustomChunk.testWrite(args[0], args[1]);
            Console.Out.WriteLine("custom chunk read: " + args[1]);
            SampleCustomChunk.testRead(args[1]);
        }

        static void testSingle(string file)
        {
            TestPngSuite.testSingle(file, null, null);
        }


        static void testSuite(string[] args)
        {
            if (args.Length < 2)
            {
                Console.Error.WriteLine("expected [origdir] [destdir] [maxfiles]");
                return;
            }
            int maxfiles = args.Length < 3 ? 0 : int.Parse(args[2]);
            TestPngSuite.testAllSuite(args[0], args[1], maxfiles);
        }

    }
}
