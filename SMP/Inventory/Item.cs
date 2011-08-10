using System;

namespace SMP
{
	public class Item
	{
		public Entity e;
		public Items item { get { return e.itype; } set { e.itype = value; } }
		public byte count { get { return e.count; } set { e.count = value; } }
		public short meta { get { return e.meta; } set { e.meta = value; } }

		/// <summary>
		/// IF THIS IS DAMAGE, CHANGE IT TO SAVE TO META
		/// </summary>
		public int use = 0; //if this is the damage, damage should be saved as meta above

		public Item (Items item)
		{
			e = new Entity(item, 1, 0, new double[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 });
			e.I = this;
		}
	}
}

