using System;
using System.Linq;
using System.Reflection;
using Composite.Core.Routing;
using Composite.Core.Routing.Foundation.PluginFacades;
using Composite.Core.Types;
using Composite.Data;

namespace Composite.AspNet.MvcFunctions.Routing
{
    class MvcFunctionDataUrlMapper: IDataUrlMapper
    {
        readonly bool _isKeyField;
        private readonly string _actionName;
        private readonly Guid? _pageId;
        private readonly Type _dataType;
        private readonly PropertyInfo _propertyInfo;

        public MvcFunctionDataUrlMapper(Type dataType, Guid? pageId, string actionName, string fieldName)
        {
            _dataType = dataType;
            _actionName = actionName;
            _pageId = pageId;

            _propertyInfo = VerifyAndGetField(_dataType, fieldName);

            var keyProperties = dataType.GetKeyProperties();
            _isKeyField = keyProperties.Count == 1 && keyProperties[0].Name == fieldName;
        }

        private static PropertyInfo VerifyAndGetField(Type dataType, string fieldName)
        {
            if (fieldName == null)
            {
                fieldName = dataType.GetKeyProperties().Single().Name;
            }

            var propertyInfo = dataType.GetPropertiesRecursively().FirstOrDefault(p => p.Name == fieldName);
            Verify.IsNotNull(propertyInfo, "Failed to get property '{0}' on type '{1}'", fieldName, dataType);

            return propertyInfo;
        }

        public IDataReference GetData(PageUrlData pageUrlData)
        {
            if (_pageId.HasValue && pageUrlData.PageId != _pageId.Value)
            {
                return null;
            }

            string pathInfo = pageUrlData.PathInfo;
            if(string.IsNullOrEmpty(pathInfo)) return null;

            string fieldUrlFragment;

            string[] urlParts = pathInfo.Split(new []{'/'}, StringSplitOptions.RemoveEmptyEntries);

            if (_actionName != null)
            {
                if (urlParts.Length != 2 || !_actionName.Equals(urlParts[0], StringComparison.OrdinalIgnoreCase))
                {
                    return null;
                }

                fieldUrlFragment = urlParts[1];
            }
            else
            {
                if (urlParts.Length != 1) return null;

                fieldUrlFragment = urlParts[0];
            }

            var fieldType = _propertyInfo.PropertyType;
            object keyValue = ValueTypeConverter.Convert(fieldUrlFragment, fieldType);

            if (keyValue == null
                || (keyValue is Guid && (Guid)keyValue == Guid.Empty && fieldUrlFragment != Guid.Empty.ToString()))
            {
                return null;
            }

            IData data = _isKeyField
                ? DataFacade.TryGetDataByUniqueKey(_dataType, keyValue)
                : GetDataByLabel(fieldUrlFragment);

            return data != null ? data.ToDataReference() : null;
        }


        private IData GetDataByLabel(string label)
        {
            foreach (IData data in DataFacade.GetData(_dataType).Cast<IData>())
            {
                string urlLabel = GetUrlLabel(data);
                if (string.IsNullOrEmpty(urlLabel)) continue;

                if (label.Equals(urlLabel, StringComparison.OrdinalIgnoreCase))
                {
                    return data;
                }
            }

            return null;
        }


        private string GetUrlLabel(IData data)
        {
            object labelValue = _propertyInfo.GetValue(data);
            if (labelValue == null)
            {
                return null;
            }

            string label = ValueTypeConverter.Convert<string>(labelValue);

            return string.IsNullOrEmpty(label) ? null : LabelToUrlPart(label);
        }


        public PageUrlData GetPageUrlData(IDataReference reference)
        {
            if(reference == null || !reference.IsSet) return null;

            Verify.That(reference.ReferencedType == _dataType, "Unexpected type of data reference");

            Guid pageId;

            if (_pageId.HasValue)
            {
                pageId = _pageId.Value;
            }
            else
            {
                if (!typeof (IPageRelatedData).IsAssignableFrom(reference.ReferencedType))
                {
                    // Cannot resolve a url for a global data type without a global url mapper
                    return null;
                }

                IData data = reference.Data;
                if(data == null) return null;

                pageId = (data as IPageRelatedData).PageId;
            }

            string pathInfo = _actionName != null ? "/" + _actionName : string.Empty;

            if (_isKeyField)
            {
                pathInfo += "/" + ValueTypeConverter.Convert<string>(reference.KeyValue);
            }
            else
            {
                string urlPart = GetUrlPart(reference);
                if(urlPart == null) return null;

                pathInfo += "/" + urlPart;
            }

            return new PageUrlData(pageId, DataScopeManager.CurrentDataScope.ToPublicationScope(), LocalizationScopeManager.CurrentLocalizationScope)
            {
                PathInfo = pathInfo
            };
        }

        private string GetUrlPart(IDataReference reference)
        {
            object labelValue = _isKeyField ? reference.KeyValue : _propertyInfo.GetValue(reference.Data);
            if (labelValue == null)
            {
                return null;
            }

            var label = ValueTypeConverter.Convert<string>(labelValue);

            return string.IsNullOrEmpty(label) ? null : LabelToUrlPart(label);
        }

        private static string LabelToUrlPart(string partnerName)
        {
            return UrlFormattersPluginFacade.FormatUrl(partnerName, true);
        }
    }
}
