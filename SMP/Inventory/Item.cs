using System;

namespace SMP
{
	public class Item
	{
		public Entity e;
		public World level { get { return e.level; } set { e.level = value; } }

		public short item = -1;
		public byte count = 1;
		public short meta = 0;
		public bool OnGround; //This is used to tell the server that this item is on the ground.

		public static Item Nothing = new Item();

		public Point3 pos;
		public byte[] rot;

		private Item() { }
		public Item (short item, World l)
		{
			this.item = item;
			OnGround = false;
			e = new Entity(this, l);
		}
		public Item(Items item, World l)
		{
			this.item = (short)item;
			OnGround = false;
			e = new Entity(this, l);
		}
		public Item(short item, byte count, short meta, World l)
		{
			this.item = (short)item;
			this.meta = meta;
			this.count = count;
			OnGround = false;
			e = new Entity(this, l);
		}
		public Item(short item, byte count, short meta, World l, double[] pos, byte[] rot)
		{
			this.item = item;
			OnGround = true;
			e = new Entity(this, l);
		}
	}
}

