using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMP
{
	public class Entity
	{
		public static Dictionary<int, Entity> Entities = new Dictionary<int, Entity>();

		public Chunk c { get { return Chunk.GetChunk((int)(pos[0] / 16), (int)(pos[2] / 16)); } }
		public Chunk CurrentChunk;
		public Player p; //Only set if this entity is a player, and it referances the player it is

		public static Random random = new Random();
		public byte type = 0; //0 for player
		public int id;

		public byte dimension = 0;
		public double Stance = 72;
		public double[] pos = new double[3];
		public double[] oldpos = new double[3];
		public float[] rot = new float[2];
		public byte OnGround = 1;

		public Entity(double[] ipos, float[] irot, Player pl)
		{
			id = FreeId();
			pos = ipos;
			rot = irot;
			p = pl;
			c.Entities.Add(this);
			CurrentChunk = c;
		}
		public Entity(byte itype, double[] ipos, float[] irot)
		{
			id = FreeId();
			type = itype;
			pos = ipos;
			rot = irot;
			c.Entities.Add(this);
			CurrentChunk = c;
		}

		public void UpdateChunk()
		{
			if (c != CurrentChunk)
			{
				if (c == null) { p.Kick("You dumbass >_>"); return; }
				try
				{
					CurrentChunk.Entities.Remove(this);
					c.Entities.Add(this);
					CurrentChunk = c;
				}
				catch
				{
					p.Kick("You dumbass >_>");
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
