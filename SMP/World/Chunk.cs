using System;
using System.IO;
using System.Collections.Generic;
using zlib;

namespace SMP
{
	public partial class Chunk
	{
		static int Width = 16;
		static int Depth = 16;
		static int Height = 128;

		public byte[] blocks;
		public byte[] Light;
		public byte[] SkyL;
		public byte[] meta;
		public int x;
		public int z;
		public bool mountain = true;

		public List<Entity> Entities = new List<Entity>();

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
			Light = new byte[16384];
			SkyL = new byte[16384];
			meta = new byte[16384];
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

		public void SetBlockLight(int x, int y, int z, byte light)
		{
			if (InBound(x, y, z))
			{
				int index = PosToInt(x, y, z);
				SetHalf(index, light, ref Light[index / 2]);
			}
		}
		public byte GetBlockLight(int x, int y, int z)
		{
			if (InBound(x, y, z))
			{
				// (y % 2 == 0) ? (data & 0x0F) : ((data >> 4) & 0x0F)
				int index = PosToInt(x, y, z);
				return getHalf(index, Light[index / 2]);
			}
			else
			{
				return 0xFF;
			}
		}
		public void SetSkyLight(int x, int y, int z, byte light)
		{
			if (InBound(x, y, z))
			{
				int index = PosToInt(x, y, z);
				SetHalf(index, light, ref SkyL[index / 2]);
			}
		}
		public byte GetSkyLight(int x, int y, int z)
		{
			if (InBound(x, y, z))
			{
				// (y % 2 == 0) ? (data & 0x0F) : ((data >> 4) & 0x0F)
				//int index = PosToInt(x, y, z);
				return getHalf(index, SkyL[PosToInt(x, y, z) / 2]);
			}
			else
			{
				return 0xFF;
			}
		}
		public void SetMetaData(int x, int y, int z, byte data)
		{
			if (InBound(x, y, z))
			{
				//int index = PosToInt(x, y, z);
				SetHalf(index, data, ref meta[PosToInt(x, y, z) / 2]);
			}
		}
		public byte GetMetaData(int x, int y, int z)
		{
			if (InBound(x, y, z))
			{
				// (y % 2 == 0) ? (data & 0x0F) : ((data >> 4) & 0x0F)
				return getHalf(index, meta[PosToInt(x, y, z) / 2]);
			}
			else
			{
				return 0xFF;
			}
		}
		private void SetHalf(int index, byte value, ref byte data)
		{
			if (index % 2 == 0)
			{
				// Set the lower 4 bits
				byte high = (byte)((data & 0xF0) >> 4);
				data = (byte)((high << 4) | value);
			}
			else
			{
				// Set the upper 4 bits
				byte low = (byte)(data & 0x0F);
				data = (byte)((value << 4) | low);
			}
		}
		private byte getHalf(int index, byte data)
		{
			return (index % 2 == 0) ? (byte)(data & 0x0F) : (byte)((data >> 4) & 0x0F);
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
                    zout.Write(blocks, 0, blocks.Length);

                    // Write metadata
                    zout.Write(meta, 0, meta.Length);

                    // Write block light
                    zout.Write(Light, 0, Light.Length);

                    // Write sky light
                    zout.Write(SkyL, 0, SkyL.Length);
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
			PlaceBlock(x, y, z, id, 0);
		}
		public void PlaceBlock(int x, int y, int z, byte id, byte meta)
		{
			//if (BlockPlaced != null) { if (BlockPlaced(x, y, z, id)) return; }
			if (InBound(x, y, z))
			{
				blocks[PosToInt(x, y, z)] = id;
				//TODO SET METADATA
			}
		}
		public byte GetBlock(int x, int y, int z)
		{
			if (InBound(x, y, z))
				return blocks[PosToInt(x, y, z)];
			return 0;
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
		/*public void PlaceBlock(int x, int y, int z, DataValues id)
		{
			if (BlockPlaced != null) { if (BlockPlaced(x, y, z, id)) return; }
			if (InBound(x, y, z))
				blocks[PosToInt(x, y, z)] = id;
		}*/
		public static int PosToInt(int x, int y, int z)
        {
			return (x * Depth + z) * Height + y;
        }
		public static Chunk GetChunk(int x, int z)
		{
			try
			{
				return Server.mainlevel.chunkData[new Point(x, z)];
			}
			catch
			{
				return null;
			}
		}
	}
}

