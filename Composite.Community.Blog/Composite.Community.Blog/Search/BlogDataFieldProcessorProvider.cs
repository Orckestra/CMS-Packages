using System.Reflection;
using Composite.Search;
using Composite.Search.Crawling;

namespace Composite.Community.Blog.Search
{
    internal class BlogDataFieldProcessorProvider : IDataFieldProcessorProvider
    {
        public IDataFieldProcessor GetDataFieldProcessor(PropertyInfo dataTypeProperty)
        {
            if (dataTypeProperty.DeclaringType.FullName == "Composite.Community.Blog.Entries")
            {
                switch (dataTypeProperty.Name)
                {
                    case "Tags":
                        return new TagsDataFieldProcessor();
                    case "Teaser":
                        return new TeaserDataFieldProcessor();
                }
            }
            return null;
        }

        internal class TagsDataFieldProcessor : DefaultDataFieldProcessor
        {
            public override string[] GetFacetValues(object value)
            {
                var str = (string)value;
                return str.Split(',');
            }

            public override DocumentFieldFacet GetDocumentFieldFacet(PropertyInfo propertyInfo)
            {
                var result = base.GetDocumentFieldFacet(propertyInfo);

                result.FacetType = FacetType.MultipleValues;

                return result;
            }
        }

        internal class TeaserDataFieldProcessor : DefaultDataFieldProcessor
        {
            public override string GetDocumentFieldName(PropertyInfo pi)
            {
                return DefaultDocumentFieldNames.Description;
            }
        }
    }
}
