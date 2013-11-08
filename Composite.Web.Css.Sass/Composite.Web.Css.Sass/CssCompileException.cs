using System;

namespace Composite.Web.Css.Sass
{
    internal class CssCompileException : Exception
    {
        public CssCompileException(string message) : base(message) { }
    }
}
