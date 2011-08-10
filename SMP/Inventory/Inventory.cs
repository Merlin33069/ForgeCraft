using System;
using System.Collections.Generic;

namespace SMP
{
	public class Inventory
	{
		public Item[] items;
		public int current_index;
		public Item current_item { get; set; }
		public Inventory () {
			items = new Item[44];
		}
		public void Add(Items item, int slot)
		{
			if (slot > 44 || slot < 0)
				return;
			Item temp = new Item(item);
			temp.count = 1;
			items[slot] = temp;
		}
		public void Add(Items item, int count, int slot)
		{	
			Item temp = new Item(item);
			temp.count = count;
			if (slot > 44 || slot < 0)
				return;
			items[slot] = temp;
		}
		public int Right_Click(int slot)
		{
			try
			{
				int temp;
				if (items.Length % 2 == 0)
				{
					temp = items.Length / 2;
					items[slot].count = items[slot].count / 2;
				}
				else
				{
					temp = items.Length / 2;
					items[slot].count = items[slot].count - temp;
				}
				return temp;
			}
			catch {
				return 0;
			}
		}
		public void SetSlot(int slot, byte window)
		{
			byte[] tosend;
			if (items[slot].item != Items.Nothing)
				tosend = new byte[9];
			else
				tosend = new byte[6];
 		}
	}
}

