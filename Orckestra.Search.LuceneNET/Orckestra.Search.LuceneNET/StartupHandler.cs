using System;
using System.Threading.Tasks;
using Composite.C1Console.Elements;
using Composite.C1Console.Search;
using Composite.Core;
using Composite.Core.Application;
using Microsoft.Extensions.DependencyInjection;

namespace Orckestra.Search.LuceneNET
{
    [ApplicationStartup(AbortStartupOnException = true)]
    public class StartupHandler
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(typeof(IIndexContainer), typeof(LuceneSearchIndex));
            services.AddSingleton(typeof(ISearchProvider), typeof(LuceneSearchProvider));
        }

        public void OnBeforeInitialize()
        {
        }

        public void OnInitialized(IIndexContainer indexContainer)
        {
            UrlToEntityTokenFacade.Register(new SearchUrlToEntityTokenMapper());

            Task.Run(() =>
            {
                try
                {
                    indexContainer.Initialize();
                    indexContainer.SubscribeToSources();
                }
                catch (Exception ex)
                {
                    Log.LogError(nameof(LuceneSearchIndex), ex);
                }
            });
        }
    }
}
