using System;
using System.Xml.Linq;
using DocumentFormat.OpenXml.Packaging;
using Composite.Core.Xml;
using System.Xml.XPath;
using Composite.Core;
using Composite.Media.WordDocumentViewer.Renderers.Images;
using Composite.Media.WordDocumentViewer.Renderers;
using Composite.Functions;
using Composite.Data.Types;

namespace Composite.Media.WordDocumentViewer
{
	/// <summary>
	/// Composite.Media.WordDocumentViewer.DocumentXsltExtensionsFunction
	/// </summary>
	public class DocumentXsltExtensionsFunction
	{
		public static XsltExtensionDefinition<DocumentRenderer> XsltExtensions()
		{
			return new XsltExtensionDefinition<DocumentRenderer>
				{
					EntensionObject = new DocumentRenderer(),
					ExtensionNamespace = "#DocumentRenderer"
				};
		}

		[FunctionParameterDescription("mediaFile","MediaFile","")]
		[FunctionParameterDescription("split","Split","")]
		public static XDocument Render(string mediaFile, bool split)
		{
			return DocumentRenderer.RenderAsDocument(mediaFile, split);
		}
	}

	public class DocumentRenderer
	{

		/// <summary>
		/// Return rendered html of media file(docs)
		/// </summary>
		/// <param name="mediaFile"></param>
		/// <returns>XPathNavigator</returns>
		public XPathNavigator RenderMediaFile(IMediaFile mediaFile, bool split)
		{
			return RenderAsDocument(mediaFile.KeyPath, split).CreateNavigator();
		}
		/// <summary>
		/// Return rendered html of media file(docs)
		/// </summary>
		/// <param name="mediaFile"></param>
		/// <returns>XPathNavigator</returns>
		public XPathNavigator Render(string mediaFile, bool split)
		{
			return RenderAsDocument(mediaFile, split).CreateNavigator();
		}

		/// <summary>
		/// Return rendered html of media file(docs)
		/// </summary>
		/// <param name="mediaFile"></param>
		/// <returns>XDocument</returns>
		public static XDocument RenderAsDocument(string mediaFile, bool split)
		{
			XDocument document;
				try
				{
					LogTime("Empty");
					using (var stream = DocumentFacade.GetMediaFileStream(mediaFile))
					{
						using (var wpdocument = WordprocessingDocument.Open(stream, false))
						{
							LogTime("using (WordprocessingDocument wpdocument = DocumentFacade.GetWordprocessingDocument(mediaFile))");
							var documentStyles = new DocumentStyles(wpdocument);
							LogTime("var documentStyles = new DocumentStyles(wpdocument);");
							document = DocumentFacade.TransformCached(wpdocument, mediaFile);
							LogTime("document = DocumentFacade.Transform(wpdocument);");
							if (split)
								document = SplitRenderer.Render(document, documentStyles);
							LogTime("document = SplitRenderer.Render(document, documentStyles);");
							document = ImageRenderer.ResolveImagePath(document, mediaFile);
							LogTime("document = ImageRenderer.ResolveImagePath(document, mediaFile);");
							document = ListRenderer.RenderLists(document, documentStyles);
							LogTime("document = ListRenderer.RenderLists(document, documentStyles);");
							document = CleanRenderer.Render(document);
							LogTime("document = CleanRenderer.Render(document);");
							document = MarkupRenderer.Render(document);
							LogTime("document = MarkupRenderer.Render(document);");
							document = LinkRenderer.RenderLinks(document, wpdocument.MainDocumentPart.ExternalRelationships);
							document = YoutubeRenderer.RenderYoutubeLinks(document);
						}
					}
					
				}
			catch(Exception e)
			{
				document = new XDocument(
					new XElement(Namespaces.Xhtml + "span",
						new XAttribute("class", "Error"),
						e.Message
					)
				);
                Log.LogError(nameof(DocumentRenderer), e);
			}

			return document;
		}

		//private static DateTime lastTime = new DateTime();

		private static void LogTime(string text)
		{
			//Logging.LoggingService.LogInformation("WordDocumentViewer"+text, (DateTime.Now - lastTime).TotalMilliseconds.ToString());
			//lastTime = DateTime.Now;
			//;
		}
	}
}
