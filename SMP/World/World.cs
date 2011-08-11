using System;
using System.Collections.Generic;
namespace SMP
{
	public class World
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
		public FCGenerator generator;
		public Dictionary<Point, Chunk> chunkData;
		public Dictionary<int, Item> items_on_ground;
		public List<Point> ToGenerate = new List<Point>();
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
			items_on_ground = new Dictionary<int, Item>();
			generator = new FCGenerator(this);

			for (int x = -3; x <= 3; x++)
			{
				for (int z = -3; z <= 3; z++)
				{
					GenerateChunk(x, z);
				}
			}
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
			generator.FlatChunk(c);
			//generator.PerlinChunk(c);
			//generator.RandMap(c, seed);
			c.RecalculateLight();
			if (GeneratedChunk != null)
				GeneratedChunk(this, c, x, z);
			if (WorldGenerateChunk != null)
				WorldGenerateChunk(this, c, x, z);
			if(!chunkData.ContainsKey(new Point(x,z))) chunkData.Add(new Point(x,z), c);
		}
		public void BlockChange(int x, int y, int z, byte type, byte meta)
		{
			//TODO generate chunk if not exist and... something else but idr what
			int cx = x >> 4, cz = z >> 4;
			Chunk chunk = Chunk.GetChunk(cx, cz, this);
			chunk.PlaceBlock(x & 0xf, y, z & 0xf, type, meta);
			if (BlockChanged != null)
				BlockChanged(x, y, z, type, meta);
			foreach (Player p in Player.players)
			{
				//TODO CHECK TO SEE IF CHUNK IS IN PLAYER RANGE
				if (p.level == this)
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
}

