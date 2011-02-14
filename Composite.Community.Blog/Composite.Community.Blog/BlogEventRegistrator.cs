using Composite.Data;

namespace Composite.Community.Blog
{
	public static class BlogEventRegistrator
	{
		static BlogEventRegistrator()
		{
			DataEventSystemFacade.SubscribeToDataBeforeUpdate<Entries>(BlogFacade.SetTitleUrl, true);
			DataEventSystemFacade.SubscribeToDataBeforeAdd<Entries>(BlogFacade.SetTitleUrl, true);
		}

		public static void Initialize()
		{
			// initialization code is in the static constructor
		}
	}
}