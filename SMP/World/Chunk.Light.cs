using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMP
{
	/// <summary>
	/// Handles chunk lighting.
	/// </summary>
	public partial class Chunk
	{
		public void RecalculateLight(int x, int z)
		{
			sbyte curLight = 0xf; byte block;
			for (int y = Chunk.Height - 1; y >= 0; y--)
			{
				block = blocks[Chunk.PosToInt(x, y, z)];
				curLight -= LightOpacity[block];
				if (curLight <= 0) break;
				SetSkyLight(x, y, z, (byte)curLight);
			}
		}

		public void RecalculateLight()
		{
			for (int x = 0; x < Chunk.Width; x++)
			{
				for (int z = 0; z < Chunk.Depth; z++)
				{
					RecalculateLight(x, z);
				}
			}
		}

        public void SpreadLight() {
            for( int x = 0; x < Chunk.Width; x++ ) {
                for( int z = 0; z < Chunk.Depth; z++ ) {
                    for( int y = 0; y < Chunk.Height; y++ ) {
                        byte type = blocks[x << 11 | z << 7 | y];
                        sbyte opacity = LightOpacity[type];
                        if( opacity == 0x0 && GetSkyLight( x, y, z ) == 0xf ) {
                            SpreadLightInternal( x, y, z );
                        }
                    }
                }
            }
        }

        private void SpreadLightInternal( int x, int y, int z ) {
            var currLight = GetSkyLight( x, y, z );
            if ( currLight == 0x0 ) return;

            ForEveryAdjacentBlock( x, y, z, delegate( int xx, int yy, int zz ) {
                var type = blocks[ xx << 11 | zz << 7 | yy ];
                var light = GetSkyLight( xx, yy, zz );
                if( LightOpacity[type] == 0x00 ) {
                    if( light < currLight ) {
                        SetSkyLight( xx, yy, zz, (byte)(currLight - 1) );
                        SpreadLightInternal( xx, yy, zz );
                    }
                }
            } );
        }

        delegate void BlockDel( int x, int y, int z );
        private void ForEveryAdjacentBlock( int x, int y, int z, BlockDel del ) {
            if( x > 0 ) del( x - 1, y, z );
            if( x < 15 ) del( x + 1, y, z );
            if( y > 0 ) del( x, y - 1, z );
            if( y < 127 ) del( x, y + 1, z );
            if( z > 0 ) del( x, y, z - 1 );
            if( z < 15 ) del( x, y, z + 1 );
        }


		private static sbyte[] LightOpacity = new sbyte[] {
            0x0, 0xf, 0xf, 0xf, 0xf, 0xf, 0x0, 0xf, 0x3, 0x3, 0x0, 0x0, 0xf, 0xf, 0xf, 0xf,
            0xf, 0xf, 0x1, 0xf, 0x0, 0xf, 0xf, 0xf, 0xf, 0xf, 0x0, 0x0, 0x0, 0x0, 0xf, 0x0,
            0x0, 0x0, 0x0, 0xf, 0xf, 0xf, 0xf, 0xf, 0xf, 0xf, 0xf, 0xf, 0x0, 0x0, 0xf, 0xf,
            0xf, 0x0, 0xf, 0xf, 0xf, 0x0, 0xf, 0xf, 0xf, 0x0, 0x0, 0x0, 0x0, 0xf, 0x0, 0x0,
            0x0, 0x0, 0x0, 0xf, 0xf, 0x0, 0x0, 0x0, 0x0, 0x0, 0xf, 0xf, 0xf, 0x0, 0xf, 0x0,
            0xf, 0xf, 0xf, 0xf, 0x0, 0xf, 0x0, 0x0, 0x0, 0xf
        };
	}
}
