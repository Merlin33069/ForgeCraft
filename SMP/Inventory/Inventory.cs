using System;
using System.Collections.Generic;

namespace SMP
{
	public class Inventory
	{
		public Item[] items;
		public Player p;
		public Item current_item;
		public int current_index;

		bool ActiveWindow;
		Windows window; //The type of window that is currently open

		public Inventory (Player pl)
		{
			p = pl;

			items = new Item[45];
			
			for (int i = 0; i < items.Length; i++)
				items[i] = Item.Nothing;

			current_item = items[36];
			current_index = 36;
		}

		public void Add(short item, int slot)
		{
			Add(item, 1, 0, slot);
		}
		public void Add(Item item)
		{
			Add(item.item, item.count, item.meta);
		}
		public void Add(short item, byte count, short meta)
		{
			//Console.WriteLine("add1");
			if (ActiveWindow)
			{
				//TODO pass action to the window
				//SUDO window.Add(item, count, meta, slot);
				return;
			}
			byte stackable = isStackable(item);
			byte c = count;
			//Console.WriteLine("add2");
			for (int i = 36; i < 45; i++)
			{
				if (c == 0) return;
				if (items[i].item == item)
					if (items[i].count < stackable)
					{
						items[i].count += c;
						c = 0;
						if (items[i].count > stackable)
						{
							c = (byte)(items[i].count - stackable);
							items[i].count -= c;
						}
						p.SendItem((short)i, item, items[i].count, meta);
					}
			}
			//Console.WriteLine("add3");
			for (int i = 9; i <= 35; i++)
			{
				if (c == 0) return;
				if (items[i].item == item)
					if (items[i].count < stackable)
					{
						items[i].count += c;
						c = 0;
						if (items[i].count > stackable)
						{
							c = (byte)(items[i].count - stackable);
							items[i].count -= c;
						}
						p.SendItem((short)i, item, items[i].count, meta);
					}
			}
			//Console.WriteLine("add4");
			Add(item, c, meta, FindEmptySlot());
		}
		public void Add(short item, byte count, short meta, int slot)
		{
			Console.WriteLine("d1");
			if (count == 0) return;
			if (ActiveWindow)
			{
				//TODO pass action to the window
				//SUDO window.Add(item, count, meta, slot);
				return;
			}

			Item I = new Item(item, count, meta, p.level);
			if (slot > 44 || slot < 0) return;
			items[slot] = I;

			p.SendItem((short)slot, item, count, meta);
		}

		public void Remove(int slot)
		{
			items[slot] = Item.Nothing;
		}
		public void Remove(int slot, byte count)
		{
			if (count >= items[slot].count)
			{
				items[slot] = Item.Nothing;
				p.SendItem((short)slot, -1, 0, 0);
				return;
			}

			items[slot].count--;
			p.SendItem((short)slot, items[slot].item, items[slot].count, items[slot].meta);
		}

		public int Right_Click(int slot)
		{
			try
			{
				int temp;
				if (items.Length % 2 == 0)
				{
					temp = items.Length / 2;
					items[slot].count = (byte)(items[slot].count / 2);
				}
				else
				{
					temp = items.Length / 2;
					items[slot].count = (byte)(items[slot].count - temp);
				}
				return temp;
			}
			catch
			{
				return 0;
			}
		}
		
		public int FindEmptySlot()
		{			
			for (int i = 36; i < 45; i++)
				if (items[i].item == (short)Items.Nothing)
				{
					return i;
				}
			
			for (int i = 9; i <= 35; i++)
				if (items[i].item == (short)Items.Nothing)
				{
					return i;
				}
			
			return -1;
		}
		public byte isStackable(short id)
		{
			//TODO
			return 64;
		}
	}
}

