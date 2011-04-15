using Composite.C1Console.Security;

namespace Composite.Tools.PackageCreator.ElementProvider
{
	[SecurityAncestorProvider(typeof(PackageCreatorProviderEntityTokenSecurityAncestorProvider))]
	public sealed class PackageCreatorCategoryElementProviderEntityToken : PackageCreatorEntityToken
	{
		private string _source;
		private string _type;

		public PackageCreatorCategoryElementProviderEntityToken(string source, string type)
		{
			this._source = source;
			this._type = type;
		}
		public override string Id
		{
			get { return ""; }
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

			return new PackageCreatorCategoryElementProviderEntityToken(source, type);
		}
	}
}
