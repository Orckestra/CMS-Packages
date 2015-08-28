using System;

namespace Composite.AspNet.MvcFunctions.Routing
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class GlobalUrlMapperAttribute: Attribute
    {
        public GlobalUrlMapperAttribute(Type dataType, string pageId, string fieldName = null)
        {
            Guid id;
            if (!Guid.TryParse(pageId, out id))
            {
                throw new ArgumentException("Invalid guid format", "pageId");
            }

            DataType = dataType;
            PageId = id;
            FieldName = fieldName;
        }

        public Type DataType { get; private set; }

        public Guid PageId { get; private set; }

        public string FieldName { get; private set; }
    }
}
