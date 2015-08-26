using System;

namespace Composite.AspNet.MvcFunctions.Routing
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class DynamicUrlMapperAttribute: Attribute
    {
        public DynamicUrlMapperAttribute(Type dataType, string fieldName = null)
        {
            DataType = dataType;
            FieldName = fieldName;
        }

        public Type DataType { get; private set; }

        public string FieldName { get; private set; }
    }
}
