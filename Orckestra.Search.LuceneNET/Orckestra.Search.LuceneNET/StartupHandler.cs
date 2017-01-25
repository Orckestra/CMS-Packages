using Composite.Search;
using Composite.Core.Application;
using Microsoft.Extensions.DependencyInjection;


namespace Orckestra.Search.LuceneNET
{
    [ApplicationStartup(AbortStartupOnException = true)]
    public class StartupHandler
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(typeof(ISearchIndex), typeof(LuceneSearchIndex));
            services.AddSingleton(typeof(ISearchProvider), typeof(LuceneSearchProvider));
        }

        public void OnBeforeInitialize()
        {
        }

        public void OnInitialized()
        {
        }
    }
}
