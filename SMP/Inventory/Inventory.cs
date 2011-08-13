using System;
using System.Collections.Generic;

namespace SMP
{
	public class Inventory
	{
		public Item[] items;
		public int current_index;
		public Item current_item;
		public Inventory () {
			items = new Item[44];
			//Prevent null
			for (int i = 0; i < items.Length; i++)
				items[i] = new Item(Items.Nothing);
			current_item = items[36];
		}
		public void Add(Items item, int slot)
		{
			if (slot > 44 || slot < 0)
				return;
			Item temp = new Item(item);
			temp.count = 1;
			items[slot] = temp;
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
					items[slot].count = (byte)(items[slot].count / 2);
				}
				else
				{
					temp = items.Length / 2;
					items[slot].count = (byte)(items[slot].count - temp);
				}
				return temp;
			}
			catch {
				return 0;
			}
		}
		public void SetSlot(short slot, byte window)
		{
			byte[] tosend;
			if (items[slot].item != Items.Nothing)
				tosend = new byte[9];
			else
				tosend = new byte[6];
 		}
		
		//the idea is there just doesn't quite work any fixes would be great!!
		public int FindEmptySlot()
		{			
			for (int i = 36; i <= 44; i++)
			{
				if (items[i].item == Items.Nothing)
				{
					Server.Log("slot: " + i);
					return i;	
				}
			}
			
			for (int i = 9; i <= 35; i++)
			{
				if (items[i].item == Items.Nothing)
				{
					return i;	
				}
			}
			
			return 43;
		}
	}
}

