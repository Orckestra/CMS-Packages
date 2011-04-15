using System;
using System.Collections.Generic;
using System.Text;
using Composite.C1Console.Security;
using Composite.Core.Serialization;

namespace Composite.Tools.PackageCreator.ElementProvider
{
	[SecurityAncestorProvider(typeof(PackageCreatorProviderEntityTokenSecurityAncestorProvider))]
	public class XmlNodeAttributeProviderEntityToken : EntityToken
	{
		public string XPath { get; private set; }

		public XmlNodeAttributeProviderEntityToken(string xpath)
		{
			XPath = xpath;
		}

		public override string Id
		{
			get { return XPath; }
		}

		public override string Serialize()
		{
			StringBuilder sb = new StringBuilder();

			StringConversionServices.SerializeKeyValuePair<Guid>(sb, "xpath", XPath);

			return sb.ToString();
		}

		public override string Source
		{
			get { return ""; }
		}

		public override string Type
		{
			get { return ""; }
		}

		public static EntityToken Deserialize(string serializedData)
		{
			Dictionary<string, string> dic = StringConversionServices.ParseKeyValueCollection(serializedData);

			var xpath = StringConversionServices.DeserializeValueString(dic["xpath"]);

			return new XmlNodeAttributeProviderEntityToken(xpath);
		}
	}
}
