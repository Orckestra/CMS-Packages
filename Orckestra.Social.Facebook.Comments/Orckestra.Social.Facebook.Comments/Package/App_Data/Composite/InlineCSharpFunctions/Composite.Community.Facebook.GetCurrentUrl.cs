using System;
using System.Web;

namespace Composite.Community.Facebook
{
	public static class InlineMethodFunction
	{
		public static string GetCurrentUrl()
		{
			return HttpContext.Current.Request.Url.ToString();
		}
	}
}
