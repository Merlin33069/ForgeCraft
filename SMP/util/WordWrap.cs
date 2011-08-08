using System;
using System.Text;
using System.Collections.Generic;

namespace SMP
{

    /// <summary>
    /// Contains methods that wrap text.
    /// </summary>
    public static class WordWrap
    {
        public static string[] GetWrappedText(string text, WrapMethod method)
        {
            List<string> lines;

            switch (method)
            {
                case WrapMethod.None:
                    lines = new List<string>(new string[] { text });
                    break;
                case WrapMethod.Default:
                    lines = GetDefaultText(text);
                    break;
                case WrapMethod.Chat:
                    lines = GetChatText(text);
                    break;
                case WrapMethod.Spaced:
                    lines = GetSpacedText(text);
                    break;
                case WrapMethod.FixedSpace:
                    lines = GetFixedSpaceText(text);
                    break;
                default:
                    throw new ArgumentException( "Unknown wrapping method" );
            }

            string[] r = lines.ToArray();
            lines.Clear();
            lines = null;
            return r;
        }

        private static List<string> GetSpacedText(string text)
        {
            string startCol = string.Empty;
            if (text[0] == '§' && text.Length > 1)
            {
                startCol = string.Concat("§", text[1].ToString());
                text = text.Remove(0, 2);
            }

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == ' ')
                    sb.Append( ' ' );
                else
                    break;
            }
            text += startCol;


            List<string> lines = Wrap(text, 64 - sb.Length);
            WrapColors(lines);

            for (int i = 1; i < lines.Count; i++)
            {
                string col;
                if (lines[i][0] == '§' && IsValidColorCode(lines[i][1]))
                {
                    col = string.Concat("§", lines[i][1]);
                    lines[i] = lines[i].Remove(0, 2).Trim();
                }
                else
                {
                    col = Color.White;
                }
                lines[i] = new StringBuilder(col).Append(sb.ToString()).Append(lines[i]).ToString();
            }
            return lines;
        }

        private static List<string> GetFixedSpaceText(string text)
        {
            List<string> lines = Wrap(text, 58);
            WrapColors(lines);

            for (int i = 1; i < lines.Count; i++)
            {
                string col;
                if (lines[i][0] == '§' && IsValidColorCode(lines[i][1]))
                {
                    col = string.Concat("§", lines[i][1]);
                    lines[i] = lines[i].Remove(0, 2).Trim();
                }
                else
                {
                    col = Color.White;
                }
                lines[i] = new StringBuilder(col).Append("    ").Append(lines[i]).ToString();
            }
            return lines;
        }

        private static List<string> GetChatText(string text)
        {
            List<string> lines = Wrap(text, 59);
            WrapColors(lines);

            for (int i = 1; i < lines.Count; i++)
            {
                lines[i] = new StringBuilder(Color.White).Append(" > ").Append(lines[i]).ToString();
            }
            return lines;
        }

        private static List<string> GetDefaultText(string text)
        {
            List<string> lines = Wrap(text);
            WrapColors(lines);
            return lines;
        }


        /// <summary>
        /// Wraps the specified text into
        /// lines of the specified length.
        /// </summary>
        private static List<string> Wrap(string text, int limit = 64)
        {
            int start = 0, end;
            var lines = new List<string>();
            text = System.Text.RegularExpressions.Regex.Replace(text, @"\s", " ").Trim();

            while ((end = start + limit) < text.Length)
            {
                while (text[end] != ' ' && end > start)
                    end -= 1;

                if (end == start)
                    end = start + limit;

                lines.Add(text.Substring(start, end - start).Trim());
                start = end + 1;
            }

            if (start < text.Length)
                lines.Add(text.Substring(start).Trim());

            return lines;
        }

        private static void WrapColors(List<string> lines)
        {
            char lc = 'f';
            for (int i = 0; i < lines.Count - 1; i++)
            {
                for (int c = lines[i].Length - 1; c >= 1; c--)
                {
                    if (lines[i][c - 1] == '§' && IsValidColorCode(lines[i][c]))
                    {
                        lc = lines[i][c];
                        break;
                    }
                }
                if (lines[i + 1].Length > 0 && lines[i + 1][0] != '§')
                {
                    lines[i + 1] = new StringBuilder("§").Append(lc).Append(lines[i + 1]).ToString();
                }
            }
        }

        private static bool IsValidColorCode(char color)
        {
            if ((color >= '0' && color <= '9') || (color >= 'a' && color <= 'f') || (color >= 'A' && color <= 'F'))
            {
                return true;
            }
            return false;
        }
    }


    public enum WrapMethod
    {
        Default,
        Chat,
        Spaced,
        FixedSpace,
        None
    }
}
