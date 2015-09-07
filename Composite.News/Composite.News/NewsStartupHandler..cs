using Composite.Core.Application;
using Composite.Core.Routing;
using Composite.Data;

namespace Composite.News
{
    [ApplicationStartup]
    class NewsStartupHandler
    {
        public static void OnBeforeInitialize() { }

        public static void OnInitialized()
        {
            DataEventSystemFacade.SubscribeToDataBeforeUpdate<NewsItem>(NewsFacade.SetTitleUrl, true);
            DataEventSystemFacade.SubscribeToDataBeforeAdd<NewsItem>(NewsFacade.SetTitleUrl, true);

            DataUrls.RegisterGlobalDataUrlMapper<NewsItem>(new NewsDataUrlMapper());
        }
    }
}
