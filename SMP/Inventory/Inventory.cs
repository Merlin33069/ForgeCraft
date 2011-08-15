using System;
using System.Collections.Generic;

namespace SMP
{
	public class Inventory
	{
		public Item[] items;
		public Player p;
		public int current_index;
		public Item current_item;
		public Inventory (Player pl)
		{
			p = pl;

			items = new Item[45];
			
			for (int i = 0; i < items.Length; i++)
				items[i] = Item.Nothing;

			current_item = items[36];
		}

		public void Add(short item, int slot)
		{
			Add(item, 1, 0, slot);
		}
		public void Add(short item, byte count, short meta, int slot)
		{
			Item I = new Item(item, count, meta, p.level);
			if (slot > 44 || slot < 0) return;
			items[slot] = I;
		}
		public void Remove(int slot)
		{
			items[slot] = Item.Nothing;
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
					Console.WriteLine("found " + i);
					return i;
				}
			
			for (int i = 9; i <= 35; i++)
				if (items[i].item == (short)Items.Nothing)
				{
					Console.WriteLine("found " + i);
					return i;
				}
			
			return -1;
		}
	}
}

