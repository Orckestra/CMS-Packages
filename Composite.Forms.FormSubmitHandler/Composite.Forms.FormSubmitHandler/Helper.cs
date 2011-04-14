using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Composite.Core.Xml;

namespace Composite.Forms.FormSubmitHandler
{
	public class Helper
	{
		private static readonly XNamespace ns = Namespaces.Xhtml;
		private static Func<XElement, bool> isSubmitInput = i => i.Attribute("type") != null && (i.Attribute("type").Value.ToLower() == "submit" || i.Attribute("type").Value.ToLower() == "image");
		
		internal static void ReplaceFormTagByDivTag(XElement document)
		{
			foreach (var form in document.DescendantsAndSelf(ns + "form"))
			{
				form.Name = ns + "div";
			}
		}


		internal static void SetNameForSubmitButtons(string name, XElement document)
		{
			foreach (var submit in document.Descendants(ns + "input").Where(isSubmitInput))
			{
				submit.SetAttributeValue("name", name);
			}
		}

		internal static string ToValidIndefiniter(string name)
		{
			return Regex.Replace(name, @"[^\w\d_]*", "");
		}
	}
}