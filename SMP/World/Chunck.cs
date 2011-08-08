using System;
using zlib;

namespace SMP
{
	public class Chunk
	{
		public byte[] blocks;
		public byte[] Light;
		public byte[] SkyL;
		public byte[] meta;
		public int x;
		public int z;
		/// <summary>
		/// When a block is placed then this is called
		/// </summary>
		/// <param name='x'>
		/// X. The x position the block was placed
		/// </param>
		/// <param name='y'>
		/// Y. The y position the block was placed
		/// </param>
		/// <param name='z'>
		/// Z. The z position the block was placed
		/// </param>
		/// <param name='id'>
		/// The block id
		/// </param>
		public delegate bool OnBlockPlaced(int x, int y, int z, byte id);
		public event OnBlockPlaced BlockPlaced;
		/// <summary>
		/// Initializes a new instance of the <see cref="SMP.Chunck"/> class with the default Block Count (32768)
		/// </summary>
		/// <param name='x'>
		/// X. The x position of the chunk
		/// </param>
		/// <param name='z'>
		/// Z. The z position of the chunk
		/// </param>
		public Chunk (int x, int z)
		{
			blocks = new byte[32768];
			Light = new byte[16348];
			SkyL = new byte[16348];
			meta = new byte[16348];
			this.x = x; this.z = z;
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="SMP.Chunck"/> class with a custom Block Count
		/// </summary>
		/// <param name='x'>
		/// X. The x position of the chunk
		/// </param>
		/// <param name='z'>
		/// Z. The z position of the chunk
		/// </param>
		/// <param name='BlockCount'>
		/// Block count. The block count
		/// </param>
		public Chunk(int x, int z, int BlockCount)
		{
			blocks = new byte[BlockCount];
			Light = new byte[BlockCount / 2];
			SkyL = new byte[BlockCount / 2];
			meta = new byte[BlockCount / 2];
			this.x = x; this.z = z;
		}
		public bool InBound(int x, int y, int z)
		{
			if (x < 0 || y < 0 || z < 0 || x >= 16 || z >= 16 || y >= 128)
				return false;
			return true;
		}
		/// <summary>
        /// Returns a compressed copy of the chunk's block data.
        /// </summary>
        public byte[] GetCompressedData()
        {
            byte[] compressed;
            using (MemoryStream ms = new MemoryStream())
            {
                using (ZOutputStream zout = new ZOutputStream(ms, zlibConst.Z_BEST_COMPRESSION))
                {
                    // Write block types
                    zout.Write(blockTypes, 0, blockTypes.Length);

                    // Write metadata
                    zout.Write(metaData, 0, metaData.Length);

                    // Write block light
                    zout.Write(blockLight, 0, blockLight.Length);

                    // Write sky light
                    zout.Write(skyLight, 0, skyLight.Length);
                }
                compressed = ms.ToArray();
            }
            return compressed;
        }
		/// <summary>
		/// Places the block at a x, y, z.
		/// </summary>
		/// <param name='x'>
		/// X. The x pos that the block will be pladed.
		/// </param>
		/// <param name='y'>
		/// Y. The y pos that the  block will be placed.
		/// </param>
		/// <param name='z'>
		/// Z. The z pos that the block will be placed.
		/// </param>
		/// <param name='id'>
		/// Block id.
		/// </param>
		public void PlaceBlock(int x, int y, int z, byte id)
		{
			if (BlockPlaced != null) { if (BlockPlaced(x, y, z, id)) return; }
			if (InBound(x, y, z))
				blocks[PosToInt(x, y, z)] = id;
		}
		/// <summary>
		/// Places the block at a x, y, z.
		/// </summary>
		/// <param name='x'>
		/// X. The x pos that the block will be pladed.
		/// </param>
		/// <param name='y'>
		/// Y. The y pos that the  block will be placed.
		/// </param>
		/// <param name='z'>
		/// Z. The z pos that the block will be placed.
		/// </param>
		/// <param name='id'>
		/// Block id.
		/// </param>
		public void PlaceBlock(int x, int y, int z, DataValues id)
		{
			if (BlockPlaced != null) { if (BlockPlaced(x, y, z, id)) return; }
			if (InBound(x, y, z))
				blocks[PosToInt(x, y, z)] = id;
		}
		public static int PosToInt(int x, int y, int z)
        {
            return (x * Depth + z) * Height + y;
        }
			
	}
}

