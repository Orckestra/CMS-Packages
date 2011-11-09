using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.IO;
using System.Xml;
using System.Text;
using Composite.Data;

namespace Composite.Media.ImageGallery.Flickr
{
    public class Functions
    {
        public Functions()
        {
            //
            // TODO: Add constructor logic here
            //
        }

		public static string GetMethodURL(string methodName, DataReference<ApiKey> apiKey, string userID, string photosetID, string extras)
        {
           
			string apiKeyValue = apiKey.Data.Key;

            string urlFormat = "http://api.flickr.com/services/rest/?method={0}&api_key={1}&format=rest{2}";

            StringBuilder extraParam = new StringBuilder();

            if (!string.IsNullOrEmpty(photosetID))
                extraParam.Append("&photoset_id=").Append(photosetID);

            if (!string.IsNullOrEmpty(extras))
                extraParam.Append("&extras=").Append(extras);

            if (!string.IsNullOrEmpty(userID))
                extraParam.Append("&user_id=").Append(userID);

			//if (!string.IsNullOrEmpty(userName) && methodName.Equals("flickr.people.findByUsername"))
			//    extraParam.Append("&username=").Append(userName);


            return string.Format(urlFormat, methodName, apiKeyValue, extraParam.ToString());
        }

        public static string GetUserId(string apiKey, string userName)
        {
            string id = string.Empty;
            string uri = @"http://api.flickr.com/services/rest/?method=flickr.people.findByUsername&api_key={0}&username={1}&format=rest";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(string.Format(uri, apiKey, userName));
            WebResponse response = request.GetResponse();
            Stream strm = response.GetResponseStream();
            XmlTextReader reader = new XmlTextReader(strm);
            while (reader.Read())
            {
                if (reader.Name == "user")
                {
                    id = reader.GetAttribute("id");
                    break;
                }
            }
            return id;
        }
    }
}