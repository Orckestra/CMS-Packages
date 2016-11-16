using System.Web.Optimization;

namespace Composite.Web.BundlingAndMinification.Extensions
{
    public static class BundleExtensions
    {
        public static Bundle ForceOrdered(this Bundle bundle)
        {
            bundle.Orderer = new AsIsBundleOrderer();
            return bundle;
        }
    }
}
