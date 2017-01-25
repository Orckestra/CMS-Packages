using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Hosting;
using Composite.C1Console.Elements;
using Composite.C1Console.Events;
using Composite.Search;
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
            IServiceProvider serviceProvider,
            IEnumerable<ISearchDocumentSourceProvider> sourceProviders)
        {
            UrlToEntityTokenFacade.Register(new SearchUrlToEntityTokenMapper());

            var searchIndex = serviceProvider.GetService<ISearchIndex>();
            if (searchIndex == null)
            {
                Log.LogWarning("Orckestra.Search", $"Search services are disabled as there's no registered instance of {typeof(ISearchIndex).FullName}");
                return;
            }

            Task.Run(() =>
            {
                try
                {
                    if (IsRestarting) return;

                    var listener = new SearchIndexUpdater();

                    var sources = sourceProviders.SelectMany(s => s.GetDocumentSources());
                    foreach (var source in sources)
                    {
                        source.Subscribe(listener);
                    }

                    var ctSource = new CancellationTokenSource();
                    GlobalEventSystemFacade.SubscribeToPrepareForShutDownEvent(a => ctSource.Cancel());

                    searchIndex.Initialize(ctSource.Token);

                    ProcessCommandQueue();
                }
                catch (Exception ex)
                {
                    Log.LogError(typeof(StartupHandler).FullName, ex);
                }
            });
        }

        private static bool IsRestarting => HostingEnvironment.ApplicationHost.ShutdownInitiated();

        private void ProcessCommandQueue()
        {
            while (!IsRestarting)
            {
                var message = CommandQueue.Dequeue();

                if (message == null)
                {
                    while (!CommandQueue.NewMessages.WaitOne(500))
                    {
                        if(IsRestarting) break;
                    }
                    continue;
                }

                CommandQueue.ExecuteCommand(message);
            }
        }
    }
}
