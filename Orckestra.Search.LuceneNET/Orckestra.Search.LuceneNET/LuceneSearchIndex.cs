//#define IN_MEMORY_INDEX

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using Composite.Search;
using Composite.Core.IO;
using Composite.Core.Types;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Store;

namespace Orckestra.Search.LuceneNET
{
    internal class LuceneSearchIndex: ISearchIndex
    {
        private const string LogTitle = nameof(LuceneSearchIndex);
        private readonly AnalyzerFactory _analyzerFactory;


        private Dictionary<CultureInfo, Directory> _directories;
        public bool IsInitialized { get; private set; }

        public LuceneSearchIndex(AnalyzerFactory analyzerFactory)
        {
            _analyzerFactory = analyzerFactory ?? throw new ArgumentNullException(nameof(analyzerFactory));
        }


        public void Initialize(
            IEnumerable<CultureInfo> activeCultures, 
            CancellationToken cancellationToken,
            out ICollection<CultureInfo> culturesToPopulate)
        {
            culturesToPopulate = new List<CultureInfo>();

#if IN_MEMORY_INDEX
            _directories = activeCultures.ToDictionary(c => c, c => new RAMDirectory() as Directory);
            culturesToPopulate.AddRange(activeCultures);
#else
            var indexRoot = PathUtil.Resolve(Constants.IndexFolderRelativePath);

            _directories = new Dictionary<CultureInfo, Directory>();

            foreach (var culture in activeCultures)
            {
                var directoryPath = $"{indexRoot}\\{culture.Name.Replace('/', '_')}";
                if (!System.IO.Directory.Exists(directoryPath))
                {
                    culturesToPopulate.Add(culture);
                }

                var directory = FSDirectory.Open(directoryPath);
                _directories[culture] = directory;
            }
#endif

            IsInitialized = true;
        }


        private void UpdateDirectory(CultureInfo culture, Action<IndexWriter> action, bool optimize = false)
        {
            var directory = _directories[culture];

            lock (this)
            {
                using (var writer = new IndexWriter(directory,
                    _analyzerFactory.GetAnalyzer(culture),
                    IndexWriter.MaxFieldLength.LIMITED))
                {
                    action(writer);

                    if (optimize)
                    {
                        writer.Optimize();
                    }

                    writer.Commit();
                }
            }
        }

        public void IndexDocuments<TContinuationToken>(CultureInfo cultureInfo,
                    IEnumerable<Tuple<SearchDocument, TContinuationToken>> documents,
                    IReadOnlyCollection<DocumentField> customFields,
                    CancellationToken cancellationToken,
                    Action<TContinuationToken> onCancel)
        {
            var lastCommited = default(TContinuationToken);
            var lastAdded = default(TContinuationToken);
            try
            {
                foreach (var chunk in documents.ChunksOf(100))
                {
                    UpdateDirectory(cultureInfo, writer =>
                    {
                        foreach (var tuple in chunk)
                        {
                            var document = tuple.Item1;

                            var doc = ToLuceneDocument(document, customFields);

                            writer.AddDocument(doc);

                            lastAdded = tuple.Item2;
                            if (cancellationToken.IsCancellationRequested)
                            {
                                break;
                            }
                        }
                    });
                    lastCommited = lastAdded;

                    if (cancellationToken.IsCancellationRequested)
                    {
                        onCancel?.Invoke(lastCommited);
                        break;
                    }
                }
            }
            catch (ThreadAbortException)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    onCancel?.Invoke(lastCommited);
                }
            }
        }

        private Document ToLuceneDocument(SearchDocument document, IReadOnlyCollection<DocumentField> customFields)
        {
            var fields = new List<Field>(new[]
                {
                    new Field(Constants.FieldNames.source, document.Source, Field.Store.YES, Field.Index.NOT_ANALYZED),
                    new Field(Constants.FieldNames.id, document.Id, Field.Store.YES, Field.Index.NOT_ANALYZED),
                    new Field(Constants.FieldNames.entityToken, document.SerializedEntityToken, Field.Store.YES, Field.Index.NO),
                    new Field(Constants.FieldNames.label, document.Label, Field.Store.YES, Field.Index.ANALYZED)
                });

            if (!string.IsNullOrWhiteSpace(document.Url))
            {
                fields.Add(new Field(Constants.FieldNames.url, document.Url, Field.Store.YES, Field.Index.NOT_ANALYZED));
            }

            if (!string.IsNullOrWhiteSpace(document.ElementBundleName))
            {
                fields.Add(new Field(Constants.FieldNames.version, document.ElementBundleName, Field.Store.YES, Field.Index.NO));
            }

            if (document.FullText != null)
            {
                var filteredText = document.FullText.Where(line => line != document.Label);
                string fullText = string.Join(" ", filteredText);
                if (!string.IsNullOrWhiteSpace(fullText))
                {
                    fields.Add(new Field(Constants.FieldNames.fulltext, fullText, Field.Store.YES, Field.Index.ANALYZED));
                }
            }

            if (document.FieldValues != null)
            {
                foreach (var customField in customFields.Where(f => f.FieldValuePreserved))
                {
                    object value;
                    if (document.FieldValues.TryGetValue(customField.Name, out value) && value != null)
                    {
                        string stringValue = ValueTypeConverter.Convert<string>(value);

                        var indexing = customField.Preview.Sortable
                            ? Field.Index.NOT_ANALYZED_NO_NORMS
                            : Field.Index.NO;

                        fields.Add(new Field(Constants.PreviewFieldPrefix + customField.Name, stringValue, Field.Store.YES, indexing));
                    }
                }
            }

            if (document.FacetFieldValues != null)
            {
                foreach (var customField in customFields.Where(f => f.FacetedSearchEnabled))
                {
                    string[] facetValues;
                    if (document.FacetFieldValues.TryGetValue(customField.Name, out facetValues) 
                        && facetValues != null)
                    {
                        foreach (var value in facetValues)
                        {
                            fields.Add(new Field(Constants.FacetFieldPrefix + customField.Name,
                            value,
                            Field.Store.NO, Field.Index.NOT_ANALYZED_NO_NORMS));
                        }
                    }
                }
            }

            var doc = new Document();
            fields.ForEach(doc.Add);

            return doc;
        }

        public void AddDocument(CultureInfo cultureInfo, SearchDocument document, IReadOnlyCollection<DocumentField> customFields)
        {
            var doc = ToLuceneDocument(document, customFields);
            if (doc == null) return;

            UpdateDirectory(cultureInfo, writer =>
            {
                writer.AddDocument(doc);
            });
        }


        public void UpdateDocument(CultureInfo cultureInfo, SearchDocument document, IReadOnlyCollection<DocumentField> customFields)
        {
            var doc = ToLuceneDocument(document, customFields);
            if(doc == null) return;

            UpdateDirectory(cultureInfo, writer =>
            {
                writer.UpdateDocument(new Term(Constants.FieldNames.id, document.Id), doc);
            });
        }



        public void RemoveDocument(CultureInfo cultureInfo, string documentId)
        {
            UpdateDirectory(cultureInfo, writer =>
            {
                writer.DeleteDocuments(new Term(Constants.FieldNames.id, documentId));
            });
        }

        public T GetCollection<T>(CultureInfo cultureInfo) where T : class
        {
            if (typeof (T) != typeof (Directory))
            {
                throw new InvalidOperationException($"The only supported type is '{typeof(Directory).FullName}'");
            }

            return GetDirectory(cultureInfo) as T;
        }

        public Directory GetDirectory(CultureInfo culture)
        {
            Directory directory;
            if (!_directories.TryGetValue(culture, out directory))
            {
                throw new InvalidOperationException($"Unexpected culture value '{culture}'");
            }

            return directory;
        }

        public void DropCollection(CultureInfo culture)
        {
            UpdateDirectory(culture, writer =>
            {
                writer.DeleteAll();
            });
        }

        public void RemoveDocuments(CultureInfo culture)
        {
            UpdateDirectory(culture, writer =>
            {
                writer.DeleteAll();
            });
        }


        public void RemoveDocuments(CultureInfo culture, string sourceName)
        {
            UpdateDirectory(culture, writer =>
            {
                writer.DeleteDocuments(new Term(Constants.FieldNames.source, sourceName));
            });
        }
    }
}
