using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMP
{
    public class ConsoleGroup : Group
    {
        public ConsoleGroup()
        {
			// temp till a better permission node system is in place
            foreach (Command c in Command.all.All())
			{
				this.PermissionList.Add(c.PermissionNode);
			}
        }
    }
}
