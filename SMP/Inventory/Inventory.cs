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
			items[slot].item = item;
		}
		public void Add(Items item, byte count, int slot)
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
					//THIS GENERATES AN ERROR, you cant edit an arrays length like this, you had array.count, but count didn't exist
					//items.Length = items.Length / 2;
				}
				else
				{
					temp = items.Length / 2;
					//items.Length = items.Length - temp;
				}
				return temp;
			}
			catch {
				return 0;
			}
		}
	}
}

