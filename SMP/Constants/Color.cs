using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*  FILE TODO:
 *  - add configurable colours for announcement, defaultcolor, etc  
 */
namespace SMP
{
    public static class Color
    {
        public const string Signal = "\x00A7"; // section symbol §

        public const string Black = Signal + "0";
        public const string DarkBlue = Signal + "1";
        public const string DarkGreen = Signal + "2";
        public const string DarkTeal = Signal + "3";
        public const string DarkRed = Signal + "4";
        public const string Purple = Signal + "5";
        public const string DarkYellow = Signal + "6";
        public const string Gray = Signal + "7";
        public const string DarkGray = Signal + "8";
        public const string Blue = Signal + "9";
        public const string Green = Signal + "a";
        public const string Teal = Signal + "b";
        public const string Red = Signal + "c";
        public const string Pink = Signal + "d";
        public const string Yellow = Signal + "e";
        public const string White = Signal + "f";

        public static string Announce = Yellow;
        public static string PrivateMsg = Purple;
        public static string CommandResult = Teal;
        public static string CommandError = DarkRed;
        public static string ServerDefaultColor = White;

        public static string ParseColors(string text)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == '§' && text.Length > i + 1 && ((text[i + 1] >= 'a' && text[i + 1] <= 'f') ||
                    (text[i + 1] >= 'A' && text[i + 1] <= 'F') || (text[i + 1] >= '0' && text[i + 1] <= '9')))
                {
                    i++;
                }
                else
                {
                    sb.Append(text[i]);
                }
            }
            return sb.ToString();
        }

        public static bool IsColorValid(char ch)
        {
            if ((ch >= '0' && ch <= '9') || (ch >= 'a' && ch <= 'f') || (ch >= 'A' && ch <= 'F'))
                return true;
            
            return false;
        }
    }
}
