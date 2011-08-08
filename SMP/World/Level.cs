using System;
using System.Collections.Generic;
namespace SMP
{
	public class World
	{
		public double SpawnX;
		public double SpawnY;
		public double SpawnZ;
		public float SpawnYaw;
		public float SpawnPitch;
		public string Map_Name;
		public FCGenerator generator = new FCGenerator();
		Dictionary<long, Chunk> chunkData;
		/// <summary>
		/// Initializes a new instance of the <see cref="SMP.World"/> class and generates 49 chunks.
		/// </summary>
		/// <param name='spawnx'>
		/// Spawnx. The x spawn pos.
		/// </param>
		/// <param name='spawny'>
		/// Spawny. The y spawn pos.
		/// </param>
		/// <param name='spawnz'>
		/// Spawnz. The z spawn pos.
		/// </param>
		public World (double spawnx, double spawny, double spawnz)
		{
			int i = 1;
			while (i != 50)
			{
				Chunk c = new Chunk(i * 16, i * 16);
				chunkData.Add(i, generator.FlatChunk(c));
				i++;
			}
			this.SpawnX = spawnx; this.SpawnY = spawny; this.SpawnZ = spawnz;
		}
		public static World LoadLVL(string filename)
		{
			//TODO Load files
		}
		public void SaveLVL(World  w)
		{
			//TODO Save files
		}
		public void SendData()
		{
			
		}
	}
}

