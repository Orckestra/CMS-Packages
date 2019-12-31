using System;

namespace Composite.Web.Js.TypeScript
{
    internal class TypeScriptCompileException : Exception
    {
        public TypeScriptCompileException(string message, string details = null) : base(message)
        {
            Details = details;
        }

        public string Details { get; }
    }
}
