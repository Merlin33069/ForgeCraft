using System;

//just used for testing-ish groups and to atleast semi-enable group-specific actions i.e help and other commented sections
namespace SMP
{
	public class DefaultGroup : Group
	{
		public DefaultGroup ()
		{
			GroupList.Add(this);
			Group.DefaultGroup = this;
			Name = "Default";
			IsDefaultGroup = true;
			CanBuild = true;
			Prefix = "";
			Suffix = "";
			GroupColor = Color.Gray;
			// temp till a better permission node system is in place
            foreach (Command c in Command.all.All())
			{
				this.PermissionList.Add(c.PermissionNode);
			}
		}
	}
}

