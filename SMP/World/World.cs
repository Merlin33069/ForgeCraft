using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SMP
{
	public partial class World
	{
		public static List<World> worlds = new List<World>();
		public double SpawnX;
		public double SpawnY;
		public double SpawnZ;
		public float SpawnYaw;
		public float SpawnPitch;
		public string Map_Name;
		public string name;
		public int seed;
		public long time;
		public System.Timers.Timer timeupdate = new System.Timers.Timer(1000);
		public GenStandard generator;
		public Dictionary<Point, Chunk> chunkData;
		public List<Point> ToGenerate = new List<Point>();
        public bool Raining = false;
		#region Custom Command / Plugin Events
		//Custom Command / Plugin Events -------------------------------------------------------------------
		public delegate void OnWorldLoad(World w); //TODO When loading levels is finished, add this event
		public static event OnWorldLoad WorldLoad;
		public delegate void OnWorldSave(World w);
		public static event OnWorldLoad OnSave;
		public event OnWorldLoad Save;
		public delegate void OnGenerateChunk(World w, Chunk c, int x, int z);
		public static event OnGenerateChunk WorldGenerateChunk;
		public event OnGenerateChunk GeneratedChunk;
		public delegate void OnBlockChange(int x, int y, int z, byte type, byte meta);
		public event OnBlockChange BlockChanged;
		//Custom Command / Plugin Events -------------------------------------------------------------------
		#endregion
		
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
		public World (double spawnx, double spawny, double spawnz, string name, int seed)
		{
			chunkData = new Dictionary<Point, Chunk>();
			generator = new GenStandard();
			Server.Log("Generating...");

			/*for (int x = -3; x <= 3; x++)
			{
			    for (int z = -3; z <= 3; z++)
			    {
			        GenerateChunk(x, z);
			    }
			    Server.Log(x + " Row Generated.");
			}*/

			Parallel.For(-3, 3, delegate(int x)
			{
				Parallel.For(-3, 3, delegate(int z)
				{
					GenerateChunk(x, z);
				});
				Console.WriteLine(x + " Row Generated.");
                
			});
            Console.WriteLine("Look distance = 3");
			this.SpawnX = spawnx; this.SpawnY = spawny; this.SpawnZ = spawnz;
			timeupdate.Elapsed += delegate {
				time += 20;
				if (time > 24000)
					time = 0;
				Player.players.ForEach(delegate(Player p) { if (p.level == this) p.SendTime(); });
			};
			timeupdate.Start();
			this.name = name;
		}
       
		public static World Find(string name)
		{
			World tempLevel = null; bool returnNull = false;

            foreach (World world in worlds)
            {
                if (world.name.ToLower() == name) return world;
                if (world.name.ToLower().IndexOf(name.ToLower()) != -1)
                {
                    if (tempLevel == null) tempLevel = world;
                    else returnNull = true;
                }
            }

            if (returnNull == true) return null;
            if (tempLevel != null) return tempLevel;
            return null;
		}
		public static World LoadLVL(string filename)
		{
			//TODO Load files
			//if (WorldLoad != null)
			//	WorldLoad(this);
			return null;
		}
		public void SaveLVL(World  w)
		{
			if (Save != null)
				Save(this);
			if (OnSave != null)
				OnSave(this);
			//TODO Save files
		}

		public void GenerateChunk(int x, int z)
		{
			Chunk c = new Chunk(x, z);
			generator.Generate(this, c);
			//generator.PerlinChunk(c);
			//generator.RandMap(c, seed);
			c.RecalculateLight();
            c.SpreadLight();
			if (GeneratedChunk != null)
				GeneratedChunk(this, c, x, z);
			if (WorldGenerateChunk != null)
				WorldGenerateChunk(this, c, x, z);
			if(!chunkData.ContainsKey(new Point(x,z))) chunkData.Add(new Point(x,z), c);
		}
		public void BlockChange(int x, int y, int z, byte type, byte meta)
		{
			int cx = x >> 4, cz = z >> 4;
			Chunk chunk = Chunk.GetChunk(cx, cz, this);
			chunk.PlaceBlock(x & 0xf, y, z & 0xf, type, meta);
			if (BlockChanged != null)
				BlockChanged(x, y, z, type, meta);
			foreach (Player p in Player.players.ToArray())
			{
				if (!p.VisibleChunks.Contains(chunk.point)) continue;
				if (p.level == this)
					p.SendBlockChange(x, (byte)y, z, type, meta);
			}
		}
		public byte GetBlock(int x, int y, int z)
		{
			int cx = x >> 4, cz = z >> 4;
			Chunk chunk = Chunk.GetChunk(cx, cz, this);
			return chunk.SGB(x & 0xf, y, z & 0xf);
		}
		public byte GetMeta(int x, int y, int z)
		{
			int cx = x >> 4, cz = z >> 4;
			Chunk chunk = Chunk.GetChunk(cx, cz, this);
			return chunk.GetMetaData(x & 0xf, y, z & 0xf);
		}
	}
	public struct Point : IEquatable<Point>
	{
		public int x
		{
			get { return X; }
			set { X = value; }
		}
		public int z
		{
			get { return Z; }
			set { Z = value; }
		}
		public int X;
		public int Z;

		public Point(int X, int Y)
		{
			this.X = X;
			this.Z = Y;
		}

		public static bool operator ==(Point a, Point b)
		{
			if (a.x == b.x && a.z == b.z) return true;
			return false;
		}
		public static bool operator !=(Point a, Point b)
		{
			if (a.x != b.x || a.z != b.z) return true;
			return false;
		}
		public static Point operator *(Point a, int b)
		{
			try
			{
				a.x = (int)(a.x * b);
				a.z = (int)(a.z * b);
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
				a.z = (int)(a.z / b);
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
	public struct Point3 : IEquatable<Point3>
	{
		public double x
		{
			get { return X; }
			set { X = value; }
		}
		public double y
		{
			get { return Y; }
			set { Y = value; }
		}
		public double z
		{
			get { return Z; }
			set { Z = value; }
		}
		public double X;
		public double Y;
		public double Z;

		public Point3(double X, double Y, double Z)
		{
			this.X = X;
			this.Y = Y;
			this.Z = Z;
		}
		public Point3(int[] iar)
		{
			X = iar[0];
			Y = iar[1];
			Z = iar[2];
		}
		public Point3(double[] iar)
		{
			X = iar[0];
			Y = iar[1];
			Z = iar[2];
		}

		public static bool operator ==(Point3 a, Point3 b)
		{
			if (a.X == b.X && a.Y == b.Y && a.Z == b.Z) return true;
			return false;
		}
		public static bool operator ==(Point3 a, int[] b)
		{
			if (RD(a.X) == b[0] && RD(a.Y) == b[1] && RD(a.Z) == b[2]) return true;
			return false;
		}
		public static bool operator ==(Point3 a, double[] b)
		{
			if (a.X == b[0] && a.Y == b[1] && a.Z == b[2]) return true;
			return false;
		}
		public static bool operator !=(Point3 a, Point3 b)
		{
			if (a.x != b.x || a.y != b.y || a.z != b.z) return true;
			return false;
		}
		public static bool operator !=(Point3 a, int[] b)
		{
			if (RD(a.x) != b[0] || RD(a.y) != b[1] || RD(a.z) != b[2]) return true;
			return false;
		}
		public static bool operator !=(Point3 a, double[] b)
		{
			if (a.x != b[0] || a.y != b[1] || a.z != b[2]) return true;
			return false;
		}
		public static Point3 operator *(Point3 a, int b)
		{
			try
			{
				a.x = (int)(a.x * b);
				a.y = (int)(a.y * b);
				a.z = (int)(a.z * b);
				return a;
			}
			catch
			{
				return Zero;
			}
		}
		public static Point3 operator /(Point3 a, int b)
		{
			try
			{
				a.x = (int)(a.x / b);
				a.y = (int)(a.y / b);
				a.z = (int)(a.z / b);
				return a;
			}
			catch
			{
				return Zero;
			}
		}
		public static Point3 operator -(Point3 a, int b)
		{
			a.x = a.x - b;
			a.y = a.y - b;
			a.z = a.z - b;
			return a;
		}
		public static Point3 operator -(Point3 a, Point3 b)
		{
			a.x = a.x - b.x;
			a.y = a.y - b.y;
			a.z = a.z - b.z;
			return a;
		}
		public static Point3 operator +(Point3 a, int b)
		{
			a.x = a.x + b;
			a.y = a.y + b;
			a.z = a.z + b;
			return a;
		}
		public static Point3 operator +(Point3 a, Point3 b)
		{
			a.x = a.x + b.x;
			a.y = a.y + b.y;
			a.z = a.z + b.z;
			return a;
		}

		public Point3 diff(Point3 a)
		{
			a.x = Math.Abs(Math.Max(a.X, X) - Math.Min(a.X, X));
			a.y = Math.Abs(Math.Max(a.Y, Y) - Math.Min(a.Y, Y));
			a.x = Math.Abs(Math.Max(a.Z, Z) - Math.Min(a.Z, Z));
			return a;
		}
		public double mdiff(Point3 a)
		{
			a.x = Math.Abs(a.X - X);
			a.y = Math.Abs(a.Y - Y);
			a.x = Math.Abs(a.Z - Z);
			return Math.Max(Math.Max(a.x, a.y), a.z);
		}

		static public implicit operator Point3(int[] value)
		{
			return new Point3(value);
		}
		static public implicit operator Point3(double[] value)
		{
			return new Point3(value);
		}
		static public explicit operator int[](Point3 po)
		{
			return new int[3] { (int)RD(po.x), (int)RD(po.y), (int)RD(po.z) };
		}
		static public explicit operator double[](Point3 po)
		{
			return new double[3] { po.x, po.y, po.z };
		}

		public static Point3 Zero { get { return new Point3(0, 0, 0); } }

		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}
		public bool Equals(Point3 other)
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

		public Point3 RD()
		{
			return new Point3(RD(x), RD(y), RD(z));
		}
		static double RD(double valueToRound)
		{
			if (valueToRound < 0)
			{
				return Math.Floor(valueToRound);
			}
			else
			{
				return Math.Floor(valueToRound);
			}
		}
       
        
	}
}

