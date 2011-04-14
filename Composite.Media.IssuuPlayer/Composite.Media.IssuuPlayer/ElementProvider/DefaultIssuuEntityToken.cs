using System.Collections.Generic;
using Composite.C1Console.Security;
using Composite.Plugins.Elements.ElementProviders.VirtualElementProvider;

namespace Composite.Media.IssuuPlayer.ElementProvider
{
	[SecurityAncestorProvider(typeof(DefaultIssuuEntityTokenAuxiliarySecurityAncestorProvider))]
	public sealed class DefaultIssuuEntityToken : EntityToken
	{

		public override string Type
		{
			get { return ""; }
		}

		public override string Source
		{
			get { return ""; }
		}

		public override string Id
		{
			get { return "DefaultIssuuEntityToken"; }
		}

		public override string Serialize()
		{
			return DoSerialize();
		}

		public static EntityToken Deserialize(string serializedEntityToken)
		{
			return new DefaultIssuuEntityToken();
		}
	}

	public sealed class DefaultIssuuEntityTokenAuxiliarySecurityAncestorProvider : ISecurityAncestorProvider
	{
		public IEnumerable<EntityToken> GetParents(EntityToken entityToken)
		{
			yield return new VirtualElementProviderEntityToken("VirtualElementProvider", "ContentPerspective");
		}
	}
}