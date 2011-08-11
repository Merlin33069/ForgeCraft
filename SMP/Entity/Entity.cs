using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMP
{
	public class Entity
	{
		public static Dictionary<int, Entity> Entities = new Dictionary<int, Entity>();

		public Chunk c { get { return Chunk.GetChunk((int)(pos[0] / 16), (int)(pos[2] / 16), p.level); } }
		public Chunk CurrentChunk;

		public Player p; //Only set if this entity is a player, and it referances the player it is
		//public Mob m; //Only set if this entity is a player

		public Item I;//Only set if this entity is an item
		public Items itype;//Item Type
		public short meta;//Items damage OR Metadata
		public byte count;

		public bool isPlayer;
		public bool isMob;
		public bool isItem;

		public static Random random = new Random();
		public byte type = 0; //0 for player
		public int id;

		public byte dimension = 0;
		public double Stance = 72;
		public double[] pos = new double[3];
		public double[] oldpos = new double[3];
		public float[] rot = new float[2];
		public byte[] irot = new byte[3]; //Used for ITEM Rotation Pitch and Roll
		public byte OnGround = 1;

		public Entity(double[] ipos, float[] irot, Player pl)
		{
			isPlayer = true;
			id = FreeId();
			pos = ipos;
			rot = irot;
			p = pl;
			c.Entities.Add(this);
			CurrentChunk = c;
			Entities.Add(id, this);
		}
		public Entity(byte itype, double[] ipos, float[] irot)
		{
			isMob = true;
			id = FreeId();
			type = itype;
			pos = ipos;
			rot = irot;
			c.Entities.Add(this);
			CurrentChunk = c;
			Entities.Add(id, this);
		}
		public Entity(Items iitem, byte icount, short imeta, double[] ipos, byte[] irot2)
		{
			isItem = true;
			id = FreeId();
			itype = iitem;
			count = icount;
			meta = imeta;
			pos = ipos;
			irot = irot2;
			if (pos[0] != 0)
			{
				c.Entities.Add(this);
				CurrentChunk = c;
			}
			Entities.Add(id, this);
		}

		public void UpdateChunks(bool force, bool forcesend)
		{
			if (c != CurrentChunk || force)
			{
				//if (c == null) { p.Kick("You dumbass >_>"); return; }
				try
				{
					CurrentChunk.Entities.Remove(this);
					c.Entities.Add(this);
					CurrentChunk = c;
				}
				catch
				{
					p.Kick("Error Updating Chunk.");
				}
				if (isPlayer || force)
				{
					List<Point> templist = new List<Point>();

					int sx = CurrentChunk.point.x - 3; //StartX
					int ex = CurrentChunk.point.x + 3; //EndX
					int sz = CurrentChunk.point.z - 3; //StartZ
					int ez = CurrentChunk.point.z + 3; //EndZ
					for (int x = sx; x <= ex; x++)
					{
						for (int z = sz; z <= ez; z++)
						{
							Point po = new Point(x, z);
							templist.Add(po);
							if (p.VisibleChunks.Contains(po) || forcesend)
							{
								continue; //Continue if the player already has this chunk
							}
							if (!p.level.chunkData.ContainsKey(po))
							{
								p.level.GenerateChunk(po.x, po.z);
							}
							p.SendChunk(p.level.chunkData[po]);
						}
					}

					//UNLOAD CHUNKS THE PLAYER CANNOT SEE
					foreach (Point point in p.VisibleChunks.ToArray())
					{
						if (!templist.Contains(point))
						{
							p.SendPreChunk(p.level.chunkData[point], 0);
							p.VisibleChunks.Remove(point);
						}
					}
				}
			}
		}
		public void UpdateEntities()
		{
			List<int> tempelist = new List<int>();

			int sx = CurrentChunk.point.x - 3; //StartX
			int ex = CurrentChunk.point.x + 3; //EndX
			int sz = CurrentChunk.point.z - 3; //StartZ
			int ez = CurrentChunk.point.z + 3; //EndZ
			for (int x = sx; x <= ex; x++)
			{
				for (int z = sz; z <= ez; z++)
				{
					if (!p.level.chunkData.ContainsKey(new Point(x, z))) { continue; }  //stuck a p. infront to compile
					if (!p.level.chunkData.ContainsKey(new Point(x, z))) { continue; }

					foreach (Entity e in p.level.chunkData[new Point(x, z)].Entities)
					{
						tempelist.Add(e.id);
						if (p.VisibleEntities.Contains(e.id))
						{
							continue; //Continue if the player already has this entity
						}
						if (e.isPlayer)
						{
							if (e.p == p) continue;
							p.VisibleEntities.Add(e.id);
							p.SendNamedEntitySpawn(e.p);
							if (!e.p.VisibleEntities.Contains(id))
							{
								e.p.VisibleEntities.Add(id);
								e.p.SendNamedEntitySpawn(p);
							}
							continue;
						}
						else if (e.isItem)
						{
							p.SendPickupSpawn(e);
							continue;
						}
						else if (e.isMob)
						{
							//TODO Spawn mob
							continue;
						}
					}
				}
			}
			foreach (int i in p.VisibleEntities.ToArray())
			{
				if (!tempelist.Contains(i))
				{
					p.VisibleEntities.Remove(i);
					p.SendDespawn(i);
				}
			}
		}
		//public void UpdateEntitiesAndChunks()
		//{
		//	if (c != CurrentChunk) UpdateChunks();
		//	if (isPlayer) UpdateEntities();
		//}
		public static int FreeId()
		{
			int i = 0;
			do {
			i = random.Next();
			} while(Entities.ContainsKey(i));
			return i;
		}
	}
}
