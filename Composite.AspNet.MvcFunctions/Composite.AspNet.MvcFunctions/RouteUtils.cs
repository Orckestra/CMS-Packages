using System;
using System.Web;
using System.Web.Routing;

namespace Composite.AspNet.MvcFunctions
{
    internal static class RouteUtils
    {
        public static RouteData GetRouteDataByUrl(RouteCollection routeCollection, string url)
        {
            return routeCollection.GetRouteData(new RewrittenHttpContextBase(url));
        }

        private class RewrittenHttpContextBase : HttpContextBase
        {
            private readonly HttpRequestBase _mockHttpRequestBase;

            public RewrittenHttpContextBase(string appRelativeUrl)
            {
                this._mockHttpRequestBase = new MockHttpRequestBase(appRelativeUrl);
            }


            public override HttpRequestBase Request
            {
                get
                {
                    return _mockHttpRequestBase;
                }
            }

            private class MockHttpRequestBase : HttpRequestBase
            {
                private readonly string _appRelativeUrl;

                public MockHttpRequestBase(string appRelativeUrl)
                {
                    this._appRelativeUrl = appRelativeUrl;
                }

                public override string AppRelativeCurrentExecutionFilePath
                {
                    get { return _appRelativeUrl; }
                }

                public override string PathInfo
                {
                    get { return ""; }
                }
            }
        }
    }
}
