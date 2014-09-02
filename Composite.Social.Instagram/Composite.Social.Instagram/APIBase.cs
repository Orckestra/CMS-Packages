using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using Composite.Core;
using System.Web.Script.Serialization;


namespace Composite.Social.Instagram
{
    public class ApiBase
    {
        protected static ICache Cache;
        protected static readonly object Threadlock = new object();
        protected static JavaScriptSerializer Serializer = new JavaScriptSerializer();

        public static T DeserializeObject<T>(string json)
        {
            return Serializer.ConvertToType<T>(Serializer.DeserializeObject(json));
        }

        public static string SerializeObject(object value)
        {
            return Serializer.Serialize(value);
        }

        public string RequestPostToUrl(string url, NameValueCollection postData, WebProxy proxy)
        {
            if (string.IsNullOrEmpty(url))
                return null;

            if (url.IndexOf("://", StringComparison.Ordinal) <= 0)
                url = "http://" + url.Replace(",", ".");

            try
            {
                using (var client = new WebClient())
                {
                    if (proxy != null)
                        client.Proxy = proxy;

                    var response = client.UploadValues(url, postData);
                    var enc = new UTF8Encoding();
                    var outp = enc.GetString(response);
                    return outp;
                }
            }
            catch (Exception ex)
            {
                Log.LogError("Composite.Social.Instagram - RequestPostToUrl", "Request URL: " + url + ". " + ex.Message);
            }

            return null;
        }

        public string RequestDeleteToUrl(string url, WebProxy proxy)
        {
            if (string.IsNullOrEmpty(url))
                return null;

            if (url.IndexOf("://", StringComparison.Ordinal) <= 0)
                url = "http://" + url.Replace(",", ".");

            try
            {
                var request = WebRequest.Create(url);
                if (proxy != null)
                    request.Proxy = proxy;
                request.Method = "DELETE";
                var str = "";
                var resp = request.GetResponse();
                var receiveStream = resp.GetResponseStream();
                var encode = Encoding.GetEncoding("utf-8");
                var readStream = new StreamReader(receiveStream, encode);
                var read = new Char[256];
                int count = readStream.Read(read, 0, 256);
                while (count > 0)
                {
                    str = str + new String(read, 0, count);
                    count = readStream.Read(read, 0, 256);
                }
                readStream.Close();
                receiveStream.Close();
                //out
                return str;
            }

            catch (Exception ex)
            {
                Log.LogError("Composite.Social.Instagram - RequestDeleteToUrl", "Request URL: " + url + ". " + ex.Message);
            }

            return null;
        }

        public string RequestGetToUrl(string url, WebProxy proxy)
        {
            if (string.IsNullOrEmpty(url))
                return null;

            if (url.IndexOf("://", StringComparison.Ordinal) <= 0)
                url = "http://" + url.Replace(",", ".");

            try
            {
                using (var client = new WebClient())
                {
                    if (proxy != null)
                        client.Proxy = proxy;
                    var response = client.DownloadData(url);
                    var enc = new UTF8Encoding();
                    var outp = enc.GetString(response);
                    return outp;
                }
            }
            catch (Exception ex)
            {
                Log.LogError("Composite.Social.Instagram - RequestGetToUrl", "Request URL: " + url + ". " + ex.Message);
            }
            return null;
        }
    }
}
