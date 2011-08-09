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
			items[slot] = item;
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
				if (items.count % 2 == 0)
				{
					temp = items.count / 2;
					items.count = items.count / 2;
				}
				else
				{
					temp = items.count / 2;
					items.count = items.count - temp;
				}
				return temp;
			}
			catch {
				return 0;
			}
		}
	}
}

