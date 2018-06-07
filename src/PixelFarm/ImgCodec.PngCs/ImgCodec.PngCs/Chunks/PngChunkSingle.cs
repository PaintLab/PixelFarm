//Apache2, 2012, Hernan J Gonzalez, https://github.com/leonbloy/pngcs
using System;
namespace Hjg.Pngcs.Chunks
{
    /// <summary>
    /// A Chunk type that does not allow duplicate in an image
    /// </summary>
    public abstract class PngChunkSingle : PngChunk
    {
        public PngChunkSingle(String id, ImageInfo imgInfo)
            : base(id, imgInfo)
        {
        }

        public sealed override bool AllowsMultiple()
        {
            return false;
        }

        public override int GetHashCode()
        {
            int prime = 31;
            int result = 1;
            result = prime * result + ((Id == null) ? 0 : Id.GetHashCode());
            return result;
        }

        public override bool Equals(object obj)
        {
            return (obj is PngChunkSingle && Id != null && Id.Equals(((PngChunkSingle)obj).Id));
        }

    }
}
