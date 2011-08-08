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
		public FCGenerator generator = new FCGenerator();
		Dictionary<long, Chunk> chunkData;
		public World ()
		{
			int i = 1;
			while (i != 50)
			{
				Chunk c = new Chunk(i * 16, i * 16);
				chunkData.Add(i, generator.FlatChunk(c));
				i++;
			}
		}
	}
}

