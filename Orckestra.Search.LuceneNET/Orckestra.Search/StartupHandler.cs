using System;
using System.Collections.Generic;
using System.Globalization;
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
using Composite.Core.Linq;
using Composite.Data;
using Microsoft.Extensions.DependencyInjection;
using Orckestra.Search.Commands;

namespace Orckestra.Search
{
    [ApplicationStartup(AbortStartupOnException = true)]
    public class StartupHandler
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(typeof(ISearchIndexUpdater), typeof (SearchIndexUpdater));
            services.AddSingleton<CommandContext>();
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
                    if (HostingEnvironment.ApplicationHost.ShutdownInitiated()) return;

                    var listener = new SearchIndexUpdater();

                    var sources = sourceProviders.SelectMany(s => s.GetDocumentSources());
                    foreach (var source in sources)
                    {
                        source.Subscribe(listener);
                    }

                    var ctSource = new CancellationTokenSource();
                    GlobalEventSystemFacade.SubscribeToPrepareForShutDownEvent(a => ctSource.Cancel());

                    IEnumerable<CultureInfo> cultures;
                    using (var dc = new DataConnection())
                    {
                        cultures = DataLocalizationFacade.ActiveLocalizationCultures.Evaluate();
                    }

                    searchIndex.Initialize(cultures, ctSource.Token, out ICollection<CultureInfo> newlyCreatedCollections);

                    foreach (var culture in newlyCreatedCollections)
                    {
                        CommandQueue.Queue(new PopulateCollectionCommand
                        {
                            Culture = culture.Name
                        });
                    }

                    CommandQueue.ProcessCommands();
                }
                catch (Exception ex)
                {
                    Log.LogError(typeof(StartupHandler).FullName, ex);
                }
            });
        }
    }
}
