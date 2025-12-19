using Composite.C1Console.Security;
using Composite.Tools.PackageCreator.ElementProvider.EntityTokens.Interfaces;

namespace Composite.Tools.PackageCreator.ElementProvider.EntityTokens
{
    [SecurityAncestorProvider(typeof(PackageCreatorProviderEntityTokenSecurityAncestorProvider))]
    public sealed class PackageCreatorCategoryElementProviderEntityToken : PackageCreatorEntityToken
    {
        public PackageCreatorCategoryElementProviderEntityToken(string source, string type)
        {
            Source = source;
            Type = type;
        }

        public override string Id => "";

        public override string Source { get; }

        public override string Type { get; }

        public override string Serialize()
        {
            return DoSerialize();
        }

        public static EntityToken Deserialize(string serializedData)
        {
            DoDeserialize(serializedData, out var type, out var source, out _);

            return new PackageCreatorCategoryElementProviderEntityToken(source, type);
        }
    }
}