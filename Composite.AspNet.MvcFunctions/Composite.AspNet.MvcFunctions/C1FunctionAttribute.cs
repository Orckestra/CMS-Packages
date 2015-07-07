using System;

namespace Composite.AspNet.MvcFunctions
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class C1FunctionAttribute: Attribute
    {
        public string Namespace { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
