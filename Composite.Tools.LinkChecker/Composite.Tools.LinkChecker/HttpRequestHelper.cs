using System;
using System.Diagnostics;
using System.IO;
using System.Net;

namespace Composite.Tools.LinkChecker
{
    static class HttpRequestHelper
    {
        private const int PageRenderingTimeout = 5000;
        private const int UrlCheckingTimeout = 5000;

        public static bool MakeHeadRequest(string url)
        {
            try
            {
                var request = (HttpWebRequest)HttpWebRequest.Create(url);

                request.UserAgent = @"Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US; rv:1.9.1.5) Gecko/20091102 Firefox/3.5.5";
                request.Timeout = UrlCheckingTimeout;

                // Get only the header information -- no need to download any content
                request.Method = "HEAD";
                // Get the response
                var response = (HttpWebResponse)request.GetResponse();
                // Get the status code
                int statusCode = (int)response.StatusCode;
                // Good requests
                if (statusCode >= 100 && statusCode < 400)
                {
                    return true;
                }

                // Server Errors
                if (statusCode >= 500 && statusCode <= 510)
                {
                    Debug.Write(String.Format("The remote server has thrown an internal error. Url is not valid: {0}", url));
                    return false;
                }
            }
            catch (WebException ex)
            {
                // 400 errors
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    return false;
                }

                Debug.Write(String.Format("Unhandled status [{0}] returned for url: {1}", ex.Status, url));
            }
            catch (Exception ex)
            {
                Debug.Write(String.Format("Could not test url {0}; throws : {1}", url, ex));
            }

            return false;
        }

        public static PageRenderingResult RenderPage(string url, out string responseBody, out string errorMessage)
        {
            try
            {
                var request = WebRequest.Create(url) as HttpWebRequest;

                request.UserAgent = @"Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US; rv:1.9.1.5) Gecko/20091102 Firefox/3.5.5";
                request.Timeout = PageRenderingTimeout;
                request.AllowAutoRedirect = false; // Some pages may contain redirects to other pages/different websites
                request.Method = "GET";


                var response = request.GetResponse() as HttpWebResponse;
                int statusCode = (int)response.StatusCode;

                if (statusCode == 200)
                {
                    using (var responseStream = response.GetResponseStream())
                    {
                        responseBody = new StreamReader(responseStream).ReadToEnd();
                    }
                    errorMessage = null;
                    return PageRenderingResult.Successful;
                }

                if (statusCode == 301 || statusCode == 302)
                {
                    responseBody = null;
                    errorMessage = null;
                    return PageRenderingResult.Redirect;
                }

                errorMessage = Localization.BrokenLinkReport_HttpStatus(statusCode);
            }
            catch (WebException ex)
            {
                var webResponse = ex.Response as HttpWebResponse;
                if (webResponse != null && webResponse.StatusCode != HttpStatusCode.OK)
                {
                    if (webResponse.StatusCode == HttpStatusCode.NotFound)
                    {
                        errorMessage = responseBody = null;
                        return PageRenderingResult.NotFound;
                    }
                    errorMessage = Localization.BrokenLinkReport_HttpStatus((int)webResponse.StatusCode + " " + webResponse.StatusCode);
                }
                else
                {
                    errorMessage = ex.ToString();
                }
            }

            responseBody = null;
            return PageRenderingResult.Failed;
        }
    }
}
