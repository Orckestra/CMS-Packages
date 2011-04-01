using Composite.Data;

namespace Composite.Community.Blog
{
	public static class BlogEventRegistrator
	{
		static BlogEventRegistrator()
		{
			DataEventSystemFacade.SubscribeToDataBeforeUpdate<Entries>(BlogFacade.SetTitleUrl, true);
			DataEventSystemFacade.SubscribeToDataBeforeAdd<Entries>(BlogFacade.SetTitleUrl, true);

			DataEventSystemFacade.SubscribeToDataAfterAdd<Entries>(BlogFacade.ClearRssFeedCache, true);
			DataEventSystemFacade.SubscribeToDataAfterUpdate<Entries>(BlogFacade.ClearRssFeedCache, true);
			DataEventSystemFacade.SubscribeToDataDeleted<Entries>(BlogFacade.ClearRssFeedCache, true);
		}

		public static void Initialize()
		{
			// initialization code is in the static constructor
		}
	}
}