using System.Web.Optimization;

namespace Orckestra.Web.BundlingAndMinification
{
    /// <summary>
    /// Class implementing custom styles bundling and minification since WebGrease has known issues with BootStrap 4 
    /// </summary>
    public class CustomStyleBundle : Bundle
	{
        public CustomStyleBundle(string virtualPath) : base(virtualPath, new CustomCssMinify()) { }

		public CustomStyleBundle(string virtualPath, string cdnPath) : base(virtualPath, cdnPath, new CustomCssMinify()) { }

        public override IBundleBuilder Builder
        {
            get
            {
                return base.Builder == null || base.Builder.GetType() != typeof(CustomBundleBuilder) ? new CustomBundleBuilder() : base.Builder;
            }
            set
            {
                base.Builder = value;
            }
        }
    }
}