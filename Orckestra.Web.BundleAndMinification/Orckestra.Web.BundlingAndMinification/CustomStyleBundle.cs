using System.Web.Optimization;

namespace Orckestra.Web.BundlingAndMinification
{
	/// <summary>
	/// Class implementing custom styles bundling and minification
	/// </summary>
	public class CustomStyleBundle : Bundle
	{
		public CustomStyleBundle(string virtualPath) : base(virtualPath, new CustomCssMinify()) { }

		public CustomStyleBundle(string virtualPath, string cdnPath) : base(virtualPath, cdnPath, new CustomCssMinify()) { }
	}
}