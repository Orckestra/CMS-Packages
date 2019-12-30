using System;

namespace Composite.Web.Js.TypeScript
{
    internal class TypeScriptCompileException : Exception
    {
        public TypeScriptCompileException(string message) : base(message) { }
    }
}
