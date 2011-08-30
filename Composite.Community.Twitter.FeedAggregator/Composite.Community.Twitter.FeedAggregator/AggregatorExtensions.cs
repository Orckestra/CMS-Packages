using System.Linq;
using System.Xml.Linq;
using Composite.Core.WebClient.Services.WysiwygEditor;
using Composite.Core.Xml;
using System.Text.RegularExpressions;
using System.Web;

namespace Composite.Community.Twitter.FeedAggregator
{
	public static class AggregatorExtensions
	{
		public static void HtmlfyNodes(this XElement element, XName name)
		{
			foreach (var item in element.Descendants(name))
			{
				try
				{
					var value = item.Value;
					value = new Regex(@"(\s+)<").Replace(value,"&#160;<");
					value = new Regex(@">(\s+)").Replace(value, ">&#160;");
					var html = MarkupTransformationServices.TidyHtml(value).Output.Root;
					foreach (var text in html.DescendantNodes().Reverse().Where(d => d is XText).Cast<XText>())
					{
						text.Value = HttpUtility.HtmlDecode(text.Value);
					}
					item.ReplaceNodes(html.Element(Namespaces.Xhtml + "body").Nodes());
				}
				//Ignore
				catch { }
			}
		}
	}
}
