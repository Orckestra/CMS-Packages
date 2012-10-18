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
		private string _id { get; set; }

		private string _source { get; set; }

		public XmlNodeAttributeProviderEntityToken(string xpath, string name)
		{
			this._id = xpath;
			this._source = name;
		}

		public string XPath { get { return this._id; } }

		public override string Id { get { return _id; } }

		public override string Source { get { return _source; } }

		public override string Type { get { return ""; } }

		public override string Serialize()
		{
			StringBuilder sb = new StringBuilder();

			StringConversionServices.SerializeKeyValuePair<Guid>(sb, "_xpath", _id);
			StringConversionServices.SerializeKeyValuePair<Guid>(sb, "_source", _source);

			return sb.ToString();
		}


		public static EntityToken Deserialize(string serializedData)
		{
			Dictionary<string, string> dic = StringConversionServices.ParseKeyValueCollection(serializedData);

			var xpath = StringConversionServices.DeserializeValueString(dic["_xpath"]);
			var source = StringConversionServices.DeserializeValueString(dic["_source"]);

			return new XmlNodeAttributeProviderEntityToken(xpath, source);
		}

		
	}
}
