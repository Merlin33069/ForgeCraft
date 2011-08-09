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
		public Dictionary<long, Chunk> chunkData;
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
			chunkData = new Dictionary<long, Chunk>();
			generator = new FCGenerator(this);
			int i = 0;
			for (int x = -3; x <= 3; x++)
			{
				for (int z = -3; z <= 3; z++)
				{
					Chunk c = new Chunk(x, z);
					generator.FlatChunk(c);
					c.RecalculateLight();
					chunkData.Add(i, c);
					i++;
				}
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
		#region Do not use
		//THIS WONT WORK...I need to fix it
		//public void SendData(Player p)
		//{
		//    for (int i = 0; i < chunkData.Count - 1; i++)
		//    {
		//        Chunk c = chunkData[i + 1];
		//        byte[] tosend1 = new byte[2];
		//        tosend1[0] = (byte)c.x;
		//        tosend1[1] = (byte)c.z;
		//        tosend1[2] = 1;
		//        p.SendRaw(0x32, tosend1);
		//        byte[] tosend = new byte[7];
		//        tosend[0] = (byte)c.x;
		//        tosend[1] = 0;
		//        tosend[2] = (byte)c.z;
		//        tosend[3] = 15;
		//        tosend[4] = 127;
		//        tosend[5] = 15;
		//        tosend[6] = 0; //idk
		//        tosend[7] = 10;  //just a hack fix to get it to compile
		//        p.SendRaw(0x33, tosend);
		//    }
		//}
		//public void SendChunk(Player p, Chunk c)
		//{
		//    Server.Log("Sending Chunk..");
		//                    byte[] tosend1 = new byte[2];
		//        tosend1[0] = (byte)c.x;
		//        tosend1[1] = (byte)c.z;
		//        tosend1[2] = 1;
		//        p.SendRaw(0x32, tosend1);
		//        byte[] tosend = new byte[7];
		//        tosend[0] = (byte)c.x;
		//        tosend[1] = 0;
		//        tosend[2] = (byte)c.z;
		//        tosend[3] = 15;
		//        tosend[4] = 127;
		//        tosend[5] = 15;
		//        tosend[6] = 255; //idk
		//        tosend[7] = 10;  //just a hack fix to get it to compile
		//        p.SendRaw(0x33, tosend);
		//}
		#endregion
	}
}

