//Apache2, 2012, Hernan J Gonzalez, https://github.com/leonbloy/pngcs
namespace Hjg.Pngcs.Chunks
{

    using Hjg.Pngcs;
    using System;
    /// <summary>
    /// IEND chunk  http://www.w3.org/TR/PNG/#11IEND
    /// </summary>
    public class PngChunkIEND : PngChunkSingle
    {
        public const String ID = ChunkHelper.IEND;

        public PngChunkIEND(ImageInfo info)
            : base(ID, info)
        {
        }

        public override ChunkOrderingConstraint GetOrderingConstraint()
        {
            return ChunkOrderingConstraint.NA;
        }

        public override ChunkRaw CreateRawChunk()
        {
            ChunkRaw c = new ChunkRaw(0, ChunkHelper.b_IEND, false);
            return c;
        }

        public override void ParseFromRaw(ChunkRaw c)
        {
            // this is not used
        }

        public override void CloneDataFromRead(PngChunk other)
        {
        }
    }
}
