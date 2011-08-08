using System;

namespace SMP
{
	public class Chunck
	{
		public byte[] blocks;
		public byte[] Light;
		public byte[] SkyL;
		public byte[] meta;
		public int x;
		public int z;
		/// <summary>
		/// Initializes a new instance of the <see cref="SMP.Chunck"/> class with the default Block Count (32768)
		/// </summary>
		/// <param name='x'>
		/// X. The x position of the chunk
		/// </param>
		/// <param name='z'>
		/// Z. The z position of the chunk
		/// </param>
		public Chunck (int x, int z)
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
		public Chunck(int x, int z, int BlockCount)
		{
			blocks = new byte[BlockCount];
			Light = new byte[BlockCount / 2];
			SkyL = new byte[BlockCount / 2];
			meta = new byte[BlockCount / 2];
			this.x = x; this.z = z;
		}
	}
}

