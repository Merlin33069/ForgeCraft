using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMP
{
	public interface AI
	{
		Entity e { get; }
		Point3 pos { get; set;}
		float yaw { get; set; }
		float pitch { get; set; }
		World level { get; }
		byte type { get; }
		byte[] meta { get; set; }

		void Update();
	}
}
