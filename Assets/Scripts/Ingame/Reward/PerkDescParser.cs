using System.Text;
using System.Text.RegularExpressions;

namespace Rogue.Ingame.Reward
{
    public static class PerkDescParser
    {
        private static readonly string regex = @"\[\[([^\]]*)\]\]"; // [[*/*/*]] 형태

        public static string HighlightCurLevel(string text, int level)
        {
            var matches = Regex.Matches(text, regex);

            for (var i = 0; i < matches.Count; i++)
            {
                var match = matches[i];
                var org = match.Groups[0].Value;
                var inner = match.Groups[1].Value;
                var splits = inner.Split('/');
                var sb = new StringBuilder();
                sb.Append("<color=#88888888>");
                for (var j = 0; j < splits.Length; j++)
                {
                    if (j != 0)
                        sb.Append("/");
                    if (j == level)
                        sb.Append("<color=#ffffffff>");
                    sb.Append(splits[j]);
                    if (j == level)
                        sb.Append("</color>");

                }
                sb.Append("</color>");
                text = text.Replace(org, sb.ToString());
            }
            return text;
        }

        public static string ShowOnlyCurLevel(string text, int level)
        {
            var matches = Regex.Matches(text, regex);

            for (var i = 0; i < matches.Count; i++)
            {
                var match = matches[i];
                var org = match.Groups[0].Value;
                var inner = match.Groups[1].Value;
                var splits = inner.Split('/');
                var curLevelText = splits[level];
                text = text.Replace(org, curLevelText);
            }

            return text;
        }
    }
}