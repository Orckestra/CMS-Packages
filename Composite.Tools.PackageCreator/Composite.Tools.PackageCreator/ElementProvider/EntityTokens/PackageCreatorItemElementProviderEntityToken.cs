using Composite.C1Console.Security;
using Composite.Tools.PackageCreator.ElementProvider.EntityTokens.Interfaces;

namespace Composite.Tools.PackageCreator.ElementProvider.EntityTokens
{
    [SecurityAncestorProvider(typeof(PackageCreatorProviderEntityTokenSecurityAncestorProvider))]
    public sealed class PackageCreatorItemElementProviderEntityToken : PackageCreatorEntityToken
    {
        public PackageCreatorItemElementProviderEntityToken(string id, string source, string type)
        {
            Id = id;
            Source = source;
            Type = type;
        }

        public override string Id { get; }

        public override string Source { get; }

        public override string Type { get; }

        public override string Serialize()
        {
            return DoSerialize();
        }

        public static EntityToken Deserialize(string serializedData)
        {
            DoDeserialize(serializedData, out var type, out var source, out var id);

            return new PackageCreatorItemElementProviderEntityToken(id, source, type);
        }
    }
}