using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;
using Composite.Data;
using Composite.Data.Types;
using Composite.Core.IO;
using DocumentFormat.OpenXml.Packaging;


namespace Composite.Media.WordDocumentViewer
{
    public static class DocumentFacade
    {
        private static readonly XNamespace XmlNs = "http://www.w3.org/1999/xhtml";
        private static readonly TransformCache _transformCache = new TransformCache();


        public static Stream GetMediaFileStream(string mediaFile)
        {
            var file = DataFacade.GetData<IMediaFile>()
                .FirstOrDefault(f => f.CompositePath == mediaFile || f.KeyPath == mediaFile);

            if (file == null)
            {
                throw new FileNotFoundException($"File '{mediaFile}' was not found.");
            }
            return file.GetReadStream();
        }

        internal static string XsltPath
            => PathUtil.Resolve("~/App_Data/Composite.Media.WordDocumentViewer/DocX2Html.xsl");

        /// <summary>
        /// Get WordprocessingDocument
        /// </summary>
        /// <param name="mediaFile">Media file</param>
        /// <returns>retuns WordprocessingDocument</returns>
        public static WordprocessingDocument GetWordprocessingDocument(string mediaFile)
        {
            return WordprocessingDocument.Open(GetMediaFileStream(mediaFile), false);
        }

        /// <summary>
        /// Transfom from OpenXml format to the xhtml
        /// </summary>
        /// <param name="document">WordprocessingDocument document</param>
        /// <returns></returns>
        public static XDocument Transform(WordprocessingDocument document)
        {
            var result = new XDocument();
            using (XmlWriter writer = result.CreateWriter())
            {
                var transform = new XslCompiledTransform();
                transform.Load(XsltPath);

                transform.Transform(document.MainDocumentPart.GetXDocument().CreateReader(), writer);
            }
            return result;
        }

        /// <summary>
        /// Transfom from OpenXml format to the xhtml
        /// </summary>
        /// <param name="document">WordprocessingDocument document</param>
        /// <returns></returns>
        public static XDocument TransformCached(WordprocessingDocument document, string mediaFile)
        {
            return _transformCache.Get(document, mediaFile);
        }
    }

}
