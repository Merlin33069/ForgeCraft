using System;
using System.Collections.Generic;

/* Notes to self (Keith)
 * load groups, load group tracks
 * try to promote along a track
 * if no track is found than try to find a group with inheritance
 * if multiple groups inherit or no inheritance is found then throw a error
 * methods (groups) to add/remove inheritance add/remove permissions, change attributes
 * methods (players) change color, canbuild, suffix/prefix, etc, etc, etc
 */

/*utility class for groups
 * mainly just group handling
 * and all the stuff
 */
namespace SMP
{
	public static class GroupUtils
	{
		#region group methods
		
		#endregion
		
		
		#region player methods
		public static bool AddSubGroup(Player p, Group g)
		{
			if(!p.SubGroups.Contains(g))
			{
				p.SubGroups.Add(g);
				return true;
			}
			else
			{
				return false;
			}
				
		}
		
		public static bool RemoveSubGroup(Player p, Group g)
		{
			if(p.SubGroups.Contains(g))
				{
					p.SubGroups.Remove(g);
					return true;
				}
			else 
				return false;
		}
		
		public static bool SetRank(Player p, Group g)
		{
			if(Group.GroupList.Contains(g))
			{
				p.Group = g;
				return true;
			}
			else 
				return false;
		}
		
		/// <summary>
		/// Tries to promote a player along a defined track, if not tries to promote based on inheritance.
		/// </summary>
		/// <param name="p">
		/// A <see cref="Player"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.Boolean"/>
		/// </returns>
		public static bool PromotePlayer(Player p)
		{
			for (int i = 0; i < p.Group.Tracks.Count; i++)
			{
				if(Group.TracksDictionary.ContainsKey(p.Group.Tracks[i]))
				{
					List<Group> tempList;
					Group.TracksDictionary.TryGetValue(p.Group.Tracks[i], out tempList);
					
					if(tempList.Count >= 1)
					{
						for(int ind = 0; i < tempList.Count; i++)
						{
							if(p.Group == tempList[ind])
							{
								if(ind + 1 > tempList.Count)
								{
									p.Group = tempList[ind + 1];
									return true;
								}
							}
						}
					}
					
				}
			}
			
			//maybe add checks to make sure there isn't multiple inheritance
			foreach(Group g in Group.GroupList)
			{
				if(g.InheritanceList.Contains(p.Group))
				{
					p.Group = g;
					return true;
				}
			}
			return false;
		}
		
		/// <summary>
		/// Tries to demote a player based on track, if not, and inheritance has only one entry uses it.
		/// </summary>
		/// <param name="p">
		/// A <see cref="Player"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.Boolean"/>
		/// </returns>
		public static bool DemotePlayer(Player p)
		{
			for (int i = 0; i < p.Group.Tracks.Count; i++)
			{
				if(Group.TracksDictionary.ContainsKey(p.Group.Tracks[i]))
				{
					List<Group> tempList;
					Group.TracksDictionary.TryGetValue(p.Group.Tracks[i], out tempList);
					
					if(tempList.Count >= 1)
					{
						for(int ind = 0; i < tempList.Count; i++)
						{
							if(p.Group == tempList[ind])
							{
								if(ind > 0)
								{
									p.Group = tempList[ind - 1];
									return true;
								}
							}
						}
					}
					
				}
			}
			
			if(p.Group.InheritanceList.Count == 1)
			{
				p.Group = p.Group.InheritanceList[0];
				return true;
			}
			return false;
		}
		
		public static void AddPlayerPermission(Player p, string perm)
		{
			if(!p.AdditionalPermissions.Contains(perm))
				p.AdditionalPermissions.Add(perm);
		}
		
		public static void DelPlayerPermission(Player p, string perm)
		{
			if(p.AdditionalPermissions.Contains(perm))
				p.AdditionalPermissions.Remove(perm);
			   
		}
		#endregion
	}
}

