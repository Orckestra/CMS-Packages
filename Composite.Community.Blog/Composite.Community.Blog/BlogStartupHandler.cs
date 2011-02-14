using Composite.Core.Application;

namespace Composite.Community.Blog
{
	[ApplicationStartup]
	public class BlogStartupHandler
	{
		public static void OnBeforeInitialize()
		{

		}

		public static void OnInitialized()
		{
			BlogEventRegistrator.Initialize();
		}
	}
}
