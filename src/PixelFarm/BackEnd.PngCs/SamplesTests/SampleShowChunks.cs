namespace SamplesTests {

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Runtime.CompilerServices;
    using Hjg.Pngcs;
    using Hjg.Pngcs.Chunks;

    /**
 * prints all chunks (remember that IDAT is shown as only one pseudo zero-length chunk)
 */
    public class SampleShowChunks {

        public static void showChunks(String file) {
            PngReader pngr = FileHelper.CreatePngReader(file);
            pngr.MaxTotalBytesRead = 1024 * 1024 * 1024L * 3; // 3Gb!
            pngr.ReadSkippingAllRows();
            Console.Out.WriteLine(pngr.ToString());
            Console.Out.WriteLine(pngr.GetChunksList().ToStringFull());
        }


    }
}