using Composite.Community.TellAFriend.Localization;

namespace Composite.Community.TellAFriend
{
	public class TellAFriendXsltExtensionsFunction
	{
		public string GetLocalized(string resourceName, string key)
		{
			return Resource.GetLocalized(resourceName, key);
		}

		public string GetUrl()
		{
			return TellAFriendFacade.GetUrl();
		}

		public string GetWebsite()
		{
			return TellAFriendFacade.GetWebsite();
		}

		public string GetCurrentCulture()
		{
			return System.Threading.Thread.CurrentThread.CurrentCulture.ToString();
		}
	}
}
