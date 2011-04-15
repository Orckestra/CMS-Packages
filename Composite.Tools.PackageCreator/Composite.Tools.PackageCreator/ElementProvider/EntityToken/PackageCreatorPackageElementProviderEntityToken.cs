using Composite.C1Console.Security;

namespace Composite.Tools.PackageCreator.ElementProvider
{
	[SecurityAncestorProvider(typeof(PackageCreatorProviderEntityTokenSecurityAncestorProvider))]
	public class PackageCreatorPackageElementProviderEntityToken : PackageCreatorEntityToken
	{
		private string _source;

		public PackageCreatorPackageElementProviderEntityToken(string source)
		{
			this._source = source;
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
			get { return ""; }
		}

		public static EntityToken Deserialize(string serializedData)
		{
			string type, source, id;

			DoDeserialize(serializedData, out type, out source, out id);

			return new PackageCreatorPackageElementProviderEntityToken(source);
		}
	}
}
