using System;
using System.Collections.Generic;

/* Notes to self (Keith)
 * load groups, load group tracks
 * try to promote along a track
 * if no track is found than try to find a group with inheritance
 * if multiple groups inherit or no inheritance is found then throw a error
 * methods to add/remove inheritance add/remove permissions, change attributes
 * methods (players) to setrank, add/remove subgroups etc, etc, etc
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
							if(tempList(ind) == p.Group)
							{
								if(ind + 1 > tempList.Count)
								{
									p.Group = tempList[ind];	
								}
							}
						}
					}
					
				}
			}
		}
		#endregion
	}
}

