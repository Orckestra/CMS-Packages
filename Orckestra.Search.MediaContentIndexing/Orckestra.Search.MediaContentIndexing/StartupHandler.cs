using Composite.Core.Application;
using Composite.Search.Crawling;
using Microsoft.Extensions.DependencyInjection;

namespace Orckestra.Search.MediaContentIndexing
{
    [ApplicationStartup]
    public static class StartupHandler
    {
        public static void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<ISearchDocumentBuilderExtension>(new MediaContentSearchExtension());
        }

        public static void OnBeforeInitialize() {}

        public static void OnInitialized() {}
    }
}
