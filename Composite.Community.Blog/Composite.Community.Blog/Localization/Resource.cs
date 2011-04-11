using System.Web;

namespace Composite.Community.Blog.Localization
{
	public static class Resource
	{
		public static string GetLocalized(string resourceName, string key)
		{
			var ro = HttpContext.GetGlobalResourceObject(resourceName, key);
			return ro == null ? string.Empty : ro.ToString();
		}
	}
}
