using System.Web.Mvc;

namespace Composite.AspNet.MvcFunctions.Views
{
    public static class HtmlHelperExtensions
    {
        public static C1HtmlHelper C1(this HtmlHelper helper)
        {
            return new C1HtmlHelper(helper);
        }
    }
}
