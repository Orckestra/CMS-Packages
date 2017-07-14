using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Composite.Data;
using System.Web.Caching;
using DocumentFormat.OpenXml.Packaging;
using System.Xml.Linq;
using Composite.Data.Types;
using System.IO;
using System.Xml;

namespace Composite.Media.WordDocumentViewer
{
	public class TransformCache
	{
		private Dictionary<string, string> _innerCache = new Dictionary<string, string>();

		public TransformCache()
		{
			DataEventSystemFacade.SubscribeToDataAfterAdd<IMediaFile>(OnDataChanged, true);
//			DataEventSystemFacade.SubscribeToDataAfterMove<IMediaFile>(OnDataChanged);
			DataEventSystemFacade.SubscribeToDataBeforeUpdate<IMediaFile>(OnDataChanged, true);
			DataEventSystemFacade.SubscribeToDataDeleted<IMediaFile>(OnDataChanged, true);
		}

		private void ClearCache(object source, FileSystemEventArgs e)
		{
			lock (this)
			{
				_innerCache = new Dictionary<string, string>();
			}
		}

		private void OnDataChanged(object sender, DataEventArgs dataeventargs)
		{
			var data = dataeventargs.Data as IMediaFile;
			if (data == null)
			{
				return;
			}

			string cacheKey = data.GetProperty<string>("KeyPath");
			lock (this)
			{
				if (_innerCache.ContainsKey(cacheKey))
				{
					_innerCache.Remove(cacheKey);
				}
			}

		}

		public XDocument Get(WordprocessingDocument wpdocument, string mediaFile)
		{
			XDocument result = null;;
			lock (this)
			{
				//#warning Plaese, enable cache
				var cacheDisabled = false;
				if(!_innerCache.ContainsKey(mediaFile) || cacheDisabled)
				{
					result = DocumentFacade.Transform(wpdocument);
					var stringBuilder = new StringBuilder();
					using (var writer = new StringWriter(stringBuilder))
					{
						result.Save(writer, SaveOptions.DisableFormatting);
					}
					_innerCache[mediaFile] = stringBuilder.ToString();
				}
				else
				{
					using (var reader = new StringReader(_innerCache[mediaFile]))
					{
						result = XDocument.Load(reader, LoadOptions.PreserveWhitespace);
					}
				}
					
			}
			return result;
		}
	}
}
