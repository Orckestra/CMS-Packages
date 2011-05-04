using System.Collections.Generic;
using System.Xml.Linq;

namespace Composite.Community
{
	public class AddThis
	{
		public static IEnumerable<XElement> GetOptionsXml()
		{
			var options = new List<XElement>
			              	{
			              		GetOption("Small Buttons", "SmallButtons"),
			              		GetOption("Large Buttons", "LargeButtons"),
			              		GetOption("Informative", "Informative"),
			              		GetOption("Simple with Buttons", "SimpleWithButtons"),
			              		GetOption("Simple no Counter", "SimpleNoCounter"),
			              		GetOption("Simple with Counter Above", "SimpleWithCounterAbove"),
			              		GetOption("Simple with Counter Beside", "SimpleWithCounterBeside"),
			              		GetOption("Bar with Buttons", "BarWithButtons"),
			              		GetOption("Bar", "Bar"),
			              	};

			return options;
		}

		private static XElement GetOption(string title, string value)
		{
			return new XElement("Option", new XAttribute("Title", title), new XAttribute("Value", value));
		}
	}
}