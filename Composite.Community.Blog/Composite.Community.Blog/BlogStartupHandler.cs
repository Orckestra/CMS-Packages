using Composite.C1Console.Elements;
using Composite.Core.Application;
using Composite.Core.Routing;
using Composite.Data;

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
            DataEventSystemFacade.SubscribeToDataBeforeUpdate<Entries>(BlogFacade.SetTitleUrl, true);
            DataEventSystemFacade.SubscribeToDataBeforeAdd<Entries>(BlogFacade.SetTitleUrl, true);

            DataEventSystemFacade.SubscribeToDataAfterAdd<Entries>(BlogFacade.ClearRssFeedCache, true);
            DataEventSystemFacade.SubscribeToDataAfterUpdate<Entries>(BlogFacade.ClearRssFeedCache, true);
            DataEventSystemFacade.SubscribeToDataDeleted<Entries>(BlogFacade.ClearRssFeedCache, true);

            DataUrls.RegisterGlobalDataUrlMapper<Entries>(new BlogEntryDataUrlMapper());
            UrlToEntityTokenFacade.Register(new BlogUrlToEntityTokenMapper());
        }
    }
}