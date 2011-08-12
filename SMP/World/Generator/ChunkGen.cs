using System;

namespace SMP {

    /// <summary>
    /// This class is used to tie together all chunk generators.
    /// </summary>
    public abstract class ChunkGen {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="w"></param>
        /// <param name="c"></param>
        public abstract void Generate( World w, Chunk c );
    }
}
