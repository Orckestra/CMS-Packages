using System;
using System.Collections.Generic;
using Composite.C1Console.Security;
using Composite.Tools.PackageCreator.ElementProvider.EntityTokens.Interfaces;

namespace Composite.Tools.PackageCreator.ElementProvider.EntityTokens
{
    [SecurityAncestorProvider(typeof(NoAncestorSecurityAncestorProvider))]
    public sealed class PackageCreatorElementProviderEntityToken : PackageCreatorEntityToken
    {
        public override string Id => "PackageCreatorElementProviderEntityToken";

        public override string Source => "";

        public override string Type => "";

        public override string Serialize()
        {
            return "";
        }

        public static EntityToken Deserialize(string serializedData)
        {
            return new PackageCreatorElementProviderEntityToken();
        }
    }

    internal sealed class NoAncestorSecurityAncestorProvider : ISecurityAncestorProvider
    {
        public IEnumerable<EntityToken> GetParents(EntityToken entityToken)
        {
            if (entityToken == null) throw new ArgumentNullException(nameof(entityToken));

            return new EntityToken[] { };
        }
    }
}