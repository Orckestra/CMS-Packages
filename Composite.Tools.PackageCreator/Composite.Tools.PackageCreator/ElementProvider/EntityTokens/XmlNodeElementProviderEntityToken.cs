using System;
using System.Text;
using Composite.C1Console.Security;
using Composite.Core.Serialization;

namespace Composite.Tools.PackageCreator.ElementProvider.EntityTokens
{
    [SecurityAncestorProvider(typeof(PackageCreatorProviderEntityTokenSecurityAncestorProvider))]
    public class XmlNodeElementProviderEntityToken : EntityToken
    {
        public XmlNodeElementProviderEntityToken(string xpath, string source)
        {
            _id = xpath;
            _source = source;
        }

        private string _id { get; }

        private string _source { get; }

        public string XPath => _id;

        public override string Id => _id;

        public override string Source => _source;

        public override string Type => "";

        public override string Serialize()
        {
            var sb = new StringBuilder();

            StringConversionServices.SerializeKeyValuePair<Guid>(sb, "_xpath", _id);
            StringConversionServices.SerializeKeyValuePair<Guid>(sb, "_source", _source);

            return sb.ToString();
        }

        public static EntityToken Deserialize(string serializedData)
        {
            var dic = StringConversionServices.ParseKeyValueCollection(serializedData);

            var xpath = StringConversionServices.DeserializeValueString(dic["_xpath"]);
            var source = StringConversionServices.DeserializeValueString(dic["_source"]);

            return new XmlNodeElementProviderEntityToken(xpath, source);
        }
    }
}