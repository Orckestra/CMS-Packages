using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel.Syndication;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using Composite.Community.Blog.Rss;
using Composite.Core;
using Composite.Core.Extensions;
using Composite.Core.IO;
using Composite.Core.Linq;
using Composite.Core.WebClient;
using Composite.Core.WebClient.Services.WysiwygEditor;
using Composite.Core.Xml;
using Composite.Data;
using Composite.Data.ProcessControlled.ProcessControllers.GenericPublishProcessController;
using Composite.Data.Types;
using Composite.Plugins.Elements.ElementProviders.MediaFileProviderElementProvider;

namespace Composite.Community.Blog
{




	internal static class LinqExtensions
	{
		/// <summary>
		/// Returns the only element of a sequence, or a default value if the sequence is empty or contains more than one element.
		/// </summary>
		public static TSource TheOneOrDefault<TSource>(this IEnumerable<TSource> source)
		{
			var elements = source.Take(2).ToArray();

			return (elements.Length == 1) ? elements[0] : default(TSource);
		}
	}

	internal class BlogImportHelper
	{
		public string FolderFormat = "/BlogMedia/{0:yyyy}/{0:yyyy MM dd} {1}";

		public void Import(string rssPath, Guid pageId)
		{
			using (var conn = new DataConnection(PublicationScope.Unpublished))
			{
				var mapLinks = new Dictionary<string, string>();

				var client = new WebClient();
				XmlReader reader = new SyndicationFeedXmlReader(client.OpenRead(rssPath));


				var feed = SyndicationFeed.Load(reader);
				reader.Close();

				var links = feed.Links.Select(d => d.Uri.ToString()).ToList();
				var defaultAuthor = DataFacade.GetData<Authors>().Select(d => d.Name).TheOneOrDefault() ?? "Anonymous";
				var blogAuthor = feed.Authors.Select(d => d.Name).FirstOrDefault()
					??feed.ElementExtensions.ReadElementExtensions<string>("creator", "http://purl.org/dc/elements/1.1/").FirstOrDefault();;


				foreach (var item in feed.Items)
				{
					using (new DataScope(PublicationScope.Published))
					{
						var itemDate = item.PublishDate == DateTimeOffset.MinValue ? DateTime.Now : item.PublishDate.DateTime;
						foreach (var itemLink in item.Links)
						{
							mapLinks[itemLink.Uri.OriginalString] = BlogFacade.GetBlogInternalPageUrl(itemDate, item.Title.Text, pageId);
						}
					}
				}

				foreach (var item in feed.Items)
				{
					try
					{
						var content = new XDocument();
						string text = null;
						var itemDate = item.PublishDate == DateTimeOffset.MinValue ? DateTime.Now : item.PublishDate.DateTime;



						if (text == null && item.Content != null)
						{
							var syndicationContent = item.Content as TextSyndicationContent;
							if (syndicationContent != null)
							{
								text = syndicationContent.Text;
							}
						}
						if (text == null)
						{

							text = item.ElementExtensions.ReadElementExtensions<string>("encoded", "http://purl.org/rss/1.0/modules/content/")
									.FirstOrDefault();
						}
						if (text == null && item.Summary != null)
						{
							text = item.Summary.Text;
						}

						content = MarkupTransformationServices.TidyHtml(text).Output;

						//somewhere empty <title></title> created
						foreach (var title in content.Descendants(Namespaces.Xhtml + "title").ToList())
						{
							if(string.IsNullOrWhiteSpace(title.Value))
								title.Remove();
						}


						foreach (var img in content.Descendants(Namespaces.Xhtml + "img"))
						{
							var src = img.GetAttributeValue("src");
							if (!string.IsNullOrEmpty(src))
							{
								foreach (var link in links)
								{
									if (src.StartsWith(link))
									{
										var newImage = ImportMedia(src, string.Format(FolderFormat, itemDate, item.Title.Text));
										if (newImage != null)
										{
											img.SetAttributeValue("src", MediaUrlHelper.GetUrl(newImage, true));
										}
										break;
									}
								}
							}
						}

						foreach (var a in content.Descendants(Namespaces.Xhtml + "a"))
						{
							var href = a.GetAttributeValue("href");
							if (!string.IsNullOrEmpty(href))
							{
								foreach (var link in links)
								{
									if (href.StartsWith(link))
									{
										if(mapLinks.ContainsKey(href))
										{
											a.SetAttributeValue("href", mapLinks[href]);
										}
										else
										{
											var extension = Path.GetExtension(href).ToLower();
											switch (extension)
											{
												case ".jpg":
												case ".png":
												case ".gif":
												case ".pdf":
												case ".doc":
												case ".docx":
													var newMedia = ImportMedia(href, string.Format(FolderFormat, itemDate, item.Title.Text));
													a.SetAttributeValue("href", MediaUrlHelper.GetUrl(newMedia, true));
													break;
												default:
													a.SetAttributeValue("href", new Uri(href).PathAndQuery);
													break;
											}
										}
										break;
									}
								}
							}
						}

						var blogItem = DataFacade.BuildNew<Entries>();

						var match = Regex.Match(item.Id, @"\b[A-F0-9]{8}(?:-[A-F0-9]{4}){3}-[A-F0-9]{12}\b", RegexOptions.IgnoreCase);
						if (match.Success)
						{
							var id = Guid.Empty;
							Guid.TryParse(match.Groups[0].Value, out id);
							if (id != Guid.Empty && !DataFacade.GetData<Entries>(d => d.Id == id).Any())
							{
								blogItem.Id = id;
							}
						}

						blogItem.Title = item.Title.Text;
						blogItem.PageId = pageId;
						blogItem.Teaser = string.Empty;

						var blogItemAuthor = item.Authors.Select(d => d.Name ?? d.Email).FirstOrDefault() ??
						                     item.ElementExtensions.ReadElementExtensions<string>("creator",
							                     "http://purl.org/dc/elements/1.1/").FirstOrDefault();


						blogItem.Author = ImportAuthor(blogItemAuthor ?? blogAuthor ?? defaultAuthor);

					    var tagType = DataFacade.GetData<TagType>().FirstOrDefault();
					    if (tagType == null)
					    {
                            tagType = DataFacade.BuildNew<TagType>();
                            tagType.Name = "Categories";
                            DataFacade.AddNew(tagType);
                        }

						foreach (var tag in item.Categories)
						{
							ImportTag(tag.Name, tagType.Id);
						}
						blogItem.Tags = string.Join(",", item.Categories.Select(d => d.Name));

						blogItem.Content = content.ToString();
						blogItem.Date = itemDate;

						blogItem.PublicationStatus = GenericPublishProcessController.Draft;
						blogItem = DataFacade.AddNew(blogItem);
						blogItem.PublicationStatus = GenericPublishProcessController.Published;
						DataFacade.Update(blogItem);



						//break;
					}
					catch (Exception ex)
					{
						Log.LogError("Import Blog", ex);
					}
				}

				//1st redirect
				var mapLinks2 = new Dictionary<string, string>();
				foreach (var maplink in mapLinks.ToList())
				{
					var request = (HttpWebRequest)WebRequest.Create(maplink.Key);
					request.AllowAutoRedirect = false;
					var response = (HttpWebResponse)request.GetResponse();
					var location = response.Headers["Location"];
					if (!string.IsNullOrWhiteSpace(location))
					{
						location = new Uri(new Uri(maplink.Key), location).OriginalString;
						foreach (var link in links)
						{
							if (location.StartsWith(link))
							{
								if (!mapLinks.ContainsKey(location))
								{
									mapLinks[location] = maplink.Value;
									mapLinks2[location] = maplink.Value;
								}
							}
						}
					}
				}
				//2nd redirect
				foreach (var maplink in mapLinks2.ToList())
				{
					var request = (HttpWebRequest)WebRequest.Create(maplink.Key);
					request.AllowAutoRedirect = false;
					var response = (HttpWebResponse)request.GetResponse();
					var location = response.Headers["Location"];
					if (!string.IsNullOrWhiteSpace(location))
					{
						location = new Uri(new Uri(maplink.Key), location).OriginalString;
						foreach (var link in links)
						{
							if (location.StartsWith(link))
							{
								if (!mapLinks.ContainsKey(location))
								{
									mapLinks[location] = maplink.Value;
								}
							}
						}
					}
				}


				var mapFile = PathUtil.Resolve(@"~\App_Data\RequestUrlRemappings.xml");
				var map = new XElement("RequestUrlRemappings");
				if (File.Exists(mapFile))
				{
					map = XElement.Load(mapFile);
				}

				map.Add(new XComment(" Imported Blog " + DateTime.Now));
				map.Add(
					mapLinks.Select(d => new XElement("Remapping",
						new XAttribute("requestPath", new Uri(d.Key).PathAndQuery),
						new XAttribute("rewritePath", d.Value)
						))

				);
				map.Add(new XComment(" "));

				map.Save(mapFile);
			}
		}

		private Dictionary<string, IMediaFile> ImportedImages = new Dictionary<string, IMediaFile>();

		private static bool Exists(IMediaFile file)
		{
			return DataFacade.GetData<IMediaFile>().Any(x => x.FolderPath == file.FolderPath && x.FileName == file.FileName);
		}

		private IMediaFile ImportMedia(string src, string folder)
		{
			if (!ImportedImages.ContainsKey("src"))
			{
				ImportedImages[src] = null;
				ForceMediaFolder(folder);

				var client = new WebClient
				{
				    Encoding = System.Text.Encoding.UTF8
				};
			    client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");

				var stream = client.OpenRead(src);

				var filename = CleanFileName(Path.GetFileName(src));
				var title = Path.GetFileNameWithoutExtension(src);

				var mediaFile = new WorkflowMediaFile
				{
				    FileName = Path.GetFileName(filename),
				    FolderPath = folder,
				    Title = title,
				    Description = string.Empty,
				    Culture = Composite.C1Console.Users.UserSettings.ActiveLocaleCultureInfo.Name,
				    MimeType = MimeTypeInfo.GetCanonicalFromExtension(Path.GetExtension(filename))
				};

			    if (mediaFile.MimeType == MimeTypeInfo.Default)
				{
					mediaFile.MimeType = MimeTypeInfo.GetCanonical(client.ResponseHeaders["content-type"]);
				}

				using (Stream readStream = stream)
				{
					using (Stream writeStream = mediaFile.GetNewWriteStream())
					{
						readStream.CopyTo(writeStream);
					}
				}

				int counter = 0;
				string extension = Path.GetExtension(mediaFile.FileName);
				string name = mediaFile.FileName.GetNameWithoutExtension();
				while (Exists(mediaFile))
				{
					counter++;
					mediaFile.FileName = name + counter.ToString() + extension;
				}
				IMediaFile addedFile = DataFacade.AddNew<IMediaFile>(mediaFile);

				ImportedImages[src] = addedFile;
			}
			return ImportedImages[src];
		}

		private void ImportTag(string tagName, Guid tagType)
		{
			if (!DataFacade.GetData<Tags>(d => d.Tag == tagName).Any())
			{
				var tag = DataFacade.BuildNew<Tags>();
			    tag.Type = tagType;
				tag.Tag = tagName;
				DataFacade.AddNew(tag);
			}
		}

		private Guid ImportAuthor(string authorName)
		{
			var author = DataFacade.GetData<Authors>(d => d.Name == authorName).FirstOrDefault();
			if (author != null)
				return author.Id;


			author = DataFacade.BuildNew<Authors>();
			author.Name = authorName;
			author = DataFacade.AddNew(author);
			return author.Id;

		}

		private static string CleanFileName(string fileName)
		{
			return Path.GetInvalidFileNameChars().Aggregate(fileName, (current, c) => current.Replace(c.ToString(), string.Empty));
		}

		private IMediaFileFolder ForceMediaFolder(string path)
		{
			if (string.IsNullOrWhiteSpace(path))
			{
				return null;
			}
			if (path == "/") //?
			{
				return DataFacade.GetData<IMediaFileFolder>(d => d.Path == path).FirstOrDefault();
			}
			ForceMediaFolder(GetParentPath(path));

			var folder = DataFacade.GetData<IMediaFileFolder>().FirstOrDefault(d => d.Path == path);
			if (folder == null)
			{
				folder = DataFacade.BuildNew<IMediaFileFolder>();
				folder.Title = Path.GetFileName(path);
				folder.Path = path;
				folder = DataFacade.AddNew<IMediaFileFolder>(folder);
			}
			return folder;
		}

		private Func<string, string> GetParentPath = path => path.Substring(0, path.LastIndexOf("/") > 0 ? path.LastIndexOf("/") : 0);

	}
}
