namespace SampleTests {

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Runtime.CompilerServices;
    using Hjg.Pngcs;
    using Hjg.Pngcs.Chunks;


    public class SampleMirrorImage {

        public static void mirror(String orig, String dest) {
            if (orig.Equals(dest)) throw new PngjException("input and output file cannot coincide");

            PngReader pngr = FileHelper.CreatePngReader(orig);
            PngWriter pngw = FileHelper.CreatePngWriter(dest, pngr.ImgInfo, true);
            pngr.SetUnpackedMode(true); // we dont want to do the unpacking ourselves, we want a sample per array element
            pngw.SetUseUnPackedMode(true); // not really necesary here, as we pass the ImageLine, but anyway...
            pngw.CopyChunksFirst(pngr, ChunkCopyBehaviour.COPY_ALL_SAFE);
            for (int row = 0; row < pngr.ImgInfo.Rows; row++) {
                ImageLine l1 = pngr.ReadRowInt(row);
                mirrorLineInt(pngr.ImgInfo, l1.Scanline);
                pngw.WriteRow(l1, row);
            }
            pngw.CopyChunksLast(pngr, ChunkCopyBehaviour.COPY_ALL_SAFE);
            pngw.End();
        }

        private static void mirrorLineInt(ImageInfo imgInfo, int[] line) { // unpacked line
            int channels = imgInfo.Channels;
            for (int c1 = 0, c2 = imgInfo.Cols - 1; c1 < c2; c1++, c2--) { // swap pixels (not samples!)
                for (int i = 0; i < channels; i++) {
                    int aux = line[c1 * channels + i];
                    line[c1 * channels + i] = line[c2 * channels + i];
                    line[c2 * channels + i] = aux;
                }
            }
        }


    }
}
