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
		public Chunk FlatChunk(Chunk c)
		{
			for (int x = 0; x < 16; x++)
			{
				for (int y = 0; y < 128; y++)
				{
					for (int z = 0; z < 16; z++)
					{
						if (y < 64)
							c.PlaceBlock(x, y, z, DataValues.Blocks.Stone);
						else if (y == 64)
							c.PlaceBlock(x, y, z, DataValues.Blocks.Dirt);
						else
							c.PlaceBlock(x, y, z, DataValues.Blocks.Air);
					}
				}
			}
		}
	}
}

