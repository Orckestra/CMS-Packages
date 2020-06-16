using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Routing;
using System.Xml;
using Composite.AspNet.MvcFunctions;
using Composite.AspNet.MvcFunctions.FunctionProvider;
using Composite.C1Console.Security;
using Composite.Core.IO;
using Composite.Core.Routing;
using Composite.Core.Routing.Pages;
using Composite.Core.WebClient.Renderings.Page;
using Composite.Core.Xml;
using Composite.Data;
using Composite.Functions;

namespace Composite.Plugins.Functions.FunctionProviders.MvcFunctions
{
    internal abstract class MvcFunctionBase: IFunction, IDynamicFunction
    {
        protected readonly FunctionCollection _functionCollection;
        private readonly IList<ParameterProfile> _parameters = new List<ParameterProfile>();

        protected List<Tuple<Type, IDataUrlMapper>> _dataUrlMappers = new List<Tuple<Type, IDataUrlMapper>>();

        protected MvcFunctionBase(string @namespace, string name, string description, FunctionCollection functionCollection)
        {
            Verify.ArgumentNotNullOrEmpty(@namespace, "namespace");
            Verify.ArgumentNotNullOrEmpty(name, "name");
            Verify.ArgumentNotNull(functionCollection, "functionCollection");

            Namespace = @namespace;
            Name = @name;
            Description = description;
            _functionCollection = functionCollection;
        }

        public string Name { get; protected set; }
        public string Namespace { get; protected set; }
        public string Description { get; protected set; }
        public bool RequireAsyncHandler { get; protected set; }

        protected abstract bool HandlesPathInfo { get; }

        protected abstract string GetMvcRoute(ParameterList parameters);

        protected abstract string GetBaseMvcRoute(ParameterList parameters);

        internal virtual IEnumerable<ParameterInfo> GetParameterInformation()
        {
            return Enumerable.Empty<ParameterInfo>();
        }

        public virtual void AddParameter(ParameterProfile parameterProfile)
        {
            _parameters.Add(parameterProfile);
        }

        public Type ReturnType => typeof (XhtmlDocument);

        public virtual IEnumerable<ParameterProfile> ParameterProfiles => _parameters;


        public EntityToken EntityToken => new MvcFunctionEntityToken(this);

        public object Execute(ParameterList parameters, FunctionContextContainer context)
        {
            var route = GetMvcRoute(parameters);

            var routeData = GetRouteData(route, parameters);
            if (routeData == null)
            {
                return null;
            }

            bool routeResolved = false;

            XhtmlDocument result;

            try
            {
                result = ExecuteRoute(routeData, parameters, out routeResolved);
            }
            catch (Exception ex)
            {
                if (ex is ThreadAbortException
                    || ex is HttpException
                    || ex is ThreadInterruptedException
                    || ex is StackOverflowException
                    || ex is OutOfMemoryException)
                {
                    throw;
                }

                throw new InvalidOperationException($"Error executing route '{route}'", ex);
            }
            finally
            {
                if (routeResolved && HandlesPathInfo && C1PageRoute.GetPathInfo() != null)
                {
                    C1PageRoute.RegisterPathInfoUsage();
                }
            }

            if (result != null)
            {
                ProcessDocument(result, parameters);
            }

            return result;
        }

        public async Task<object> ExecuteAsync(ParameterList parameters, FunctionContextContainer context)
        {
            string virtualRoute = GetMvcRoute(parameters);
            var routeData = GetRouteData(virtualRoute, parameters);
            if (routeData == null)
            {
                return null;
            }

            CultureInfo culture = C1PageRoute.PageUrlData.LocalizationScope;
            var publicationScope = C1PageRoute.PageUrlData.PublicationScope;

            var httpContext = HttpContext.Current;
            var result = await ExecuteRouteAsync(routeData, httpContext, culture, publicationScope);

            if (result != null && C1PageRoute.GetPathInfo() != null)
            {
                C1PageRoute.RegisterPathInfoUsage();
            }

            return result;
        }

        public bool SupportsAsync { get; private set; }


        private void CopyHttpContextData(HttpContext copyFrom, HttpContext copyTo)
        {
            copyTo.User = copyFrom.User;
            copyTo.Items["AspSession"] = copyFrom.Items["AspSession"];
        }

        private XhtmlDocument ExecuteRoute(RouteData routeData, ParameterList parameters, out bool routeResolved)
        {
            AttachDynamicDataUrlMappers();

            var parentContext = HttpContext.Current;

            using (var writer = new StringWriter())
            {
                var httpResponse = new HttpResponse(writer);
                var httpContext = new HttpContext(parentContext.Request, httpResponse);
                var requestContext = new RequestContext(new HttpContextWrapper(httpContext), routeData);

                CopyHttpContextData(parentContext, httpContext);

                var handler = routeData.RouteHandler.GetHttpHandler(requestContext);
                Verify.IsNotNull(handler, $"No handler found for the function '{Namespace}.{Name}'");

                try
                {
                    handler.ProcessRequest(httpContext);
                }
                catch (HttpException httpException)
                {
                    routeResolved = httpException.GetHttpCode() != 404;
                    throw;
                }
                catch (Exception ex)
                {
                    EmbedExceptionSourceCode(ex);
                    routeResolved = true;

                    throw;
                }

                string xhtml = writer.ToString();
                routeResolved = httpResponse.StatusCode != 404;

                if (!routeResolved)
                {
                    parentContext.Response.StatusCode = 404;
                    throw new HttpException((Int32)HttpStatusCode.NotFound, "Controller returns '404' http response code");
                }

                if (httpResponse.IsRequestBeingRedirected)
                {
                    string redirectUrl = httpResponse.RedirectLocation;
                    if (ActionLinkHelper.IsRawActionLink(redirectUrl))
                    {
                        redirectUrl = ActionLinkHelper.ConvertActionLink(redirectUrl, requestContext, _functionCollection.RouteCollection);
                        redirectUrl = ActionLinkHelper.ControllerLinkToC1PageLink(redirectUrl, GetBaseMvcRoute(parameters));
                    }

                    HttpContext.Current.Response.Redirect(redirectUrl, false);
                    return null;
                }

                // TODO: Handle other HTTP Response Codes

                var document = ParseOutput(xhtml);
                ActionLinkHelper.ConvertActionLinks(document, requestContext, _functionCollection.RouteCollection);

                ProcessDocument(document, parameters);

                return document;
            }
        }

        private void AttachDynamicDataUrlMappers()
        {
            var currentPageId = PageRenderer.CurrentPageId;
            if (currentPageId == Guid.Empty)
            {
                return;
            }

            foreach (var mapperInfo in _dataUrlMappers)
            {
                DataUrls.RegisterDynamicDataUrlMapper(currentPageId, mapperInfo.Item1, mapperInfo.Item2);
            }
        }

        private void EmbedExceptionSourceCode(Exception ex)
        {
            if (ex is ThreadAbortException
                   || ex is StackOverflowException
                   || ex is OutOfMemoryException
                   || ex is ThreadInterruptedException)
            {
                return;
            }

            var stackTrace = new StackTrace(ex, true);


            foreach (var frame in stackTrace.GetFrames())
            {
                string fileName = frame.GetFileName();

                if (fileName != null && File.Exists(fileName))
                {
                    var sourceCode = C1File.ReadAllLines(fileName);

                    XhtmlErrorFormatter.EmbedSourceCodeInformation(ex, sourceCode, frame.GetFileLineNumber());
                    return;
                }
            }
        }

        private async Task<XhtmlDocument> ExecuteRouteAsync(RouteData routeData, HttpContext parentContext, CultureInfo culture, PublicationScope publicationScope)
        {
            if (HttpContext.Current == null)
            {
                HttpContext.Current = parentContext;
            }

            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture = culture;

            string xhtml;

            using (var writer = new StringWriter())
            {
                var httpResponse = new HttpResponse(writer);
                var httpContext = new HttpContext(parentContext.Request, httpResponse);
                var requestContext = new RequestContext(new HttpContextWrapper(httpContext), routeData);

                CopyHttpContextData(parentContext, httpContext);

                var handler = routeData.RouteHandler.GetHttpHandler(requestContext);
                Verify.IsNotNull(handler, $"No handler found for the function '{Namespace}.{Name}'");

                var asyncHandler = handler as IHttpAsyncHandler;
                Verify.IsNotNull(asyncHandler, "The handler class '{0}' does not implement IHttpAsyncHandler interface", handler.GetType());

                // var asyncResult = asyncHandler.BeginProcessRequest(httpContext, null, null);

                try
                {
                    using (new DataScope(publicationScope, culture))
                    {
                        await Task.Factory.FromAsync((a, b) => asyncHandler.BeginProcessRequest(httpContext, a, b), asyncHandler.EndProcessRequest, null);
                    }
                }
                catch (HttpException httpException)
                {
                    if (httpException.GetHttpCode() == 404)
                    {
                        return null;
                    }

                    // TODO: embed exception with the route path

                    throw;
                }

                xhtml = writer.ToString();
            }

            return ParseOutput(xhtml);
        }


        private RouteData GetRouteData(string virtualUrl, ParameterList parameters)
        {
            var routeData = RouteUtils.GetRouteDataByUrl(_functionCollection.RouteCollection, virtualUrl);

            var routeDataToUpdate = new List<RouteData> {routeData};

            object directRouteMatches;
            if (routeData.Values.TryGetValue("MS_DirectRouteMatches", out directRouteMatches))
            {
                var directMatchRouteData = (directRouteMatches as ICollection<RouteData>)?.FirstOrDefault();
                if (directMatchRouteData != null)
                {
                    routeDataToUpdate.Add(directMatchRouteData);
                }
            }

            foreach (var parameterName in parameters.AllParameterNames)
            {
                object value;
                if (parameters.TryGetParameter(parameterName, out value))
                {
                    routeDataToUpdate.ForEach(r => r.Values[parameterName] = value);
                }
            }

            return routeData;
        }

        private void ProcessDocument(XhtmlDocument document, ParameterList parameters)
        {
            string baseControllerUrl = GetBaseMvcRoute(parameters);

            ActionLinkHelper.ControllerLinksToC1PageLinks(document, baseControllerUrl);
        }

        private XhtmlDocument ParseOutput(string xhtml)
        {
            try
            {
                return XhtmlDocument.ParseXhtmlFragment(xhtml);
            }
            catch (XmlException ex)
            {
                string[] codeLines = xhtml.Split(new[] { Environment.NewLine, "\n" }, StringSplitOptions.None);

                XhtmlErrorFormatter.EmbedSourceCodeInformation(ex, codeLines, ex.LineNumber);

                throw;
            }
        }


        public virtual void UsePathInfoForRouting()
        {
            throw new NotSupportedException();
        }

        public virtual void AssignDynamicUrlMapper(Type dataType, IDataUrlMapper dataUrlMapper)
        {
            _dataUrlMappers.Add(new Tuple<Type, IDataUrlMapper>(dataType, dataUrlMapper));
        }

        public bool PreventFunctionOutputCaching
        {
            get;
            set;
        }
    }
}
