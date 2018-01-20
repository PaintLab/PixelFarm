using System;
using System.Collections.Generic;
using System.Text;
using Hjg.Pngcs;
using Hjg.Pngcs.Chunks;

namespace SamplesTests {

    class SampleDecreaseRed {

        public static void DecreaseRed(String origFilename, String destFilename) {
            if (origFilename.Equals(destFilename)) throw new PngjException("input and output file cannot coincide");
            PngReader pngr = FileHelper.CreatePngReader(origFilename);
            PngWriter pngw = FileHelper.CreatePngWriter(destFilename, pngr.ImgInfo, true);
            Console.WriteLine(pngr.ToString());
            int chunkBehav = ChunkCopyBehaviour.COPY_ALL_SAFE; // copy all 'safe' chunks
            // this can copy some metadata from reader
            pngw.CopyChunksFirst(pngr, chunkBehav);
            int channels = pngr.ImgInfo.Channels;
            if (channels < 3)
                throw new Exception("This method is for RGB/RGBA images");
            for (int row = 0; row < pngr.ImgInfo.Rows; row++) {
                ImageLine l1 = pngr.ReadRow(row);
                for (int j = 0; j < pngr.ImgInfo.Cols; j++)
                    l1.Scanline[j * channels] /= 2;
                pngw.WriteRow(l1, row);
            }
            // just in case some new metadata has been read after the image
            pngw.CopyChunksLast(pngr, chunkBehav);
            pngw.End();
        }


    }

}
