using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Facebook;
using System.Xml.Linq;

namespace Composite.Media.ImageGallery.Facebook
{
	public class Functions
	{
		public Functions()
		{
			//
			// TODO: Add constructor logic here
			//
		}
		public static IEnumerable<XElement> GetAlbums(string accessToken, string objectUniqueID, string albumTypes)
		{
			var albumTypesValues = albumTypes.Split(',');
			var fb = new FacebookClient();
			if (!string.IsNullOrEmpty(accessToken.Trim()))
				fb = new FacebookClient(HttpUtility.UrlDecode(accessToken));
			List<XElement> albums = new List<XElement>();
			try
			{
				dynamic result = fb.Get(objectUniqueID + "/albums");
				foreach (var album in result.data)
				{
					try
					{
						if (albumTypesValues.Contains((string)album.type))
						{
							albums.Add(new XElement("Album",
													 new XAttribute("Id", album.id),
													 new XAttribute("Link", album.link),
													 new XAttribute("Name", album.name),
													 new XAttribute("Cover_Photo", album.cover_photo),
													 new XAttribute("Count", album.count),
													 new XAttribute("Type", album.type)));
						}
					}
					catch { }
				}
			}
			catch (Exception ex)
			{
				albums.Add(new XElement("Error",
								 new XAttribute("Message", ex.Message)));
			}
			return albums;
		}

		public static IEnumerable<XElement> GetPhotos(string accessToken, string albumID)
		{
			var fb = new FacebookClient();
			if (!string.IsNullOrEmpty(accessToken.Trim()))
				fb = new FacebookClient(HttpUtility.UrlDecode(accessToken));
			List<XElement> albums = new List<XElement>();
			try
			{
				dynamic result = fb.Get(albumID + "/photos");
				foreach (var photo in result.data)
				{
					try
					{
						albums.Add(new XElement("Photo",
												 new XAttribute("Id", photo.id),
												 new XAttribute("Name", photo.name ?? string.Empty),
												 new XAttribute("Link", photo.link ?? string.Empty),
												 new XAttribute("Picture", photo.picture),
												 new XAttribute("Source", photo.source)));
					}
					catch { }
				}
			}
			catch (Exception ex)
			{
				albums.Add(new XElement("Error",
								 new XAttribute("Message", ex.Message)));
			}
			return albums;
		}
	}
}