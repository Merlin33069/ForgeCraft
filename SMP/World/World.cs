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
		public Dictionary<Point, Chunk> chunkData;
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
			chunkData = new Dictionary<Point, Chunk>();
			generator = new FCGenerator(this);

			for (int x = -3; x <= 3; x++)
			{
				for (int z = -3; z <= 3; z++)
				{
					Chunk c = new Chunk(x, z);
					generator.FlatChunk(c);
					c.RecalculateLight();
					//Server.Log(x + " " + z + " <Inserted");
					chunkData.Add(new Point(x,z), c);
				}
			}
			//for (int x = -20; x <= 20; x++)
			//{
			//	for (int z = -20; z <= 20; z++)
			//	{
			//		Chunk c = new Chunk(x, z);
			//		chunkData.Add(i, generator.Rand(c, 100));
			//		i++;
			//	}
			//}
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

		public void BlockChange(int x, int y, int z, byte type, byte meta)
		{
			Chunk c = Chunk.GetChunk((int)(x / 16), (int)(z / 16));
			c.PlaceBlock(x-(c.x*16), y, z-(c.z*16), type, 0);
			foreach (Player p in Player.players)
			{
				//TODO CHECK TO SEE IF CHUNK IS IN PLAYER RANGE
				p.SendBlockChange(x, (byte)y, z, type, meta);
			}
		}
	}
	public struct Point : IEquatable<Point>
	{
		public int x
		{
			get { return X; }
			set { X = value; }
		}
		public int y
		{
			get { return Y; }
			set { Y = value; }
		}
		public int X;
		public int Y;

		public Point(int X, int Y)
		{
			this.X = X;
			this.Y = Y;
		}

		public static bool operator ==(Point a, Point b)
		{
			if (a.x == b.x && a.y == b.y) return true;
			return false;
		}
		public static bool operator !=(Point a, Point b)
		{
			if (a.x != b.x || a.y != b.y) return true;
			return false;
		}
		public static Point operator *(Point a, int b)
		{
			try
			{
				a.x = (int)(a.x * b);
				a.y = (int)(a.y * b);
				return a;
			}
			catch
			{
				return Zero;
			}
		}
		public static Point operator /(Point a, int b)
		{
			try
			{
				a.x = (int)(a.x / b);
				a.y = (int)(a.y / b);
				return a;
			}
			catch
			{
				return Zero;
			}
		}

		public static Point Zero
		{
			get
			{
				return new Point(0, 0);
			}
		}

		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}
		public bool Equals(Point other)
		{
			if (this == other) return true;
			return false;
		}
		public override string ToString()
		{
			return base.ToString();
		}
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}

