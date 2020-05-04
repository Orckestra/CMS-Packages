using System;

namespace Orckestra.Web.Css.Sass
{
    internal class CssCompileException : Exception
    {
        public CssCompileException(string message) : base(message) { }
    }
}
