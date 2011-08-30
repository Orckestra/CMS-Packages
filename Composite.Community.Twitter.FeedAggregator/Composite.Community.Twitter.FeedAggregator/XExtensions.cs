using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Composite.Community.Twitter.FeedAggregator
{
	/// <summary>
	/// Extensions for Xml.Linq v1.0
	/// </summary>
	internal static class XExtensions
	{
		public static string AttributeValue(this XElement element, XName attributeName)
		{
			return element.Attributes(attributeName).Select(d => d.Value).FirstOrDefault();
		}

		public static string ElementValue(this XElement element, XName elementName)
		{
			return element.Elements(elementName).Select(d => d.Value).FirstOrDefault();
		}

		public static IEnumerable<T> NotNull<T>(this IEnumerable<T> source)
		{
			return source.Where(d => d != null);
		}
	}
}
