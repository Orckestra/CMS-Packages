using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Composite.Core;
using Composite.Search;

namespace Orckestra.Search.Commands
{
    class PopulateFromDataSourceCommand : IIndexUpdateCommand, ILongRunningCommand
    {
        private const string LogTitle = "Orckestra.Search";

        private bool _persistOnShutdown = true;

        public string CultureName { get; set; }

        public string DocumentSourceName { get; set; }

        public string ContinuationToken { get; set; }

        public void Execute(CommandContext context)
        {
            var culture = new CultureInfo(CultureName);

            var dataSource = SearchFacade.DocumentSources.FirstOrDefault(ds => ds.Name == DocumentSourceName);
            if (dataSource == null)
            {
                Log.LogWarning(nameof(PopulateFromDataSourceCommand), $"Data source with name '{DocumentSourceName}' does not exist.");
                return;
            }

            var customFields = dataSource.CustomFields.ToList();

            // If the populate action is cancelled by a restart, queueing an index action that 
            // will start from the current point.
            Action<string> onCancelAction = conToken =>
            {
                Log.LogInformation("Orckestra.Search", $"Suspending indexing, continuation token = '{conToken}'");

                CommandQueue.Queue(new PopulateFromDataSourceCommand
                {
                    DocumentSourceName = DocumentSourceName,
                    CultureName = CultureName,
                    ContinuationToken = conToken ?? ContinuationToken
                });

                _persistOnShutdown = false;
            };

            var msg = $"Indexing document source '{DocumentSourceName}' for culture '{CultureName}'"
                      + (ContinuationToken != null ? $", continuationToken = '{ContinuationToken}'" : "");

            Log.LogInformation(LogTitle, msg);

            EnumerableWithCounter<DocumentWithContinuationToken> documents = null;

            var stopwatch = new Stopwatch();
            try
            {
                stopwatch.Start();
                documents = new EnumerableWithCounter<DocumentWithContinuationToken>(
                    dataSource.GetSearchDocuments(culture, ContinuationToken));

                context.Index.IndexDocuments(culture,
                    documents.Select(_ => new Tuple<SearchDocument, string>(_.Document, _.ContinuationToken)),
                    customFields,
                    context.RestartCancellationToken,
                    onCancelAction);
            }
            finally
            {
                stopwatch.Stop();

                string message = "";
                if (documents != null)
                {
                    message = $"{documents.Counter} documents; ";
                }

                message += $"Completed in {stopwatch.ElapsedMilliseconds} ms";
                Log.LogInformation(LogTitle, message);
            }
        }

        public bool ShouldBePersistedOnShutdown() => _persistOnShutdown;


        class EnumerableWithCounter<T> : IEnumerable<T>
        {
            public int Counter;

            private readonly IEnumerable<T> _innerEnumerable;

            public EnumerableWithCounter(IEnumerable<T> innerEnumerable)
            {
                _innerEnumerable = innerEnumerable;
            }

            public IEnumerator<T> GetEnumerator()
            {
                return new EnumeratorWithCounting<T>(_innerEnumerable.GetEnumerator(), this);
            }

            IEnumerator IEnumerable.GetEnumerator() => _innerEnumerable.GetEnumerator();
        }

        class EnumeratorWithCounting<T> : IEnumerator<T>
        {
            private readonly IEnumerator<T> _innerEnumerator;
            private readonly EnumerableWithCounter<T> _parent;

            public EnumeratorWithCounting(IEnumerator<T> innerEnumerator, EnumerableWithCounter<T> parent)
            {
                _innerEnumerator = innerEnumerator;
                _parent = parent;
            }

            public void Dispose() => _innerEnumerator.Dispose();

            public bool MoveNext()
            {
                var result = _innerEnumerator.MoveNext();
                if (result)
                {
                    _parent.Counter++;
                }

                return result;
            }

            public void Reset() => _innerEnumerator.Reset();

            public T Current => _innerEnumerator.Current;

            object IEnumerator.Current => _innerEnumerator.Current;
        }
    }
}
