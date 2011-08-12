using System;
using LibNoise;

namespace SMP {

    /// <summary>
    /// Default terrain generator.
    /// </summary>
    public class GenStandard : ChunkGen {
        Random random;
        Perlin perlin;

        public GenStandard() {
            random = new Random();

            perlin = new Perlin();
            perlin.Frequency = 0.009;
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
                    
                    int v = (int)( 64 + perlin.GetValue( cx + x, 10, cz + z ) * 15 );

                    // Bedrock
                    int bedrockHeight = random.Next( 1, 6 );
                    for( int y = 0; y < bedrockHeight; y++ )
                        c.PlaceBlock( x, y, z, 0x07 );

                    // Stone
                    for ( int y = bedrockHeight; y < v - 5; y++ )
                        c.PlaceBlock( x, y, z, 0x01 );

                    // Dirt
                    for( int y = v - 5; y < v; y++ )
                        c.PlaceBlock( x, y, z, 0x03 );


                    if( v <= waterLevel ) {
                        c.PlaceBlock( x, v, z, 0x0C ); // Send
                        for( int y = v + 1; y <= waterLevel; y++ )
                            c.PlaceBlock( x, y, z, 0x08 );
                    }
                    else {
                        c.PlaceBlock( x, v, z, 0x02 );
                    }
                }
            }
        }
    }
}
