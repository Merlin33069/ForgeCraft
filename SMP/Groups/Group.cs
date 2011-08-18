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

namespace SMP
{
    public class Group
    {
        public static List<Group> GroupList = new List<Group>();
        public static Group DefaultGroup;
        public string Name;
        public bool IsDefaultGroup = false;
        public bool CanBuild = false;
        public string Prefix = "";
        public string Suffix = "";
        public string GroupColor = Color.Gray;
        public List<string> PermissionList = new List<string>();
        public List<Group> InheritanceList = new List<Group>();
        public List<string> tempInheritanceList = new List<string>();

        /// <summary>
        /// Checks if a player has permission to use a command
        /// </summary>
        /// <param name="p"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static bool CheckPermission(Player p, String perm)
        {

            if (p.AdditionalPermissions.Contains(perm))
            {
                return true;
            }
            else if (p.Group.PermissionList.Contains(perm))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Finds a group by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Group FindGroup(string name)
        {
            foreach (Group g in GroupList)
            {
                if (g.Name.ToLower() == name.ToLower())
                    return g;
            }
            return null;
        }
    }
}
