using Composite.C1Console.Security;
using Composite.Tools.PackageCreator.ElementProvider.EntityTokens.Interfaces;

namespace Composite.Tools.PackageCreator.ElementProvider.EntityTokens
{
    [SecurityAncestorProvider(typeof(PackageCreatorProviderEntityTokenSecurityAncestorProvider))]
    public class PackageCreatorPackageElementProviderEntityToken : PackageCreatorEntityToken
    {
        private readonly string _source;

        public PackageCreatorPackageElementProviderEntityToken(string source)
        {
            _source = source;
        }

        public override string Id => "";

        public override string Source => _source;

        public override string Type => "";

        public override string Serialize()
        {
            return DoSerialize();
        }

        public static EntityToken Deserialize(string serializedData)
        {
            DoDeserialize(serializedData, out _, out var source, out _);

            return new PackageCreatorPackageElementProviderEntityToken(source);
        }
    }
}