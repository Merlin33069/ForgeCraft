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
		Dictionary<long, Chunck> chunkData;
		public World ()
		{
		}
	}
}

