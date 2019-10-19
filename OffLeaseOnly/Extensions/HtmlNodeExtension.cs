using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;

namespace OffLeaseOnly
{
    public static class HtmlNodeExtension
    {
        public static IEnumerable<HtmlNode> ChildNodes(this HtmlNode node, string name, string className = null)
        {
            var x = node?.Descendants(name);
            if (x != null && className != null)
                x = x.Where(d =>
                    d.Attributes.Contains("class") &&
                    d.Attributes["class"].Value.Contains(className)
                );
            return x;
        }

        public static HtmlNode ChildNode(this HtmlNode node, string name, string className= null, int count = 0)
        {
            return node.ChildNodes(name, className)?.Skip(count)?.FirstOrDefault();
        }

        public static List<string> GetOptionValues(this HtmlNode node, int count = 0)
        {
            var options = new List<string>();
            foreach (var opt in node.ChildNodes("option")?.Skip(count))
            {
                string txt = opt.InnerText;
                int pos = txt.IndexOf('(');
                if (pos > 0)
                    txt = txt.Substring(0, pos);
                options.Add(txt.Trim().ToLower());
            }
            return options;
        }
    }
}