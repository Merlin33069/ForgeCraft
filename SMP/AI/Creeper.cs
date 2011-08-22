using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMP
{
	class Creeper : AI
	{
		public Entity e
		{
			get
			{
				return mye;
			}
		}
		public World level
		{
			get
			{
				return mylevel;
			}
		}
		public Point3 pos
		{
			get
			{
				return mypos;
			}
			set
			{
				mypos = value;
			}
		}
		public float yaw
		{
			get
			{
				return myyaw;
			}
			set
			{
				myyaw = value;
			}
		}
		public float pitch
		{
			get
			{
				return mypitch;
			}
			set
			{
				mypitch = value;
			}
		}
		public byte type
		{
			get { return mytype; }
		}
		public byte[] meta
		{
			get
			{
				return mymeta;
			}
			set
			{
				mymeta = value;
			}
		}

		Entity mye;
		World mylevel = Server.mainlevel;
		Point3 mypos = new Point3( 0, 0, 0 );
		float myyaw = 1;
		float mypitch = 1;
		byte mytype = 50;
		byte[] mymeta = new byte[4] { 0, 0, 0, 0 };

		public Creeper(Point3 pos, World level)
		{
			mye = new Entity(this, level);
			mylevel = level;
			mypos = pos;

			e.UpdateChunks(false, false);
		}

		public void Update()
		{
			//TODO move this entity, you dont need to send movement, thats done in the player class... i think
		}
	}
}
