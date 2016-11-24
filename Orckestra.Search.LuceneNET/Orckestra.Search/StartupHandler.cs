using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Hosting;
using Composite.C1Console.Elements;
using Composite.C1Console.Search;
using Composite.Core;
using Composite.Core.Application;
using Composite.Core.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Orckestra.Search
{
    [ApplicationStartup(AbortStartupOnException = true)]
    public class StartupHandler
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(typeof(ISearchIndexUpdater), typeof (SearchIndexUpdater));
        }

        public void OnBeforeInitialize()
        {
        }

        public void OnInitialized(
            ISearchIndex searchIndex, 
            IEnumerable<ISearchDocumentSourceProvider> sourceProviders)
        {
            UrlToEntityTokenFacade.Register(new SearchUrlToEntityTokenMapper());

            Task.Run(() =>
            {
                try
                {
                    var listener = new SearchIndexUpdater();

                    var sources = sourceProviders.SelectMany(s => s.GetDocumentSources());
                    foreach (var source in sources)
                    {
                        source.Subscribe(listener);
                    }
                    
                    searchIndex.Initialize();

                    ProcessCommandQueue();
                }
                catch (Exception ex)
                {
                    Log.LogError(typeof(StartupHandler).FullName, ex);
                }
            });
        }

        private void ProcessCommandQueue()
        {
            Func<bool> shutdownInitiated = () => HostingEnvironment.ApplicationHost.ShutdownInitiated();

            while (!shutdownInitiated())
            {
                var message = CommandQueue.Dequeue();

                if (message == null)
                {
                    while (!CommandQueue.NewMessages.WaitOne(500))
                    {
                        if(shutdownInitiated()) break;
                    }
                    continue;
                }

                CommandQueue.ExecuteCommand(message);
            }
        }
    }
}
