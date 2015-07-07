using System.Reflection;
using System.Web.Mvc;
using Composite.Core.WebClient.Renderings.Page;

namespace Composite.AspNet.MvcFunctions.Routing
{
    public class RenderingReasonAttribute : ActionMethodSelectorAttribute
    {
        private readonly RenderingReason _renderingReason;

        public RenderingReasonAttribute(RenderingReason renderingReason)
        {
            _renderingReason = renderingReason;
        }

        public override bool IsValidForRequest(ControllerContext controllerContext, MethodInfo methodInfo)
        {
            // TODO: fix?

            //if (PageRenderer.RenderingReason == RenderingReason.Undefined)
            //{
            //    PageRenderer.RenderingReason = new UrlSpace(controllerContext.HttpContext).ForceRelativeUrls ? RenderingReason.C1ConsoleBrowserPageView : RenderingReason.PageView;
            //}

            return _renderingReason == RenderingReason.Undefined || _renderingReason == PageRenderer.RenderingReason;
        }
    }
}
