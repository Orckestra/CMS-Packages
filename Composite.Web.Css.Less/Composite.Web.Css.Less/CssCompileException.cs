using System;

namespace Composite.Web.Css.Less
{
    internal class CssCompileException : Exception
    {
        public CssCompileException(string message) : base(message) { }
    }
}
