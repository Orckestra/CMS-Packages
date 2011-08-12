using System;
using System.Collections.Generic;
using Composite.C1Console.Security;
using Composite.Functions;

namespace Composite.Forms.Renderer.FormsRendererFunctionProvider
{
    [SecurityAncestorProvider(typeof(StandardFunctionSecurityAncestorProvider))]
	public sealed class FormsRendererFunctionProviderEntityToken : EntityToken
	{
		private string _id;
		private string _source;

		public FormsRendererFunctionProviderEntityToken(string source, string id)
		{
			_source = source;
			_id = id;
		}

		public override string Type
		{
			get { return ""; }
		}

		public override string Source
		{
			get { return _source; }
		}

		public override string Id
		{
			get { return _id; }
		}

		public override string Serialize()
		{
			return DoSerialize();
		}

		public static EntityToken Deserialize(string serializedEntityToken)
		{
			string type, source, id;

			DoDeserialize(serializedEntityToken, out type, out source, out id);

			return new FormsRendererFunctionProviderEntityToken(source, id);
		}
	}

	internal sealed class NoAncestorSecurityAncestorProvider : ISecurityAncestorProvider
	{
		public IEnumerable<EntityToken> GetParents(EntityToken entityToken)
		{
			if (entityToken == null) throw new ArgumentNullException("entityToken");

			return new EntityToken[] { };
		}
	}
}
