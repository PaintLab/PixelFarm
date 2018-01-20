//Apache2, 2012, Hernan J Gonzalez, https://github.com/leonbloy/pngcs
namespace Hjg.Pngcs.Chunks
{

    using Hjg.Pngcs;
    using System; 
    /// <summary>
    /// IDAT chunk http://www.w3.org/TR/PNG/#11IDAT
    /// 
    /// This object is dummy placeholder - We treat this chunk in a very different way than ancillary chnks
    /// </summary>
    public class PngChunkIDAT : PngChunkMultiple
    {
        public const String ID = ChunkHelper.IDAT;

        public PngChunkIDAT(ImageInfo i, int len, long offset)
            : base(ID, i)
        {
            this.Length = len;
            this.Offset = offset;
        }

        public override ChunkOrderingConstraint GetOrderingConstraint()
        {
            return ChunkOrderingConstraint.NA;
        }

        public override ChunkRaw CreateRawChunk()
        {// does nothing
            return null;
        }

        public override void ParseFromRaw(ChunkRaw c)
        { // does nothing
        }

        public override void CloneDataFromRead(PngChunk other)
        {
        }
    }
}
