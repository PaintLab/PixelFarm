//Apache2, 2014-present, WinterDev


using PixelFarm.Contours;
using System;
using Typography.Text;
namespace TestGraphicPackage2
{
    static class Program
    {

        [STAThread]
        static void Main(string[] args)
        {

            PixelFarm.Platforms.StorageService.RegisterProvider(new YourImplementation.LocalFileStorageProvider(""));

            //2.2 Icu Text Break info
            //test Typography's custom text break,
            //check if we have that data?            
            //------------------------------------------- 
            //string typographyDir = @"brkitr_src/dictionaries";
            //string icu_datadir = YourImplementation.RelativePathBuilder.SearchBackAndBuildFolderPath(System.IO.Directory.GetCurrentDirectory(), "PixelFarm", @"..\Typography\Typography.TextBreak\icu62\brkitr");

            dbugTestBreak();
            string icu_datadir = "brkitr"; //see brkitr folder, we link data from Typography project and copy to output if newer
            if (!System.IO.Directory.Exists(icu_datadir))
            {
                throw new System.NotSupportedException("dic");
            }
            var dicProvider = new Typography.TextBreak.IcuSimpleTextFileDictionaryProvider() { DataDir = icu_datadir };
            Typography.TextBreak.CustomBreakerBuilder.Setup(dicProvider);

            PixelFarm.CpuBlit.MemBitmapExt.DefaultMemBitmapIO = new PixelFarm.Drawing.WinGdi.GdiBitmapIO();

            YourImplementation.TestBedStartup.Setup();
            //-------------------------------------------

            //load demo list from specific asm             
            YourImplementation.TestBedStartup.RunDemoList(typeof(LayoutFarm.Demo_SingleButton).Assembly);
        }

#if DEBUG
        static void dbugTestBreak()
        {




            //string msg = "aBこん😁";
            //string msg = "1😁";
            string msg = "1😁\r\n\r\n\r\nxyz";

            //System.IO.StringReader rre = new System.IO.StringReader(msg);

            //string ln = rre.ReadLine();
            //while (ln != null)
            //{
            //    ln = rre.ReadLine();
            //}


            //TextBufferSpan buff;
            char[] utf16 = msg.ToCharArray();

            char[] x1 = "😁".ToCharArray();
            int utf32_x1 = char.ConvertToUtf32(x1[0], x1[1]);

            //https://www.unicode.org/faq/utf_bom.html#utf16-1
            //the first snippet calculates the high (or leading) surrogate from a character code C.

            //    const UTF16 HI_SURROGATE_START = 0xD800

            //    UTF16 X = (UTF16) C;
            //    UTF32 U = (C >> 16) & ((1 << 5) - 1);
            //    UTF16 W = (UTF16) U - 1;
            //    UTF16 HiSurrogate = HI_SURROGATE_START | (W << 6) | X >> 10;

            //where X, U and W correspond to the labels used in Table 3-5 UTF-16 Bit Distribution. The next snippet does the same for the low surrogate.

            //    const UTF16 LO_SURROGATE_START = 0xDC00

            //    UTF16 X = (UTF16) C;
            //    UTF16 LoSurrogate = (UTF16) (LO_SURROGATE_START | X & ((1 << 10) - 1));

            //Finally, the reverse, where hi and lo are the high and low surrogate, and C the resulting character

            //    UTF32 X = (hi & ((1 << 6) -1)) << 10 | lo & ((1 << 10) -1);
            //    UTF32 W = (hi >> 6) & ((1 << 5) - 1);
            //    UTF32 U = W + 1;

            //    UTF32 C = U << 16 | X;

            //------------------
            // constants
            const int LEAD_OFFSET = 0xD800 - (0x10000 >> 10);
            const int SURROGATE_OFFSET = 0x10000 - (0xD800 << 10) - 0xDC00;

            //from codepoint to upper and lower 
            ushort lead = (ushort)(LEAD_OFFSET + (utf32_x1 >> 10));
            ushort trail = (ushort)(0xDC00 + (utf32_x1 & 0x3FF));

            //compute back
            int codepoint_x = (lead << 10) + trail + SURROGATE_OFFSET;
            //UTF32 codepoint = (lead << 10) + trail + SURROGATE_OFFSET;



            char x1_0 = (char)(utf32_x1 >> 16);
            char x1_1 = (char)(utf32_x1);


            int[] utf32 = GetUtf32Buffer(msg, out int utf32Len);
            var reader = new Typography.TextBreak.InputReader(utf32, 0, utf32Len);
            //var reader = new Typography.TextBreak.InputReader(utf16);

            while (reader.ReadLine(
                out int begin,
                out int len,
                out Typography.TextBreak.InputReader.LineEnd endlineWith))
            {
                System.Diagnostics.Debug.WriteLine(begin + "," + len);
            }


            //buff = new TextBufferSpan(utf32, 1, utf32Len - 1);
            ////buff = new TextBufferSpan(msg_buffer, 1, 2);
            //Typography.TextLayout.LayoutWordVisitor visitor = new Typography.TextLayout.LayoutWordVisitor();

            //var line_segs = new Typography.TextLayout.LineSegmentList<Typography.TextLayout.LineSegment>();
            //visitor.SetLineSegmentList(line_segs);

            ////mew text service
            //var openFontTextService = new Typography.Text.OpenFontTextService();
            //Typography.Text.GlobalTextService.TxtClient = openFontTextService.CreateNewServiceClient();
            //Typography.Text.GlobalTextService.TxtClient.BreakToLineSegments(buff, visitor);

        }
        static int[] GetUtf32Buffer(string str, out int output_i)
        {
            char[] buffer = str.ToCharArray();
            int[] buffer2 = new int[buffer.Length];


            output_i = 0;
            for (int i = 0; i < buffer.Length; ++i)
            {
                char c = buffer[i];
                if (char.IsHighSurrogate(c))
                {
                    if (i < buffer.Length - 1 && char.IsLowSurrogate(buffer[i + 1]))
                    {
                        buffer2[output_i] = char.ConvertToUtf32(c, buffer[i + 1]);
                        output_i++;
                        i++;
                    }
                    else
                    {
                        //error
                    }
                }
                else
                {
                    buffer2[output_i] = c;
                    output_i++;
                }
            }

            return buffer2;
        }
#endif
    }
}
