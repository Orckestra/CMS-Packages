using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using Composite.C1Console.Events;
using Composite.Data;
using Composite.Search;

namespace Orckestra.Search
{
    sealed class CommandContext
    {
        private readonly ISearchIndex _index;
        private readonly IEnumerable<ISearchDocumentSourceProvider> _sourceProviders;
        private CancellationToken _restartCancellationToken = CancellationToken.None;

        public CommandContext(
            ISearchIndex index, 
            IEnumerable<ISearchDocumentSourceProvider> sourceProviders)
        {
            _index = index;
            _sourceProviders = sourceProviders;
        }

        public ISearchIndex Index => _index;

        public IEnumerable<ISearchDocumentSource> DocumentSources =>
            _sourceProviders.SelectMany(_ => _.GetDocumentSources());

        public IEnumerable<CultureInfo> DataLocalizationCultures => 
            DataLocalizationFacade.ActiveLocalizationCultures;

        public CancellationToken RestartCancellationToken
        {
            get
            {
                if (_restartCancellationToken == CancellationToken.None)
                {
                    var ctSource = new CancellationTokenSource();
                    GlobalEventSystemFacade.SubscribeToPrepareForShutDownEvent(a => ctSource.Cancel());

                    _restartCancellationToken = ctSource.Token;
                }

                return _restartCancellationToken;
            }
        }
    }
}
