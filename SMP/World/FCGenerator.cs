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
		

