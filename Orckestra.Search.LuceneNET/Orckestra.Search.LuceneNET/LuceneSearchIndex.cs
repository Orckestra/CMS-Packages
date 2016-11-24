//#define IN_MEMORY_INDEX

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Composite.C1Console.Search;
using Composite.Core;
using Composite.Core.IO;
using Composite.Core.Linq;
using Composite.Core.Types;
using Composite.Data;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Store;
using Version = Lucene.Net.Util.Version;

namespace Orckestra.Search.LuceneNET
{
    internal class LuceneSearchIndex: ISearchIndex
    {
        private const string LogTitle = nameof(LuceneSearchIndex);

        private readonly ISearchDocumentSourceProvider[] _sourceProviders;
       
        private Dictionary<CultureInfo, Directory> _directories;
        public bool IsInitialized { get; private set; }

        public LuceneSearchIndex(IEnumerable<ISearchDocumentSourceProvider> sourceProviders)
        {
            _sourceProviders = sourceProviders.ToArray();
        }

        private IEnumerable<ISearchDocumentSource> DocumentSources =>
            _sourceProviders.SelectMany(sp => sp.GetDocumentSources());


        public void Initialize()
        {
            ICollection<CultureInfo> activeCultures;
            var culturesToPopulate = new List<CultureInfo>();

            using (new DataConnection())
            {
                activeCultures = DataLocalizationFacade.ActiveLocalizationCultures.Evaluate();
            }

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

            PopulateIndex(culturesToPopulate);

            IsInitialized = true;
        }


        private void PopulateIndex(ICollection<CultureInfo> cultures)
        {
            var sources = DocumentSources.Evaluate();

            foreach (var culture in cultures)
            {
                using (new MeasureTime($"Building index for culture '{culture.Name}'"))
                {
                    foreach (var source in sources)
                    {
                        var customFields = source.CustomFields.Evaluate();

                        IndexDocuments(culture, source.GetAllSearchDocuments(culture), customFields);
                    }

                    // TODO: optimize the index here?
                }
            }
        }


        private void UpdateDirectory(CultureInfo culture, Action<IndexWriter> action, bool optimize = false)
        {
            var directory = _directories[culture];

            // TODO: cache and reuse the writer instance

            lock (this)
            {
                using (var writer = new IndexWriter(directory,
                    new StandardAnalyzer(Version.LUCENE_30),
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

        private void IndexDocuments(CultureInfo culture, IEnumerable<SearchDocument> documents, ICollection<DocumentField> customFields)
        {
            UpdateDirectory(culture, writer =>
            {
                foreach (var document in documents)
                {
                    var doc = ToLuceneDocument(document, customFields.Evaluate());

                    writer.AddDocument(doc);
                }
            }); 
        }

        private Document ToLuceneDocument(SearchDocument document, ICollection<DocumentField> customFields)
        {
            var fields = new List<Field>(new[]
                {
                    new Field(Constants.FieldNames.source, document.Source, Field.Store.YES, Field.Index.NOT_ANALYZED),
                    new Field(Constants.FieldNames.id, document.Id, Field.Store.YES, Field.Index.NOT_ANALYZED),
                    new Field(Constants.FieldNames.entityToken, document.SerializedEntityToken, Field.Store.YES, Field.Index.NO),
                    new Field(Constants.FieldNames.label, document.Label, Field.Store.YES, Field.Index.ANALYZED)
                });

            if (!string.IsNullOrWhiteSpace(document.ElementBundleName))
            {
                fields.Add(new Field(Constants.FieldNames.version, document.ElementBundleName, Field.Store.YES, Field.Index.NO));
            }

            if (document.FullText != null)
            {
                string fullText = string.Join(" ", document.FullText);
                if (!string.IsNullOrWhiteSpace(fullText))
                {
                    fields.Add(new Field(Constants.FieldNames.fulltext, fullText, Field.Store.NO, Field.Index.ANALYZED));
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

        public void AddDocument(CultureInfo cultureInfo, SearchDocument document)
        {
            var doc = ToLuceneDocument(document);
            if (doc == null) return;

            UpdateDirectory(cultureInfo, writer =>
            {
                writer.AddDocument(doc);
            });
        }


        public void UpdateDocument(CultureInfo cultureInfo, SearchDocument document)
        {
            var doc = ToLuceneDocument(document);
            if(doc == null) return;

            UpdateDirectory(cultureInfo, writer =>
            {
                writer.UpdateDocument(new Term(Constants.FieldNames.id, document.Id), doc);
            });
        }

        private Document ToLuceneDocument(SearchDocument document)
        {
            var source = document.Source;
            var documentSource = SearchFacade.DocumentSources.FirstOrDefault(ds => ds.Name == source);
            if (documentSource == null) return null;

            var fields = documentSource.CustomFields.Evaluate();
            return ToLuceneDocument(document, fields);
        }

        public void RemoveDocument(CultureInfo cultureInfo, string documentId)
        {
            UpdateDirectory(cultureInfo, writer =>
            {
                writer.DeleteDocuments(new Term(Constants.FieldNames.id, documentId));
            });
        }

        public void Rebuild(CultureInfo cultureInfo, string source)
        {
            throw new NotImplementedException();
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

        public void RebuildAll()
        {
            foreach (var culture in DataLocalizationFacade.ActiveLocalizationCultures)
            {
                UpdateDirectory(culture, writer =>
                {
                    writer.DeleteAll();
                });

                PopulateIndex(new[] {culture});
            }
        }

        public void PopulateCollection(CultureInfo culture)
        {
            PopulateIndex(new [] {culture});
        }

        public void DropCollection(CultureInfo culture)
        {
            UpdateDirectory(culture, writer =>
            {
                writer.DeleteAll();
            });
        }

        public void DeleteDocumentsBySource(string sourceName)
        {
            using (new MeasureTime($"Deleting documents from source '{sourceName}'"))
            {
                foreach (var culture in DataLocalizationFacade.ActiveLocalizationCultures)
                {
                    UpdateDirectory(culture, writer =>
                    {
                        writer.DeleteDocuments(new Term(Constants.FieldNames.source, sourceName));
                    });
                }
            }
        }

        public void PopulateDocumentsFromSource(string sourceName)
        {
            var source = DocumentSources.FirstOrDefault(ds => ds.Name == sourceName);
            if (source == null)
            {
                return;
            }

            var customFields = source.CustomFields.Evaluate();

            foreach (var culture in DataLocalizationFacade.ActiveLocalizationCultures)
            {
                using (new MeasureTime($"Indexing documents from source '{sourceName}' for '{culture.Name}' culture"))
                {
                    IndexDocuments(culture, source.GetAllSearchDocuments(culture), customFields);
                }
            }
        }

        private class MeasureTime : IDisposable
        {
            private readonly Stopwatch _stopwatch = new Stopwatch();

            public MeasureTime(string message)
            {
                Log.LogInformation(LogTitle, message);
                _stopwatch.Start();
            }

            public void Dispose()
            {
                _stopwatch.Stop();
                Log.LogInformation(LogTitle, $"Completed in {_stopwatch.ElapsedMilliseconds} ms");
            }
        }
    }
}
