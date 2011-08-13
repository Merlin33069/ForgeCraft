//using System;

//namespace SMP
//{
//    public class FCGenerator
//    {
//        public World l;
//        PerlinNoise perlinNoise = new PerlinNoise(99);

//        public FCGenerator(World l)
//        {
//            this.l = l;
//        }
//        public void FlatChunk(Chunk c)
//        {
//            for (int x = 0; x < 16; x++)
//            {
//                for (int y = 0; y < 128; y++)
//                {
//                    for (int z = 0; z < 16; z++)
//                    {
//                        if (y == 1)
//                            c.PlaceBlock(x, y, z, 7);
//                        if (y < 64)
//                            c.PlaceBlock(x, y, z, 1);
//                        else if (y == 64)
//                            c.PlaceBlock(x, y, z, 2);
//                        else
//                            c.PlaceBlock(x, y, z, 0);
//                    }
//                }
//            }
//            return;
//        }
//        public void PerlinChunk(Chunk c)
//        {
//            for (int x = 0; x < 16; x++)
//            {
//                for (int z = 0; z < 16; z++)
//                {
//                    double v = //perlinNoise.Noise(x * 16, z * 16, 0.1);
//                        perlinNoise.Noise(x * 16, z * 16, 1);
//                        //perlinNoise.Noise(x * 16, z * 16, -0.065);
//                        //(perlinNoise.Noise(x * 16, z * 16, -0.5) + 1) / 2 * 0.7 +
//                        //(perlinNoise.Noise(x * 16, z * 16, 0) + 1) / 2 * 0.2 +
//                        //(perlinNoise.Noise(x * 16, z * 16, +0.5) + 1) / 2 * 0.1;

//                    v = Math.Min(1, Math.Max(0, v));
//                    byte y = (byte)(v * 128);
//                    c.PlaceBlock(x, y, z, 2);
//                }
//            }
//            return;
//        }
		
//        public void RandMap(Chunk c, int seed)
//        {
//            Random rand = new Random(seed);
//            int randominfo1 = rand.Next(150);
//            int randominfo2 = rand.Next(100);
//            byte[] tempinfo = new byte[16 * 128 * 16];
//            while (randominfo2 >= randominfo1)
//                randominfo2 = rand.Next(100);
//            for (int x = 0; x < 16; x++)
//            {
//                for (int y = 0; y < 128; y++)
//                {
//                    for (int z = 0; z < 16; z++)
//                    {
//                        if (y < 64)
//                        {
//                            /*if (rand.Next(200) < 50)
//                            {
//                                if ((c.GetBlock(x - 1, y, z) == 0 || c.GetBlock(x, y - 1, z) == 0 || c.GetBlock(x, y, z - 1) == 0) || rand.Next(randominfo1) < randominfo2)
//                                {
//                                    c.PlaceBlock(x, y, z, 0);
//                                    continue;
//                                }
//                                else if (rand.Next(50) < 5)
//                                {
//                                    double v = (perlinNoise.Noise(x * 16, z * 16, -0.5) + 1) / 2 * 0.7 + (perlinNoise.Noise(x * 16, z * 16, 0) + 1) / 2 * 0.2 +  (perlinNoise.Noise(x * 16, z * 16, +0.5) + 1) / 2 * 0.1;
//                                    v = Math.Min(1, Math.Max(0, v));
//                                    int yy = (int)v * 128;
//                                    c.PlaceBlock(x, yy, z, 0);
//                                }
//                                else
//                                    c.PlaceBlock(x, y, z, 1);
//                            }
//                            else*/
//                                c.PlaceBlock(x, y, z, 1);
//                        }
//                        else if (y == 64)
//                            c.PlaceBlock(x, y, z, 2);
//                        else if (y > 64)
//                        {
//                            try
//                            {
//                            //is a mountain already there?
//                                if ((tempinfo[Chunk.PosToInt(x - 1, y, z)] == 1 || tempinfo[Chunk.PosToInt(x, y - 1, z)] == 1 || tempinfo[Chunk.PosToInt(x, y, z - 1)] == 1) && rand.Next(250) < 100)
//                                {
//                                    int i = 1;
//                                    while (c.GetBlock(x, y - i, z) == 0)
//                                    {
//                                        if (c.GetBlock(x - 1, y, z) != 0 && c.GetBlock(x, y - 1, z) != 0 && c.GetBlock(x, y, z - 1) != 0 && rand.Next(2) == 0)
//                                            c.PlaceBlock(x, y - i, z, 1);
//                                        else
//                                            c.PlaceBlock(x, y - i, z, 2);
//                                        i++;
//                                    }
//                                    tempinfo[Chunk.PosToInt(x, y, z)] = (byte)rand.Next(2);
//                                    continue;
//                                }
//                            //Start one
//                            else if (c.GetBlock(x, y - 1, z) != 0 && tempinfo[Chunk.PosToInt(x, y - 1, z)] != 1)
//                            {
//                                if (c.GetBlock(x - 1, y, z) != 0 && c.GetBlock(x, y - 1, z) != 0 && c.GetBlock(x, y, z - 1) != 0 && rand.Next(2) == 0)
//                                    c.PlaceBlock(x, y, z, 1);
//                                else
//                                    c.PlaceBlock(x, y, z, 2);
//                                tempinfo[Chunk.PosToInt(x, y, z)] = 1;
//                            }
//                            else
//                                c.PlaceBlock(x, y, z, 0);
//                            }
//                            catch { c.PlaceBlock(x, y, z, 0); }
//                        }
//                        else
//                            c.PlaceBlock(x, y, z, 0);
//                    }
//                }
//            }
//            return;
//        }
//    }
//}

