using System;
using System.Xml.Linq;
using Composite.Data;
using Composite.Data.Types;
using Composite.Functions;
using Composite.Media.IssuuPlayer.Plugins;

namespace Composite.Media.IssuuPlayer
{
	public class Functions
	{
		private static object _lock = new object();

		[FunctionParameterDescription("mediaFile", "MediaFile", "MediaFile")]
		[FunctionParameterDescription("apiKey", "ApiKey", "ApiKey")]
		public static XElement GetDocument(DataReference<IMediaFile> mediaFile, IDataReference apiKey)
		{
			var media = mediaFile.Data;
			using (new IssuuApi(apiKey.Data))
			{
				lock (_lock)
				{
					var document = IssuuPlayerFacade.GetDocument(media);
					if (document == null)
					{
						document = IssuuPlayerFacade.UploadDocument(media);
					}
					else
					{
						var publishDateAttribute = document.AttributeValue("publishDate");
						DateTime publishDate = DateTime.Now;
						DateTime.TryParse(publishDateAttribute, out publishDate);
						if (publishDate < media.LastWriteTime)
						{
							IssuuPlayerFacade.DeleteDocument(media);
							document = IssuuPlayerFacade.UploadDocument(media);
						}
					}
					return document;
				}
			}
		}

		public static Guid GetDefaultApiKeyId()
		{
			return IssuuApi.GetDefaultId();
		}
	}
}