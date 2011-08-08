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
		public FCGenerator generator;
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
			generator = new FCGenerator(this);
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
			return null;
		}
		public void SaveLVL(World  w)
		{
			//TODO Save files
		}
		//THIS WONT WORK...I need to fix it
		public void SendData(Chunk c)
		{
			Player.players.ForEach(delegate(Player p)
			{
				byte[] tosend1 = new byte[2];
				tosend1[0] = (byte)c.x;
				tosend1[1] = (byte)c.z;
				tosend1[2] = 1;
				p.SendRaw(0x32, tosend1);
				byte[] tosend = new byte[7];
				tosend[0] = (byte)c.x;
				tosend[1] = 0;
				tosend[2] = (byte)c.z;
				tosend[3] = 15;
				tosend[4] = 127;
				tosend[5] = 15;
				tosend[6] = 0; //idk
				tosend[7] = 10;  //just a hack fix to get it to compile
				p.SendRaw(0x33, tosend);
			});
		}
	}
}

