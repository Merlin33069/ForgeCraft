using System;

namespace SMP
{
	public class FCGenerator
	{
		public World l;
		public FCGenerator (World l)
		{
			this.l = l;
		}
		public void FlatChunk(Chunk c)
		{
			for (int x = 0; x < 16; x++)
			{
				for (int y = 0; y < 128; y++)
				{
					for (int z = 0; z < 16; z++)
					{
						if (y == 1)
							c.PlaceBlock(x, y, z, 7);
						if (y < 64)
							c.PlaceBlock(x, y, z, 1);
						else if (y == 64)
							c.PlaceBlock(x, y, z, 2);
						else
							c.PlaceBlock(x, y, z, 0);
					}
				}
			}
			return;
		}
		//Makes a random map..
		public Chunk Rand(Chunk c, int seed)
		{
			Random rand = new Random(seed);
			int[] tempinfo = new int[32768];
			for (int x = 0; x < 16; x++)
			{
				for (int y = 0; y < 128; y++)
				{
					for (int z = 0; z < 16; z++)
					{
						if (y == 1 || y == 0)
							c.PlaceBlock(x, y, z, 7);
						else if (y < 64 && tempinfo[Chunk.PosToInt(x, y - 1, z)] == 1 && rand.Next(100) < 20)
						{
							c.PlaceBlock(x, y, z, 0);
							tempinfo[Chunk.PosToInt(x,y,z)] = 1;
						}
						else if (y == 64 && rand.Next(100) < 5)
							c.PlaceBlock(x, y, z, 1);
						else if (y == 64)
							c.PlaceBlock(x, y, z, 2);
						else if (y > 64)
						{
							if (rand.Next(265) <= 100 || tempinfo[Chunk.PosToInt(x,y - 1, z)] == 2)
							{
								if (rand.Next(249) <= 100 && tempinfo[Chunk.PosToInt(x, y, z)] != 2)
								{
									if (c.GetBlock(x - 1, y, z) != 0 && c.GetBlock(x, y - 1, z) != 0 && c.GetBlock(x, y, z - 1) != 0)
									{
										if (rand.Next(2) == 1)
											c.PlaceBlock(x, y, z, 1);
										else
											c.PlaceBlock(x, y, z, 3);
									}
									else
										c.PlaceBlock(x, y, z, 2);
									tempinfo[Chunk.PosToInt(x, y, z)] = 2;
 								}
								try
								{
									if (c.GetBlock(x - 1, y, z) == 1 || c.GetBlock(x, y, z - 1) == 1 || c.GetBlock(x, y - 1, z) == 1 || c.GetBlock(x - 1, y, z) == 2 || c.GetBlock(x, y - 1, z) == 2 || c.GetBlock(x, y, z - 1) == 2)
									{
										byte id = 2;
										if (c.GetBlock(x - 1, y, z) != 0 && c.GetBlock(x, y - 1, z) != 0 && c.GetBlock(x, y, z - 1) != 0)
											id = 1;
										//WATER
										//if (rand.Next(300) < 50 && y > 70 && (c.GetBlock(x - 1, y, z) != 1 || c.GetBlock(x, y, z - 1) != 1 || c.GetBlock(x, y - 1, z) != 1 || c.GetBlock(x - 1, y, z) != 2 || c.GetBlock(x, y - 1, z) != 2 || c.GetBlock(x, y, z - 1) != 2))
										//{
										//	c.PlaceBlock(x, y, z, 9);
										//	c.PlaceBlock(x - 1, y, z, 9);
										//}
										c.PlaceBlock(x, y, z, id);
										int i = 1;
										while (c.GetBlock(x, y, z - i) == 0 && y - i > 64 && c.mountain)
										{
											if (c.GetBlock(x - 1, y, z) != 0 && c.GetBlock(x, y - 1, z) != 0 && c.GetBlock(x, y, z - 1) != 0)
												id = 1;
											//WATER	
											//else if (rand.Next(50) < 10)
											//	id = 8;
											else
												id = 3;
											if (rand.Next(5) == 1)
												tempinfo[Chunk.PosToInt(x, y, z)] = 1;
											c.PlaceBlock(x, y - 1, z, id);
											i++;
										}
										if (!c.mountain)
										{
											if (c.GetBlock(x, y - 1, z) == 0)
												c.PlaceBlock(x, y - 1, z, 0);
										}
										tempinfo[Chunk.PosToInt(x, y, z)] = 2;
									}
									else
										c.PlaceBlock(x, y, z, 0);
								}
								catch { c.PlaceBlock(x, y, z, 0); }
							}
							else
								c.PlaceBlock(x, y, z, 0);
						}
						else if (y < 64)
							c.PlaceBlock(x, y, z, 1);
					}
				}
			}
			return c;
		}
	}
}

