using System.Linq;
using System.Xml.Linq;

namespace Composite.Forms.FormSubmitHandler
{
	internal static class HandlerExtensions
	{
		public static string AttributeValue(this XElement element, XName name)
		{
			return element.Attributes(name).Select(d => d.Value).FirstOrDefault();
		}
	}
}