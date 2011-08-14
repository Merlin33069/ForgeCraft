using System;
using LibNoise;

namespace SMP {

    /// <summary>
    /// Default terrain generator.
    /// </summary>
    public class GenStandard : ChunkGen {
        Random random;
        Perlin perlin;
        Perlin perlin2;
        Perlin desertSelector;
        Voronoi voronoi;
        RidgedMultifractal caves;

        public GenStandard() {
            random = new Random();

            perlin = new Perlin();
            perlin.Frequency = 0.009;
            perlin.Persistence = 0.3;
            perlin.Seed = (int)( DateTime.Now.Ticks & 0xffffffff );

            perlin2 = new Perlin();
            perlin2.Frequency = 0.007;
            perlin2.Persistence = 0.6;
            perlin2.Lacunarity = 0.1;
            perlin2.Seed = perlin.Seed - 7;

            caves = new RidgedMultifractal();
            caves.Frequency = 0.0089;
            caves.Seed = perlin2.Seed + 99;

            voronoi = new Voronoi();
            voronoi.Frequency = 0.008;
            voronoi.Seed = perlin.Seed + 2;

            desertSelector = new Perlin();
            desertSelector.Frequency = 0.01;
            desertSelector.Persistence = 0.4;
            desertSelector.Lacunarity = 0.14;
            desertSelector.Seed = perlin.Seed + 245;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="w"></param>
        /// <param name="c"></param>
        public override void Generate( World w, Chunk c ) {
            int cx = c.x << 4, cz = c.z << 4;
            int waterLevel = 64 + 15 / 2 - 4;

            for( int x = 0; x < 16; x++ ) {
                for( int z = 0; z < 16; z++ ) {

                    int v = (int) ( 64 + voronoi.GetValue( cx + x, 5, cz + z ) * 7 + ( perlin2.GetValue( cx + x, 7, cz + z ) + ( perlin.GetValue( cx + x, 10, cz + z ) ) ) * 15 );

                    // Bedrock
                    int bedrockHeight = random.Next( 1, 6 );
                    for( int y = 0; y < bedrockHeight; y++ )
                        c.PlaceBlock( x, y, z, 0x07 );

                    // Stone
                    for ( int y = bedrockHeight; y < v - 5; y++ )
                        c.PlaceBlock( x, y, z, 0x01 );

                    // Dirt
                    for( int y = v - 5; y < v; y++ )
                        c.PlaceBlock( x, y, z, (byte)( desertSelector.GetValue( cx + x, y, cz + z ) > 0.5 ? 0x0C : 0x03 ) );


                    if( v <= waterLevel ) {
                        c.PlaceBlock( x, v, z, 0x0C ); // Send
                    }
                    else {
                        c.PlaceBlock( x, v, z, (byte) ( desertSelector.GetValue( cx + x, v, cz + z ) > 0.5 ? 0x0C : 0x02 ) );
                    }
                }
            }


            for( int x = 0; x < 16; x++ ) {
                for( int z = 0; z < 16; z++ ) {
                    for( int y = 0; y < 128; y++ ) {
                        if( caves.GetValue( cx + x, y, cz + z ) > ( 128 - y ) * 0.0132 ) {
                            c.PlaceBlock( x, y, z, 0x00 );
                            if( c.SGB( x, y - 1, z ) == 0x03 )
                                c.PlaceBlock( x, y - 1, z, 0x02 );
                        }

                        if( y <= waterLevel && c.SGB( x, y, z ) == 0x00 ) {
                            c.PlaceBlock( x, y, z, 0x08 );
                        }
                    }
                }
            }
        }
    }
}
