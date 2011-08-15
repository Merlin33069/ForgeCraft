using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMP
{
	public class Entity
	{
		public static Dictionary<int, Entity> Entities = new Dictionary<int, Entity>();

		public Chunk c
		{
			get
			{
				return Chunk.GetChunk((int)(pos[0] / 16), (int)(pos[2] / 16), level);
			}
		}
		public Chunk CurrentChunk;

		public double[] pos
		{
			get
			{
				if(isPlayer) return p.pos;
				if(isItem) return I.pos;
				//if(isAI) return ai.pos;
				//if(isObject) return obj.pos;

				return new double[3] { -1, -1, -1 };
			}
		}

		public Player p; //Only set if this entity is a player, and it referances the player it is
		public Item I;//Only set if this entity is an item
		public AI ai; //Only set if this entity is an AI
		public McObject obj;

		//MUST BE SET
		public World level;

		public bool isPlayer;
		public bool isAI;
		public bool isItem;
		public bool isObject; //Vehicles and arrows and stuffs

		public static Random random = new Random();
		public int id;

		public Entity(Player pl, World l)
		{
			p = pl;
			id = FreeId();
			isPlayer = true;
			level = l;

			UpdateChunks(false, false);

			Entities.Add(id, this);
		}
		public Entity(Item i, World l)
		{
			I = i;
			id = FreeId();
			isItem = true;
			level = l;

			if (I.OnGround)
				UpdateChunks(false, false);

			Entities.Add(id, this);
		}

		public void UpdateChunks(bool force, bool forcesend)
		{
			if (c != CurrentChunk || force)
			{
				try
				{
					if(CurrentChunk != null) CurrentChunk.Entities.Remove(this);
					c.Entities.Add(this);
					CurrentChunk = c;
				}
				catch
				{
					Server.Log("Error Updating chunk for " + isPlayer.ToString() + " " + isItem.ToString() + " " + isAI.ToString() + " " + id);
				}
				if (isPlayer && p.LoggedIn)
				{
					List<Point> templist = new List<Point>();

					int sx = CurrentChunk.point.x - p.viewdistance; //StartX
					int ex = CurrentChunk.point.x + p.viewdistance; //EndX
					int sz = CurrentChunk.point.z - p.viewdistance; //StartZ
					int ez = CurrentChunk.point.z + p.viewdistance; //EndZ
					for (int x = sx; x <= ex; x++)
					{
						for (int z = sz; z <= ez; z++)
						{
							Point po = new Point(x, z);
							templist.Add(po);
							if (p.VisibleChunks.Contains(po) && !forcesend)
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
					if (!level.chunkData.ContainsKey(new Point(x, z))) { continue; }

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
							p.VisibleEntities.Add(e.id);
							p.SendPickupSpawn(e);
							continue;
						}
						else if (e.isAI)
						{
							//TODO Spawn ai
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
