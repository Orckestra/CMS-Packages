using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Linq;
using Composite.Core.WebClient.Renderings.Page;
using Composite.Core.Xml;

namespace Composite.AppFeed.Server
{
	[DataContract]
	public class Content
	{
		private string _groupViewImage = null;
		private string _backupGroupViewImage = null;
		private string _executedDescription = null;

		public Content()
		{
			_backupGroupViewImage = ImageUtilities.RandomGenericImage(ImageType.GroupView);
		}

		[DataMember]
		public string Id { get; set; }

		[DataMember]
		public string GroupName { get; set; }

		[DataMember]
		public string Title { get; set; }

		[DataMember]
		public string SubTitle { get; set; }

		[DataMember]
		public string Url { get; set; }

		public string ImageId
		{
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					this.GroupViewImage = ImageUtilities.GetImagePathFromId(value, Settings.GroupViewImageWidth, Settings.GroupViewImageHeight, "fill");

					if (string.IsNullOrEmpty(this.Image))
					{
						this.Image = ImageUtilities.GetImagePathFromId(value, Settings.PrimaryImageWidth, Settings.PrimaryImageHeight, "fit");
					}
				}
			}
		}


		[DataMember]
		public string Description
		{
			get
			{
				return _executedDescription;
			}
			set
			{
				try
				{
					if (string.IsNullOrEmpty(value))
					{
						_executedDescription = new XhtmlDocument().ToString();
						return;
					}

					XhtmlDocument doc = XhtmlDocument.Parse(value);

					if (value.Contains("<f:function"))
					{
						Composite.Functions.FunctionContextContainer fContext = new Composite.Functions.FunctionContextContainer();
						PageRenderer.ExecuteEmbeddedFunctions(doc.Root, fContext);
						PageRenderer.NormalizeXhtmlDocument(doc);
					}

					XName aName = XName.Get("a", "http://www.w3.org/1999/xhtml");

					foreach (var link in doc.Descendants(aName))
					{
						var href = link.Attribute("href");

						if (href != null)
						{
							string url = href.Value;

							if (url.IndexOf("~") == 0)
								url = url.Substring(1);

							href.Value = url;
						}
					}

					// Find an image
					XName imgName = XName.Get("img", "http://www.w3.org/1999/xhtml");
					var imageElement = doc.Descendants(imgName).Where(f => f.Attribute("src") != null).FirstOrDefault();

					if (imageElement != null)
					{
						string mediaPath = imageElement.Attribute("src").Value;

						if (mediaPath.Contains("/media("))
						{
							mediaPath = mediaPath.Contains("?") ? mediaPath.Split('?').First() : mediaPath;
							mediaPath = mediaPath + string.Format("?w={0}&h={1}&action=fill", Settings.GroupViewImageWidth, Settings.GroupViewImageHeight);
						}

						_backupGroupViewImage = mediaPath;
					}

					_executedDescription = doc.ToString().Replace("xhtml:", "").Replace("xmlns:xhtml=", "xmlns=").Replace("x:", "").Replace("xmlns:x=", "xmlns=");
				}
				catch (Exception ex)
				{
					Composite.Core.Log.LogError("App Feed", ex);
				}
			}
		}


		[DataMember]
		public string Image { get; set; }


		[DataMember]
		public string GroupViewImage
		{
			get
			{
				if (_groupViewImage != null)
				{
					return _groupViewImage;
				}

				return _backupGroupViewImage;
			}
			set
			{
				_groupViewImage = value;
			}
		}


		[DataMember]
		public string GroupKey
		{
			get
			{
				if (!FeedManager.Instance.Groups.Any(g => g.Name == this.GroupName))
				{
					throw new InvalidOperationException(string.Format("Unknown group '{0}' - it has not been registered?", this.GroupName));
				}
				Group group = FeedManager.Instance.Groups.First(g => g.Name == this.GroupName);
				return group.Key;
			}
			internal set { throw new NotImplementedException("Do not set the GroupKey"); } // DataContractJsonSerializer is lame
		}
	}
}