using System;

namespace Orckestra.Web.Css.Less
{
    internal class CssCompileException : Exception
    {
        public CssCompileException(string message) : base(message) { }
    }
}
