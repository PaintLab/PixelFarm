//Apache2, 2012, Hernan J Gonzalez, https://github.com/leonbloy/pngcs
using System;

namespace Hjg.Pngcs.Chunks
{
    /// <summary>
    /// Match if have same Chunk Id
    /// </summary>
    internal class ChunkPredicateId : ChunkPredicate
    {
        private readonly string id;
        public ChunkPredicateId(String id)
        {
            this.id = id;
        }
        public bool Matches(PngChunk c)
        {
            return c.Id.Equals(id);
        }
    }
}
