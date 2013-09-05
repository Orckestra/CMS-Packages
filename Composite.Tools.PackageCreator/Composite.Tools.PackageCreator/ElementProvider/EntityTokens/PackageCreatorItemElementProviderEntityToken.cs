using Composite.C1Console.Security;
using Composite.Tools.PackageCreator.ElementProvider.EntityTokens.Interfaces;

namespace Composite.Tools.PackageCreator.ElementProvider.EntityTokens
{
    [SecurityAncestorProvider(typeof(PackageCreatorProviderEntityTokenSecurityAncestorProvider))]
    public sealed class PackageCreatorItemElementProviderEntityToken : PackageCreatorEntityToken
    {
        private string _id;
        private string _source;
        private string _type;

        public PackageCreatorItemElementProviderEntityToken(string id, string source, string type)
        {
            this._id = id;
            this._source = source;
            this._type = type;
        }
        public override string Id
        {
            get { return _id; }
        }

        public override string Serialize()
        {
            return this.DoSerialize();
        }

        public override string Source
        {
            get { return _source; }
        }

        public override string Type
        {
            get { return _type; }
        }

        public static EntityToken Deserialize(string serializedData)
        {
            string type, source, id;

            DoDeserialize(serializedData, out type, out source, out id);

            return new PackageCreatorItemElementProviderEntityToken(id, source, type);
        }
    }
}
