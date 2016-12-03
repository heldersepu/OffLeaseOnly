using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;

namespace OffLeaseOnly
{
    public static class HtmlNodeExtension
    {
        public static IEnumerable<HtmlNode> DescendantsWithClass(this HtmlNode node, string name, string className)
        {
            return node.Descendants(name)
                .Where(d =>
                    d.Attributes.Contains("class") &&
                    d.Attributes["class"].Value.Contains(className)
                );
        }
    }
}